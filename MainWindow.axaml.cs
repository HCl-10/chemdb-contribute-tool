using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using chemdb_contribute_tool.InputPanels;
using SharpDX.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace chemdb_contribute_tool
{
    public class MainWindow : Window
    {
        bool SignedIn = false;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            // Change window width and height
            Width = 600;
            Height = 400;

            string username = "";

            // Read user info
            try
            {
                Stream stream = File.Open(".chemdbUser", FileMode.Open, FileAccess.Read, FileShare.Read);
                List<char> list = new List<char>();
                while (true)
                {
                    int b = stream.ReadByte();
                    if (b == -1) break;
                    list.Add((char)b);
                }
                byte[] vs = Convert.FromBase64String(string.Join("", list));
                username = System.Text.Encoding.UTF8.GetString(vs);
                Button Username = this.FindControl<Button>("Username");
                Username.Content = username;
                this.FindControl<Button>("AddButton").IsEnabled = true;
                this.FindControl<Button>("StatButton").IsEnabled = true;
                stream.Close();
                SignedIn = true;
            }
            catch (Exception) { }

            // Initialize AtomDB
            AtomDB.init();

            // Initialize Database
            Restore.db.Read("Database.db");
            if (Restore.db.version != "")
            {
                this.FindControl<Button>("UpdateButton").Content =
                    (string)this.FindControl<Button>("UpdateButton").Content + " (当前数据库版本 " + Restore.db.version + ")";
            }
            else
            {
                this.FindControl<Button>("UpdateButton").Content =
                    (string)this.FindControl<Button>("UpdateButton").Content + " (未找到数据库)";
                Restore.db.version = "Initial";
            }

            if(SignedIn)
            {
                Restore.db.Append(Path.Combine("User", username + ".udb"), username);
            }
            this.FindControl<ListBox>("list").Items = Restore.db.GetList();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void AboutClick(object s, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            // await about.ShowDialog(this);
            about.Show();
        }

        private async void LoginClick(object s, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            await window.ShowDialog(this);
            string name = window.name;
            if (!window.exitNormally) return;
            if(!Directory.Exists("User"))
            {
                Directory.CreateDirectory("User");
            }
            if(!File.Exists(Path.Combine("User", name + ".udb")))
            {
                Stream emm = File.Open(Path.Combine("User", name + ".udb"), FileMode.Create, FileAccess.Write, FileShare.Write);
                emm.WriteByte(0);
                emm.Close();
            }
            Restore.db = new Database();
            Restore.db.Read("Database.db");
            Restore.db.Append(Path.Combine("User", name + ".udb"), name);
            Button Username = this.FindControl<Button>("Username");
            Username.Content = name;
            this.FindControl<Button>("AddButton").IsEnabled = true;
            this.FindControl<Button>("StatButton").IsEnabled = true;
            Stream stream = File.Open(".chemdbUser", FileMode.Create, FileAccess.Write, FileShare.Write);
            byte[] bt = System.Text.Encoding.UTF8.GetBytes(name);
            string b64 = Convert.ToBase64String(bt);
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(b64);
            stream.Write(bs, 0, bs.Length); stream.Close();
            this.FindControl<ListBox>("list").Items = Restore.db.GetList();
            SignedIn = true;
        }

        private async void AddClick(object s, RoutedEventArgs e)
        {
            EditPanel panel = new EditPanel();
            await panel.ShowDialog(this);
            if (!panel.exitNormally) return;
            this.FindControl<ListBox>("list").SelectedItem = null;
            // this.FindControl<ListBox>("list").Items = new List<ListBoxItem>().ToImmutableArray();
            this.FindControl<ListBox>("list").Items = Restore.db.Add(panel.F, panel.N, panel.C, panel.S);
            this.FindControl<TextBox>("Searchbox").Text = "";
        }

        private async void EditClick(object s, RoutedEventArgs e)
        {
            if (this.FindControl<ListBox>("list").SelectedItem == null) return;
            Database.Data data = (Database.Data)((ListBoxItem)this.FindControl<ListBox>("list").SelectedItem).DataContext;
            EditPanel panel = new EditPanel();
            panel.Change(data.formula, data.name, data.cas, data.smiles);
            await panel.ShowDialog(this);
            if (!panel.exitNormally) return;
            this.FindControl<ListBox>("list").SelectedItem = null;
            // this.FindControl<ListBox>("list").Items = new List<ListBoxItem>().ToImmutableArray();
            this.FindControl<ListBox>("list").Items = Restore.db.Add(panel.F, panel.N, panel.C, panel.S);
            this.FindControl<TextBox>("Searchbox").Text = "";
        }

        private async void DeleteClick(object s, RoutedEventArgs e)
        {
            Msgbox msgbox = new Msgbox();
            string name = ((Database.Data)((ListBoxItem)this.FindControl<ListBox>("list").SelectedItem).DataContext).formula;
            msgbox.init("将删除 \"" + name + "\"\n是否确认?\n此操作不可撤回!");
            await msgbox.ShowDialog(this);
            if (!msgbox.exitNormally) return;
            this.FindControl<ListBox>("list").SelectedItem = null;
            // this.FindControl<ListBox>("list").Items = new List<ListBoxItem>().ToImmutableArray();
            this.FindControl<ListBox>("list").Items = Restore.db.Delete(name);
            this.FindControl<TextBox>("Searchbox").Text = "";
        }

        private void InfoClick(object s, RoutedEventArgs e)
        {
            Database.Data data = (Database.Data)((ListBoxItem)this.FindControl<ListBox>("list").SelectedItem).DataContext;
            InfoWindow window = new InfoWindow();
            List<string> cont = new List<string>();
            foreach (int i in data.contrib) cont.Add(Database.names[i]);
            window.init(data.formula, data.name, data.cas, data.smiles, data.mol, cont);
            window.Show();
        }

        private void StatClick(object s, RoutedEventArgs e)
        {
            InfoWindow window = new InfoWindow
            {
                Title = "统计"
            };
            int yr = 0, sz = 0, ys = 0, ca = 0, cs = 0;
            long fz = 0;
            foreach(Database.Data data in Restore.db.datas)
            {
                if (data.contrib.Contains(Restore.db.id)) ++yr;
                sz += data.formula.Length + 1;
                sz += System.Text.Encoding.UTF8.GetBytes(data.name).Length + 1;
                sz += data.cas.Length + 1;
                sz += data.smiles.Length + 1;
                fz += data.mol;
                if (!data.mode) ++ys;
                if (data.cas != null && data.cas != "") ++ca;
                if (data.smiles != null && data.smiles != "") ++cs;
            }
            window.FindControl<TextBox>("text").Text = string.Join("\n\n", new List<string>{
                "总数: " + Restore.db.datas.Count.ToString("N0"),
                "已知 CAS 号的分子数: " + ca.ToString("N0"),
                "已知 SMILES 的分子数: " + cs.ToString("N0"),
                "你的贡献: " + yr.ToString("N0"),
                "新增的贡献: " + (Restore.db.datas.Count - ys).ToString("N0"),
                "数据大小: " + sz.ToString("N0"),
                "总分子质量: " + (fz / 10.0).ToString("N1") + " g/mol"
            });
            window.Show();
        }

        private void reselect(object s, SelectionChangedEventArgs e)
        {
            if (this.FindControl<ListBox>("list").SelectedItem == null)
            {
                this.FindControl<Button>("InfoButton").IsEnabled = false;
                this.FindControl<Button>("EditButton").IsEnabled = false;
                this.FindControl<Button>("DeleteButton").IsEnabled = false;
            }
            else
            {
                this.FindControl<Button>("InfoButton").IsEnabled = true;
                if(SignedIn) this.FindControl<Button>("EditButton").IsEnabled = true;
                List<int> ctb = ((Database.Data)((ListBoxItem)this.FindControl<ListBox>("list").SelectedItem).DataContext).contrib;
                if(SignedIn && ctb.Count == 1 && ctb[0] == Restore.db.id) this.FindControl<Button>("DeleteButton").IsEnabled = true;
                else this.FindControl<Button>("DeleteButton").IsEnabled = false;
            }
        }

        private void DoSearch(string s)
        {
            this.FindControl<ListBox>("list").Items = Restore.db.Search(s.Trim());
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            string s = this.FindControl<TextBox>("Searchbox").Text;
            if (s == null || s == "")
            {
                this.FindControl<ListBox>("list").Items = Restore.db.GetList();
                return;
            }
            DoSearch(s);
        }

        private void SearchChanged(object sender, KeyEventArgs e)
        {
            string s = this.FindControl<TextBox>("Searchbox").Text;
            if(s == null || s == "")
            {
                this.FindControl<ListBox>("list").Items = Restore.db.GetList();
                return;
            }
            if(s.Contains('\n') || s.Contains('\r'))
            {
                string t = "";
                for (int i = 0; i < s.Length; ++i)
                    if (s[i] != '\n' && s[i] != '\r')
                        t += s[i];
                this.FindControl<TextBox>("Searchbox").Text = t;
                DoSearch(t);
            }
        }

        private async void ExportClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            FileDialogFilter filter = new FileDialogFilter();
            filter.Extensions.Add("db");
            filter.Name = "数据库文件";
            dialog.Filters.Add(filter);
            string filename = await dialog.ShowAsync(this);
            if (filename == null) return;
            Restore.db.Write(filename);
        }

        private async void UpdateClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // 由于软件的主体用户是中国大陆用户，所以使用 Gitee。
                WebResponse response = WebRequest.Create("https://gitee.com/HCl-10/chemdb/raw/main/Database.db").GetResponse();
                Stream stream = response.GetResponseStream();
                string ver = "";
                int ch = stream.ReadByte();
                while(ch != 0)
                {
                    ver += (char)ch;
                    ch = stream.ReadByte();
                }
                stream.Close();
                if(ver == Restore.db.version)
                {
                    this.FindControl<Button>("UpdateButton").Content = "您的数据库是最新版本";
                    return;
                }
                Msgbox msgbox = new Msgbox();
                msgbox.Title = "更新";
                msgbox.init("您的数据库不是最新版本。\n是否更新?");
                await msgbox.ShowDialog(this);
                if (!msgbox.exitNormally) return;
                bool completed = false;
                response = WebRequest.Create("https://gitee.com/HCl-10/chemdb/raw/main/Database.db").GetResponse();
                BufferedStream buff = new BufferedStream(response.GetResponseStream(), 1048576);
                long Dloaded = 0;
                Thread thread = new Thread(() =>
                {
                    BufferedStream wr = new BufferedStream(File.Open("Database.db", FileMode.Create, FileAccess.Write, FileShare.Write), 1048576);
                    int bt = buff.ReadByte();
                    while(bt != -1)
                    {
                        wr.WriteByte((byte)bt);
                        bt = buff.ReadByte(); ++Dloaded;
                    }
                    wr.Close();
                    buff.Close();
                    completed = true;
                });
                thread.Start();
                while(!completed)
                {
                    this.FindControl<Button>("UpdateButton").Content = "下载中，已下载：" + Dloaded.ToString("N0");
                    Thread.Sleep(100);
                }
                this.FindControl<Button>("UpdateButton").Content = "重启程序以完成更新";
            }
            catch (Exception ex)
            {
                Msgbox msgbox = new Msgbox();
                msgbox.Title = "错误";
                msgbox.init(ex.Message);
                await msgbox.ShowDialog(this);
            }
        }


        private void WindowClosing(object s, CancelEventArgs e)
        {
            if (!SignedIn) return;
            Button Username = this.FindControl<Button>("Username");
            string username = (string)Username.Content;
            Restore.db.SaveUser(Path.Combine("User", username + ".udb"));
        }

        private void Gugugu(object s, RoutedEventArgs e)
        {
            Gugugubox ggb = new Gugugubox();
            ggb.Show();
        }
    }
}
