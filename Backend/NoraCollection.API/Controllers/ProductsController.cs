using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.CategoryDtos;
using NoraCollection.Shared.Dtos.ProductDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : CustomControllerBase
    {
        private readonly IProductService _productManager;
        private readonly ICategoryService _categoryManager;

        public ProductsController(IProductService productManager, ICategoryService categoryManager)
        {
            _productManager = productManager;
            _categoryManager = categoryManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterDto? filter = null)
        {
            filter ??= new ProductFilterDto();
            var response = await _productManager.GetAllAsync(filter.IncludeCategories, filter.CategoryId, filter.StoneTypeId, filter.ColorId, filter.MinPrice, filter.MaxPrice, filter.OrderBy);
            return CreateResult(response);
        }
        [HttpGet("count")]
        public async Task<IActionResult> Count([FromQuery] int? categoryId = null)
        {
            var response = await _productManager.CountAsync(isDeleted: false, categoryId: categoryId);
            return CreateResult(response);
        }
        [HttpGet("category/{categorySlug}")]
        public async Task<IActionResult> GetByCategorySlug(string categorySlug, [FromQuery] int? stoneTypeId = null, [FromQuery] int? colorId = null, [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null)
        {
            var response = await _productManager.GetByCategorySlugAsync(categorySlug, stoneTypeId, colorId, minPrice, maxPrice);
            return CreateResult(response);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            var response = await _productManager.SearchAsync(searchTerm);
            return CreateResult(response);
        }
        [HttpGet("home")]
        public async Task<IActionResult> GetHomePage([FromQuery] int? top = null)
        {
            var response = await _productManager.GetHomePageAsync(top);
            return CreateResult(response);
        }
        [HttpGet("best-seller")]
        public async Task<IActionResult> GetBestSellers([FromQuery] int? top = null)
        {
            var response = await _productManager.GetBestSellersAsync(top);
            return CreateResult(response);
        }
        [HttpGet("new-arrivals")]
        public async Task<IActionResult> GetNewArrivals([FromQuery] int? top = null)
        {
            var response = await _productManager.GetNewArrivalsAsync(top);
            return CreateResult(response);
        }
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeatured([FromQuery] int? top = null)
        {
            var response = await _productManager.GetFeaturedAsync(top);
            return CreateResult(response);
        }
        [HttpGet("on-sale")]
        public async Task<IActionResult> GetOnSale([FromQuery] int? top = null)
        {
            var response = await _productManager.GetOnSaleAsync(top);
            return CreateResult(response);
        }
        [HttpGet("{id:int}/similar")]
        public async Task<IActionResult> GetSimilar(int id, [FromQuery] int? top = null)
        {
            var response = await _productManager.GetSimilarAsync(id, top);
            return CreateResult(response);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool includeCategories = false)
        {
            var response = await _productManager.GetAsync(id, includeCategories);
            return CreateResult(response);
        }
        [HttpGet("{id:int}/with-variants")]
        public async Task<IActionResult> GetWithVariantsById(int id)
        {
            var response = await _productManager.GetWithVariantsByIdAsync(id);
            return CreateResult(response);
        }
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug, [FromQuery] bool includeCategories = false)
        {
            var response = await _productManager.GetBySlugAsync(slug, includeCategories);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateDto productCreateDto)
        {
            var response = await _productManager.AddAsync(productCreateDto);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/count")]
        public async Task<IActionResult> CountAdmin([FromQuery] bool? isDeleted = null, [FromQuery] int? categoryId = null)
        {
            var response = await _productManager.CountAsync(isDeleted, categoryId);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductUpdateDto productUpdateDto)
        {
            var response = await _productManager.UpdateAsync(productUpdateDto);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int stock)
        {
            var response = await _productManager.UpdateStockAsync(id, stock);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/discounted-price")]
        public async Task<IActionResult> UpdateDiscountedPrice(int id, [FromBody] decimal? discountedPrice)
        {
            var response = await _productManager.UpdateDiscountedPriceAsync(id, discountedPrice);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/toogle-home")]
        public async Task<IActionResult> ToggleIsHome(int id)
        {
            var response = await _productManager.UpdateIsHomeAsync(id);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/toogle-best-seller")]
        public async Task<IActionResult> ToggleIsBestSeller(int id)
        {
            var response = await _productManager.UpdateIsBestSellerAsync(id);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/toogle-new-arrival")]
        public async Task<IActionResult> ToggleIsNewArrival(int id)
        {
            var response = await _productManager.UpdateIsNewArrivalAsync(id);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/toogle-featured")]
        public async Task<IActionResult> ToggleIsFeatured(int id)
        {
            var response = await _productManager.UpdateIsFeaturedAsync(id);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("soft-delete/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var response = await _productManager.SoftDeleteAsync(id);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("Hard-delete/{id:int}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var response = await _productManager.HardDeleteAsync(id);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("soft-delete/category/{id:int}")]
        public async Task<IActionResult> SoftDeleteByCategory(int categoryId)
        {
            var response = await _productManager.SoftDeleteByCategoryIdAsync(categoryId);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/categories-with-product-count")]
        public async Task<IActionResult> GetCategoriesWithProductCount()
        {
            var categoriesResponse = await _categoryManager.GetAllAsync();
            if (!categoriesResponse.IsSuccessful)
            {
                return CreateResult(categoriesResponse);
            }
            var categoryDtos = new List<CategoryDto>();
            foreach (var category in categoriesResponse.Data)
            {
                var productCountResponse = await _productManager.CountAsync(isDeleted:false,categoryId: category.Id);
                if (productCountResponse.IsSuccessful)
                {
                    categoryDtos.Add(new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        ProductCount = productCountResponse.Data
                    });
                }
            }
            var response = ResponseDto<List<CategoryDto>>.Success(categoryDtos,StatusCodes.Status200OK);
            return CreateResult(response);
        }
    }
}
