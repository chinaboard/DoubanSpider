using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DoubanSpider
{
    public static class HttpHelper
    {

        public static string Get(string url,  WebProxy proxy = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            request.Headers.Add("Accept-Language", "zh-CN");
            request.Headers.Add("DNT", "1");
            request.Accept = "text/plain, */*; q=0.01";
            request.Host = "www.douban.com";
            request.Referer = "http://www.douban.com/group/asshole/discussion?start=0";
            request.UserAgent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.0 Chrome/30.0.1599.101 Safari/537.36";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }

            }
        }

        /// <summary>
        /// Deflate解压函数
        /// JS:var details = eval('(' + utf8to16(zip_depress(base64decode(hidEnCode.value))) + ')')对应的C#压缩方法
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static byte[] GZipDecompress(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
                {
                    byte[] bytes = new byte[40960];
                    int n;
                    while ((n = gZipStream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        stream.Write(bytes, 0, n);
                    }
                    gZipStream.Close();
                }
                return stream.ToArray();
            }
        }
    }
}
