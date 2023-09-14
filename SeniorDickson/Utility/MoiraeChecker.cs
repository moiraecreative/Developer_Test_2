using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Moirae.Utility
{

    public static class MoiraeChecker
    {
        public static string CheckTitle(object model)
        {
            IPublishedContent content = null;
            try
            {
                content = (IPublishedContent)model;
            } catch (Exception ex)
            {
                throw ex;
            }
            var title = content.Value("title") ?? content.Name;
            return title.ToString();
        }
    }
}
