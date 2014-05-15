using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefExtractor.Data
{
    public class Reference
    {
        public long ID { get; set; }
        public long PageID { get; set; }
        public string LinkUrl { get; set; }
        public byte LinkType { get; set; }
        public DateTime DateCollected { get; set; }
    }
}
