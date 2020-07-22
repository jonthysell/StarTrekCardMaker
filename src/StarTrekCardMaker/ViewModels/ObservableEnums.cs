// 
// ObservableEnums.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StarTrekCardMaker.ViewModels
{
    public static class ObservableEnums
    {
        public static readonly Dictionary<string, ObservableCollection<string>> EnumCache = new Dictionary<string, ObservableCollection<string>>();

        public static ObservableCollection<string> GetCollection<TEnum>()
        {
            if (!EnumCache.TryGetValue(typeof(TEnum).Name, out var result))
            {
                result = new ObservableCollection<string>(EnumUtils.GetFriendlyValues(Enum.GetNames(typeof(TEnum))));
                EnumCache[typeof(TEnum).Name] = result;
            }

            return result;
        }
    }
}
