using AngleSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

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

            var topicList = await Formater.TopicFormat(rootHtml);
            int i = 1;
            foreach (var topic in topicList)
            {
                Thread.Sleep(rand.Next(100, 400));
                try
                {
                    var commentsHtml = HttpHelper.Get(topic.Href);
                    if (string.IsNullOrWhiteSpace(commentsHtml))
                        continue;

                    Stopwatch sw = Stopwatch.StartNew();

                    topic.TopicFormat(commentsHtml);
                    var nextUrl = topic.CommentFormat(commentsHtml);
                    while (!string.IsNullOrWhiteSpace(nextUrl))
                    {
                        commentsHtml = HttpHelper.Get(nextUrl);
                        nextUrl = topic.CommentFormat(commentsHtml);
                    }
                    Console.WriteLine($"{i++}\t{topic.PageCount}\t{topic.Comments.Count}\tformat:{sw.ElapsedMilliseconds}ms\t{topic.Title}");
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
