using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefExtractor.Processing.TagProcessors
{
    static class UriHelper
    {
        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        public static bool IsLocalUrl(string baseUrl, string url)
        {
            if (!IsAbsoluteUrl(url))
                return true;

            Uri baseUri = new Uri(baseUrl);

            if (url.StartsWith("//"))
                url = baseUri.Scheme + url;

            if (!IsAbsoluteUrl(url))
                return true;

            Uri uri = new Uri(url);

            string baseUrlHost = new Uri(baseUrl).DnsSafeHost;
            string urlHost = new Uri(url).DnsSafeHost;

            if (baseUrlHost.StartsWith("www."))
                baseUrlHost = baseUrlHost.Substring(4);
            if (urlHost.StartsWith("www."))
                urlHost = urlHost.Substring(4);

            return baseUrlHost.Equals(urlHost, StringComparison.InvariantCultureIgnoreCase);

            /*
            return Uri.Compare(baseUri, uri.Uri, UriComponents.Host, UriFormat.UriEscaped, StringComparison.InvariantCultureIgnoreCase) == 0;
             */
        }
    }
}
