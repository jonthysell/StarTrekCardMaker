// 
// MainViewModel.cs
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
using System.Text;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace StarTrekCardMaker.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties

        public ObservableCard Card
        {
            get
            {
                return _card;
            }
            private set
            {
                if (_card != null)
                {
                    _card.PropertyChanged -= Card_PropertyChanged;
                }

                if (value != null)
                {
                    value.PropertyChanged += Card_PropertyChanged;
                }

                _card = value;

                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(CardLoaded));
                OnRenderCard();
            }
        }
        private ObservableCard _card;

        public override string Title
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (null != Card)
                {
                    if (Card.IsDirty)
                    {
                        sb.Append("*");
                    }

                    if (!string.IsNullOrWhiteSpace(Card.FileName))
                    {
                        sb.Append(Card.FileName);
                    }
                    else
                    {
                        sb.Append("Untitled");
                    }

                    sb.Append(" - ");
                }

                sb.Append("Star Trek Card Maker");

                return sb.ToString();
            }
        }

        public bool CardLoaded => null != Card;

        #endregion

        #region Commands

        public RelayCommand NewCard
        {
            get
            {
                return _newCard ?? (_newCard = new RelayCommand(() =>
                {
                    try
                    {
                        Card = ObservableCard.NewCard();
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _newCard;

        public RelayCommand OpenCard
        {
            get
            {
                return _openCard ?? (_openCard = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new OpenFileMessage("Open Card", Card?.FileName, (filename) =>
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(filename))
                            {
                                Card = ObservableCard.OpenCard(filename);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionUtils.HandleException(ex);
                        }
                    }));
                }));
            }
        }
        private RelayCommand _openCard;

        public RelayCommand Close
        {
            get
            {
                return _close ?? (_close = new RelayCommand(() =>
                {
                    try
                    {
                        RequestClose?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _close;

        public event EventHandler RenderCard;

        #endregion

        public MainViewModel() : base() { }

        private void OnRenderCard()
        {
            RenderCard?.Invoke(this, null);
        }

        private void Card_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Card.FileName):
                case nameof(Card.IsDirty):
                    RaisePropertyChanged(nameof(Title));
                    break;
                default:
                    OnRenderCard();
                    break;
            }
        }
    }
}
