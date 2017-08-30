using MarkdownSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Markdown_Markup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

        private void renderPreviewBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            // This prevents links in the page from navigating, this also means we cannot call WebBrowser.Navigate for any browsers with this event.
            if (e.Uri != null)
            {
                e.Cancel = true;
            }
        }

        private void renderPreviewBrowser_Navigated(object sender, NavigationEventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ViewModel is ISaveableView model && model.MustSave)
            {
                var result = MessageBox.Show($"You have the following unsaved changes: {model.UnsavedChanges}. Would you like to save them?", "Save Changes", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {
                    e.Cancel = !model.Save();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
