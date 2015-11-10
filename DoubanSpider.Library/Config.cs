using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DoubanSpider.Library
{
    internal static class Config
    {
        private const string _proxyHost = "61.93.246.50:8080;218.57.200.15:8080;62.180.27.25:80;151.80.195.189:8080;201.243.101.119:8080";
        public static string[] ProxyList { get; private set; }

        public static string ProxyUrl { get; private set; } = string.Empty;
        public static int ThreadNum { get; private set; } = 1;
        public static int TimeLimit { get; private set; } = 1000;

        static Config()
        {
            ProxyList = _proxyHost.Split(';');

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DoubanSpider.config");
            if (!File.Exists(path))
                return;

            var xml = new XMLHelper(path);

            var proxyList = xml.GetValue<string>($"//DoubanSpider//{nameof(ProxyList)}");
            ProxyList = proxyList == null ? _proxyHost.Split(';') : proxyList.Trim().Split(';');
            ProxyUrl = xml.GetValue<string>($"//DoubanSpider//{nameof(ProxyUrl)}") ?? string.Empty;
            ThreadNum = xml.GetValue<int>($"//DoubanSpider//{nameof(ThreadNum)}");
            TimeLimit = xml.GetValue<int>($"//DoubanSpider//{nameof(TimeLimit)}");
        }
    }
}
