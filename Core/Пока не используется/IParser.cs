using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp.Dom.Html;

namespace ZaraCut.Core
{
    public interface IParser<T> where T : class
    {
        T Parse(AngleSharp.Dom.Html.IHtmlDocument document);
    }
}
