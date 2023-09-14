using Lucene.Net.Search;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Razor.Templating.Core;

namespace Moirae.Utility
{
    public static class DefaultImageMaker
    {
        public static async Task<IHtmlContent> BuildDefaultImage()
        {
            var defaultImage = await RazorTemplateEngine.RenderAsync("PlaceholderImage.cshtml");
            return new HtmlString(defaultImage);
        }

        public static async Task<IHtmlContent> BuildDefaultPerson()
        {
            var defaultImage = await RazorTemplateEngine.RenderAsync("PlaceholderPerson.cshtml");
            return new HtmlString(defaultImage);
        }
    }
}
