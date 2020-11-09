using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace chemdb_contribute_tool.InputPanels
{
    public class LoginWindow : Window
    {
        public bool exitNormally = false;
        public string name = "";
        public LoginWindow()
        {
            this.InitializeComponent();
    #if DEBUG
            this.AttachDevTools();
#endif
            Width = 300;
            Height = 120;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void TextChanged(object s, KeyEventArgs e)
        {
            TextBox text = this.FindControl<TextBox>("username");
            string name = text.Text;
            if (name == null) return;
            string finame = "";
            for(int i = 0; i < name.Length; ++i)
            {
                if(name[i] == '\\' || name[i] == '/' || name[i] == ':' || name[i] == '*' || name[i] == '?' || name[i] == '\"' || name[i] == '<' || name[i] == '>' || name[i] == '|' || name[i] < 32)
                {
                    continue;
                }
                finame += name[i];
            }
            if (finame.Length > 100) finame = finame.Substring(0, 100);
            this.FindControl<TextBox>("username").Text = finame;
        }

        private void OKClick(object s, RoutedEventArgs e)
        {
            exitNormally = true;
            name = this.FindControl<TextBox>("username").Text;
            Close();
        }
    }
}
