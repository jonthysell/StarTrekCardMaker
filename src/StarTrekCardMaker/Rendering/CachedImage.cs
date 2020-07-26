// 
// CachedImage.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2020 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

using StarTrekCardMaker.Models;

namespace StarTrekCardMaker.Rendering
{
    public class CachedImage
    {
        public static IAssetLoader Assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

        public Uri Source { get; protected set; }

        public double X { get; protected set; }

        public double Y { get; protected set; }

        public IBitmap Bitmap => _bitmap ??= new Bitmap(Source.IsFile ? File.OpenRead(Source.LocalPath) : Assets.Open(Source));
        private IBitmap _bitmap;

        public CachedImage(Uri source, double x, double y)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            X = x;
            Y = y;
        }

        public CachedImage(string source, double x, double y) : this(new Uri(source, UriKind.RelativeOrAbsolute), x, y) { }

        public IControl ToControl(ImageBoxDescriptor imageBoxDescriptor = null)
        {
            var control = new Image()
            {
                Source = Bitmap
            };

            if (null == imageBoxDescriptor)
            {
                control.SetValue(Canvas.LeftProperty, X);
                control.SetValue(Canvas.TopProperty, Y);
            }

            if (null != imageBoxDescriptor)
            {
                control.Width = imageBoxDescriptor.Width;
                control.Height = imageBoxDescriptor.Height;

                control.Stretch = Stretch.Uniform;

                control.SetValue(Canvas.LeftProperty, imageBoxDescriptor.X);
                control.SetValue(Canvas.TopProperty, imageBoxDescriptor.Y);
            }

            return control;
        }
    }
}
