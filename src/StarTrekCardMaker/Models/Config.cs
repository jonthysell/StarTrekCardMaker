// 
// Config.cs
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
using System.Linq;
using System.Xml;

namespace StarTrekCardMaker.Models
{
    public class Config
    {
        public Edition Edition { get; private set; }

        public string Name { get; private set; } = null;

        public Dictionary<string, double> Constants { get; private set; } = new Dictionary<string, double>();

        public Dictionary<string, DynamicEnum> Enums { get; private set; } = new Dictionary<string, DynamicEnum>();

        public Dictionary<string, ImageAsset> ImageAssets { get; private set; } = new Dictionary<string, ImageAsset>();

        public Dictionary<string, ImageBoxDescriptor> ImageBoxDescriptors { get; private set; } = new Dictionary<string, ImageBoxDescriptor>();

        public Dictionary<string, TextBoxDescriptor> TextBoxDescriptors { get; private set; } = new Dictionary<string, TextBoxDescriptor>();

        public Config() { }

        public static Config Read(XmlReader xmlReader, string baseDir)
        {
            if (null == xmlReader)
            {
                throw new ArgumentNullException(nameof(xmlReader));
            }

            if (string.IsNullOrWhiteSpace(baseDir))
            {
                throw new ArgumentNullException(nameof(baseDir));
            }

            Config config = new Config();

            while (xmlReader.Read())
            {
                if (xmlReader.IsStartElement())
                {
                    string name = xmlReader.Name.ToLowerInvariant();

                    if (name == "config")
                    {
                        config.Edition = Enum.Parse<Edition>(xmlReader.GetAttribute("edition"));
                        config.Name = xmlReader.GetAttribute("name");
                    }
                    else if (name == "constant")
                    {
                        string id = xmlReader.GetAttribute("id");

                        double.TryParse(xmlReader.GetAttribute("value"), out double value);

                        config.Constants[id] = value;
                    }
                    else if (name == "enum")
                    {
                        string id = xmlReader.GetAttribute("id");
                        string values = xmlReader.GetAttribute("values");

                        bool.TryParse(xmlReader.GetAttribute("optional"), out bool optional);

                        string[] split = values.Split('|', StringSplitOptions.RemoveEmptyEntries);

                        config.Enums[id] = new DynamicEnum(id, split.Select(v => EnumUtils.GetFriendlyValue(v)), optional);
                    }
                    else if (name == "image")
                    {
                        string id = xmlReader.GetAttribute("id");
                        string path = xmlReader.GetAttribute("path");

                        if (!Path.IsPathFullyQualified(path))
                        {
                            path = Path.Combine(baseDir, path);
                        }

                        double.TryParse(xmlReader.GetAttribute("x"), out double x);
                        double.TryParse(xmlReader.GetAttribute("y"), out double y);

                        bool.TryParse(xmlReader.GetAttribute("enum"), out bool isEnum);

                        bool.TryParse(xmlReader.GetAttribute("optional"), out bool optional);

                        if (File.Exists(path))
                        {
                            config.ImageAssets[id] = new ImageAsset(id, path, x, y);
                        }
                        else if (Directory.Exists(path))
                        {
                            var files = Directory.EnumerateFiles(path).OrderBy(f => f);

                            foreach (var file in files)
                            {
                                string fileId = GetFileId(file);
                                string assetId = $"{id}.{fileId}";

                                config.ImageAssets[assetId] = new ImageAsset(assetId, file, x, y);
                            }

                            if (isEnum)
                            {
                                config.Enums[id] = new DynamicEnum(id, files.Select(f => EnumUtils.GetFriendlyValue(GetFileId(f))), optional);
                            }
                        }
                    }
                    else if (name == "imagebox")
                    {
                        string id = xmlReader.GetAttribute("id");

                        double.TryParse(xmlReader.GetAttribute("x"), out double x);
                        double.TryParse(xmlReader.GetAttribute("y"), out double y);
                        double.TryParse(xmlReader.GetAttribute("width"), out double width);
                        double.TryParse(xmlReader.GetAttribute("height"), out double height);
                        double.TryParse(xmlReader.GetAttribute("rotate"), out double rotateDegrees);

                        config.ImageBoxDescriptors[id] = new ImageBoxDescriptor(id, x, y, width, height, rotateDegrees);
                    }
                    else if (name == "textbox")
                    {
                        string id = xmlReader.GetAttribute("id");
                        string fontFamily = xmlReader.GetAttribute("font");

                        double.TryParse(xmlReader.GetAttribute("size"), out double fontSize);

                        Enum.TryParse(xmlReader.GetAttribute("align"), out TextBoxAlignment alignment);
                        Enum.TryParse(xmlReader.GetAttribute("color"), out TextBoxColor color);

                        bool.TryParse(xmlReader.GetAttribute("caps"), out bool caps);

                        double.TryParse(xmlReader.GetAttribute("x"), out double x);
                        double.TryParse(xmlReader.GetAttribute("y"), out double y);
                        double.TryParse(xmlReader.GetAttribute("width"), out double width);
                        double.TryParse(xmlReader.GetAttribute("height"), out double height);
                        double.TryParse(xmlReader.GetAttribute("rotate"), out double rotateDegrees);

                        config.TextBoxDescriptors[id] = new TextBoxDescriptor(id, fontFamily, fontSize, alignment, color, caps, x, y, width, height, rotateDegrees);
                    }
                }
            }

            return config;
        }

        private static string GetFileId(string file)
        {
            return Path.GetFileNameWithoutExtension(file).TrimStart(Numbers);
        }

        private static readonly char[] Numbers = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    }
}
