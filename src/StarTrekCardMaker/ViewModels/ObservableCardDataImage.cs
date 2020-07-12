// 
// ObservableCardDataImage.cs
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

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using StarTrekCardMaker.Models;

namespace StarTrekCardMaker.ViewModels
{
    public class ObservableCardDataImage : ObservableCardData
    {
        public override string Value
        {
            get
            {
                return Parent.InternalObject.GetValue(Key);
            }
            set
            {
                if (Parent.InternalObject.SetValue(Key, value))
                {
                    RaisePropertyChanged();
                }
            }
        }

        public RelayCommand OpenImage
        {
            get
            {
                return _openImage ??= new RelayCommand(() =>
                {
                    Messenger.Default.Send(new OpenFileMessage("Open Image", FileType.ImportedImage, null, (filename) =>
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(filename))
                            {
                                Value = Base64Utils.ReadFileToBase64(filename);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionUtils.HandleException(ex);
                        }
                    }));
                });
            }
        }
        private RelayCommand _openImage;

        public RelayCommand ClearImage
        {
            get
            {
                return _clearImage ??= new RelayCommand(() =>
                {
                    Value = "";
                });
            }
        }
        private RelayCommand _clearImage;

        #region Creation

        public ObservableCardDataImage(ObservableCard parent, string key, Func<ObservableCard, bool> isEnabled = null) : base(parent, key, isEnabled) { }

        #endregion
    }
}
