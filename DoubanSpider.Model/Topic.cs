using System;
using System.Collections.Generic;

namespace DoubanSpider.Model
{
    public class Topic
    {
        private string _title;
        private string _herf;
        public string _context;

        public string Title { get { return _title; } set { _title = value == null ? string.Empty : value.Trim(); } }
        public string Href { get { return _herf; } set { _herf = value == null ? string.Empty : value.Trim(); } }
        public Author Author { get; set; }
        public string Count { get; set; }
        public string Context { get { return _context; } set { _context = value == null ? string.Empty : value.Trim(); } }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public DateTime PublishTime { get; set; }
        public DateTime LastCommentTime { get; set; }
        public ushort PageCount { get { return (ushort)((Comments.Count + 99) / 100.0); } }

    }
}
