using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RefExtractor.Data;
using RefExtractor.Html;

namespace RefExtractor.Processing.TagProcessors
{
    public class AnchorProcessor : ITagProcessor
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
                    DateCollected = DateTime.Now
                };

                if(href.Value.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase))
                    reference.LinkType = 3;
                else 
                    reference.LinkType = (byte)(UriHelper.IsLocalUrl(parentPage.Url, href.Value) ? 0 : 1);

                reference.LinkUrl = reference.LinkType == 3 ? href.Value.Substring("mailto:".Length) : href.Value;

                repo.AddReference(reference);
            }
        }

        
    }
}
