using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using SharpDX.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

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
                string username = System.Text.Encoding.UTF8.GetString(vs);
                Button Username = this.FindControl<Button>("Username");
                Username.Content = username;
                this.FindControl<Button>("AddButton").IsEnabled = true;
                this.FindControl<Button>("EditButton").IsEnabled = true;
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
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void AboutClick(object s, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            // await about.ShowDialog(this);
            about.Show();
        }



        private void WindowClosing(object s, CancelEventArgs e)
        {
        }
    }
}
