using System;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IEmailService
{
    Task<ResponseDto<bool>> SendEmailAsync(string to, string subject, string body, bool isHtml=true);
}
