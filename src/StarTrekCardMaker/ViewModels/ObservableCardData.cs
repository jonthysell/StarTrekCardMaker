// 
// ObservableCardData.cs
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

using StarTrekCardMaker.Models;
using System;
using System.ComponentModel;

namespace StarTrekCardMaker.ViewModels
{
    public abstract class ObservableCardData : GalaSoft.MvvmLight.ObservableObject
    {
        public string Key { get; private set; }

        public virtual string FriendlyKey => EnumUtils.GetFriendlyValue(Key);

        public abstract string Value { get; set; }

        public bool IsEnabled => _isEnabled(Parent);
        private readonly Func<ObservableCard, bool> _isEnabled;

        public readonly ObservableCard Parent;

        #region Creation

        public ObservableCardData(ObservableCard parent, string key, Func<ObservableCard, bool> isEnabled = null) : base()
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _isEnabled = isEnabled ?? ((card) => true);
            PropertyChanged += ObservableCardData_PropertyChanged;
        }

        private void ObservableCardData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Value):
                    Parent.RaisePropertyChanged(Key);
                    Parent.RaisePropertyChanged(nameof(Parent.IsDirty));
                    break;
            }
        }

        #endregion
    }
}
