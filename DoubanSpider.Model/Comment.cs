using System;

namespace DoubanSpider.Model
{
    public class Comment
    {
        private string _context;

        public Author Author { get; set; } = new Author();
        public string Context { get { return _context; } set { _context = value == null ? string.Empty : value.Trim(); } }
        public Comment Quote { get; set; }
        public DateTime Time { get; set; }
    }
}