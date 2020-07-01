// 
// AppViewModel.cs
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
using System.IO;

using StarTrekCardMaker.Models;

namespace StarTrekCardMaker.ViewModels
{
    public class AppViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        #region Singleton Statics

        public static AppViewModel Instance
        {
            get
            {
                if (null == _instance)
                {
                    Initialize();
                }
                return _instance;
            }
        }
        private static AppViewModel _instance;

        public static void Initialize(string[] args = null)
        {
            if (null != _instance)
            {
                throw new InvalidOperationException($"{ nameof(Instance) } is already initialized.");
            }

            _instance = new AppViewModel();
            _instance.ParseArgs(args);
        }

        #endregion

        public ConfigManager Configs { get; private set; }

        public bool DebugMode { get; set; } = false;

        private AppViewModel() { }

        private void ParseArgs(string[] args)
        {
            string configFile = "config.xml";

            if (null != args && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLowerInvariant())
                    {
                        case "--config":
                            configFile = args[++i];
                            break;
                    }
                }
            }

            using var configStream = File.OpenRead(configFile);

            Configs = ConfigManager.LoadXml(configStream);
        }
    }
}
