// 
// ObservableCard.cs
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
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using StarTrekCardMaker.Models;

namespace StarTrekCardMaker.ViewModels
{
    public class ObservableCard : ObservableObject<Card>
    {
        public AppViewModel AppVM => AppViewModel.Instance;

        #region Properties

        public string FileName
        {
            get
            {
                return _fileName;
            }
            private set
            {
                _fileName = value?.Trim() ?? "";
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsDirty));
            }
        }
        private string _fileName = "";

        public override bool IsDirty => _newCardDirty || base.IsDirty;

        public ObservableCollection<ObservableCardData> Data => _data ?? (_data = GetData());
        private ObservableCollection<ObservableCardData> _data = null;

        #endregion

        #region Commands

        public RelayCommand Save
        {
            get
            {
                return _save ?? (_save = new RelayCommand(() =>
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(FileName))
                        {
                            TrySaveAs();
                        }
                        else
                        {
                            TrySave();
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _save;

        public RelayCommand SaveAs
        {
            get
            {
                return _saveAs ?? (_saveAs = new RelayCommand(() =>
                {
                    try
                    {
                        TrySaveAs();
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _saveAs;

        public RelayCommand Export
        {
            get
            {
                return _export ?? (_export = new RelayCommand(() =>
                 {
                     Messenger.Default.Send(new SaveFileMessage("Export Card", FileType.ExportedImage, FileName, (filename) =>
                     {
                         try
                         {
                             if (!string.IsNullOrWhiteSpace(filename))
                             {
                                 Rendering.CardRenderer.RenderToFile(this, filename);
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
        private RelayCommand _export;

        #endregion

        private bool _newCardDirty = false;

        #region Creation

        private ObservableCard() : this(new Card())
        {
            _newCardDirty = true;
        }

        private ObservableCard(Card Card) : base(Card)
        {
            PropertyChanged += ObservableCard_PropertyChanged;
        }

        private void ObservableCard_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Edition":
                    _data = null;
                    RaisePropertyChanged(nameof(Data));
                    break;
                default:
                    Data.ToList().ForEach(ocd => ocd.RaisePropertyChanged(nameof(ObservableCardData.IsEnabled)));
                    break;
            }
        }

        public static ObservableCard NewCard()
        {
            return new ObservableCard();
        }

        public static ObservableCard OpenCard(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            filename = Path.GetFullPath(filename);

            using var fs = new FileStream(filename, FileMode.Open);

            return new ObservableCard(Card.LoadXml(fs))
            {
                FileName = filename
            };
        }

        private ObservableCollection<ObservableCardData> GetData()
        {
            var data = new ObservableCollection<ObservableCardData>
            {
                new ObservableCardDataEnum<Edition>(this),
            };

            if (InternalObject.Edition == Edition.FirstEdition)
            {
                data.Add(new ObservableCardDataEnum<CardType>(this));
                data.Add(new ObservableCardDataDynamicEnum(this, AppVM.Configs.GetConfig(Edition.FirstEdition).Enums[Card.BorderKey]));
                data.Add(new ObservableCardDataDynamicEnum(this, AppVM.Configs.GetConfig(Edition.FirstEdition).Enums[Card.PropertyLogoKey]));
                data.Add(new ObservableCardDataDynamicEnum(this, AppVM.Configs.GetConfig(Edition.FirstEdition).Enums[Card.ExpansionIconKey]));
                data.Add(new ObservableCardDataDynamicEnum(this, AppVM.Configs.GetConfig(Edition.FirstEdition).Enums[Card.TypedTextBoxKey], (card) => EnumUtils.IsTypedCard(card.InternalObject.CardType)));
                data.Add(new ObservableCardDataDynamicEnum(this, AppVM.Configs.GetConfig(Edition.FirstEdition).Enums[Card.QTypedTextBoxKey], (card) => EnumUtils.IsQTypedCard(card.InternalObject.CardType)));
                data.Add(new ObservableCardDataText(this, Card.TitleKey));
                data.Add(new ObservableCardDataText(this, Card.LoreKey));
                data.Add(new ObservableCardDataText(this, Card.GametextKey));
            }

            return data;
        }

        #endregion

        private void TrySaveAs()
        {
            Messenger.Default.Send(new SaveFileMessage("Save Card As...", FileType.CardXml, FileName, (filename) =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(filename))
                    {
                        TrySave(filename);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionUtils.HandleException(ex);
                }
            }));
        }

        private void TrySave(string filename = null)
        {
            filename ??= FileName;

            using Stream outputStream = new FileStream(filename, FileMode.Create);

            InternalObject.SaveXml(outputStream);
            _newCardDirty = false;
            FileName = filename;

            SaveToOriginal();
        }
    }
}
