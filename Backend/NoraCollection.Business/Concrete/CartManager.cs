using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.CartDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class CartManager : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Cart> _cartRepository;
    private readonly IGenericRepository<CartItem> _cartItemRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IGenericRepository<ProductVariant> _productVariantRepository;
    private readonly IGenericRepository<GiftOption> _giftOptionRepository;
    private readonly IGenericRepository<Coupon> _couponRepository;
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public CartManager(IUnitOfWork unitOfWork, IGenericRepository<Cart> cartRepository, IGenericRepository<CartItem> cartItemRepository, IGenericRepository<Product> productRepository, IGenericRepository<ProductVariant> productVariantRepository, IGenericRepository<GiftOption> giftOptionRepository, IProductService productService, IMapper mapper, IGenericRepository<Coupon> couponRepository)
    {
        _unitOfWork = unitOfWork;
        _cartRepository = _unitOfWork.GetRepository<Cart>();
        _cartItemRepository = _unitOfWork.GetRepository<CartItem>();
        _productRepository = _unitOfWork.GetRepository<Product>();
        _productVariantRepository = _unitOfWork.GetRepository<ProductVariant>();
        _giftOptionRepository = _unitOfWork.GetRepository<GiftOption>();
        _productService = productService;
        _mapper = mapper;
        _couponRepository = _unitOfWork.GetRepository<Coupon>();
    }

    public async Task<ResponseDto<CartItemDto>> AddToCartAsync(AddToCartDto addToCartDto)
    {
        try
        {
            // Ürün var mı?
            var product = await _productRepository.GetAsync(
                x => x.Id == addToCartDto.ProductId && !x.IsDeleted,
                includeDeleted: false
            );
            if (product is null)
            {
                return ResponseDto<CartItemDto>.Fail("Ürün bulunamadı!", StatusCodes.Status404NotFound);
            }
            // Varyant var mı, bu ürüne ait mi, satışta mı?
            var variant = await _productVariantRepository.GetAsync(
                x => x.Id == addToCartDto.ProductVariantId && x.ProductId == addToCartDto.ProductId && x.IsAvailable && !x.IsDeleted,
                includeDeleted: false
            );
            if (variant is null)
            {
                return ResponseDto<CartItemDto>.Fail("Geçersiz veya satışta olmayan ürün seçeneği!", StatusCodes.Status404NotFound);

            }
            // Birim fiyat: Product.Price + (PriceAdjustment ?? 0)
            decimal unitPrice = product.Price + (variant.PriceAdjustment ?? 0);
            // Stok kontrolü (ProductManager üzerinden merkezi; productId + variantId ile)
            var requestedQuantity = addToCartDto.Quantity;
            var stockCheck = await _productService.CheckStockAsync(addToCartDto.ProductId, addToCartDto.ProductVariantId, requestedQuantity);
            if (!stockCheck.IsSuccessful || !stockCheck.Data)
            {
                var stockMessage = stockCheck.IsSuccessful ? "Yeterli stok yok." : (stockCheck.Errors?.FirstOrDefault() ?? "Stok kontrolü yapılamadı.");
                return ResponseDto<CartItemDto>.Fail(stockMessage, StatusCodes.Status400BadRequest);
            }
            // Kullanıcının sepeti (yoksa oluştur)
            var cart = await _cartRepository.GetAsync(
                predicate: x => x.UserId == addToCartDto.UserId && x.IsActive,
                includeDeleted: false,
                includes: q => q.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                    .Include(c => c.CartItems).ThenInclude(ci => ci.ProductVariant)
            );
            //Sepet yoksa
            if (cart is null)
            {
                cart = new Cart(addToCartDto.UserId);
                await _cartRepository.AddAsync(cart);
                var carts = await _unitOfWork.SaveAsync();
                if (carts < 1)
                {
                    return ResponseDto<CartItemDto>.Fail("Sepet oluşturulamadı!", StatusCodes.Status500InternalServerError);
                }
            }
            // Aynı ürün + aynı varyant zaten sepette mi? (Quantity artır)
            var existsCartItem = cart.CartItems?.FirstOrDefault(
                x => x.ProductId == addToCartDto.ProductId && x.ProductVariantId == addToCartDto.ProductVariantId
            );
            if (existsCartItem is not null)
            {
                var newQuantity = existsCartItem.Quantity + addToCartDto.Quantity;
                var stockCheckExisting = await _productService.CheckStockAsync(addToCartDto.ProductId, addToCartDto.ProductVariantId, newQuantity);
                if (!stockCheckExisting.IsSuccessful || !stockCheckExisting.Data)
                {
                    var stockMessageExisting = stockCheckExisting.IsSuccessful ? "Yeterli stok yok. Sepetteki miktar ile birlikte stok aşılamaz." : (stockCheckExisting.Errors?.FirstOrDefault() ?? "Stok kontrolü yapılamadı.");
                    return ResponseDto<CartItemDto>.Fail(stockMessageExisting, StatusCodes.Status400BadRequest);
                }
                existsCartItem.Quantity = newQuantity;
                _cartItemRepository.Update(existsCartItem);
                var existsCart = await _unitOfWork.SaveAsync();
                if (existsCart < 1)
                {
                    return ResponseDto<CartItemDto>.Fail("Sepet güncellenirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
                }
                var cartDto = _mapper.Map<CartItemDto>(existsCartItem);
                return ResponseDto<CartItemDto>.Success(cartDto, StatusCodes.Status200OK);
            }
            // Yeni kalem ekle
            var cartItem = new CartItem(
                cart.Id,
                addToCartDto.ProductId,
                addToCartDto.ProductVariantId,
                addToCartDto.Quantity,
                unitPrice
            );
            await _cartItemRepository.AddAsync(cartItem);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<CartItemDto>.Fail("Ürün sepete eklenirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            // Veritabanına tekrar gitmeden map için Product ve ProductVariant atanıyor
            cartItem.Product = product;
            cartItem.ProductVariant = variant;
            var addToCartDtos = _mapper.Map<CartItemDto>(cartItem);
            return ResponseDto<CartItemDto>.Success(addToCartDtos, StatusCodes.Status201Created);

        }
        catch (Exception ex)
        {
            return ResponseDto<CartItemDto>.Fail($"Beklenmedik bir hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> ChangeQuantityAsync(ChangeQuantityDto changeQuantityDto)
    {
        try
        {
            // Kullanıcı kontrolü (UserId controller'da token'dan set edilmiş olmalı)
            if (string.IsNullOrWhiteSpace(changeQuantityDto.UserId))
            {
                return ResponseDto<NoContentDto>.Fail("Kullanıcı bilgisi bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            //Adet 0 veya negatifse: Bu kalemi sepette sil (kullanıcı "sepetten çıkar" demiş sayılır)
            if (changeQuantityDto.Quantity <= 0)
            {
                // Silinecek kalemi bul; Cart include ederek sonra "bu sepet bu kullanıcıya mı ait?" diye kontrol edeceğiz
                var itemToDelete = await _cartItemRepository.GetAsync(
                    x => x.Id == changeQuantityDto.CartItemId,
                    includeDeleted: false,
                    q => q.Include(ci => ci.Cart)
                );
                if (itemToDelete is null)
                {
                    return ResponseDto<NoContentDto>.Fail("Sepet kalemi bulunamadı!", StatusCodes.Status404NotFound);
                }
                // Güvenlik: Sadece kendi sepetindeki kalemi silebilsin
                if (itemToDelete.Cart?.UserId != changeQuantityDto.UserId)
                {
                    return ResponseDto<NoContentDto>.Fail("Bu sepet kalemi size ait değil!", StatusCodes.Status403Forbidden);
                }
                _cartItemRepository.Delete(itemToDelete);
                await _unitOfWork.SaveAsync();
                return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
            }
            //Adet 1 veya daha fazlaysa: Kalemi bul, sahiplik ve stok kontrolü yap, adeti güncelle
            var cartItem = await _cartItemRepository.GetAsync(
                x => x.Id == changeQuantityDto.CartItemId,
                includeDeleted: false,
                q => q.Include(ci => ci.Cart).Include(ci => ci.Product).Include(ci => ci.ProductVariant)
            );
            if (cartItem is null)
            {
                return ResponseDto<NoContentDto>.Fail("İlgili ürün sepette bulunamadı!", StatusCodes.Status404NotFound);
            }
            if (cartItem.Cart?.UserId != changeQuantityDto.UserId)
            {
                return ResponseDto<NoContentDto>.Fail("Bu sepet size ait değil!", StatusCodes.Status403Forbidden);
            }
            // Varyant bilgisi yoksa stok kontrolü yapılamaz (bu kalem geçersiz sayılır)
            if (!cartItem.ProductVariantId.HasValue)
            {
                return ResponseDto<NoContentDto>.Fail("Bu sepet kalemi için ürün seçeneği bulunamadı; adet güncellenemez.", StatusCodes.Status400BadRequest);
            }
            // Stok kontrolü
            var stockCheck = await _productService.CheckStockAsync(cartItem.ProductId, cartItem.ProductVariantId.Value, changeQuantityDto.Quantity);
            if (!stockCheck.IsSuccessful || !stockCheck.Data)
            {
                return ResponseDto<NoContentDto>.Fail("Yeterli stok bulunmuyor!", StatusCodes.Status400BadRequest);
            }
            //Adet güncelleme ve kaydetme yeri
            cartItem.Quantity = changeQuantityDto.Quantity;
            _cartItemRepository.Update(cartItem);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Ürün adedi güncellenirken hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik bir hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> ClearCartAsync(string userId)
    {
        try
        {
            // Kullanıcı kontrolü (UserId controller'da token'dan set edilmiş olmalı)
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResponseDto<NoContentDto>.Fail("Kullanıcı bilgisi bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            var cart = await _cartRepository.GetAsync(
                x => x.UserId == userId && x.IsActive,
                includeDeleted: false,
                q => q.Include(c => c.CartItems)
            );
            if (cart is null)
            {
                return ResponseDto<NoContentDto>.Fail("Sepet bulunamadı!", StatusCodes.Status404NotFound);
            }
            if (cart.CartItems?.Any() == true)
            {
                foreach (var item in cart.CartItems.ToList())
                {
                    _cartItemRepository.Delete(item);
                }
            }
            cart.GiftNote = null;
            cart.IsGiftPackage = false;
            cart.GiftOptionId = null;
            cart.CouponId = null;
            cart.UpdatedAt = DateTimeOffset.UtcNow;
            _cartRepository.Update(cart);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Sunucuda bir sorun oluştuğu için sepet temizlenemedi!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik bir hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<CartDto>> CreateCartAsync(CartCreateDto cartCreateDto)
    {
        try
        {
            // Kullanıcı kontrolü (UserId controller'da token'dan set edilmiş olmalı)
            if (string.IsNullOrWhiteSpace(cartCreateDto.UserId))
            {
                return ResponseDto<CartDto>.Fail("Kullanıcı bilgisi bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            var isExists = await _cartRepository.ExistsAsync(
               x => x.UserId == cartCreateDto.UserId && x.IsActive);
            if (isExists)
            {
                return ResponseDto<CartDto>.Fail("Kullanıcının zaten sepeti var!", StatusCodes.Status400BadRequest);
            }
            var cart = new Cart(cartCreateDto.UserId);
            await _cartRepository.AddAsync(cart);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<CartDto>.Fail("Sunucuda bir sorun oluştuğu için yeni sepet oluşturulamadı!", StatusCodes.Status500InternalServerError);
            }
            var cartDto = _mapper.Map<CartDto>(cart);
            return ResponseDto<CartDto>.Success(cartDto, StatusCodes.Status200OK);

        }
        catch (Exception ex)
        {
            return ResponseDto<CartDto>.Fail($"Beklenmedik bir hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<CartDto>> GetCartAsync(string userId)
    {
        try
        {
            // Kullanıcı kontrolü (UserId controller'da token'dan set edilmiş olmalı)
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResponseDto<CartDto>.Fail("Kullanıcı bilgisi bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            var cart = await _cartRepository.GetAsync(
                x => x.UserId == userId && x.IsActive,
                includeDeleted: false,
                q => q.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .Include(c => c.CartItems).ThenInclude(ci => ci.ProductVariant).ThenInclude(pv => pv!.Color)
                .Include(c => c.Coupon)
                .Include(c => c.GiftOption)
            );
            if (cart is null)
            {
                return ResponseDto<CartDto>.Fail("Kullanıcıya ait sepet bulunamadı!", StatusCodes.Status404NotFound);
            }
            var cartDto = _mapper.Map<CartDto>(cart);
            return ResponseDto<CartDto>.Success(cartDto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<CartDto>.Fail($"Beklenmedik bir hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<int>> GetCartItemsCountAsync(string userId)
    {
        try
        {
            // Kullanıcı kontrolü (UserId controller'da token'dan set edilmiş olmalı)
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResponseDto<int>.Fail("Kullanıcı bilgisi bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            var cart = await _cartRepository.GetAsync(
                x => x.UserId == userId && x.IsActive,
                includeDeleted: false,
                q => q.Include(c => c.CartItems)
            );
            if (cart is null)
            {
                return ResponseDto<int>.Success(0, StatusCodes.Status200OK);
            }
            var totalQuantity = cart.CartItems?.Sum(ci => ci.Quantity) ?? 0;
            return ResponseDto<int>.Success(totalQuantity, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<int>.Fail($"Beklenmedik bir hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> RemoveFromCartAsync(int cartItemId, string userId)
    {
        try
        {
            // Kullanıcı kontrolü (UserId controller'da token'dan set edilmiş olmalı)
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResponseDto<NoContentDto>.Fail("Kullanıcı bilgisi bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            var cartItem = await _cartItemRepository.GetAsync(
                x => x.Id == cartItemId,
                includeDeleted: false,
                q => q.Include(ci => ci.Cart)
            );
            if (cartItem is null)
            {
                return ResponseDto<NoContentDto>.Fail("Ürün sepette bulunamadığı için silinemedi", StatusCodes.Status404NotFound);
            }
            if (cartItem.Cart?.UserId != userId)
            {
                return ResponseDto<NoContentDto>.Fail("Bu sepet kalemi size ait değil!", StatusCodes.Status403Forbidden);
            }
            _cartItemRepository.Delete(cartItem);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Sunucuda bir sorun oluştuğu için sepet temizlenemedi!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik bir hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateCartOptionAsync(string userId, UpdateCartOptionsDto updateCartOptionsDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResponseDto<NoContentDto>.Fail("Kullanıcı bilgisi bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            var cart = await _cartRepository.GetAsync(
                x => x.UserId == userId && x.IsActive,
                includeDeleted: false
            );
            if (cart is null)
            {
                return ResponseDto<NoContentDto>.Fail("Kullanıcıya ait sepet bulunamadı!", StatusCodes.Status404NotFound);
            }
            // GiftOptionId doluysa bu id'nin veritabanında var olduğunu kontrol et
            if (updateCartOptionsDto.GiftOptionId.HasValue)
            {
                var isGiftOptionExists = await _giftOptionRepository.ExistsAsync(
                    x => x.Id == updateCartOptionsDto.GiftOptionId.Value
                );
                if (!isGiftOptionExists)
                {
                    return ResponseDto<NoContentDto>.Fail("Belirtilen hediye seçeneği bulunamadı.", StatusCodes.Status400BadRequest);
                }
            }
            cart.GiftOptionId = updateCartOptionsDto.GiftOptionId;
            cart.IsGiftPackage = updateCartOptionsDto.IsGiftPackage;
            cart.GiftNote = updateCartOptionsDto.GiftNote;
            cart.UpdatedAt = DateTimeOffset.UtcNow;
            _cartRepository.Update(cart);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Sunucuda bir sorun oluştuğu için sepet güncellenemedi!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik bir hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> ApplyCouponAsync(string userId, string couponCode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResponseDto<NoContentDto>.Fail("Kullanıcı bilgisi bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                return ResponseDto<NoContentDto>.Fail("Kupon kodu girilmedi!", StatusCodes.Status400BadRequest);
            }
            var cart = await _cartRepository.GetAsync(
                x => x.UserId == userId && x.IsActive,
                includeDeleted: false
            );
            if (cart is null)
            {
                return ResponseDto<NoContentDto>.Fail("Kullanıcıya ait sepet bulunamadı!", StatusCodes.Status404NotFound);
            }
            var now = DateTime.UtcNow;
            var code = couponCode.Trim().ToUpperInvariant();
            var coupon = await _couponRepository.GetAsync(
                x => x.Code != null
                     && x.Code.Trim().ToUpperInvariant() == code
                     && x.IsActive && !x.IsDeleted && x.ExpiryDate >= now
                     && x.UsedCount < x.UsageLimit,
                includeDeleted: false
            );
            if (coupon is null)
            {
                return ResponseDto<NoContentDto>.Fail("Geçersiz, süresi dolmuş veya kullanım limiti aşılmış kupon.", StatusCodes.Status400BadRequest);
            }
            cart.CouponId = coupon.Id;
            cart.UpdatedAt = DateTimeOffset.UtcNow;
            _cartRepository.Update(cart);
            if (await _unitOfWork.SaveAsync() < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Kupon uygulanırken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik bir hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> RemoveCouponAsync(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResponseDto<NoContentDto>.Fail("Kullanıcı bilgisi bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            var cart = await _cartRepository.GetAsync(
                x => x.UserId == userId && x.IsActive,
                includeDeleted: false
            );
            if (cart is null)
            {
                return ResponseDto<NoContentDto>.Fail("Kullanıcıya ait sepet bulunamadı!", StatusCodes.Status404NotFound);
            }
            cart.CouponId = null;
            cart.UpdatedAt = DateTimeOffset.UtcNow;
            _cartRepository.Update(cart);
            if (await _unitOfWork.SaveAsync() < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Kupon kaldırılırken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik bir hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }
}
