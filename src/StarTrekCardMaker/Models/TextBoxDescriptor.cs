// 
// TextBoxDescriptor.cs
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

namespace StarTrekCardMaker.Models
{
    public enum TextBoxAlignment
    {
        left,
        center,
        right,
    };

    public enum TextBoxColor
    {
        black,
        white,
    }

    public class TextBoxDescriptor : BoxDescriptorBase
    {
        public readonly string FontFamily;

        public readonly double FontSize;

        public readonly TextBoxAlignment Alignment;

        public readonly TextBoxColor Color;

        public readonly bool AllCaps;

        public TextBoxDescriptor(string id, string fontFamily, double fontSize, TextBoxAlignment alignment, TextBoxColor color, bool allCaps, double x, double y, double width, double height) : base(id, x, y, width, height)
        {
            FontFamily = fontFamily ?? throw new ArgumentNullException(nameof(fontFamily));

            FontSize = fontSize;

            Alignment = alignment;
            Color = color;

            AllCaps = allCaps;
        }
    }
}
