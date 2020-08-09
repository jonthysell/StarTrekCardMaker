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

        public bool DebugMode => AppVM.DebugMode;

        public bool CursorMode => AppVM.CursorMode;

        public string CursorInfo
        {
            get
            {
                return $"[ {_cursorInfo} ]";
            }
            set
            {
                _cursorInfo = value;
                RaisePropertyChanged();
            }
        }
        private string _cursorInfo;

        public bool ShowMenu => AppInfo.IsWindows || AppInfo.IsLinux;

        public bool ValidConfig => AppVM.ValidConfig;

        #endregion

        #region Commands

        public RelayCommand NewCard
        {
            get
            {
                return _newCard ??= new RelayCommand(() =>
                {
                    try
                    {
                        if (TryCloseCard(() => NewCard.Execute(null)))
                        {
                            Card = ObservableCard.NewCard();
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }, () => ValidConfig);
            }
        }
        private RelayCommand _newCard;

        public RelayCommand OpenCard
        {
            get
            {
                return _openCard ??= new RelayCommand(() =>
                {
                    try
                    {
                        if (TryCloseCard(() => OpenCard.Execute(null)))
                        {
                            Messenger.Default.Send(new OpenFileMessage("Open Card", FileType.CardXml, Card?.FileName, (filename) =>
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
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }, () => ValidConfig);
            }
        }
        private RelayCommand _openCard;

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

        public RelayCommand ToggleDebugMode
        {
            get
            {
                return _toggleDebugMode ??= new RelayCommand(() =>
                {
                    try
                    {
                        AppVM.DebugMode = !AppVM.DebugMode;
                        RaisePropertyChanged(nameof(DebugMode));
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }, () => ValidConfig);
            }
        }
        private RelayCommand _toggleDebugMode;

        public RelayCommand ToggleCursorMode
        {
            get
            {
                return _toggleCursorMode ??= new RelayCommand(() =>
                {
                    try
                    {
                        AppVM.CursorMode = !AppVM.CursorMode;
                        RaisePropertyChanged(nameof(CursorMode));
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }, () => ValidConfig);
            }
        }
        private RelayCommand _toggleCursorMode;

        public RelayCommand ShowAbout => AppVM.ShowAbout;

        public RelayCommand CheckForUpdates => AppVM.CheckForUpdates;

        #endregion

        public event EventHandler RenderCard;

        private bool _canClose = false;

        public MainViewModel() : base()
        {
            PropertyChanged += MainViewModel_PropertyChanged;
        }

        public bool TryClose()
        {
            return TryCloseCard(RequestClose);
        }

        private bool TryCloseCard(Action callback)
        {
            if (_canClose || !CardLoaded || (CardLoaded && !Card.IsDirty))
            {
                _canClose = false;
                return true;
            }

            Messenger.Default.Send(new ConfirmationMessage("Your card has unsaved changes. Would you like to save first?", (result) =>
            {
                try
                {
                    switch (result)
                    {
                        case ConfirmationResult.No:
                            _canClose = true;
                            callback?.Invoke();
                            break;
                        case ConfirmationResult.Yes:
                            Card.TryClose(callback);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionUtils.HandleException(ex);
                }
            }));

            return false;
        }

        private void OnRenderCard()
        {
            RenderCard?.Invoke(this, null);
        }

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DebugMode):
                    OnRenderCard();
                    break;
            }
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
