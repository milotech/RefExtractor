using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefExtractor.Data
{
    public class Page
    {
        public long ID { get; set; }
        public string Url { get; set; }

        public Page(long id, string url)
        {
            ID = id;
            Url = url;
            Console.WriteLine(id);
        }

        public Page() { }
    }
}
