using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.ProductImageDtos;

namespace NoraCollection.API.Controllers
{
    [Route("productImages")]
    [ApiController]
    public class ProductImagesController : CustomControllerBase
    {
        private readonly IProductImageService _productImageManager;

        public ProductImagesController(IProductImageService productImageManager)
        {
            _productImageManager = productImageManager;
        }
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var response = await _productImageManager.GetByProductIdAsync(productId);
            return CreateResult(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _productImageManager.GetAsync(id);
            return CreateResult(response);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] ProductImageCreateDto productImageCreateDto)
        {
            var response = await _productImageManager.AddAsync(productImageCreateDto);
            return CreateResult(response);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductImageUpdateDto productImageUpdateDto)
        {
            var response = await _productImageManager.UpdateAsync(productImageUpdateDto);
            return CreateResult(response);
        }
        [HttpPatch("set-main/{id}")]
        public async Task<IActionResult> SetAsMain(int id)
        {
            var response = await _productImageManager.SetAsMainAsync(id);
            return CreateResult(response);
        }
        [HttpPatch("display-order/{id}")]
        public async Task<IActionResult> UpdateDisplayOrder(int id, [FromBody] int displayOrder)
        {
            var response = await _productImageManager.UpdateDisplayOrderAsync(id, displayOrder);
            return CreateResult(response);
        }
        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var response = await _productImageManager.SoftDeleteAsync(id);
            return CreateResult(response);
        }
        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var response = await _productImageManager.HardDeleteAsync(id);
            return CreateResult(response);
        }
    }
}
