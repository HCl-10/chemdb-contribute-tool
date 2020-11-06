using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;

namespace chemdb_contribute_tool
{
    public class AboutWindow : Window
    {
        public AboutWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            Width = 300;
            Height = 500;

            TextBox text = this.FindControl<TextBox>("text");
            text.Text = string.Join("\n\n", new List<string>{
                "ChemDB 全名 Chemistry Database，是一个化学分子式对应名称的数据库，该数据库中所有的内容均由各化学爱好者上传，数据库基于 GPL3.0 开源，同时提供各大语言的数据库解析工具，以便使用。",
                "为方便大家对 ChemDB 进行贡献，我们制作了 ChemDB Contribute Tool 即 ChemDB 贡献工具，这个软件不仅能方便您贡献数据，还可以快速从数据库中查找数据，是 ChemDB 项目不可缺少的一部分。",
                "软件使用 C# .NET Core 3.1 制作: https://dotnet.microsoft.com/ \n图形界面由 Avalonia UI 强力驱动: http://www.avaloniaui.net/",
                "同时也欢迎大家来 Star 和 Fork ChemDB 这个项目，项目网址如下:\nChemDB: https://github.com/HCl-10/chemdb \nChemDB 贡献工具: https://github.com/HCl-10/chemdb-contribute-tool \n同时欢迎关注我们: https://github.com/HCl-10"
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
