using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restScraptor
{
    class Rest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Desc { get; set; }
        public List<string> Classifications { get; set; }
    }

    public class Class1
    {
        static void Main(string[] args)
        {
            List<Rest> rests = new List<Rest>();

            HtmlWeb hw = new HtmlWeb();
            hw.OverrideEncoding = Encoding.GetEncoding("ISO-8859-8");
            HtmlDocument doc = hw.Load("http://www.rest.co.il/Search.aspx?regions=4&city=172");

            var baseAddress = "//div[@class='item_box']";

            HtmlNodeCollection rest = doc.DocumentNode.SelectNodes(baseAddress);
            var restInfo = rest.ToList();

            foreach (var item in rest)
            {
                var name = item.SelectSingleNode(".//div[@class='name']").SelectSingleNode(".//a").InnerText;
                var desc = item.SelectSingleNode(".//div[@class='descript']").SelectSingleNode(".//a").InnerText;
                var address = item.SelectSingleNode(".//div[@class='contact']").SelectSingleNode(".//a").InnerText;
                var classifications = item.SelectSingleNode(".//div[@class='category']").SelectNodes(".//span").ToDictionary(k => Guid.NewGuid(), v => v.InnerText);

                rests.Add(new Rest() { Name = name, Desc = desc, Address = address, Classifications = classifications.Values.ToList() });
            }

            Console.Read();  
        }
    }
}
