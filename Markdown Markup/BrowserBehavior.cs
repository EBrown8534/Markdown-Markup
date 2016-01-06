using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Markdown_Markup
{
    /// <summary>
    /// Represents a behavior to control WebBrowser binding to an HTML string.
    /// </summary>
    /// <remarks>
    /// Adopted from: http://stackoverflow.com/a/4204350/4564272
    /// </remarks>
    public class BrowserBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
                "Html",
                typeof(string),
                typeof(BrowserBehavior),
                new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d) => (string)d.GetValue(HtmlProperty);

        public static void SetHtml(WebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var webBrowser = dependencyObject as WebBrowser;
            webBrowser?.NavigateToString(e.NewValue as string ?? "&nbsp;");
        }
    }
}
