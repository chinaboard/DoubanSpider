using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DoubanSpider
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
                    Herf = topic.Href,
                    Count = count,
                    LastCommentTime = DateTime.Parse($"{DateTime.Now.ToShortDateString()} {time.InnerHtml.Split(' ').Last()}"),
                    Author = new Author() { Href = author.Href, Name = author.InnerHtml }
                };
            }
        }

        public static void CommentFormat(Topic topic, string htmlText)
        {
            var document = BrowsingContext.New(config).OpenAsync(act => act.Content(htmlText)).Result;
            var topicContent = document.QuerySelector("div.topic-doc");
            var topicTime = DateTime.Parse(topicContent.QuerySelector(".color-green").InnerHtml);
            var topicContext = topicContent.QuerySelector(".topic-content .topic-doc p").OuterHtml;

            topic.PublishTime = topicTime;
            topic.Context = topicContext;


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

                    var context = string.Empty;
                    Comment quoteComment = null;

                    context = commentContextEntity.QuerySelector(".topic-reply li .reply-doc p").OuterHtml;
                    var quoteContextEntity = commentContextEntity.QuerySelector("span.short");
                    var quoteContext = quoteContextEntity == null ? string.Empty : quoteContextEntity.OuterHtml;
                    var quoteAuthorEntity = quoteContextEntity == null ? null : commentContextEntity.Children[1].QuerySelector("span.pubdate").Children.First() as AngleSharp.Dom.Html.IHtmlAnchorElement;
                    if (quoteAuthorEntity != null)
                    {
                        quoteComment = new Comment();
                        quoteComment.Author.Href = quoteAuthorEntity.Href;
                        quoteComment.Author.Name = quoteAuthorEntity.InnerHtml;
                        quoteComment.Context = quoteContext;
                    }




                    var author = new Author() { Href = commentAuthorHref, Name = commentAuthorName, Pic = commentAuthorPic };
                    var comment = new Comment() { Author = author, Context = context, Quote = quoteComment, Time = commentTime };

                    topic.Comments.Add(comment);
                }
                catch (Exception ex)
                {
                    var pc = ex;
                }
            }
        }

        public static IEnumerable<Comment> CommentFormat(Topic topic, IDocument document)
        {
            return null;
        }
    }
}
