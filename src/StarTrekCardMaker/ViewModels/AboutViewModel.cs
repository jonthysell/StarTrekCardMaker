// 
// AboutViewModel.cs
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

namespace StarTrekCardMaker.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        #region Properties

        public override string Title => $"About {AppInfo.Name}";

        public string Text => string.Join(Environment.NewLine + Environment.NewLine,
            $"{AppInfo.Name} v{AppInfo.Version}",
            "Star Trek Card Maker is an application for creating cards for the Star Trek Customizable Card Game.",
            "Image and font assets were borrowed from github.com/makeitTim/McKinleyStation.",
            "Star Trek in all forms is copyright and trademark of CBS Paramount Studios which has no affiliation with this application.",
            AppInfo.MitLicenseName,
            AppInfo.Copyright,
            AppInfo.MitLicenseBody);

        #endregion

        #region Commands

        public RelayCommand Close
        {
            get
            {
                return _close ??= new RelayCommand(() =>
                {
                    try
                    {
                        RequestClose?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                });
            }
        }
        private RelayCommand _close;

        #endregion

        public AboutViewModel() : base()
        {
        }
    }
}
