using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

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

        public static Bitmap ZoomImage(Bitmap bitmap, int destHeight, int destWidth)
        {
            // 代码来源于网络
            try
            {
                System.Drawing.Image sourImage = bitmap;
                int width = 0, height = 0;
                int sourWidth = sourImage.Width;
                int sourHeight = sourImage.Height;
                if (sourHeight > destHeight || sourWidth > destWidth)
                {
                    if ((sourWidth * destHeight) > (sourHeight * destWidth))
                    {
                        width = destWidth;
                        height = destWidth * sourHeight / sourWidth;
                    }
                    else
                    {
                        height = destHeight;
                        width = sourWidth * destHeight / sourHeight;
                    }
                }
                else
                {
                    width = sourWidth;
                    height = sourHeight;
                }
                Bitmap destBitmap = new Bitmap(destWidth, destHeight);
                Graphics g = Graphics.FromImage(destBitmap);
                g.Clear(Color.White);
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(sourImage, new Rectangle((destWidth - width) / 2, (destHeight - height) / 2, width, height), 0, 0, sourImage.Width, sourImage.Height, GraphicsUnit.Pixel);
                g.Dispose();
                EncoderParameters encoderParams = new EncoderParameters();
                long[] quality = new long[1];
                quality[0] = 100;
                EncoderParameter encoderParam = new EncoderParameter(Encoder.Quality, quality);
                encoderParams.Param[0] = encoderParam;
                sourImage.Dispose();
                return destBitmap;
            }
            catch
            {
                return bitmap;
            }
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
            string can = "";
            foreach(char ch in S)
            {
                int hex = ch;
                can += "%" + Convert.ToString(hex, 16);
            }
            try
            {
                // 使用 KingDraw 的 API 将 SMILES 转换为图片
                WebResponse response = WebRequest.Create("http://baike.kingdraw.com/api/smilesToImage?smiles=" + can).GetResponse();
                Stream stream = response.GetResponseStream();
                System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                Bitmap bitmap = new Bitmap(image);
                if (image.Width > 280) bitmap = ZoomImage(bitmap, image.Height * 280 / image.Width, 280);
                Color clr;
                for(int x = 0; x < bitmap.Width; ++x)
                    for(int y = 0; y < bitmap.Height; ++y)
                    {
                        clr = bitmap.GetPixel(x, y);
                        bitmap.SetPixel(x, y, Color.FromArgb(Math.Max(40, 255 - clr.R), Math.Max(40, 255 - clr.G), Math.Max(40, 255 - clr.B)));
                    }
                Stream str = new MemoryStream();
                bitmap.Save(str, ImageFormat.Png);
                str.Position = 0;
                Avalonia.Media.Imaging.Bitmap bitmap1 = new Avalonia.Media.Imaging.Bitmap(str);
                this.FindControl<Avalonia.Controls.Image>("img").Source = bitmap1;
                Height += bitmap1.Size.Height;
                this.FindControl<Avalonia.Controls.Image>("img").Height = bitmap1.Size.Height;
                this.FindControl<Avalonia.Controls.Image>("img").Width = bitmap1.Size.Width;
            }
            catch (Exception) { }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
