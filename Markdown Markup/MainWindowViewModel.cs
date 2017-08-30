using MarkdownSharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace Markdown_Markup
{
    public class MainWindowViewModel : INotifyPropertyChanged, ISaveableView
    {
        private Markdown _markdown;

        private string _markdownContent;
        private string _cssContent;
        private string _htmlContent;
        private string _htmlRenderContent;

        private string _markdownFile;
        private string _cssFile;
        private bool _markdownEdited;
        private bool _cssEdited;

        private System.Timers.Timer _htmlUpdateTimer = new System.Timers.Timer() { AutoReset = true, Enabled = true, Interval = 2000 };
        public MainWindowViewModel()
        {
            _markdown = new Markdown();
            SaveMarkdownCommand = new DelegateCommand((object p) => { SaveMarkdown(p); }, CanSaveMarkdown) { Gesture = new KeyGesture(Key.S, ModifierKeys.Control) };
            SaveCssCommand = new DelegateCommand((object p) => { SaveCss(p); }, CanSaveCss) { Gesture = new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) };
            SaveGeneratedHtmlCommand = new DelegateCommand(SaveGeneratedHtml, CanSaveGeneratedHtml);
            SaveRenderedHtmlCommand = new DelegateCommand(SaveRenderedHtml, CanSaveRenderedHtml);
            OpenMarkdownCommand = new DelegateCommand(OpenMarkdown, CanOpenMarkdown) { Gesture = new KeyGesture(Key.O, ModifierKeys.Control) };
            OpenCssCommand = new DelegateCommand(OpenCss, CanOpenCss) { Gesture = new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift) };

            _htmlUpdateTimer.Elapsed += _htmlUpdateTimer_Elapsed;
            _htmlUpdateTimer.Start();
        }

        private void _htmlUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateHtml();
        }

        public ICommand SaveMarkdownCommand { get; }
        public ICommand SaveAsMarkdownCommand { get; }
        public ICommand SaveCssCommand { get; }
        public ICommand SaveGeneratedHtmlCommand { get; }
        public ICommand SaveRenderedHtmlCommand { get; }
        public ICommand OpenMarkdownCommand { get; }
        public ICommand OpenCssCommand { get; }

        public string MarkdownFile
        {
            get { return _markdownFile; }
            set
            {
                _markdownFile = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(MarkdownFile)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        public string CssFile
        {
            get { return _cssFile; }
            set
            {
                _cssFile = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CssFile)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CssTitle)));
            }
        }

        public bool MarkdownEdited
        {
            get { return _markdownEdited; }
            set
            {
                _markdownEdited = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(MarkdownEdited)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        public bool MustSave => MarkdownEdited || CssEdited;
        public string UnsavedChanges
        {
            get
            {
                var changes = new List<string>();
                if (MarkdownEdited)
                {
                    changes.Add("Markdown");
                }
                if (CssEdited)
                {
                    changes.Add("CSS");
                }
                return string.Join(", ", changes);
            }
        }

        public bool CssEdited
        {
            get { return _cssEdited; }
            set
            {
                _cssEdited = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CssEdited)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CssTitle)));
            }
        }

        public string Title => "Markdown Markup" + (MarkdownFile?.Length > 0 ? " - " + MarkdownFile : "") + (MarkdownEdited ? "*" : "");
        public string CssTitle => CssFile?.Length > 0 ? "(" + CssFile + (CssEdited ? "*" : "") + ")" : "";

        private bool _htmlNeedsUpdate = false;
        public string MarkdownContent
        {
            get { return _markdownContent; }
            set
            {
                _markdownContent = value;
                MarkdownEdited = true;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(MarkdownContent)));
                _htmlNeedsUpdate = true;
            }
        }

        public string CssContent
        {
            get { return _cssContent; }
            set
            {
                _cssContent = value;
                CssEdited = true;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CssContent)));
                _htmlNeedsUpdate = true;
            }
        }

        public void UpdateHtml()
        {
            if (_htmlNeedsUpdate)
            {
                var html = _markdown.Transform(MarkdownContent);

                HtmlContent = html;

                html = $"<html>\r\n\t<head>\r\n\t\t<style>\r\n\t\t\t{CssContent}\r\n\t\t</style>\r\n\t</head>\r\n\t<body>\r\n\t\t{html}\r\n\t</body>\r\n</html>";

                HtmlRenderContent = html;

                _htmlNeedsUpdate = false;
            }
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

        public bool SaveMarkdown(object parameter)
        {
            if (parameter as string == "SaveAs" || string.IsNullOrEmpty(MarkdownFile))
            {
                var dialog = new SaveFileDialog();
                dialog.AddExtension = true;
                dialog.Filter = "Markdown Files|*.md|All Files|*.*";
                var result = dialog.ShowDialog();

                if (result.Value)
                {
                    MarkdownFile = dialog.FileName;
                }
                else
                {
                    return false;
                }
            }

            using (var sw = new StreamWriter(MarkdownFile))
            {
                sw.Write(MarkdownContent);
            }
            MarkdownEdited = false;

            return true;
        }

        public bool Save()
        {
            return SaveMarkdown(null) & SaveCss(null);
        }

        public bool CanSaveCss(object parameter) => !string.IsNullOrWhiteSpace(CssContent);

        public bool SaveCss(object parameter)
        {
            if (parameter as string == "SaveAs" || string.IsNullOrEmpty(CssFile))
            {
                var dialog = new SaveFileDialog()
                {
                    AddExtension = true,
                    Filter = "CSS Files|*.css|All Files|*.*"
                };

                var result = dialog.ShowDialog();

                if (result.Value)
                {
                    CssFile = dialog.FileName;
                }
                else
                {
                    return false;
                }
            }

            using (var sw = new StreamWriter(CssFile))
            {
                sw.Write(CssContent);
            }
            CssEdited = false;

            return true;
        }

        public bool CanSaveGeneratedHtml(object parameter) => !string.IsNullOrWhiteSpace(HtmlContent);

        public void SaveGeneratedHtml(object parameter)
        {
            var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "HTML Files|*.html|All Files|*.*"
            };

            var result = dialog.ShowDialog();

            if (result.Value)
            {
                using (var sw = new StreamWriter(dialog.FileName))
                {
                    sw.WriteLine(HtmlContent);
                }
            }
        }

        public bool CanSaveRenderedHtml(object parameter) => !string.IsNullOrWhiteSpace(HtmlRenderContent);

        public void SaveRenderedHtml(object parameter)
        {
            var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "HTML Files|*.html|All Files|*.*"
            };

            var result = dialog.ShowDialog();

            if (result.Value)
            {
                using (var sw = new StreamWriter(dialog.FileName))
                {
                    sw.WriteLine(HtmlRenderContent);
                }
            }
        }

        public bool CanOpenMarkdown(object parameter) => true;

        public void OpenMarkdown(object parameter)
        {
            var dialog = new OpenFileDialog()
            {
                AddExtension = true,
                Filter = "Markdown Files|*.md|All Files|*.*"
            };

            var result = dialog.ShowDialog();

            if (result.Value)
            {
                MarkdownFile = dialog.FileName;
                using (var sr = new StreamReader(_markdownFile))
                {
                    MarkdownContent = sr.ReadToEnd();
                }
                MarkdownEdited = false;
            }
        }

        public bool CanOpenCss(object parameter) => true;

        public void OpenCss(object parameter)
        {
            var dialog = new OpenFileDialog()
            {
                AddExtension = true,
                Filter = "CSS Files|*.css|All Files|*.*",
                FileName = CssFile
            };

            var result = dialog.ShowDialog();

            if (result.Value)
            {
                CssFile = dialog.FileName;
                using (var sr = new StreamReader(CssFile))
                {
                    CssContent = sr.ReadToEnd();
                }
                CssEdited = false;
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
