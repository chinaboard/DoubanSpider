using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubanSpider
{
    public class Topic
    {
        public string Title { get; set; }
        public string Herf { get; set; }
        public Author Author { get; set; }
        public string Count { get; set; }

        public string Context { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public DateTime PublishTime { get; set; }
        public DateTime LastCommentTime { get; set; }

    }
}
