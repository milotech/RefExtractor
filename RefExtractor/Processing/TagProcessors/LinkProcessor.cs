using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RefExtractor.Data;
using RefExtractor.Html;

namespace RefExtractor.Processing.TagProcessors
{
    public class LinkProcessor : ITagProcessor
    {
        public void Process(Page parentPage, HtmlTag tag, IRepository repo)
        {
            HtmlTagAttribute href;
            if(tag.Attributes != null
                && (href = tag.Attributes.FirstOrDefault(a => a.Key.Equals("href", StringComparison.InvariantCultureIgnoreCase))) != null
                && href.Value != null)
            {
                var reference = new Reference
                {
                    PageID = parentPage.ID,
                    LinkType = (byte)(UriHelper.IsLocalUrl(parentPage.Url, href.Value) ? 0 : 1),
                    LinkUrl = href.Value,
                    DateCollected = DateTime.Now
                };

                repo.AddReference(reference);
            }
        }
    }
}
