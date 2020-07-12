// 
// ObservableObject.cs
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

using StarTrekCardMaker.Models;

namespace StarTrekCardMaker.ViewModels
{
    public delegate void IsDirtyChangedEventHandler(object sender, EventArgs e);

    public abstract class ObservableObject<T> : GalaSoft.MvvmLight.ObservableObject, IEquatable<ObservableObject<T>> where T : IEquatable<T>, ICloneable<T>
    {
        #region Properties

        public virtual bool IsDirty => !InternalObject.Equals(OriginalInternalObject);

        #endregion

        internal T InternalObject { get; private set; }

        internal T OriginalInternalObject { get; private set; }

        public event IsDirtyChangedEventHandler IsDirtyChanged;

        #region Creation

        protected ObservableObject(T @object)
        {
            OriginalInternalObject = @object ?? throw new ArgumentNullException(nameof(@object));
            InternalObject = OriginalInternalObject.Clone();

            PropertyChanged += ObservableObject_PropertyChanged;
        }

        private void ObservableObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsDirty))
            {
                IsDirtyChanged?.Invoke(this, null);
            }
        }

        protected static ObservableType Create<ObservableType, InternalType>(InternalType item, Func<InternalType, ObservableType> create, IsDirtyChangedEventHandler onIsDirtyChanged) where ObservableType : ObservableObject<InternalType>, IComparable<ObservableObject<InternalType>>, IEquatable<ObservableObject<InternalType>> where InternalType : IComparable<InternalType>, IEquatable<InternalType>, ICloneable<InternalType>
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));

            }

            if (null == create)
            {
                throw new ArgumentNullException(nameof(create));

            }

            var observableItem = create(item);

            if (null != onIsDirtyChanged)
            {
                observableItem.IsDirtyChanged += onIsDirtyChanged;
            }

            return observableItem;
        }

        protected static ObservableType CreateNew<ObservableType, InternalType>(Func<ObservableType> createNew, IsDirtyChangedEventHandler onIsDirtyChanged) where ObservableType : ObservableObject<InternalType>, IComparable<ObservableObject<InternalType>>, IEquatable<ObservableObject<InternalType>> where InternalType : IComparable<InternalType>, IEquatable<InternalType>, ICloneable<InternalType>
        {
            if (null == createNew)
            {
                throw new ArgumentNullException(nameof(createNew));
            }

            var observableItem = createNew();

            if (null != onIsDirtyChanged)
            {
                observableItem.IsDirtyChanged += onIsDirtyChanged;
            }

            return observableItem;
        }

        protected void SaveToOriginal()
        {
            OriginalInternalObject = InternalObject.Clone();
            RaisePropertyChanged(nameof(IsDirty));
        }

        protected void ReloadFromOriginal()
        {
            InternalObject = OriginalInternalObject.Clone();
            RaisePropertyChanged(nameof(IsDirty));
        }

        #endregion

        public bool Equals(ObservableObject<T> other)
        {
            return InternalObject.Equals(other.InternalObject);
        }
    }
}
