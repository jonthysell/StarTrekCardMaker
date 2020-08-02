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
using System.Reflection;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using StarTrekCardMaker.Models;
using StarTrekCardMaker.Utils;

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

        public bool ValidConfig => null != Configs;

        public RelayCommand ShowAbout
        {
            get
            {
                return _showAbout ??= new RelayCommand(() =>
                {
                    try
                    {
                        Messenger.Default.Send(new AboutMessage());
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                });
            }
        }
        private RelayCommand _showAbout;

        public RelayCommand CheckForUpdates
        {
            get
            {
                return _checkForUpdates ??= new RelayCommand(async () =>
                {
                    try
                    {
                        var latestRelease = await UpdateUtils.GetLatestGitHubReleaseInfoAsync("jonthysell", "StarTrekCardMaker");

                        if (null == latestRelease)
                        {
                            Messenger.Default.Send(new InformationMessage("Unable to check for updates at this time. Please try again later."));
                        }
                        else if (latestRelease.LongVersion <= AppInfo.LongVersion)
                        {
                            Messenger.Default.Send(new InformationMessage($"{AppInfo.Name} is already up-to-date."));
                        }
                        else
                        {
                            // Update available
                            Messenger.Default.Send(new ConfirmationMessage($"{latestRelease.Name} is now avaliable. Would you like to open the release page?", (result) =>
                            {
                                try
                                {
                                    if (result == ConfirmationResult.Yes)
                                    {
                                        Messenger.Default.Send(new LaunchUrlMessage(latestRelease.HtmlUrl));
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
                });
            }
        }
        private RelayCommand _checkForUpdates;

        private AppViewModel() { }

        private void ParseArgs(string[] args)
        {
            string configFile = ConfigFileName;

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

            if (!Path.IsPathFullyQualified(configFile))
            {
                configFile = Path.GetFullPath(configFile);
            }

            if (!TryLoadConfig(configFile))
            {
                // External config not found, try the "bundled" config
                configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Config)).Location), ConfigFileName);
                if (!TryLoadConfig(configFile))
                {
                    ExceptionUtils.HandleException(new FileNotFoundException("Unable to load config file."));
                }
            }
        }

        private bool TryLoadConfig(string configFile)
        {
            if (File.Exists(configFile))
            {
                try
                {
                    Configs = ConfigManager.LoadXml(configFile);
                    return true;
                }
                catch (FileNotFoundException) { }
                catch (Exception ex)
                {
                    ExceptionUtils.HandleException(ex);
                }
            }

            return false;
        }

        private const string ConfigFileName = "config.xml";
    }
}
