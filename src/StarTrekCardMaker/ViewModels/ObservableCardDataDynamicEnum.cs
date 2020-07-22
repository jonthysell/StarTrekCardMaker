// 
// ObservableCardDataDynamicEnum.cs
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

using StarTrekCardMaker.Models;

namespace StarTrekCardMaker.ViewModels
{
    public class ObservableCardDataDynamicEnum : ObservableCardDataEnumBase
    {
        public override string Value
        {
            get
            {
                return DynamicEnum.GetFriendlyValue(Parent.InternalObject.GetValue(Key, DynamicEnum.DefaultValue));
            }
            set
            {
                if (Parent.InternalObject.SetValue(Key, DynamicEnum.GetValue(value)))
                {
                    RaisePropertyChanged();
                }
            }
        }

        public override ObservableCollection<string> Values
        {
            get
            {
                if (!ObservableEnums.EnumCache.TryGetValue(Key, out ObservableCollection<string> values))
                {
                    values = new ObservableCollection<string>(DynamicEnum.FriendlyValues);
                    ObservableEnums.EnumCache[Key] = values;
                }
                return values;
            }
        }

        public DynamicEnum DynamicEnum { get; private set; }

        #region Creation

        public ObservableCardDataDynamicEnum(ObservableCard parent, string key, DynamicEnum dynamicEnum, Func<ObservableCard, bool> isEnabled = null) : base(parent, key, isEnabled)
        {
            DynamicEnum = dynamicEnum ?? throw new ArgumentNullException(nameof(dynamicEnum));
        }

        public ObservableCardDataDynamicEnum(ObservableCard parent, DynamicEnum dynamicEnum, Func<ObservableCard, bool> isEnabled = null) : this(parent, dynamicEnum?.Id, dynamicEnum, isEnabled) { }

        #endregion
    }
}
