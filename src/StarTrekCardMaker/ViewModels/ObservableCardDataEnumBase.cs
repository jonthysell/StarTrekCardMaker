// 
// ObservableCardDataEnumBase.cs
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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StarTrekCardMaker.ViewModels
{
    public abstract class ObservableCardDataEnumBase : ObservableCardData
    {
        public abstract ObservableCollection<string> Values { get; }

        public virtual bool IsCustom => false;

        public virtual string CustomValue
        {
            get
            {
                return Parent.InternalObject.GetValue($"{Key}.Custom");
            }
            set
            {
                if (Parent.InternalObject.SetValue($"{Key}.Custom", value))
                {
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCardDataEnumBase(ObservableCard parent, string key, Func<ObservableCard, bool> isEnabled = null) : base(parent, key, isEnabled)
        {
            PropertyChanged += ObservableCardDataEnumBase_PropertyChanged;
        }

        private void ObservableCardDataEnumBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Value):
                    RaisePropertyChanged(nameof(IsCustom));
                    break;
                case nameof(CustomValue):
                    Parent.RaisePropertyChanged(Key);
                    Parent.RaisePropertyChanged(nameof(Parent.IsDirty));
                    break;
            }
        }
    }
}
