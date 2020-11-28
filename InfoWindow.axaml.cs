using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;

namespace chemdb_contribute_tool
{
    public class InfoWindow : Window
    {
        public InfoWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            Width = 300;
            Height = 400;
        }

        public void init(string F, string N, string C, string S, int mol, List<string> vs)
        {
            TextBox text = this.FindControl<TextBox>("text");
            text.Text = string.Join("\n\n", new List<string>{
                "化学式: " + F,
                "名称: " + N,
                "CAS 号: " + (C == "" || C == null ? "未知" : C),
                "分子质量: " + (mol / 10.0).ToString("N1") + " g/mol",
                "SMILES: " + S,
                "贡献者: " + string.Join(", ", vs)
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
