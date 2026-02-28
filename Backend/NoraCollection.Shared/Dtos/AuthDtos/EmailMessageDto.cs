using System;

namespace NoraCollection.Shared.Dtos.AuthDtos;

public class EmailMessageDto
{
    public string? To { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public bool IsHtml { get; set; }
}
