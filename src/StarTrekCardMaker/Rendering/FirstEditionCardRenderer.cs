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
using System.Linq;

using Avalonia;
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

        public static double CardWidth => CurrentConfig.GetDoubleConstant("CardWidth");

        public static double CardHeight => CurrentConfig.GetDoubleConstant("CardHeight");

        public static double InnerRectMargin => CurrentConfig.GetDoubleConstant("InnerRectMargin");

        public readonly static string DefaultCopyrightText = "Not endorsed by CBS or Paramount Pictures";

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
                case CardType.MissionBoth:
                case CardType.MissionPlanet:
                case CardType.MissionSpace:
                    RenderMission(target, card);
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

            AddArt(target, card);

            AddTitleBar(target, card);

            AddTypedTitle(target, card);

            AddPropertyLogo(target, card);

            AddTextBox(target, card);

            AddText(target, card);

            AddCopyrightText(target, card);

            AddExpansionIcon(target, card);

            AddBorder(target, card);

            AddInnerBorder(target);

            AddRarityText(target, card);

            AddArtBorder(target, card);
        }

        private static void RenderMission(Canvas target, Card card)
        {
            AddInnerRect(target);

            AddArt(target, card);

            AddTextBox(target, card);

            AddText(target, card);

            AddCopyrightText(target, card);

            AddRarityText(target, card);

            AddMissionAffiliations(target, card);

            AddExpansionIcon(target, card);

            AddBorder(target, card);
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
                //case CardType.Tactic:
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
                //case CardType.Tactic:
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

        private static void AddPropertyLogo(Canvas target, Card card)
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
                case CardType.MissionBoth:
                case CardType.MissionPlanet:
                case CardType.MissionSpace:
                    AddCardArt(target, card, "ArtBox.Full", Card.ArtKey);
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
                    AddCachedImage(target, $"{Card.TypedTextBoxKey}.SevenGametext", GetOverrides(Card.TypedTextBoxKey, "SevenGametext"));
                    break;
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    AddCachedImage(target, $"{Card.TypedTextBoxKey}.Q");
                    break;
                case CardType.MissionBoth:
                    AddCachedImageByEnumKey(target, card, Card.MissionTextBoxKey);
                    AddCachedImage(target, $"{Card.MissionTextBoxKey}.{card.GetValue(CurrentConfig.Enums[Card.MissionTextBoxKey])}.Both");
                    break;
                case CardType.MissionPlanet:
                    AddCachedImageByEnumKey(target, card, Card.MissionTextBoxKey);
                    AddCachedImage(target, $"{Card.MissionTextBoxKey}.{card.GetValue(CurrentConfig.Enums[Card.MissionTextBoxKey])}.Planet");
                    break;
                case CardType.MissionSpace:
                    AddCachedImageByEnumKey(target, card, Card.MissionTextBoxKey);
                    AddCachedImage(target, $"{Card.MissionTextBoxKey}.{card.GetValue(CurrentConfig.Enums[Card.MissionTextBoxKey])}.Space");
                    break;
            }
        }

        private static void AddExpansionIcon(Canvas target, Card card)
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
                    AddCachedImageByEnumKey(target, card, Card.ExpansionIconKey, Card.TypedTextBoxKey);
                    break;
                case CardType.Doorway:
                case CardType.Incident:
                case CardType.Objective:
                    string expansionValue = card.GetValue(CurrentConfig.Enums[Card.ExpansionIconKey]);
                    var imageBoxIds = GetOverrides(Card.TypedTextBoxKey, "SevenGametext", Card.ExpansionIconKey, expansionValue);

                    if (expansionValue == DynamicEnum.CustomValue)
                    {
                        AddCardArt(target, card, imageBoxIds, $"{Card.ExpansionIconKey}.{expansionValue}");
                    }
                    else
                    {
                        AddCachedImage(target, $"{Card.ExpansionIconKey}.{expansionValue}", imageBoxIds);
                    }
                    break;
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    AddCachedImageByEnumKey(target, card, Card.ExpansionIconKey, Card.QTypedTextBoxKey);
                    break;
                case CardType.MissionBoth:
                case CardType.MissionPlanet:
                case CardType.MissionSpace:
                    AddCachedImageByEnumKey(target, card, Card.ExpansionIconKey, Card.MissionTextBoxKey);
                    break;
            }
        }

        private static void AddMissionAffiliations(Canvas target, Card card)
        {
            var affiliations = card.GetAffiliations();

            for (int i = 0; i < affiliations.Count; i++)
            {
                TryGetCachedBrush($"MissionAffiliation.Color.{affiliations[i]}", out IBrush affiliationMissionBrush);
                var playerBoxBorder = AddBorderByImageBoxId(target, $"MissionAffiliation.Player.{i + 1}-{affiliations.Count}", AppVM.DebugMode ? Brushes.Magenta : affiliationMissionBrush ?? Brushes.Transparent);

                if (null != playerBoxBorder && TryGetCachedImage($"MissionAffiliation.{affiliations[i]}", out CachedImage result))
                {
                    playerBoxBorder.Child = new Image()
                    {
                        Source = result.Bitmap,
                    };
                }

                AddBorderByImageBoxId(target, $"MissionAffiliation.Opponent.{i + 1}-{affiliations.Count}", AppVM.DebugMode ? Brushes.Magenta : affiliationMissionBrush ?? Brushes.Transparent);
            }
        }

        private static Border AddBorderByImageBoxId(Canvas target, string key, IBrush fill)
        {
            if (CurrentConfig.ImageBoxDescriptors.TryGetValue(key, out ImageBoxDescriptor descriptor))
            {
                var border = new Border()
                {
                    Width = descriptor.Width,
                    Height = descriptor.Height,
                    Background = fill,
                };

                Canvas.SetLeft(border, descriptor.X);
                Canvas.SetTop(border, descriptor.Y);
                target.Children.Add(border);

                return border;
            }

            return null;
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
                case CardType.MissionBoth:
                case CardType.MissionPlanet:
                case CardType.MissionSpace:
                    AddMissionTextBlocks(target, card, Card.MissionTextBoxKey);
                    break;
            }
        }

        private static void AddCopyrightText(Canvas target, Card card)
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
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    AddTextBlock(target, "ArtBox.Medium.Copyright", card.GetValue(Card.CopyrightKey, DefaultCopyrightText));
                    break;
                case CardType.Doorway:
                    AddTextBlock(target, "ArtBox.ExtraLarge.Copyright", card.GetValue(Card.CopyrightKey, DefaultCopyrightText));
                    break;
                case CardType.MissionBoth:
                case CardType.MissionPlanet:
                case CardType.MissionSpace:
                    AddTextBlock(target, "ArtBox.Full.Copyright", card.GetValue(Card.CopyrightKey, DefaultCopyrightText));
                    break;
            }
        }

        private static void AddRarityText(Canvas target, Card card)
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
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                case CardType.Doorway:
                    AddTextBlock(target, $"Typed.{card.GetFullyQualifiedValue(CurrentConfig.Enums[Card.RarityTextBoxKey])}.{Card.RarityKey}", card.GetValue(Card.RarityKey));
                    break;
                case CardType.MissionBoth:
                case CardType.MissionPlanet:
                case CardType.MissionSpace:
                    AddTextBlock(target, $"Mission.{card.GetFullyQualifiedValue(CurrentConfig.Enums[Card.RarityTextBoxKey])}.{Card.RarityKey}", card.GetValue(Card.RarityKey));
                    break;
            }
        }

        private static void AddCardArt(Canvas target, Card card, string artboxId, string artKey)
        {
            AddCardArt(target, card, new[] { artboxId }, artKey);
        }

        private static void AddCardArt(Canvas target, Card card, IEnumerable<string> imageBoxOverrides, string artKey)
        {
            string base64 = card.GetValue(artKey);
            if (TryCreateImage(base64, imageBoxOverrides, out IControl result))
            {
                target.Children.Add(result);
            }
        }

        private static bool TryCreateImage(string base64, IEnumerable<string> imageBoxOverrides, out IControl result)
        {
            ImageBoxDescriptor imageBoxDescriptor = null;

            if (null != imageBoxOverrides)
            {
                foreach (var imageBoxId in imageBoxOverrides)
                {
                    if (CurrentConfig.ImageBoxDescriptors.TryGetValue(imageBoxId, out imageBoxDescriptor))
                    {
                        break;
                    }
                }
            }

            if (null == imageBoxDescriptor)
            {
                result = null;
                return false;
            }

            var border = new Border()
            {
                Width = imageBoxDescriptor.Width,
                Height = imageBoxDescriptor.Height,
            };

            if (AppVM.DebugMode)
            {
                border.Background = Brushes.Magenta;
            }

            Canvas.SetLeft(border, imageBoxDescriptor.X);
            Canvas.SetTop(border, imageBoxDescriptor.Y);

            try
            {
                var stream = Base64Utils.CreateStreamFromBase64(base64);

                Image image = new Image()
                {
                    Source = new Bitmap(stream),
                    Width = imageBoxDescriptor.Width,
                    Height = imageBoxDescriptor.Height,
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

        private static void AddMissionTextBlocks(Canvas target, Card card, string textboxKey)
        {
            AddMissionTextBlocks(target, card, textboxKey, card.GetValue(CurrentConfig.Enums[textboxKey]));
        }

        private static void AddMissionTextBlocks(Canvas target, Card card, string textboxKey, string textboxValue)
        {
            AddTextBlock(target, $"{textboxKey}.{textboxValue}.{Card.TitleKey}", card.GetValue(Card.TitleKey));
            AddTextBlock(target, $"{textboxKey}.Player.{Card.SpanKey}", card.GetValue(Card.SpanKey));
            AddTextBlock(target, $"{textboxKey}.Opponent.{Card.SpanKey}", card.GetValue(Card.SpanKey));
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

            double textOffsetX = GetTextOffsetX();
            double textOffsetY = GetTextOffsetY();

            var textBlock = new TextBlock()
            {
                Text = descriptor.AllCaps ? textboxContents.ToUpperInvariant() : textboxContents,
                TextAlignment = (TextAlignment)descriptor.Alignment,
                Foreground = descriptor.Color == TextBoxColor.white ? Brushes.White : Brushes.Black,
                FontFamily = Fonts[descriptor.FontFamily],
                FontSize = descriptor.FontSize,
                Width = descriptor.Width - textOffsetX,
                Height = descriptor.Height - textOffsetY,
            };

            if (AppVM.DebugMode)
            {
                textBlock.Background = Brushes.Magenta;
            }

            Canvas.SetLeft(textBlock, descriptor.X + textOffsetX);
            Canvas.SetTop(textBlock, descriptor.Y + textOffsetY);

            if (descriptor.RotateDegrees != 0.0)
            {
                textBlock.RenderTransform = new RotateTransform(descriptor.RotateDegrees);
                textBlock.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);
            }

            result = textBlock;
            return true;
        }

        private static double GetTextOffsetX() => CurrentConfig.TryGetPlatformConstant("TextOffset.X", out double value) ? value : 0;

        private static double GetTextOffsetY() => CurrentConfig.TryGetPlatformConstant("TextOffset.Y", out double value) ? value : 0;

        private static bool TryGetCachedBrush(string brushId, out IBrush result)
        {
            string configName = CurrentConfig.Name;
            string cacheKey = $"{Edition.FirstEdition}.{configName}";

            if (!CachedBrushes.TryGetValue(cacheKey, out Dictionary<string, IBrush> configBrushes))
            {
                configBrushes = new Dictionary<string, IBrush>();
                CachedBrushes[cacheKey] = configBrushes;
            }

            if (!configBrushes.TryGetValue(brushId, out IBrush cachedBrush))
            {
                if (!AppVM.Configs.TryGetConfig(Edition.FirstEdition, configName, out Config config) || !config.TryGetConstant(brushId, out string brushValue))
                {
                    result = null;
                    return false;
                }

                cachedBrush = new SolidColorBrush(Color.Parse(brushValue));
                configBrushes[brushId] = cachedBrush;
            }

            result = cachedBrush;
            return true;
        }

        private static void AddCachedImageByEnumKey(Canvas target, Card card, string enumKey, string imageBoxEnumKey = null)
        {
            string fullyQualifiedEnumValue = card.GetFullyQualifiedValue(CurrentConfig.Enums[enumKey]);
            List<string> defaultImageBoxIds = new List<string>(null == imageBoxEnumKey ? GetOverrides(card, enumKey) : GetOverrides(card, imageBoxEnumKey, enumKey));

            if (fullyQualifiedEnumValue.EndsWith($".{DynamicEnum.CustomValue}"))
            {
                AddCardArt(target, card, defaultImageBoxIds, fullyQualifiedEnumValue);
            }
            else
            {
                AddCachedImage(target, fullyQualifiedEnumValue, defaultImageBoxIds);
            }
        }

        private static void AddCachedImage(Canvas target, string imageId, IEnumerable<string> imageBoxOverrides = null)
        {
            if (TryGetCachedImage(imageId, out CachedImage cachedImage))
            {
                ImageBoxDescriptor imageBoxDescriptor = null;

                if (null == imageBoxOverrides)
                {
                    var defaultOverrides = new List<string>();

                    string[] split = imageId.Split('.', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < split.Length; i++)
                    {
                        defaultOverrides.Add(string.Join('.', split.Skip(i).ToArray()));
                    }
                    imageBoxOverrides = defaultOverrides;
                }

                foreach (var imageBoxId in imageBoxOverrides)
                {
                    if (CurrentConfig.ImageBoxDescriptors.TryGetValue(imageBoxId, out imageBoxDescriptor))
                    {
                        break;
                    }
                }

                target.Children.Add(cachedImage.ToControl(imageBoxDescriptor));
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

        private static IEnumerable<string> GetOverrides(Card card, string enumKey)
        {
            return GetOverrides(enumKey, card.GetValue(CurrentConfig.Enums[enumKey]));
        }

        private static IEnumerable<string> GetOverrides(string enumKey, string value)
        {
            string fqEnumValue = $"{enumKey}.{value}";

            List<string> overrides = new List<string>() // ExpansionIcon
            {
                fqEnumValue, // ExpansionIcon.Premiere
                enumKey,     // ExpansionIcon
            };

            return overrides;
        }

        private static IEnumerable<string> GetOverrides(Card card, string enumKey1, string enumKey2)
        {
            return GetOverrides(enumKey1, card.GetValue(CurrentConfig.Enums[enumKey1]), enumKey2, card.GetValue(CurrentConfig.Enums[enumKey2]));
        }

        private static IEnumerable<string> GetOverrides(string enumKey1, string value1, string enumKey2, string value2)
        {
            string fqEnumValue1 = $"{enumKey1}.{value1}";
            string fqEnumValue2 = $"{enumKey2}.{value2}";

            List<string> overrides = new List<string>() // TypedTextBox, ExpansionIcon
            {
                $"{fqEnumValue1}.{fqEnumValue2}", // TypedTextBox.ThreeLoreThreeGametext.ExpansionIcon.Premiere
                $"{fqEnumValue1}.{enumKey2}",     // TypedTextBox.ThreeLoreThreeGametext.ExpansionIcon
                $"{enumKey1}.{fqEnumValue2}",     // TypedTextBox.ExpansionIcon.Premiere
                $"{enumKey1}.{enumKey2}",         // TypedTextBox.ExpansionIcon
                fqEnumValue2,                     // ExpansionIcon.Premiere
                enumKey2,                         // ExpansionIcon
            };

            return overrides;
        }

        private static readonly Dictionary<string, Dictionary<string, IBrush>> CachedBrushes = new Dictionary<string, Dictionary<string, IBrush>>();

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
