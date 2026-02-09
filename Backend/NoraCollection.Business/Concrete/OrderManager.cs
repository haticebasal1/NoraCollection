using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.OrderDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;
using NoraCollection.Shared.Enums;

namespace NoraCollection.Business.Concrete;

public class OrderManager : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly ICartService _cartManager;
    private readonly IProductService _productManager;
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<OrderItem> _orderItemRepository;
    private readonly IGenericRepository<OrderCoupon> _orderCouponRepository;
    private readonly IGenericRepository<Coupon> _couponRepository;
    private readonly IGenericRepository<ProductVariant> _productVariantRepository;
    private readonly IGenericRepository<GiftOption> _giftOptionRepository;
    private readonly IGenericRepository<Shipping> _shippingRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IOrderRepository _order;

    /// <summary>
    /// Hangi sipariş durumundan hangi duruma geçilebileceği. Geçersiz geçişler (örn. Delivered → Pending) engellenir.
    /// </summary>
    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
    {
        [OrderStatus.PendingPayment] = new[] { OrderStatus.Paid },
        [OrderStatus.Paid] = new[] { OrderStatus.Pending, OrderStatus.Proccessing },
        [OrderStatus.Pending] = new[] { OrderStatus.Proccessing },
        [OrderStatus.Proccessing] = new[] { OrderStatus.Shipped },
        [OrderStatus.Shipped] = new[] { OrderStatus.Delivered },
        [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
        [OrderStatus.Cancelled] = Array.Empty<OrderStatus>(),
    };

    public OrderManager(IUnitOfWork unitOfWork, IMapper mapper, IGenericRepository<Product> productRepository, ICartService cartManager, IGenericRepository<Order> orderRepository, IGenericRepository<OrderItem> orderItemRepository, IGenericRepository<OrderCoupon> orderCouponRepository, IGenericRepository<Coupon> couponRepository, IGenericRepository<ProductVariant> productVariantRepository, IGenericRepository<GiftOption> giftOptionRepository, IGenericRepository<Shipping> shippingRepository, IGenericRepository<User> userRepository, IOrderRepository order, IProductService productManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productRepository = _unitOfWork.GetRepository<Product>();
        _cartManager = cartManager;
        _orderRepository = _unitOfWork.GetRepository<Order>();
        _orderItemRepository = _unitOfWork.GetRepository<OrderItem>();
        _orderCouponRepository = _unitOfWork.GetRepository<OrderCoupon>();
        _couponRepository = _unitOfWork.GetRepository<Coupon>();
        _productVariantRepository = _unitOfWork.GetRepository<ProductVariant>();
        _giftOptionRepository = _unitOfWork.GetRepository<GiftOption>();
        _shippingRepository = _unitOfWork.GetRepository<Shipping>();
        _userRepository = _unitOfWork.GetRepository<User>();
        _order = order;
        _productManager = productManager;
    }

    public async Task<ResponseDto<NoContentDto>> CancelOrderAsync(int id, string userId)
    {
        try
        {
            //Id kontrolü
            var order = await _orderRepository.GetAsync(
                predicate: x => x.Id == id,
                includes: query => query.Include(o => o.OrderItems)
                              .ThenInclude(oi => oi.Product)
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.ProductVariant)
                              .Include(o => o.OrderCoupons)
                         .ThenInclude(oc => oc.Coupon)
            );
            if (order is null)
            {
                return ResponseDto<NoContentDto>.Fail("Sipariş bulunamadığı için sipariş iptal edilemedi!", StatusCodes.Status404NotFound);
            }
            //User kontrolü
            if (order.UserId != userId)
            {
                return ResponseDto<NoContentDto>.Fail("Bu siparişi iptal etme yetkiniz yok!", StatusCodes.Status403Forbidden);
            }
            //İptal etme durumu
            var cancellableStatuses = new[] { OrderStatus.Pending, OrderStatus.PendingPayment };
            if (!cancellableStatuses.Contains(order.OrderStatus))
            {
                return ResponseDto<NoContentDto>.Fail("Hazırlanan veya kargolanan siparişler iptal edilemez!", StatusCodes.Status400BadRequest);
            }
            // --- STOK İADESİ MANTIĞI ---
            if (order.OrderItems != null)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    // Eğer ürünün varyantı varsa (Beden/Renk), varyant stoğunu artır
                    if (orderItem.ProductVariantId.HasValue && orderItem.ProductVariant != null)
                    {
                        orderItem.ProductVariant.Stock += orderItem.Quantity;
                        _productVariantRepository.Update(orderItem.ProductVariant);
                    }
                    // Varyant yoksa doğrudan ana ürün stoğunu artır
                    else if (orderItem.Product != null)
                    {
                        orderItem.Product.Stock += orderItem.Quantity;
                        _productRepository.Update(orderItem.Product);
                    }
                }
            }
            // --- KUPON İADESİ MANTIĞI ---
            if (order.OrderCoupons != null && order.OrderCoupons.Any())
            {
                foreach (var orderCoupon in order.OrderCoupons)
                {
                    if (orderCoupon != null && orderCoupon.Coupon != null)
                    {
                        // Kullanım sayısını 1 azaltarak kuponu tekrar "kullanılabilir" yapıyoruz
                        orderCoupon.Coupon.UsedCount = Math.Max(0, orderCoupon.Coupon.UsedCount - 1);
                        _couponRepository.Update(orderCoupon.Coupon);
                    }
                }
            }
            //Durum GÜNCELLEMESİ
            order.OrderStatus = OrderStatus.Cancelled;
            order.IsDeleted = true;
            order.DeletedAt = DateTimeOffset.UtcNow;
            _orderRepository.Update(order);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> ChangeOrderStatusAsync(ChangeOrderStatusDto changeOrderStatusDto)
    {
        try
        {
            var order = await _orderRepository.GetAsync(
                predicate: x => x.Id == changeOrderStatusDto.OrderId
            );
            if (order is null)
            {
                return ResponseDto<NoContentDto>.Fail("Sipariş bulunamadığı için durum güncellenemedi!", StatusCodes.Status404NotFound);
            }
            // 1. Zaten aynı durumdaysa işlem yapma
            if (order.OrderStatus == changeOrderStatusDto.OrderStatus)
            {
                return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
            }
            if (changeOrderStatusDto.OrderStatus == OrderStatus.Cancelled)
            {
                return ResponseDto<NoContentDto>.Fail("Siparişi iptal etmek için lütfen İptal metodunu kullanın.", StatusCodes.Status400BadRequest);
            }
            // Geçerli durum geçişi kontrolü (örn. Delivered → Pending engellenir)
            if (!AllowedTransitions.TryGetValue(order.OrderStatus, out var allowed) || !allowed.Contains(changeOrderStatusDto.OrderStatus))
            {
                return ResponseDto<NoContentDto>.Fail("Bu sipariş için geçersiz durum geçişi.", StatusCodes.Status400BadRequest);
            }
            order.OrderStatus = changeOrderStatusDto.OrderStatus;
            order.UpdatedAt = DateTimeOffset.UtcNow;
            _orderRepository.Update(order);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<OrderDto>> CreateOrderAsync(CheckoutDto checkoutDto)
    {
        try
        {
            // Kullanıcı kontrolü: UserId boşsa sipariş oluşturulamaz.
            if (string.IsNullOrWhiteSpace(checkoutDto.UserId))
            {
                return ResponseDto<OrderDto>.Fail("Kullanıcı bilgisi bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            // Sepeti getir: Kullanıcının aktif sepeti yoksa veya yüklenemezse hata dön.
            var cartResponse = await _cartManager.GetCartAsync(
              checkoutDto.UserId
            );
            if (!cartResponse.IsSuccessful || cartResponse.Data is null)
            {
                return ResponseDto<OrderDto>.Fail("Sepet bulunamadı!", StatusCodes.Status404NotFound);
            }
            var cart = cartResponse.Data;
            // Sepette en az bir ürün olmalı; boş sepetle sipariş açılmaz.
            if (cart.CartItems == null || !cart.CartItems.Any())
            {
                return ResponseDto<OrderDto>.Fail("Sepetiniz boş!", StatusCodes.Status400BadRequest);
            }
            // Stok kontrolü: Her kalem için yeterli stok var mı? Varyantlı ürünlerde CheckStockAsync, diğerlerinde Product.Stock kontrol edilir.
            foreach (var item in cart.CartItems)
            {
                if (item.ProductVariantId.HasValue)
                {
                    var stock = await _productManager.CheckStockAsync(
                        item.ProductId, item.ProductVariantId.Value, item.Quantity);
                    if (!stock.IsSuccessful || stock.Data == false)
                    {
                        return ResponseDto<OrderDto>.Fail("Bu ürün için yeterli stok yok.", StatusCodes.Status400BadRequest);
                    }
                }
                else
                {
                    var product = await _productRepository.GetAsync(x => x.Id == item.ProductId);
                    if (product == null || product.Stock < item.Quantity)
                    {
                        return ResponseDto<OrderDto>.Fail("Bu ürün için yeterli stok yok.", StatusCodes.Status400BadRequest);
                    }
                }
            }
            // Tutar hesapları: Ara toplam, kupon indirimi, kargo (şu an 0), son ödenecek tutar.
            decimal totalAmount = cart.CartItems.Sum(x => x.UnitPrice * x.Quantity);
            decimal discountAmount = cart.CouponDiscountAmount;
            decimal shippingFee = 0m;
            decimal finalTotal = totalAmount - discountAmount + shippingFee;
            // Sipariş entity'sini oluştur, checkout ve hediye bilgilerini ata, veritabanına ekle ve kaydet.
            var orderDate = DateTime.UtcNow;
            var order = new Order(
                checkoutDto.UserId,
                checkoutDto.CustomerName,
                checkoutDto.PhoneNumber,
                checkoutDto.Email,
                checkoutDto.Address,
                checkoutDto.City,
                checkoutDto.District,
                checkoutDto.ZipCode ?? "",
                orderDate,
                totalAmount,
                discountAmount,
                shippingFee,
                finalTotal);
            order.PaymentMethod = checkoutDto.PaymentMethod;
            order.GiftNote = checkoutDto.GiftNote;
            order.GiftOptionId = checkoutDto.GiftOptionId;
            await _orderRepository.AddAsync(order);
            var saveResult = await _unitOfWork.SaveAsync();
            if (saveResult < 1)
            {
                return ResponseDto<OrderDto>.Fail("Sipariş oluşturulurken bir hata oluştu.", StatusCodes.Status500InternalServerError);
            }
            // Her sepet kalemini sipariş kalemi (OrderItem) olarak ekle ve kaydet.
            foreach (var item in cart.CartItems)
            {
                var orderItem = new OrderItem(
                    order.Id,
                    item.ProductId,
                    item.Quantity,
                    item.UnitPrice,
                    item.ProductVariantId
                );
                await _orderItemRepository.AddAsync(orderItem);
            }
            // Sepette kupon kullanıldıysa: OrderCoupon kaydı oluştur, kuponun kullanım sayısını (UsedCount) 1 artır.
            if (cart.CouponId.HasValue)
            {
                var orderCoupon = new OrderCoupon
                {
                    OrderId = order.Id,
                    CouponId = cart.CouponId.Value
                };
                await _orderCouponRepository.AddAsync(orderCoupon);

                var coupon = await _couponRepository.GetAsync(
                    x=>x.Id == cart.CouponId.Value
                );
                if (coupon!=null)
                {
                    coupon.UsedCount++;
                    _couponRepository.Update(coupon);
                }
            }
            // Stok düşümü: Varyantlı ürünlerde varyant stoğunu, diğerlerinde ana ürün stoğunu sipariş miktarı kadar azalt.
            foreach (var item in cart.CartItems)
            {
                if (item.ProductVariantId.HasValue)
                {
                    var variant = await _productVariantRepository.GetAsync(
                        x => x.Id == item.ProductVariantId.Value);
                    if (variant!=null)
                    {
                        variant.Stock -= item.Quantity;
                        _productVariantRepository.Update(variant);
                    }
                }else
                {
                    var product = await _productRepository.GetAsync(x=>x.Id == item.ProductId);
                    if (product!=null)
                    {
                        product.Stock -= item.Quantity;
                        _productRepository.Update(product);
                    }
                }
            }
            await _unitOfWork.SaveAsync();
            // Sipariş başarıyla oluşturulduğu için sepeti temizle (ClearCartAsync).
            await _cartManager.ClearCartAsync(checkoutDto.UserId!);
            // Oluşan siparişi OrderItems, OrderCoupons ve ilişkili Product/ProductVariant/Coupon ile tekrar çek; DTO'ya map'le ve 201 ile dön.
            var orderWithDetails = await _orderRepository.GetAsync(
                predicate: x => x.Id == order.Id,
                includeDeleted: false,
                includes: q => q.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.ProductVariant)
                    .Include(o => o.OrderCoupons).ThenInclude(oc => oc.Coupon));
            if (orderWithDetails is null)
            {
                return ResponseDto<OrderDto>.Fail("Sipariş yüklenemedi.", StatusCodes.Status500InternalServerError);
            }
            var orderDto = _mapper.Map<OrderDto>(orderWithDetails);
            return ResponseDto<OrderDto>.Success(orderDto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<OrderDto>.Fail($"Beklenmedik hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public Task<ResponseDto<IEnumerable<OrderDto>>> GetAllAsync(OrderFiltersDto orderFiltersDto)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<OrderDto>> GetMyOrderByIdAsync(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<IEnumerable<OrderDto>>> GetMyOrdersAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<OrderDto>> GetOrderAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<decimal>> GetOrdersTotalAsync(OrderFiltersDto orderFiltersDto)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<OrderDto>> OrderNowAsync(OrderNowDto orderNowDto)
    {
        throw new NotImplementedException();
    }
}
