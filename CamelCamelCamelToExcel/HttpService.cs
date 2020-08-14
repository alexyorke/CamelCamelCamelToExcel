using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace CamelCamelCamelToExcel
{
    internal static class HttpService
    {
        public static string DownloadWebpage(string url)
        {
            using var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                             "Windows NT 5.2; .NET CLR 1.0.3705;)");
            var stream = client.OpenRead(url);
            using var reader =
                new StreamReader(stream ?? throw new InvalidOperationException("Webpage could not be downloaded"));
            return reader.ReadToEnd();
        }

        public static Bitmap DownloadImage(string url)
        {
            using var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                             "Windows NT 5.2; .NET CLR 1.0.3705;)");
            using var stream = client.OpenRead(url);
            return new Bitmap(stream);
        }
    }
}