using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfUtils.BitmapGraphics
{
    public class SimpleBitmap
    {
        public SimpleBitmap(int width, int height)
        {
            if (width * height > 0)
            {
                BaseBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            }
        }

        public WriteableBitmap BaseBitmap { get; private set; }

        public uint GetPixel(int x, int y)
        {
            unsafe
            {
                return *(uint*)(((byte*)BaseBitmap.BackBuffer.ToPointer()) + BaseBitmap.BackBufferStride * y + x * 4);
            }
        }

        public void SetPixel(int x, int y, uint c)
        {
            BaseBitmap.Lock();
            SetPixelUnlocked(x, y, c);
            BaseBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            BaseBitmap.Unlock();
        }

        private void SetPixelUnlocked(int x, int y, uint c)
        {
            unsafe
            {
                *(uint*)(((byte*)BaseBitmap.BackBuffer.ToPointer()) + BaseBitmap.BackBufferStride * y + x * 4) = c;
            }
        }

        public void SetPixelUnlocked(int offset, int c)
        {
            unsafe
            {
                *(uint*)(((byte*)BaseBitmap.BackBuffer.ToPointer()) + offset * 4) = (uint)c;
            }
        }

        public Color GetColor(int x, int y)
        {
            var p = BitConverter.GetBytes(GetPixel(x, y));
            return Color.FromArgb(p[3], p[2], p[1], p[0]);
        }

        public void SetColor(int x, int y, Color c, bool perfromLock)
        {
            if (perfromLock)
                SetPixel(x, y, (uint)c.A << 24 | ((uint)c.R << 16) | ((uint)c.G << 8) | c.B);
            else
                SetPixelUnlocked(x, y, (uint)c.A << 24 | ((uint)c.R << 16) | ((uint)c.G << 8) | c.B);
        }
    }

}
