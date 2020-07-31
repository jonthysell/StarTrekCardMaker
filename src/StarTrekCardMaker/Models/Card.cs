// 
// Card.cs
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
using System.IO;
using System.Text;
using System.Xml;

namespace StarTrekCardMaker.Models
{
    public class Card : IEquatable<Card>, ICloneable<Card>
    {
        public readonly Dictionary<string, string> Data = new Dictionary<string, string>();

        public const string BorderKey = "Border";

        public const string PropertyLogoKey = "PropertyLogo";
        public const string ExpansionIconKey = "ExpansionIcon";

        public const string ArtKey = "Art";

        public const string TypedTextBoxKey = "TypedTextBox";
        public const string QTypedTextBoxKey = "QTypedTextBox";
        public const string MissionTextBoxKey = "MissionTextBox";

        public const string TitleKey = "Title";
        public const string LoreKey = "Lore";
        public const string GametextKey = "Gametext";
        public const string CopyrightKey = "Copyright";

        public const string AffiliationKey = "Affiliation";

        public const string SpanKey = "Span";

        public const int MaxAffiliations = 5;

        public Edition Edition
        {
            get
            {
                return GetEnumValue<Edition>();
            }
            set
            {
                SetEnumValue(value);
            }
        }

        public CardType CardType
        {
            get
            {
                return GetEnumValue<CardType>();
            }
            set
            {
                SetEnumValue(value);
            }
        }

        public Card() { }

        public TEnum GetEnumValue<TEnum>() where TEnum : struct
        {
            return GetEnumValue<TEnum>(typeof(TEnum).Name);
        }

        public TEnum GetEnumValue<TEnum>(string key) where TEnum : struct
        {
            if (Data.TryGetValue(key, out string value) && Enum.TryParse(value, out TEnum result))
            {
                return result;
            }

            return default;
        }

        public bool SetEnumValue<TEnum>(TEnum value) where TEnum : struct
        {
            return SetEnumValue(typeof(TEnum).Name, value);
        }

        public bool SetEnumValue<TEnum>(string key, TEnum value) where TEnum : struct
        {
            return SetValue(key, value.ToString());
        }

        public string GetValue(DynamicEnum dynamicEnum)
        {
            string key = dynamicEnum?.Id;
            string defaultValue = dynamicEnum?.DefaultValue;

            if (Data.TryGetValue(key, out string value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return defaultValue;
        }

        public string GetFullyQualifiedValue(DynamicEnum dynamicEnum)
        {
            return $"{dynamicEnum?.Id}.{GetValue(dynamicEnum)}";
        }

        public string GetValue(string key, string defaultValue = "")
        {
            if (Data.TryGetValue(key, out string value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return defaultValue;
        }

        public bool SetValue(string key, string value)
        {
            Data.TryGetValue(key, out string oldValue);
            Data[key] = value;
            return value != oldValue;
        }

        public List<string> GetAffiliations()
        {
            List<string> values = new List<string>();
            for (int i = 1; i <= MaxAffiliations; i++)
            {
                string value = GetValue($"{AffiliationKey}{(i > 1 ? i.ToString() : "")}");
                if (string.IsNullOrWhiteSpace(value) || value == DynamicEnum.NoneValue)
                {
                    break;
                }
                values.Add(value);
            }
            return values;
        }

        public static Card LoadXml(Stream inputStream)
        {
            if (null == inputStream)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            Card card = new Card();

            using XmlReader xmlReader = XmlReader.Create(inputStream);

            while (xmlReader.Read())
            {
                if (xmlReader.IsStartElement())
                {
                    if (xmlReader.Name == nameof(Card))
                    {
                        ReadData(xmlReader.ReadSubtree(), card);
                    }
                }
            }

            return card;
        }

        private static void ReadData(XmlReader xmlReader, Card target)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.IsStartElement())
                {
                    if (xmlReader.Name != nameof(Card))
                    {
                        string key = xmlReader.Name;
                        string value = xmlReader.ReadElementContentAsString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            target.Data[key] = value;
                        }
                    }
                }
            }
        }

        public void SaveXml(Stream outputStream)
        {
            if (null == outputStream)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            XmlWriterSettings outputSettings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                Indent = true,
            };

            using XmlWriter xmlWriter = XmlWriter.Create(outputStream, outputSettings);

            xmlWriter.WriteComment($" Created with { AppInfo.Name } v{ AppInfo.Version }. ");

            xmlWriter.WriteStartElement(nameof(Card));

            foreach (var kvp in Data)
            {
                if (!string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value))
                {
                    xmlWriter.WriteElementString(kvp.Key, kvp.Value);
                }
            }

            xmlWriter.WriteEndElement();
        }

        public bool Equals(Card other)
        {
            foreach (var kvp in Data)
            {
                if (!other.Data.TryGetValue(kvp.Key, out string value) || kvp.Value != value)
                {
                    return false;
                }
            }

            foreach (var kvp in other.Data)
            {
                if (!Data.TryGetValue(kvp.Key, out string value) || kvp.Value != value)
                {
                    return false;
                }
            }

            return true;
        }

        public Card Clone()
        {
            Card clone = new Card();

            foreach (var kvp in Data)
            {
                clone.Data[kvp.Key] = kvp.Value;
            }

            return clone;
        }
    }
}
