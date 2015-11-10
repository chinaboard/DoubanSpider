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

            Console.WriteLine(sw.ElapsedMilliseconds + "ms");
            Console.Read();
        }

        static async void Test()
        {
            var rootUrl = "http://www.douban.com/group/asshole/discussion";
            var rootHtml = HttpHelper.Get(rootUrl);
            var refererUrl = rootUrl;
            var topicList = await Formater.TopicFormat(rootHtml);
            int i = 1;
            foreach (var topic in topicList)
            {
                Thread.Sleep(rand.Next(100, 400));
                try
                {
                    var commentsHtml = HttpHelper.Get(topic.Href);
                    refererUrl = topic.Href;
                    if (string.IsNullOrWhiteSpace(commentsHtml))
                        continue;
                    topic.TopicFormat(commentsHtml);
                    var nextUrl = topic.CommentFormat(commentsHtml);
                    while (!string.IsNullOrWhiteSpace(nextUrl))
                    {
                        commentsHtml = HttpHelper.Get(nextUrl, referer: refererUrl);
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
