﻿// 
// MainWindow.xaml.cs
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using StarTrekCardMaker.ViewModels;

namespace StarTrekCardMaker.Views
{
    public class MainWindow : Window
    {
        public MainViewModel VM
        {
            get
            {
                return (MainViewModel)DataContext;
            }
            set
            {
                DataContext = value;
                value.RequestClose = Close;
                value.RenderCard += VM_RenderCard;
            }
        }

        public Grid CardRenderTarget => _cardRenderTarget ??= this.FindControl<Grid>("CardRenderTarget");
        private Grid _cardRenderTarget;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void VM_RenderCard(object sender, EventArgs e)
        {
            CardRenderTarget.Children.Clear();

            var renderedCard = Rendering.CardRenderer.Render(VM.Card);

            if (null != renderedCard)
            {
                CardRenderTarget.Children.Add(renderedCard);
            }
        }
    }
}
