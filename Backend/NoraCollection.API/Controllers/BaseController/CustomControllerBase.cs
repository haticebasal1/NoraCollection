using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.API.Controllers.BaseController;

public class CustomControllerBase : ControllerBase
{
    protected static IActionResult CreateResult<T>(ResponseDto<T> response)
    {
        return new ObjectResult(response) { StatusCode = response.StatusCode };
    }
    protected string UserId =>User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    protected string UserMail =>User.FindFirst(ClaimTypes.Email)!.Value;

    protected bool IsAdmin =>User.IsInRole("Admin");
}
