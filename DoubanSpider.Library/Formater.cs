using AngleSharp;
using AngleSharp.Dom;
using DoubanSpider.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoubanSpider.Library
{
    public static class Formater
    {
        static IConfiguration config = Configuration.Default.WithDefaultLoader();

        public static async Task<IEnumerable<Topic>> TopicFormat(string htmlText)
        {
            var document = await BrowsingContext.New(config).OpenAsync(act => act.Content(htmlText));

            return TopicFormat(document);
        }

        public static IEnumerable<Topic> TopicFormat(IDocument document)
        {
            var topicList = document.QuerySelectorAll(".olt tr").Skip(2);

            foreach (var tp in topicList)
            {
                var topic = tp.Children[0].Children.First() as AngleSharp.Dom.Html.IHtmlAnchorElement;
                var author = tp.Children[1].Children.First() as AngleSharp.Dom.Html.IHtmlAnchorElement;
                var count = tp.Children[2].InnerHtml;
                var time = tp.Children[3];
                yield return new Topic()
                {
                    Title = topic.InnerHtml,
                    Href = topic.Href,
                    Count = count,
                    LastCommentTime = DateTime.Parse($"{DateTime.Now.ToShortDateString()} {time.InnerHtml.Split(' ').Last()}"),
                    Author = new Author() { Href = author.Href, Name = author.InnerHtml }
                };
            }
        }

        public static void TopicFormat(this Topic topic, string htmlText)
        {
            var document = BrowsingContext.New(config).OpenAsync(act => act.Content(htmlText)).Result;
            var topicContent = document.QuerySelector("div.topic-doc");
            var topicTime = DateTime.Parse(topicContent.QuerySelector(".color-green").InnerHtml);
            var topicContextEntity = topicContent.QuerySelector("div.topic-content");
            var topicContext = topicContextEntity == null ? string.Empty : topicContextEntity.InnerHtml;
            topic.PublishTime = topicTime;
            topic.Context = topicContext;
        }

        public static string CommentFormat(this Topic topic, string htmlText)
        {


            var nextPageUrl = string.Empty;

            if (string.IsNullOrWhiteSpace(htmlText))
            {
                return nextPageUrl;
            }

            var document = BrowsingContext.New(config).OpenAsync(act => act.Content(htmlText)).Result;


            var commentsList = document.QuerySelector("ul#comments.topic-reply");
            foreach (var commentEntity in commentsList.Children)
            {
                try
                {
                    var commentAuthorEntity = commentEntity.QuerySelectorAll("a:link").First() as AngleSharp.Dom.Html.IHtmlAnchorElement;

                    var commentAuthorPicEntity = commentAuthorEntity.Children.Count() == 0 ? null : commentAuthorEntity.Children.First() as AngleSharp.Dom.Html.IHtmlImageElement;
                    var commentAuthorName = commentAuthorPicEntity == null ? commentAuthorEntity.InnerHtml : commentAuthorPicEntity.AlternativeText;
                    var commentAuthorPic = commentAuthorPicEntity == null ? string.Empty : commentAuthorPicEntity.Source;
                    var commentAuthorHref = commentAuthorEntity.Href;
                    var commentTime = DateTime.Parse(commentEntity.QuerySelector(".reply-doc .pubtime").InnerHtml);
                    var commentContextEntity = commentEntity.QuerySelector(".topic-reply li .reply-doc");

                    var context = commentContextEntity.QuerySelector(".topic-reply li .reply-doc p").InnerHtml;
                    var quoteContextEntity = commentContextEntity.QuerySelector("span.short");
                    var quoteContext = quoteContextEntity == null ? string.Empty : quoteContextEntity.InnerHtml;
                    var quoteAuthorEntity = quoteContextEntity == null ? null : commentContextEntity.Children[1].QuerySelector("span.pubdate").Children.First() as AngleSharp.Dom.Html.IHtmlAnchorElement;

                    Comment quoteComment = null;
                    if (quoteAuthorEntity != null)
                    {
                        quoteComment = new Comment() { Context = quoteContext };
                        quoteComment.Author.Href = quoteAuthorEntity.Href;
                        quoteComment.Author.Name = quoteAuthorEntity.InnerHtml;
                    }

                    var author = new Author() { Href = commentAuthorHref, Name = commentAuthorName, PicUrl = commentAuthorPic };
                    var comment = new Comment() { Author = author, Context = context, Quote = quoteComment, Time = commentTime };

                    topic.Comments.Add(comment);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            var paginatorEntity = document.QuerySelector("div.paginator");
            if (paginatorEntity != null)
            {
                var nextPageEntity = paginatorEntity.QuerySelector("span.next").QuerySelector("a") as AngleSharp.Dom.Html.IHtmlAnchorElement;
                nextPageUrl = nextPageEntity == null ? string.Empty : nextPageEntity.Href;
            }
            return nextPageUrl;
        }
    }
}
