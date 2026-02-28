using System;

namespace NoraCollection.Business.Abstract;
//Template i okuması için yardımcı interface
public interface IEmailTemplateService
{
    string GetTemplate(string templateName, Dictionary<string, string> placeholders);
}
