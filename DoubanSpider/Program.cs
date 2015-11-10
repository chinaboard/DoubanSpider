using DoubanSpider.Library;
using System;
using System.Diagnostics;
using System.Threading;

namespace DoubanSpider
{
    class Program
    {
        static Random rand = new Random();
        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var str = string.Empty;

            Test();
            //while (true)
            //{

            //    var proxy = ProxyPool.GetProxy();
            //    var address = proxy == null ? "null" : proxy.Address.ToString();
            //    var html = HttpHelper.Get("http://www.baidu.com", proxy: proxy);
            //    Console.WriteLine(DateTime.Now + "\t" + address + "\t" + html.Length);
            //    Thread.Sleep(5000);
            //}
            Console.WriteLine(sw.ElapsedMilliseconds + "ms");
            Console.Read();
        }

        static async void Test()
        {
            var rootUrl = "http://www.douban.com/group/asshole/discussion";
            var rootHtml = HttpHelper.Get(rootUrl, proxy: ProxyPool.GetProxy());
            var refererUrl = rootUrl;
            var topicList = await Formater.TopicFormat(rootHtml);
            int i = 1;
            foreach (var topic in topicList)
            {
                Thread.Sleep(rand.Next(100, 400));
                try
                {
                    var commentsHtml = HttpHelper.Get(topic.Href, proxy: ProxyPool.GetProxy());
                    refererUrl = topic.Href;
                    if (string.IsNullOrWhiteSpace(commentsHtml))
                        continue;
                    topic.TopicFormat(commentsHtml);
                    var nextUrl = topic.CommentFormat(commentsHtml);
                    while (!string.IsNullOrWhiteSpace(nextUrl))
                    {
                        commentsHtml = HttpHelper.Get(nextUrl, referer: refererUrl, proxy: ProxyPool.GetProxy());
                        refererUrl = nextUrl;
                        nextUrl = topic.CommentFormat(commentsHtml);
                    }
                    Console.WriteLine($"{i++}\t{topic.PageCount}\t{topic.Comments.Count}\t{topic.Title}");
                    //topic.Comments.ForEach(p =>
                    //{
                    //    if (p.Quote != null)
                    //        Console.WriteLine($"quote {p.Quote.Author.Name}\t{p.Quote.Context}");
                    //    Console.WriteLine($"{p.Author.Name}\t{p.Context}\r\n");
                    //});
                }
                catch (Exception ex)
                {
                    Console.Write("error\r\n" + ex);
                }
            }


        }

    }
}
