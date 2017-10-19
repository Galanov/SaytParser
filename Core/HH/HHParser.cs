using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZaraCut.Core;
using AngleSharp.Dom.Html;

namespace ZaraCut.Core.HH
{
    public class HHParser : IParser<string[]>
    {        public HHParser()
        {
            //anketa = new 
        }
        //public IAnketa Parse(IHtmlDocument document)
        //{
        //    var list = new List<string>();
        //    var items = document.QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("resume-header-main") && item.InnerHtml.Contains("")).FirstOrDefault();
        //    //foreach(var item in items)
        //    //{
        //    //    list.Add(item.TextContent);
        //    //}
        //    return list.ToArray();
        //}

        string[] IParser<string[]>.Parse(IHtmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}