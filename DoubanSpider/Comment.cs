using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubanSpider
{
    public class Comment
    {
        public Author Author { get; set; } = new Author();
        public string Context { get; set; }

        public Comment Quote { get; set; }
        public DateTime Time { get; set; }
    }
}
