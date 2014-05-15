using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RefExtractor.Html;
using RefExtractor.Data;

namespace RefExtractor.Processing
{
    public interface ITagProcessor
    {
        void Process(Page parentPage, HtmlTag tag, IRepository repo);
    }
}
