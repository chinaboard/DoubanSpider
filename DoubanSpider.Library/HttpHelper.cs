using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace DoubanSpider.Library
{
    public static class HttpHelper
    {

        public static string Get(string url, int timeOut = 1000, int retry = 3, WebProxy proxy = null)
        {
            while (retry-- > 0)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (proxy != null)
                {
                    request.Proxy = proxy;
                }
                request.Timeout = timeOut;
                request.Headers.Add("Accept-Language", "zh-CN");
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Accept-Encoding", "gzip,deflate");
                request.Accept = "text/plain, */*; q=0.01";
                request.Host = "www.douban.com";
                request.Referer = "http://www.douban.com/group/asshole/discussion?start=0";
                request.UserAgent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.0 Chrome/30.0.1599.101 Safari/537.36";

                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        return GetResponseBody(response);
                    }
                }
                catch
                {
                    //do nothing
                }
            }
            return string.Empty;

        }

        private static string GetResponseBody(HttpWebResponse response)
        {

            string responseBody = string.Empty;
            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
            {
                using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            return responseBody;
        }
    }
}
