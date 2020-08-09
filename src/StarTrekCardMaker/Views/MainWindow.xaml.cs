// 
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
using System.ComponentModel;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

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

        public Grid CardRenderTarget => _cardRenderTarget ??= this.FindControl<Grid>(nameof(CardRenderTarget));
        private Grid _cardRenderTarget;

        private Point? _dragStartingPoint = null;

        private Border _lastRectangle = null;

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            CardRenderTarget.PointerMoved += CardRenderTarget_PointerMoved;
            CardRenderTarget.PointerPressed += CardRenderTarget_PointerPressed;
            CardRenderTarget.PointerReleased += CardRenderTarget_PointerReleased;
            CardRenderTarget.PointerLeave += CardRenderTarget_PointerLeave;
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!VM.TryClose())
            {
                e.Cancel = true;
            }
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

        private void CardRenderTarget_PointerMoved(object sender, PointerEventArgs e)
        {
            if (VM.CursorMode && CardRenderTarget.Children.Count > 0 && CardRenderTarget.Children[0] is Canvas renderedCard)
            {
                var currentPoint = e.GetCurrentPoint(renderedCard).Position;

                if (_dragStartingPoint.HasValue)
                {
                    var rect = new Rect(_dragStartingPoint.Value, currentPoint);
                    
                    if (null == _lastRectangle)
                    {
                        _lastRectangle = new Border();
                        _lastRectangle.BorderBrush = Brushes.Red;
                        _lastRectangle.BorderThickness = new Thickness(1);
                        renderedCard.Children.Add(_lastRectangle);
                    }

                    _lastRectangle.SetValue(Canvas.LeftProperty, Math.Min(rect.X, rect.X + rect.Width));
                    _lastRectangle.SetValue(Canvas.TopProperty, Math.Min(rect.Y, rect.Y + rect.Height));
                    _lastRectangle.Width = Math.Abs(rect.Width);
                    _lastRectangle.Height = Math.Abs(rect.Height);

                    VM.CursorInfo = $"X: {rect.X} Y: {rect.Y} W: {rect.Width} H: {rect.Height}";
                }
                else
                {
                    VM.CursorInfo = $"X: {currentPoint.X} Y: {currentPoint.Y}";
                }
            }
        }

        private void CardRenderTarget_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (VM.CursorMode && CardRenderTarget.Children.Count > 0 && CardRenderTarget.Children[0] is Canvas renderedCard)
            {
                _dragStartingPoint = e.GetCurrentPoint(renderedCard).Position;
            }
        }

        private void CardRenderTarget_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            _dragStartingPoint = null;
            if (null != _lastRectangle && CardRenderTarget.Children.Count > 0 && CardRenderTarget.Children[0] is Canvas renderedCard)
            {
                renderedCard.Children.Remove(_lastRectangle);
            }
            _lastRectangle = null;
        }

        private void CardRenderTarget_PointerLeave(object sender, PointerEventArgs e)
        {
            VM.CursorInfo = "";
        }
    }
}
