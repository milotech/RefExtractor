using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RefExtractor.Data;
using RefExtractor.Html;

namespace RefExtractor.Processing.TagProcessors
{
    public class ImageProcessor : ITagProcessor
    {
        public void Process(Page parentPage, HtmlTag tag, IRepository repo)
        {
            HtmlTagAttribute src;
            if(tag.Attributes != null 
                && (src = tag.Attributes.FirstOrDefault(a => a.Key.Equals("src", StringComparison.InvariantCultureIgnoreCase))) != null
                && src.Value != null)
            {
                var reference = new Reference
                {
                    PageID = parentPage.ID,
                    LinkType = (byte)(UriHelper.IsLocalUrl(parentPage.Url, src.Value) ? 0 : 1),
                    LinkUrl = src.Value,
                    DateCollected = DateTime.Now
                };

                repo.AddReference(reference);
            }
        }
    }
}
