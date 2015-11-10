using System.Linq;
using System.Xml;

namespace DoubanSpider.Library
{
    internal class XMLHelper
    {
        private XmlDocument doc = new XmlDocument();
        public XMLHelper(string xmlFileName)
        {
            doc.Load(xmlFileName); //加载XML文档
        }
        /// <summary>
        /// 获取XML中的标签之间的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">要匹配的Path表达式(例如:"//节点名//子节点名")</param>
        /// <returns>返回值</returns>
        public T GetValue<T>(string path)
        {
            XmlNodeList xmlNodeList = doc.SelectNodes(path);
            if (xmlNodeList.Count == 0)
            {
                return default(T);
            }
            else
            {
                XmlNode node = xmlNodeList[0];
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromString(node.InnerText);
            }
        }
    }
}
