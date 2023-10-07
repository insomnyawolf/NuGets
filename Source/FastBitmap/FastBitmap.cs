using System.Drawing.Imaging;

namespace FastBitmapProcessing
{
    public static class FastBitmapExtension
    {
        public static void FastProcessing(this Bitmap bitmap, Action<FastBitmap> action)
        {
            var rect = new Rectangle(0, 0, bitmap.Width - 1, bitmap.Height - 1);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            var fastBitmap = new FastBitmap(bitmapData);

            action(fastBitmap);

            bitmap.UnlockBits(bitmapData);
        }
    }

    public unsafe class FastBitmap
    {
        private readonly BitmapData BitmapData;
        private readonly int* PixelArrayBeginning;

        public int Height => BitmapData.Height;
        public int Width => BitmapData.Width;

        public Color this[int index]
        {
            get => GetPixel(index);
            set => SetPixel(index, value);
        }

        public Color this[int x, int y]
        {
            get => GetPixel(x, y);
            set => SetPixel(x, y, value);
        }

        public FastBitmap(BitmapData bitmapData)
        {
            this.BitmapData = bitmapData;
            this.PixelArrayBeginning = (int*)BitmapData.Scan0.ToPointer();
        }

        private int GetIndexFromCoordinates(int x, int y)
        {
            return y * Width + x;
        }

        private int* GetPixelPointer(int pixelIndex)
        {
            // * BytePerPixel // Black magic (?)
            return PixelArrayBeginning + pixelIndex;
        }

        public Color GetPixel(int x, int y)
        {
            var index = GetIndexFromCoordinates(x, y);
            return GetPixel(index);
        }

        public Color GetPixel(int pixelIndex)
        {
            var pointer = GetPixelPointer(pixelIndex);
            var color = Color.FromArgb(*pointer);
            return color;
        }

        public void SetPixel(int x, int y, Color color)
        {
            var pixelIndex = GetIndexFromCoordinates(x, y);
            SetPixel(pixelIndex, color);
        }

        public void SetPixel(int pixelIndex, Color color)
        {
            var pointer = GetPixelPointer(pixelIndex);
            var col = color.ToArgb();
            *pointer = col;
        }
    }
}