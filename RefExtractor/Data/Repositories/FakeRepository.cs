using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefExtractor.Data.Repositories
{
    // для тестовых целей
    public class FakeRepository : IRepository
    {
        private List<Page> _pages;
        private List<Reference> _references = new List<Reference>();

        public FakeRepository()
        {
            _pages = new List<Page> {
                new Page { ID = 1, Url = "http://www.sports.ru/" },
                new Page { ID = 2, Url = "http://www.ria.ru/" },
                new Page { ID = 3, Url = "http://pikabu.ru/" },
                new Page { ID = 4, Url = "http://www.drive2.ru/r/volkswagen/" },
                new Page { ID = 5, Url = "http://blogs.msdn.com/b/pfxteam/archive/2011/09/17/10212961.aspx" },
                new Page { ID = 6, Url = "http://хреновый-урл.com" },
                new Page { ID = 7, Url = "http://www.rsdn.ru/" },
                new Page { ID = 8, Url = "http://еще-один-хреновый-урл.com" },
                new Page { ID = 9, Url = "http://www.steelfactor.ru/" },
                new Page { ID = 10, Url = "http://gismeteo.ru/" },
                new Page { ID = 11, Url = "http://www.ingate.ru/" },
                new Page { ID = 12, Url = "http://минобрнауки.рф"},
                new Page { ID = 13, Url = "https://www.google.ru/#newwindow=1&q=%D0%BF%D1%83%D1%82%D0%B8%D0%BD"},
                new Page { ID = 14, Url = "https://www.openstat.ru/" },
                new Page { ID = 15, Url = "beliberda"},
                // необходим нормализованный урл, с указанием схемы (http/https), иначе страница должна быть проигнорирована
                new Page { ID = 16, Url = "stackexchange.com/sites" },
                new Page { ID = 17, Url = "http://stackexchange.com/sites" }
            };
        }

        public IEnumerable<Page> GetPages()
        {
            return _pages;
        }

        public void AddReference(Reference reference)
        {
            lock (_references)
                _references.Add(reference);
        }

       
    }
}
