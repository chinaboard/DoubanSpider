using AngleSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DoubanSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var str = string.Empty;
            Random rand = new Random();



            Test();

            //Console.WriteLine(str.Length + "    " + sw.ElapsedMilliseconds);
            Console.Read();
        }

        static async void Test()
        {
            var rootUrl = "http://www.douban.com/group/asshole/discussion";
            var rootHtml = HttpHelper.Get(rootUrl);

            var topicList = await Formater.TopicFormat(rootHtml);
            var topic = topicList.First();

            Console.WriteLine(topic.Title);

            var commentsHtml = File.ReadAllText(@"z:\topic.html");// HttpHelper.Get(topic.Herf);
            Formater.CommentFormat(topic, commentsHtml);
            Console.WriteLine(topic.Context);
            topic.Comments.ForEach(p =>
            {
                if (p.Quote != null)
                    Console.WriteLine($"quote {p.Quote.Author.Name}\t{p.Quote.Context}");
                Console.WriteLine($"{p.Author.Name}\t{p.Context}\r\n");
            });
        }

    }
}
