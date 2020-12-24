using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace chemdb_contribute_tool
{
    public class Gugugubox : Window
    {
        public Gugugubox()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Width = 300;
            Height = 60;
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void SmilesDrawer(object s, RoutedEventArgs e)
        {
            Toolbox.SmilesDrawer drawer = new Toolbox.SmilesDrawer();
            drawer.Show();
        }
    }
}
