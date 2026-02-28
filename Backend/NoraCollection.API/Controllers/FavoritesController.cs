using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.FavoriteDtos;

namespace NoraCollection.API.Controllers
{
    [Route("api/favorites")]
    [ApiController]
    [Authorize]
    public class FavoritesController : CustomControllerBase
    {
        private readonly IFavoriteService _favoriteManager;

        public FavoritesController(IFavoriteService favoriteManager)
        {
            _favoriteManager = favoriteManager;
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] FavoriteCreateDto favoriteCreateDto)
        {
            favoriteCreateDto.UserId = UserId;
            var response = await _favoriteManager.AddAsync(favoriteCreateDto);
            return CreateResult(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetMyFavorites()
        {
            var response = await _favoriteManager.GetByUserId(UserId!);
            return CreateResult(response);
        }
        [HttpGet("check")]
        public async Task<IActionResult> IsInFavorites([FromQuery] int productId)
        {
            var response = await _favoriteManager.IsInFavoritesAsync(UserId!, productId);
            return CreateResult(response);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _favoriteManager.GetByIdAsync(id);
            return CreateResult(response);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Remove(int id)
        {
            var response = await _favoriteManager.RemoveAsync(id, UserId!);
            return CreateResult(response);
        }
    }
}
