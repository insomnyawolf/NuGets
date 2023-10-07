using System.Reflection;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Extensions.Images
{
    public static class ExtensionsImage
    {
        public static Image ResizeImage(Image image, int? maxHeight = null, int? maxWidth = null, bool keepProportion = true)
        {
            var actualSize = image.Size();

            Size newSize;

            if (keepProportion)
            {
                newSize = CalculateProportionalSize(image.Size(), maxHeight, maxWidth, true);
            }
            else
            {
                newSize = new Size(maxHeight.Value, maxWidth.Value);
            }

            if (actualSize == newSize)
            {
                return image;
            }

            image.Mutate(x => x.Resize(newSize));

            return image;
        }

        private static Size CalculateProportionalSize(Size currentSize, int? maxHeight = null, int? maxWidth = null, bool allowUpscaling = false)
        {
            if (!maxWidth.HasValue)
            {
                maxWidth = 0;
            }

            if (!maxHeight.HasValue)
            {
                maxHeight = 0;
            }

            if (maxHeight == 0 && maxWidth == 0)
            {
                return currentSize;
            }

            // Evita que las imagenes se escalen a un tamaño superior al que tienen a menos que se indique explicitamente
            if (!allowUpscaling && (currentSize.Width < maxWidth || currentSize.Height < maxHeight))
            {
                return currentSize;
            }

            decimal coeficiente;

            if (maxHeight == 0)
            {
                coeficiente = CoeficienteCambio(currentSize.Width, maxWidth.Value);
            }
            else if (maxWidth == 0)
            {
                coeficiente = CoeficienteCambio(currentSize.Height, maxHeight.Value);
            }
            else
            {
                decimal coeficienteFitWidth = CoeficienteCambio(currentSize.Width, maxWidth.Value);
                decimal coeficienteFitHeight = CoeficienteCambio(currentSize.Height, maxHeight.Value);

                coeficiente = coeficienteFitWidth < coeficienteFitHeight ? coeficienteFitWidth : coeficienteFitHeight;
            }

            var finalSize = new Size()
            {
                Height = decimal.ToInt32(coeficiente * currentSize.Height),
                Width = decimal.ToInt32(coeficiente * currentSize.Width),
            };

            // Minimo tamaño lateral para evitar que las imagenes tengan 0px de lado
            // Calculo el coeficiente de cambio del tamaño minimo con un +1
            coeficiente = 0;

            if (finalSize.Height < 1)
            {
                coeficiente = CoeficienteCambio(currentSize.Height, 1);
            }
            else if (finalSize.Width < 1)
            {
                coeficiente = CoeficienteCambio(currentSize.Width, 1);
            }

            if (coeficiente != 0)
            {
                finalSize.Height = decimal.ToInt32(coeficiente * currentSize.Height);
                finalSize.Width = decimal.ToInt32(coeficiente * currentSize.Width);
            }

            return finalSize;
        }

        private static decimal CoeficienteCambio(int valorInicial, int valorFinal)
        {
            return 100M / valorInicial * valorFinal / 100;
        }


        public static FontCollection LoadDefaultFonts()
        {
            var collection = new FontCollection();
            collection.AddSystemFonts();

            var asm = Assembly.GetAssembly(typeof(ExtensionsImage));
            var embedResources = asm.GetManifestResourceNames();

            foreach (var embed in embedResources)
            {
                var stream = asm.GetManifestResourceStream(embed);
                collection.Add(stream);
            }

            return collection;
        }

        public static readonly FontCollection FontCollection = LoadDefaultFonts();

        public static IImageProcessingContext DrawCustomText(this IImageProcessingContext processingContext, string text, string font, Color color)
        {
            if (FontCollection.TryGet(font, out FontFamily FontFamily))
            {
                // family will not be null here
                Font fontObj = FontFamily.CreateFont(12, FontStyle.Italic);

                // TextOptions provides comprehensive customization options support
                TextOptions textOptions = new(fontObj)
                {
                    TabWidth = 4,
                    WrappingLength = 0,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    FallbackFontFamilies = new FontFamily[] { },
                    Origin = new PointF(0, 0),
                    WordBreaking = WordBreaking.Normal,                    
                };

                return processingContext.DrawText(textOptions, text, color);
            }
            return processingContext;
        }
    }
}
