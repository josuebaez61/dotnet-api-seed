using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IEmailTemplateService
    {
        Task<string> RenderTemplateAsync(string templateName, string culture, Dictionary<string, object> parameters);
        Task<string> RenderEmailAsync(string templateName, string culture, Dictionary<string, object> parameters);
    }
}
