using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefExtractor.Data
{
    public interface IRepository
    {
        //void AddPage(Page page);
        IEnumerable<Page> GetPages();

        void AddReference(Reference reference);
        //IEnumerable<Reference> GetReferences();
    }
}
