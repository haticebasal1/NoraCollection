using System;
using NoraCollection.Business.Abstract;

namespace NoraCollection.API.Services;

public class EmailTemplateServices : IEmailTemplateService
{
    private readonly IWebHostEnvironment _env;

    public EmailTemplateServices(IWebHostEnvironment env)
    {
        _env = env;
    }

    public string GetTemplate(string templateName, Dictionary<string, string> placeholders)
    {
        // Dosya yolu: wwwroot/email-templates/welcome.html
        var path = Path.Combine(_env.ContentRootPath,"wwwroot","email-templates",$"{templateName}.html");
        if (!File.Exists(path))
        {
            return string.Empty;
        }
        var content = File.ReadAllText(path);
        // {{FirstName}} gibi placeholder'ları gerçek değerlerle değiştir
        foreach (var (key,value) in placeholders)
        {
            content = content.Replace($"{{{{{key}}}}}", value ?? "");
        }
        return content;
    }
}
