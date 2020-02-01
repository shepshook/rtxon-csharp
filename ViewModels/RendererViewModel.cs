using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace rtxon.ViewModels
{
    class RendererViewModel
    {
        private WriteableBitmap Bitmap;

        public RendererViewModel()
        {
            Bitmap = new WriteableBitmap(
                500,
                500,
                96,
                96,
                PixelFormats.Rgb24,
                null);
        }

        public Image GetImage()
        {
            Image Image = new Image();

            for (int i = 0; i < Bitmap.Height; i++)
                for (int j = 0; j < Bitmap.Width; j++)

        }
    }
}
