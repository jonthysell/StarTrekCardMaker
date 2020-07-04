// 
// FirstEditionCardRenderer.cs
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

using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using StarTrekCardMaker.Models;
using StarTrekCardMaker.ViewModels;

namespace StarTrekCardMaker.Rendering
{
    public class FirstEditionCardRenderer : ICardRenderer
    {
        public static AppViewModel AppVM => AppViewModel.Instance;

        public static Config CurrentConfig => _currentConfig ??= AppVM.Configs.GetConfig(Edition.FirstEdition);
        private static Config _currentConfig = null;

        public static double CardWidth => CurrentConfig.Constants["CardWidth"];

        public static double CardHeight => CurrentConfig.Constants["CardHeight"];

        public static double InnerRectMargin => CurrentConfig.Constants["InnerRectMargin"];

        public IControl GetControl(Card card)
        {
            if (null == card)
            {
                throw new ArgumentNullException(nameof(card));
            }

            if (card.Edition != Edition.FirstEdition)
            {
                throw new ArgumentException();
            }

            Canvas target = CreateTarget();

            switch (card.CardType)
            {
                case CardType.Artifact:
                case CardType.DilemmaBoth:
                case CardType.DilemmaPlanet:
                case CardType.DilemmaSpace:
                case CardType.Doorway:
                case CardType.Event:
                case CardType.Equipment:
                case CardType.Incident:
                case CardType.Interrupt:
                case CardType.Objective:
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    RenderTypedCard(target, card);
                    break;
            }

            return target;
        }

        private static Canvas CreateTarget()
        {
            return new Canvas()
            {
                Width = CardWidth,
                Height = CardHeight,
            };
        }

        private static void RenderTypedCard(Canvas target, Card card)
        {
            AddInnerRect(target);

            AddBorder(target, card);

            AddArt(target, card);

            AddInnerBorder(target);

            AddTitleBar(target, card);

            AddTypedTitle(target, card);

            AddProperty(target, card);

            AddArtBorder(target, card);

            AddTextBox(target, card);

            AddText(target, card);

            AddExpansionIcon(target, card);
        }

        private static void AddInnerRect(Canvas target)
        {
            var rect = new Rectangle()
            {
                Width = CardWidth - 2 * InnerRectMargin,
                Height = CardHeight - 2 * InnerRectMargin,
                Fill = Brushes.Black,
            };
            Canvas.SetLeft(rect, InnerRectMargin);
            Canvas.SetTop(rect, InnerRectMargin);
            target.Children.Add(rect);
        }

        private static void AddBorder(Canvas target, Card card)
        {
            AddCachedImageByEnumKey(target, card, Card.BorderKey);
        }

        private static void AddInnerBorder(Canvas target)
        {
            AddCachedImage(target, "InnerBorder");
        }

        private static void AddTitleBar(Canvas target, Card card)
        {
            switch (card.CardType)
            {
                case CardType.Artifact:
                case CardType.DilemmaBoth:
                case CardType.DilemmaPlanet:
                case CardType.DilemmaSpace:
                case CardType.Doorway:
                case CardType.Event:
                case CardType.Equipment:
                case CardType.Incident:
                case CardType.Interrupt:
                case CardType.Objective:
                case CardType.Tactic:
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    AddCachedImage(target, "TypedTitleBar");
                    break;
            }
        }

        private static void AddTypedTitle(Canvas target, Card card)
        {
            switch (card.CardType)
            {
                case CardType.Artifact:
                case CardType.DilemmaBoth:
                case CardType.DilemmaPlanet:
                case CardType.DilemmaSpace:
                case CardType.Doorway:
                case CardType.Event:
                case CardType.Equipment:
                case CardType.Incident:
                case CardType.Interrupt:
                case CardType.Objective:
                case CardType.Tactic:
                    AddCachedImage(target, $"TypedTitle.{card.CardType}");
                    break;
                case CardType.QDilemma:
                    AddCachedImage(target, "TypedTitle.DilemmaBoth");
                    AddCachedImage(target, "TypedTitle.Q");
                    break;
                case CardType.QArtifact:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    AddCachedImage(target, $"TypedTitle.{card.CardType.ToString().Substring(1)}");
                    AddCachedImage(target, "TypedTitle.Q");
                    break;
            }
        }

        private static void AddProperty(Canvas target, Card card)
        {
            AddCachedImageByEnumKey(target, card, Card.PropertyLogoKey);
        }

        private static void AddArt(Canvas target, Card card)
        {
            switch (card.CardType)
            {
                case CardType.Artifact:
                case CardType.DilemmaBoth:
                case CardType.DilemmaPlanet:
                case CardType.DilemmaSpace:
                case CardType.Event:
                case CardType.Equipment:
                case CardType.Incident:
                case CardType.Interrupt:
                case CardType.Objective:
                case CardType.QDilemma:
                case CardType.QArtifact:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    AddCardArt(target, card, "ArtBox.Medium", Card.ArtKey);
                    break;
                case CardType.Doorway:
                    AddCardArt(target, card, "ArtBox.ExtraLarge", Card.ArtKey);
                    break;
            }
        }

        private static void AddArtBorder(Canvas target, Card card)
        {
            switch (card.CardType)
            {
                case CardType.Artifact:
                case CardType.DilemmaBoth:
                case CardType.DilemmaPlanet:
                case CardType.DilemmaSpace:
                case CardType.Event:
                case CardType.Equipment:
                case CardType.Incident:
                case CardType.Interrupt:
                case CardType.Objective:
                case CardType.QDilemma:
                case CardType.QArtifact:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    AddCachedImage(target, "ArtBorder.Medium");
                    break;
            }
        }

        private static void AddTextBox(Canvas target, Card card)
        {
            switch (card.CardType)
            {
                case CardType.Artifact:
                case CardType.DilemmaBoth:
                case CardType.DilemmaPlanet:
                case CardType.DilemmaSpace:
                case CardType.Event:
                case CardType.Equipment:
                case CardType.Interrupt:
                    AddCachedImageByEnumKey(target, card, Card.TypedTextBoxKey);
                    break;
                case CardType.Doorway:
                case CardType.Incident:
                case CardType.Objective:
                    AddCachedImage(target, $"{Card.TypedTextBoxKey}.SevenGametext");
                    break;
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    AddCachedImage(target, $"{Card.TypedTextBoxKey}.Q");
                    break;
            }
        }

        private static void AddExpansionIcon(Canvas target, Card card)
        {
            AddCachedImageByEnumKey(target, card, Card.ExpansionIconKey);
        }

        private static void AddText(Canvas target, Card card)
        {
            switch (card.CardType)
            {
                case CardType.Artifact:
                case CardType.DilemmaBoth:
                case CardType.DilemmaPlanet:
                case CardType.DilemmaSpace:
                case CardType.Event:
                case CardType.Equipment:
                case CardType.Interrupt:
                    AddTypedTextBlocks(target, card, Card.TypedTextBoxKey);
                    break;
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    AddTypedTextBlocks(target, card, Card.QTypedTextBoxKey);
                    break;
                case CardType.Doorway:
                case CardType.Incident:
                case CardType.Objective:
                    AddTypedTextBlocks(target, card, Card.TypedTextBoxKey, "SevenGametext");
                    break;
            }
        }

        private static void AddCardArt(Canvas target, Card card, string artboxId, string artKey)
        {
            string base64 = card.GetValue(artKey);
            if (TryCreateImage(base64, artboxId, out IControl result))
            {
                target.Children.Add(result);
            }
        }

        private static bool TryCreateImage(string base64, string imageboxId, out IControl result)
        {
            if (!CurrentConfig.ImageBoxDescriptors.TryGetValue(imageboxId, out ImageBoxDescriptor descriptor))
            {
                result = null;
                return false;
            }

            var border = new Border()
            {
                Width = descriptor.Width,
                Height = descriptor.Height,
            };

            if (AppVM.DebugMode)
            {
                border.Background = Brushes.Magenta;
            }

            Canvas.SetLeft(border, descriptor.X);
            Canvas.SetTop(border, descriptor.Y);

            try
            {
                var stream = Base64Utils.CreateStreamFromBase64(base64);

                Image image = new Image()
                {
                    Source = new Bitmap(stream),
                    Width = descriptor.Width,
                    Height = descriptor.Height,
                    Stretch = Stretch.UniformToFill,
                };

                border.Child = image;
            }
            catch (Exception) { }

            result = border;
            return true;
        }

        private static void AddTypedTextBlocks(Canvas target, Card card, string textboxKey)
        {
            AddTypedTextBlocks(target, card, textboxKey, card.GetValue(CurrentConfig.Enums[textboxKey]));
        }

        private static void AddTypedTextBlocks(Canvas target, Card card, string textboxKey, string textboxValue)
        {
            AddTextBlock(target, $"{textboxKey}.{Card.TitleKey}", card.GetValue(Card.TitleKey));
            AddTextBlock(target, $"{textboxKey}.{textboxValue}.{Card.LoreKey}", card.GetValue(Card.LoreKey));
            AddTextBlock(target, $"{textboxKey}.{textboxValue}.{Card.GametextKey}", card.GetValue(Card.GametextKey));
        }

        private static void AddTextBlock(Canvas target, string textboxId, string textboxContents)
        {
            if (TryCreateTextBlock(textboxId, textboxContents, out IControl result))
            {
                target.Children.Add(result);
            }
        }

        private static bool TryCreateTextBlock(string textboxId, string textboxContents, out IControl result)
        {
            if (!CurrentConfig.TextBoxDescriptors.TryGetValue(textboxId, out TextBoxDescriptor descriptor))
            {
                result = null;
                return false;
            }

            var textBlock = new TextBlock()
            {
                Text = descriptor.AllCaps ? textboxContents.ToUpperInvariant() : textboxContents,
                TextAlignment = (TextAlignment)descriptor.Alignment,
                Foreground = descriptor.Color == TextBoxColor.white ? Brushes.White : Brushes.Black,
                FontFamily = Fonts[descriptor.FontFamily],
                FontSize = descriptor.FontSize,
                Width = descriptor.Width,
                Height = descriptor.Height
            };

            if (AppVM.DebugMode)
            {
                textBlock.Background = Brushes.Magenta;
            }

            Canvas.SetLeft(textBlock, descriptor.X);
            Canvas.SetTop(textBlock, descriptor.Y);

            result = textBlock;
            return true;
        }

        private static void AddCachedImageByEnumKey(Canvas target, Card card, string enumKey)
        {
            string enumValue = card.GetValue(CurrentConfig.Enums[enumKey]);

            AddCachedImage(target, $"{enumKey}.{enumValue}");
        }

        private static void AddCachedImage(Canvas target, string imageId)
        {
            if (TryGetCachedImage(imageId, out CachedImage cachedImage))
            {
                target.Children.Add(cachedImage.ToControl());
            }
        }

        private static bool TryGetCachedImage(string imageId, out CachedImage result)
        {
            string configName = CurrentConfig.Name;
            string cacheKey = $"{Edition.FirstEdition}.{configName}";

            if (!CachedImages.TryGetValue(cacheKey, out Dictionary<string, CachedImage> configImages))
            {
                configImages = new Dictionary<string, CachedImage>();
                CachedImages[cacheKey] = configImages;
            }

            if (!configImages.TryGetValue(imageId, out CachedImage cachedImage))
            {
                if (!AppVM.Configs.TryGetConfig(Edition.FirstEdition, configName, out Config config) || !config.ImageAssets.TryGetValue(imageId, out ImageAsset imageAsset))
                {
                    result = null;
                    return false;
                }

                cachedImage = new CachedImage(new Uri("file://" + imageAsset.Path), imageAsset.X, imageAsset.Y);
                configImages[imageId] = cachedImage;
            }

            result = cachedImage;
            return true;
        }

        private static readonly Dictionary<string, Dictionary<string, CachedImage>> CachedImages = new Dictionary<string, Dictionary<string, CachedImage>>();

        private static readonly Dictionary<string, FontFamily> Fonts = new Dictionary<string, FontFamily>()
        {
            { "Gametext", new FontFamily(new Uri("avares://StarTrekCardMaker"), "/Fonts/*.ttf#Gametext") },
            { "Lore", new FontFamily(new Uri("avares://StarTrekCardMaker"), "/Fonts/*.ttf#Lore") },
            { "Skill", new FontFamily(new Uri("avares://StarTrekCardMaker"), "/Fonts/*.ttf#Skill") },
            { "Verb", new FontFamily(new Uri("avares://StarTrekCardMaker"), "/Fonts/*.ttf#Verb") },
            { "VerbSpacing", new FontFamily(new Uri("avares://StarTrekCardMaker"), "/Fonts/*.ttf#Verb Spacing") },
        };
    }
}
