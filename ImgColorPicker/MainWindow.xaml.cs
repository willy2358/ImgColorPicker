using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace ImgColorPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter =  "image files(*.png *.jpg)|*.png;*.jpg;";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "jpg";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.imgFile.Text = openFileDialog.FileName;
            }
        }

        System.Drawing.Bitmap _curBitmap = null;
        private void imgFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.imgFile.Text.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                || this.imgFile.Text.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                _curBitmap = null;
                this.img.Source = new ImageSourceConverter().ConvertFrom(this.imgFile.Text) as ImageSource;
                string size = string.Format("width:{0}, height:{1}", (int)img.Source.Width, (int)img.Source.Height);
                this.img.Width = img.Source.Width;
                this.img.Height = img.Source.Height;
                this.imgRect.Width = this.img.Width + 2;
                this.imgRect.Height = this.img.Height + 2;
                this.txtImgSize.Text = size;
                this.txtColorValues.Text = "";
                _curBitmap = FromBitmapSourceToBitmap(this.img.Source as BitmapSource);
            }
        }

        void ShowPixelColor()
        {
            int x = 0;
            int y = 0;
            int.TryParse(this.txtX.Text, out x);
            int.TryParse(this.txtY.Text, out y);

            double xOffset = this.targetPixel.Width / 2;
            double yOffset = this.targetPixel.Height / 2;
            this.targetPixel.Margin = new Thickness(x - xOffset, y - yOffset, 0, 0);


            string strColor = "";
            if (_curBitmap != null)
            {
                var color = _curBitmap.GetPixel(x, y);
                strColor = string.Format($"R:{color.R},G:{color.G},B:{color.B},A:{color.A}");
            }

            this.txtColorValues.Text += string.Format($"X:{x},Y:{y},color:{strColor}\n");
        }

        private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                var pt = e.GetPosition(this.img);
                this.txtX.Text = string.Format("{0}", (int)pt.X);
                this.txtY.Text = string.Format("{0}", (int)pt.Y);

                ShowPixelColor();
            }
        }

        public static System.Drawing.Bitmap FromBitmapSourceToBitmap(BitmapSource source)
        {
            if (null == source)
            {
                return null;
            }

            try
            {
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size), ImageLockMode.WriteOnly,
                                                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
                bitmap.UnlockBits(data);

                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void btnPreX_Click(object sender, RoutedEventArgs e)
        {
            string sX = this.txtX.Text;
            int nX = 0;
            int.TryParse(sX, out nX);
            nX -= 1;
            if (nX < 0)
            {
                nX = 0;
            }
            this.txtX.Text = string.Format("{0}", nX);
            ShowPixelColor();
        }

        private void btnNextX_Click(object sender, RoutedEventArgs e)
        {
            string sX = this.txtX.Text;
            int nX = 0;
            int.TryParse(sX, out nX);
            nX += 1;
            if (nX > this.img.Width)
            {
                nX = (int)this.img.Width;
            }
            this.txtX.Text = string.Format("{0}", nX);
            ShowPixelColor();
        }

        private void btnPreY_Click(object sender, RoutedEventArgs e)
        {
            string sY = this.txtY.Text;
            int nY = 0;
            int.TryParse(sY, out nY);
            nY -= 1;
            if (nY < 0)
            {
                nY = 0;
            }
            this.txtY.Text = string.Format("{0}", nY);
            ShowPixelColor();
        }

        private void btnNextY_Click(object sender, RoutedEventArgs e)
        {
            string sY = this.txtY.Text;
            int nY = 0;
            int.TryParse(sY, out nY);
            nY += 1;
            if (nY > this.img.Height)
            {
                nY = (int)this.img.Height;
            }
            this.txtY.Text = string.Format("{0}", nY);
            ShowPixelColor();
        }

        private void txtX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ShowPixelColor();
            }
        }

        private void txtY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ShowPixelColor();
            }
        }

        private void BtnSetIndicatorColor_Click(object sender, RoutedEventArgs e)
        {
            this.colorPicker.IsOpen = !this.colorPicker.IsOpen;
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.colorPicker.SelectedColor.HasValue)
            {
                var brush = new SolidColorBrush(this.colorPicker.SelectedColor.Value);
                this.vertLine1.Stroke = brush;
                this.vertLine2.Stroke = brush;

                this.horzLine1.Stroke = brush;
                this.horzLine2.Stroke = brush;
            }
            
        }
    }
}
