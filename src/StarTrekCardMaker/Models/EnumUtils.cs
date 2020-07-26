// 
// EnumUtils.cs
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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarTrekCardMaker.Models
{
    public static class EnumUtils
    {
        public static bool IsTypedCard(CardType cardType)
        {
            switch (cardType)
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
                    return true;
            }

            return false;
        }

        public static bool IsQCard(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                //case CardType.QMission:
                    return true;
            }

            return false;
        }

        public static bool IsQTypedCard(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    return true;
            }

            return false;
        }

        public static bool IsMissionCard(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.MissionBoth:
                case CardType.MissionPlanet:
                case CardType.MissionSpace:
                    return true;
            }

            return false;
        }

        public static bool ShowPropertyLogoOption(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Artifact:
                case CardType.DilemmaBoth:
                case CardType.DilemmaPlanet:
                case CardType.DilemmaSpace:
                case CardType.Doorway:
                case CardType.Event:
                case CardType.Equipment:
                //case CardType.Facility:
                case CardType.Incident:
                case CardType.Interrupt:
                case CardType.Objective:
                //case CardType.Personnel:
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                //case CardType.Ship:
                    return true;
            }

            return false;
        }

        public static bool ShowBorderOption(CardType cardType)
        {
            return true;
        }

        public static bool ShowExpansionIconOption(CardType cardType)
        {
            return true;
        }

        public static bool ShowAffiliationOption(CardType cardType)
        {
            switch (cardType)
            {
                //case CardType.Facility:
                case CardType.MissionBoth:
                case CardType.MissionPlanet:
                case CardType.MissionSpace:
                //case CardType.Personnel:
                //case CardType.Ship:
                    return true;
            }

            return false;
        }

        public static bool ShowTypedTextBoxOption(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Artifact:
                case CardType.DilemmaBoth:
                case CardType.DilemmaPlanet:
                case CardType.DilemmaSpace:
                case CardType.Event:
                case CardType.Equipment:
                case CardType.Interrupt:
                    return true;
            }

            return false;
        }

        public static bool ShowQTypedTextBoxOption(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.QArtifact:
                case CardType.QDilemma:
                case CardType.QEvent:
                case CardType.QInterrupt:
                    return true;
            }

            return false;
        }

        public static bool ShowMissionTextBoxOption(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.MissionBoth:
                case CardType.MissionPlanet:
                case CardType.MissionSpace:
                    return true;
            }

            return false;
        }

        public static bool ShowTitleOption(CardType cardType)
        {
            return true;
        }

        public static bool ShowLoreOption(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Doorway:
                case CardType.Incident:
                case CardType.Objective:
                    return false;
            }

            return true;
        }

        public static bool ShowGametextOption(CardType cardType)
        {
            return true;
        }

        public static bool ShowCopyrightOption(CardType cardType)
        {
            return true;
        }

        public static IEnumerable<string> GetFriendlyValues(IEnumerable<string> values)
        {
            return values.Select(item => GetFriendlyValue(item));
        }

        public static string GetFriendlyValue(string value)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0 && (
                    (char.IsLetterOrDigit(value[i-1]) && char.IsUpper(value[i])) ||
                    (char.IsLower(value[i-1]) && char.IsDigit(value[i]))
                   ))
                {
                    sb.Append(' ');
                }
                sb.Append(value[i]);
            }

            return sb.ToString();
        }

        public static string GetValue(string friendlyValue)
        {
            return friendlyValue.Replace(" ", "");
        }
    }
}
