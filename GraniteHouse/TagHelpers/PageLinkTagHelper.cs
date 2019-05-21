using GraniteHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        //Is a interface for creating IUrlHelper instances. 
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        
        //The property sets to ViewContext
        [ViewContext]
        [HtmlAttributeNotBound]
        //ViewContext Property gives you access to the following:
        //  - httpContext, httpResponse, httpRequest etc...
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; } 
        public string PageAction { get; set; } 
        public bool PageClassesEnabled { get; set; } 
        public string PageClass { get; set; } 
        public string PageClassNormal { get; set; } 
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //Defines the contract for the helper to build URLs for ASP.NET MVC within an application.
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            //Creates a tag in Html, I.E. : div, a, p etc...
            TagBuilder result = new TagBuilder("div");

            for (int i = 1; i <= PageModel.TotalPage; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                string url = PageModel.UrlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;

                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
