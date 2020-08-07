// 
// DynamicEnum.cs
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
using System.Collections.Generic;
using System.Linq;

namespace StarTrekCardMaker.Models
{
    public class DynamicEnum
    {
        public readonly string Id;

        public readonly string FriendlyId;

        public int Count => _values.Count;

        public IEnumerable<string> Values => _values;
        private readonly List<string> _values;

        public IEnumerable<string> FriendlyValues => _friendlyValues;
        private readonly List<string> _friendlyValues;

        public readonly string DefaultValue;

        public readonly string DefaultFriendlyValue;

        public DynamicEnum(string friendlyId, IEnumerable<string> friendlyValues, bool addNone, bool addCustom)
        {
            FriendlyId = friendlyId ?? throw new ArgumentNullException(nameof(friendlyId));

            if (addNone)
            {
                friendlyValues = NoneEnum.Concat(friendlyValues);
            }

            if (addCustom)
            {
                friendlyValues = friendlyValues.Concat(CustomEnum);
            }

            _friendlyValues = new List<string>(friendlyValues);
            DefaultFriendlyValue = _friendlyValues.FirstOrDefault();

            Id = EnumUtils.GetValue(FriendlyId);
            _values = new List<string>(friendlyValues.Select(v => EnumUtils.GetValue(v)));
            DefaultValue = _values.FirstOrDefault();
        }

        public string GetValue(string friendlyValue)
        {
            int index = Math.Max(0, Math.Min(_friendlyValues.IndexOf(friendlyValue), _values.Count - 1));
            return _values[index];
        }

        public string GetFriendlyValue(string value)
        {
            int index = Math.Max(0, Math.Min(_values.IndexOf(value), _friendlyValues.Count - 1));
            return _friendlyValues[index];
        }

        public const string NoneValue = "None";
        public const string CustomValue = "Custom";

        private static readonly IEnumerable<string> NoneEnum = new[] { NoneValue };
        private static readonly IEnumerable<string> CustomEnum = new[] { CustomValue };
    }
}
