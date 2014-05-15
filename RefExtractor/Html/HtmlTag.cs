using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefExtractor.Html
{
    public class HtmlTagAttribute
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public HtmlTagAttribute() { }
        public HtmlTagAttribute(string key, string val)
        {
            Key = key;
            Value = val;
        }
    }

    public class HtmlTag
    {
        public string Name { get; set; }
        public List<HtmlTagAttribute> Attributes { get; set; }

        public HtmlTag ()
        {
            Attributes = new List<HtmlTagAttribute>();
        }

        public override string ToString()
        {
            return string.Format("<{0} {1}>", Name, string.Join(" ", Attributes.Select(a => a.Key + (a.Value == null ? "" : "='" + a.Value + "'"))));
        }
    }
}
