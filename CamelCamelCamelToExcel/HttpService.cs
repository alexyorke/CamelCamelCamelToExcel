using System.Drawing;
using System.Net;

namespace CamelCamelCamelToExcel
{
    internal static class HttpService
    {
        /// <summary>
        ///     Downloads an image file
        /// </summary>
        /// <param name="url">The URL where to get the image from</param>
        /// <returns>An image file</returns>
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