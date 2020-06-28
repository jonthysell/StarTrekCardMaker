// 
// ConfigManager.cs
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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;

namespace StarTrekCardMaker.Models
{
    public class ConfigManager
    {
        private Dictionary<Edition, Dictionary<string, Config>> _configs = new Dictionary<Edition, Dictionary<string, Config>>();

        public ConfigManager() { }


        public bool TryGetConfig(Edition edition, out Config result)
        {
            return TryGetConfig(edition, DefaultConfigName, out result);
        }

        public bool TryGetConfig(Edition edition, string name, out Config result)
        {
            result = GetConfig(edition, name);
            return result != null;
        }

        public Config GetConfig(Edition edition, string name = DefaultConfigName)
        {
            if (_configs.TryGetValue(edition, out Dictionary<string, Config> configs))
            {
                if (configs.TryGetValue(name, out Config result))
                {
                    return result;
                }
            }

            return null;
        }

        public void Add(Config config)
        {
            if (null == config)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (!_configs.TryGetValue(config.Edition, out Dictionary<string, Config> configs))
            {
                configs = new Dictionary<string, Config>();
                _configs.Add(config.Edition, configs);
            }

            configs.Add(config.Name, config);
        }

        public const string DefaultConfigName = "Default";

        public static ConfigManager LoadXml(Stream inputStream)
        {
            if (null == inputStream)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            var configs = new ConfigManager();

            using XmlReader xmlReader = XmlReader.Create(inputStream);

            while (xmlReader.Read())
            {
                if (xmlReader.IsStartElement())
                {
                    string name = xmlReader.Name.ToLowerInvariant();

                    if (name == "config")
                    {
                        Config config = Config.Read(xmlReader.ReadSubtree());
                        configs.Add(config);
                    }
                }
            }

            return configs;
        }
    }
}
