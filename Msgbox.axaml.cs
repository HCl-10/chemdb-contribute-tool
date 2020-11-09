using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace chemdb_contribute_tool
{
    public class Msgbox : Window
    {
        public Msgbox()
        {
            this.InitializeComponent();
    #if DEBUG
            this.AttachDevTools();
#endif
            Width = 200;
            Height = 150;
        }

        public bool exitNormally = false;

        public void init(string s)
        {
            this.FindControl<TextBox>("Msg").Text = s;
        }

        private void YClick(object s, RoutedEventArgs e)
        {
            exitNormally = true;
            Close();
        }

        private void NClick(object s, RoutedEventArgs e)
        {
            exitNormally = false;
            Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
