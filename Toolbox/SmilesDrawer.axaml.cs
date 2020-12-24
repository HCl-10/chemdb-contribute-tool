using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace chemdb_contribute_tool.Toolbox
{
    public class SmilesDrawer : Window
    {
        public SmilesDrawer()
        {
            this.InitializeComponent();
    #if DEBUG
            this.AttachDevTools();
    #endif
            Width = 500;
            Height = 50;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        Bitmap bitmap, nbitmap;

        private void DoSearch(string s)
        {
            WebResponse response = null;
            string can = "";
            foreach (char ch in s)
            {
                int hex = ch;
                can += "%" + Convert.ToString(hex, 16);
            }
            try
            {
                // 使用 KingDraw 的 API 将 SMILES 转换为图片
                response = WebRequest.Create("http://baike.kingdraw.com/api/smilesToImage?smiles=" + can).GetResponse();
            }
            catch (Exception)
            {
                Height = 85;
                this.FindControl<Avalonia.Controls.Image>("img").IsVisible = false;
                this.FindControl<Button>("save").IsVisible = false;
                this.FindControl<TextBlock>("err").IsVisible = false;
                this.FindControl<TextBlock>("nerr").IsVisible = true;
                return;
            }
            try
            {
                Stream stream = response.GetResponseStream();
                System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                bitmap = new Bitmap(image);
                nbitmap = (Bitmap)bitmap.Clone();
                if (image.Width > Width - 20) bitmap = InfoWindow.ZoomImage(bitmap, (int)(image.Height * (Width - 20) / image.Width), (int)(Width - 20));
                Color clr;
                for (int x = 0; x < bitmap.Width; ++x)
                    for (int y = 0; y < bitmap.Height; ++y)
                    {
                        clr = bitmap.GetPixel(x, y);
                        bitmap.SetPixel(x, y, Color.FromArgb(Math.Max(40, 255 - clr.R), Math.Max(40, 255 - clr.G), Math.Max(40, 255 - clr.B)));
                    }
                Stream str = new MemoryStream();
                bitmap.Save(str, ImageFormat.Png);
                str.Position = 0;
                Avalonia.Media.Imaging.Bitmap bitmap1 = new Avalonia.Media.Imaging.Bitmap(str);
                this.FindControl<Avalonia.Controls.Image>("img").Source = bitmap1;
                Height = 50;
                Height += bitmap1.Size.Height + 50;
                this.FindControl<Avalonia.Controls.Image>("img").Height = bitmap1.Size.Height;
                this.FindControl<Avalonia.Controls.Image>("img").Width = bitmap1.Size.Width;
                this.FindControl<Avalonia.Controls.Image>("img").IsVisible = true;
                this.FindControl<Button>("save").IsVisible = true;
                this.FindControl<TextBlock>("err").IsVisible = false;
                this.FindControl<TextBlock>("nerr").IsVisible = false;
            } catch(Exception)
            {
                Height = 85;
                this.FindControl<Avalonia.Controls.Image>("img").IsVisible = false;
                this.FindControl<Button>("save").IsVisible = false;
                this.FindControl<TextBlock>("err").IsVisible = true;
                this.FindControl<TextBlock>("nerr").IsVisible = false;
            }
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            string s = this.FindControl<TextBox>("Searchbox").Text;
            if (s == null) s = "";
            DoSearch(s);
        }

        private void SearchChanged(object sender, KeyEventArgs e)
        {
            string s = this.FindControl<TextBox>("Searchbox").Text;
            if (s == null) return;
            if (s.Contains('\n') || s.Contains('\r'))
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
            filter.Extensions.Add("png");
            filter.Name = "PNG 图片文件";
            dialog.Filters.Add(filter);
            string filename = await dialog.ShowAsync(this);
            if (filename == null) return;
            nbitmap.Save(filename, ImageFormat.Png);
        }
    }
}
