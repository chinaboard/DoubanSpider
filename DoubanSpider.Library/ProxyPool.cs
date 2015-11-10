using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DoubanSpider.Library
{
    public static class ProxyPool
    {
        private static List<string> _proxyList = Config.ProxyList;
        private static Random _rand = new Random();
        private static string _proxyUrl = Config.ProxyUrl;
        private static ActionScheduler _astFlushProxy = new ActionScheduler();
        private static ActionScheduler _astGetProxy = new ActionScheduler();
        private static ActionScheduler _astInsertProxy = new ActionScheduler();
        private static ActionScheduler _astPrintProxy = new ActionScheduler();

        public static ConcurrentDictionary<string, WebProxy> ProxyBag = new ConcurrentDictionary<string, WebProxy>();

        static ProxyPool()
        {
            _astGetProxy.Start(TimeSpan.FromSeconds(5), () => GetProxyList());
            _astInsertProxy.Start(TimeSpan.FromSeconds(5), () => InsertProxy());
            _astFlushProxy.Start(TimeSpan.FromSeconds(30), () => FlushProxy());
            _astPrintProxy.Start(TimeSpan.FromSeconds(30), () => PrintProxy());
        }

        public static WebProxy GetProxy()
        {
            if (ProxyBag.IsEmpty)
                return null;
            try
            {
                var proxyList = ProxyBag.Values.ToList();
                var proxy = proxyList[_rand.Next(0, proxyList.Count)];
                return proxy;
            }
            catch
            {
                return null;
            }
        }


        static void GetProxyList()
        {
            if (string.IsNullOrWhiteSpace(_proxyUrl))
                return;

            var proxyListText = HttpHelper.Get(_proxyUrl);
            if (string.IsNullOrWhiteSpace(proxyListText))
                return;

            var proxyList = proxyListText.Trim().Split(';');
            if (proxyList.Length > 0)
            {
                _proxyList.AddRange(proxyList);
                _proxyList = _proxyList.Distinct().ToList();
            }
        }

        static void InsertProxy()
        {
            if (_proxyList.Count > 0)
            {
                foreach (var proxyAddress in _proxyList)
                {
                    Task.Run(() =>
                    {
                        if (CheckProxy(proxyAddress))
                        {
                            if (ProxyBag.ContainsKey(proxyAddress))
                                return;
                            ProxyBag[proxyAddress] = new WebProxy(proxyAddress);
                        }
                    });
                }
            }
        }

        static void FlushProxy()
        {
            foreach (var proxyAddress in ProxyBag.Keys)
            {
                WebProxy proxy = null;
                if (ProxyBag.TryGetValue(proxyAddress, out proxy))
                {
                    if (!CheckProxy(proxy))
                    {
                        Console.WriteLine($"{DateTime.Now}\tFlushProxy:{proxy.Address}被干掉");
                        ProxyBag.TryRemove(proxyAddress, out proxy);
                    }
                }
            }
        }

        static void PrintProxy()
        {
            ProxyBag.Keys.ToList().ForEach(proxy => Console.WriteLine($"Alive:{proxy}"));
        }

        static bool CheckProxy(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return false;
            return CheckProxy(new WebProxy(address));
        }

        static bool CheckProxy(WebProxy proxy)
        {

            if (proxy == null)
                return false;
            var html = HttpHelper.Get("http://m.baidu.com/", retry: 2, proxy: proxy);
            var result = !string.IsNullOrWhiteSpace(html) && html.Length > 1000;//这里的1k是因为百度首页大概在1kb左右
            //Console.WriteLine($"{DateTime.Now}\tCheck:{proxy.Address}....{result}");
            return result;
        }
    }
}
