using System;
using AutoMapper;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.OrderDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class OrderManager : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly ICartService _cartManager;
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<OrderItem> _orderItemRepository;
    private readonly IGenericRepository<OrderCoupon> _orderCouponRepository;
    private readonly IGenericRepository<Coupon> _couponRepository;
    private readonly IGenericRepository<ProductVariant> _productVariantRepository;
    private readonly IGenericRepository<GiftOption> _giftOptionRepository;
    private readonly IGenericRepository<Shipping> _shippingRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IOrderRepository _order;

    public OrderManager(IUnitOfWork unitOfWork, IMapper mapper, IGenericRepository<Product> productRepository, ICartService cartManager, IGenericRepository<Order> orderRepository, IGenericRepository<OrderItem> orderItemRepository, IGenericRepository<OrderCoupon> orderCouponRepository, IGenericRepository<Coupon> couponRepository, IGenericRepository<ProductVariant> productVariantRepository, IGenericRepository<GiftOption> giftOptionRepository, IGenericRepository<Shipping> shippingRepository, IGenericRepository<User> userRepository, IOrderRepository order)
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
    }

    public Task<ResponseDto<NoContentDto>> CancelOrderAsync(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> ChangeOrderStatusAsync(ChangeOrderStatusDto changeOrderStatusDto)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<OrderDto>> CreateOrderAsync(CheckoutDto checkoutDto)
    {
        throw new NotImplementedException();
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
