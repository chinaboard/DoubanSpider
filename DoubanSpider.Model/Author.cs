namespace DoubanSpider.Model
{
    public class Author
    {
        private string _name;
        private string _href;
        public string _picUrl;

        public string Name { get { return _name; } set { _name = value == null ? string.Empty : value.Trim(); } }
        public string Href { get { return _href; } set { _href = value == null ? string.Empty : value.Trim(); } }
        public string PicUrl { get { return _picUrl; } set { _picUrl = value == null ? string.Empty : value.Trim(); } }
    }
}
