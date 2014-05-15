using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RefExtractor.Html
{
    public static class HtmlRetriever
    {
        public static async Task<string> GetContent(string uri, CancellationToken cancelToken)
        {
            Uri uriObject = new Uri(uri);

            using(HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(uriObject, cancelToken);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                else
                    return null;                
            }
        }
    }
}
