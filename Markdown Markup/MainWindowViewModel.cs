using MarkdownSharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Markdown_Markup
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Markdown _markdown;

        private string _markdownContent;
        private string _cssContent;
        private string _htmlContent;
        private string _htmlRenderContent;

        public MainWindowViewModel()
        {
            _markdown = new Markdown();
            SaveMarkdownCommand = new DelegateCommand(SaveMarkdown, CanSaveMarkdown);
            SaveCssCommand = new DelegateCommand(SaveCss, CanSaveCss);
            SaveGeneratedHtmlCommand = new DelegateCommand(SaveGeneratedHtml, CanSaveGeneratedHtml);
            SaveRenderedHtmlCommand = new DelegateCommand(SaveRenderedHtml, CanSaveRenderedHtml);
            OpenMarkdownCommand = new DelegateCommand(OpenMarkdown, CanOpenMarkdown);
            OpenCssCommand = new DelegateCommand(OpenCss, CanOpenCss);
        }
        
        public ICommand SaveMarkdownCommand { get; }
        public ICommand SaveCssCommand { get; }
        public ICommand SaveGeneratedHtmlCommand { get; }
        public ICommand SaveRenderedHtmlCommand { get; }
        public ICommand OpenMarkdownCommand { get; }
        public ICommand OpenCssCommand { get; }

        public string MarkdownContent
        {
            get { return _markdownContent; }
            set
            {
                _markdownContent = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(MarkdownContent)));
                UpdateHtml();
            }
        }

        public string CssContent
        {
            get { return _cssContent; }
            set
            {
                _cssContent = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CssContent)));
                UpdateHtml();
            }
        }

        public void UpdateHtml()
        {
            var html = _markdown.Transform(MarkdownContent);

            HtmlContent = html;

            html = $"<html>\r\n\t<head>\r\n\t\t<style>\r\n\t\t\t{CssContent}\r\n\t\t</style>\r\n\t</head>\r\n\t<body>\r\n\t\t{html}\r\n\t</body>\r\n</html>";

            HtmlRenderContent = html;
        }
        
        public string HtmlContent
        {
            get { return _htmlContent; }
            set
            {
                _htmlContent = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HtmlContent)));
            }
        }
        
        public string HtmlRenderContent
        {
            get { return _htmlRenderContent; }
            set
            {
                _htmlRenderContent = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HtmlRenderContent)));
            }
        }

        public bool CanSaveMarkdown(object parameter) => !string.IsNullOrWhiteSpace(MarkdownContent);

        public void SaveMarkdown(object parameter)
        {
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.Filter = "Markdown Files|*.md|All Files|*.*";
            var result = dialog.ShowDialog();

            if (result.Value)
            {
                // Save the Markdown file
                using (var sw = new StreamWriter(dialog.FileName))
                {
                    sw.WriteLine(MarkdownContent);
                }
            }
        }

        public bool CanSaveCss(object parameter) => !string.IsNullOrWhiteSpace(CssContent);

        public void SaveCss(object parameter)
        {
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.Filter = "CSS Files|*.css|All Files|*.*";
            var result = dialog.ShowDialog();

            if (result.Value)
            {
                // Save the Markdown file
                using (var sw = new StreamWriter(dialog.FileName))
                {
                    sw.WriteLine(CssContent);
                }
            }
        }

        public bool CanSaveGeneratedHtml(object parameter) => !string.IsNullOrWhiteSpace(HtmlContent);

        public void SaveGeneratedHtml(object parameter)
        {
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.Filter = "HTML Files|*.html|All Files|*.*";
            var result = dialog.ShowDialog();

            if (result.Value)
            {
                // Save the Markdown file
                using (var sw = new StreamWriter(dialog.FileName))
                {
                    sw.WriteLine(HtmlContent);
                }
            }
        }

        public bool CanSaveRenderedHtml(object parameter) => !string.IsNullOrWhiteSpace(HtmlRenderContent);

        public void SaveRenderedHtml(object parameter)
        {
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.Filter = "HTML Files|*.html|All Files|*.*";
            var result = dialog.ShowDialog();

            if (result.Value)
            {
                // Save the Markdown file
                using (var sw = new StreamWriter(dialog.FileName))
                {
                    sw.WriteLine(HtmlRenderContent);
                }
            }
        }

        public bool CanOpenMarkdown(object parameter) => true;

        public void OpenMarkdown(object parameter)
        {
            var dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.Filter = "Markdown Files|*.md|All Files|*.*";
            var result = dialog.ShowDialog();

            if (result.Value)
            {
                // Open the Markdown file
                using (var sr = new StreamReader(dialog.FileName))
                {
                    MarkdownContent = sr.ReadToEnd();
                }
            }
        }

        public bool CanOpenCss(object parameter) => true;

        public void OpenCss(object parameter)
        {
            var dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.Filter = "CSS Files|*.css|All Files|*.*";
            var result = dialog.ShowDialog();

            if (result.Value)
            {
                // Open the Markdown file
                using (var sr = new StreamReader(dialog.FileName))
                {
                    CssContent = sr.ReadToEnd();
                }
            }
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
