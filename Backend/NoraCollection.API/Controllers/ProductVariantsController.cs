using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.ProductVariantDtos;

namespace NoraCollection.API.Controllers
{
    [Route("productVariants")]
    [ApiController]
    public class ProductVariantsController : CustomControllerBase
    {
        private readonly IProductVariantService _productVariantManager;

        public ProductVariantsController(IProductVariantService productVariantManager)
        {
            _productVariantManager = productVariantManager;
        }
        // Belirli bir ürünün tüm varyantlarını getirir.
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var response = await _productVariantManager.GetByProductIdAsync(productId);
            return CreateResult(response);
        }
        // Belirli bir ürünün sadece satışta olan (IsAvailable = true ve Stock > 0) varyantlarını getirir.
        [HttpGet("product/{productId}/available")]
        public async Task<IActionResult> GetAvailableByProductId(int productId)
        {
            var response = await _productVariantManager.GetAvailableByProductIdAsync(productId);
            return CreateResult(response);
        }
        // Tek bir varyantı ID'ye göre getirir.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _productVariantManager.GetAsync(id);
            return CreateResult(response);
        }
        // SKU'ya göre varyant bulur (stok takibi için).
        [HttpGet("sku/{sku}")]
        public async Task<IActionResult> GetBySku(string sku)
        {
            var response = await _productVariantManager.GetBySkuAsync(sku);
            return CreateResult(response);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductVariantCreateDto productVariantCreateDto)
        {
            var response = await _productVariantManager.AddAsync(productVariantCreateDto);
            return CreateResult(response);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProductVariantUpdateDto productVariantUpdateDto)
        {
            var response = await _productVariantManager.UpdateAsync(productVariantUpdateDto);
            return CreateResult(response);
        }
        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var response = await _productVariantManager.SoftDeleteAsync(id);
            return CreateResult(response);
        }
        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var response = await _productVariantManager.HardDeleteAsync(id);
            return CreateResult(response);
        }
        // Varyantın stok miktarını günceller (sipariş sonrası vs.).
        [HttpPatch("stock/{id}")]
        public async Task<IActionResult> UpdateStock(int id,[FromQuery] int stock)
        {
            var response = await _productVariantManager.UpdateStockAsync(id,stock);
            return CreateResult(response);
        }
        // Varyantın stok durumunu kontrol eder (yeterli stok var mı?).
        [HttpGet("{id}/check-stock")]
        public async Task<IActionResult> CheckStock(int id,[FromQuery] int requestedQuantity)
        {
            var response = await _productVariantManager.CheckStockAsync(id,requestedQuantity);
            return CreateResult(response);
        }
        // Varyantın satış durumunu (IsAvailable) günceller
        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> UpdateAvailability(int id, [FromQuery] bool isAvailable)
        {
            var response = await _productVariantManager.UpdateAvailabilityAsync(id, isAvailable);
            return CreateResult(response);
        }
    }
}
