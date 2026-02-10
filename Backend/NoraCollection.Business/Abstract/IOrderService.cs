using System;
using NoraCollection.Shared.Dtos.OrderDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IOrderService
{

  // Sepetten checkout bilgileri ile sipariş oluşturur. Sepetteki ürünler sipariş kalemlerine dönüşür,
  Task<ResponseDto<OrderDto>> CreateOrderAsync(CheckoutDto checkoutDto);

  // Ürün detay sayfasından "Hemen Al" ile tek ürün için anında sipariş oluşturur.
  Task<ResponseDto<OrderDto>> OrderNowAsync(OrderNowDto orderNowDto);

  // Belirtilen kullanıcıya ait tüm siparişleri getirir (kullanıcının "Siparişlerim" listesi).
  Task<ResponseDto<IEnumerable<OrderDto>>> GetMyOrdersAsync(string userId , int pageNumber = 1, int pageSize = 20);

  // Id ile tek bir sipariş getirir. Genelde admin veya detay sayfası için kullanılır; kullanıcı kontrolü yoktur.
  Task<ResponseDto<OrderDto>> GetOrderAsync(int id);
  // Kullanıcının kendi siparişini id ile getirir. Siparişin o kullanıcıya ait olduğu doğrulanır;
  Task<ResponseDto<OrderDto>> GetMyOrderByIdAsync(int id, string userId);
  // Siparişin durumunu günceller (örn. Onaylandı, Kargoda, Teslim Edildi). Genelde admin panelinden kullanılır.
  Task<ResponseDto<NoContentDto>> ChangeOrderStatusAsync(ChangeOrderStatusDto changeOrderStatusDto);

  // Filtrelere göre sipariş listesini getirir. Tarih aralığı, durum, kullanıcı id, silinmiş mi vb. filtreler uygulanır;
  Task<ResponseDto<IEnumerable<OrderDto>>> GetAllAsync(OrderFiltersDto orderFiltersDto);

  // Filtrelere uyan siparişlerin toplam tutarını (ciro) hesaplar. Raporlama ve dashboard için kullanılır.
  Task<ResponseDto<decimal>> GetOrdersTotalAsync(OrderFiltersDto orderFiltersDto);

  // Kullanıcının kendi siparişini iptal eder. Siparişin o kullanıcıya ait olduğu kontrol edilir; uygun durumdaysa sipariş iptal (veya silinmiş) olarak işaretlenir.
  Task<ResponseDto<NoContentDto>> CancelOrderAsync(int id, string userId);
}
