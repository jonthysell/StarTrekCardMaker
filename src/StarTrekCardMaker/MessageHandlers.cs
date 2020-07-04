// 
// MessageHandlers.cs
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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using GalaSoft.MvvmLight.Messaging;

using StarTrekCardMaker.ViewModels;
using StarTrekCardMaker.Views;

namespace StarTrekCardMaker
{
    public class MessageHandlers
    {
        public static Window MainWindow => (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow;

        public static void RegisterMessageHandlers(object recipient)
        {
            Messenger.Default.Register<ExceptionMessage>(recipient, async (message) => await ShowExceptionDialogAsync(message));
            Messenger.Default.Register<OpenFileMessage>(recipient, async (message) => await ShowOpenFileDialogAsync(message));
            Messenger.Default.Register<SaveFileMessage>(recipient, async (message) => await ShowSaveFileDialogAsync(message));
        }

        public static void UnregisterMessageHandlers(object recipient)
        {
            Messenger.Default.Unregister<ExceptionMessage>(recipient);
            Messenger.Default.Unregister<OpenFileMessage>(recipient);
            Messenger.Default.Unregister<SaveFileMessage>(recipient);
        }

        private static async Task ShowExceptionDialogAsync(ExceptionMessage message)
        {
            Trace.TraceError($"Exception: { message.VM.Details }");

            var window = new ExceptionWindow()
            {
                VM = message.VM
            };

            await window.ShowDialog(MainWindow);
        }

        private static async Task ShowOpenFileDialogAsync(OpenFileMessage message)
        {
            string filename = null;

            try
            {
                var dialog = new OpenFileDialog()
                {
                    AllowMultiple = false,
                    Title = message.Title,
                    Filters = GetOpenFilters(message.FileType == FileType.ImportedImage),
                    Directory = null != message.ExistingFile ? Path.GetDirectoryName(message.ExistingFile) : null,
                    InitialFileName = null != message.ExistingFile ? Path.GetFileName(message.ExistingFile) : null,
                };

                string[] filenames = await dialog.ShowAsync(MainWindow);

                if (null != filenames && filenames.Length > 0 && !string.IsNullOrWhiteSpace(filenames[0]))
                {
                    filename = filenames[0];
                }
            }
            catch (Exception ex)
            {
                ExceptionUtils.HandleException(ex);
            }
            finally
            {
                message.Process(filename);
            }
        }

        private static async Task ShowSaveFileDialogAsync(SaveFileMessage message)
        {
            string filename = null;

            try
            {
                var dialog = new SaveFileDialog()
                {
                    Title = message.Title,
                    Filters = GetSaveFilters(message.FileType == FileType.ExportedImage),
                    DefaultExtension = GetSaveDefaultExtension(message.FileType == FileType.ExportedImage),
                    Directory = null != message.ExistingFile ? Path.GetDirectoryName(message.ExistingFile) : null,
                    InitialFileName = null != message.ExistingFile ? Path.GetFileName(message.ExistingFile) : null,
                };

                filename = await dialog.ShowAsync(MainWindow);
            }
            catch (Exception ex)
            {
                ExceptionUtils.HandleException(ex);
            }
            finally
            {
                message.Process(filename);
            }
        }

        private static List<FileDialogFilter> GetOpenFilters(bool image = false)
        {
            var filters = new List<FileDialogFilter>
            {
                new FileDialogFilter()
                {
                    Name = image ? "Image Files" : "Card Files",
                    Extensions = image ? new List<string>() { "jpg", "png", "gif" } : new List<string>() { "xml" }
                },
                new FileDialogFilter()
                {
                    Name = "All Files",
                    Extensions = new List<string>() { "*" }
                }
            };

            return filters;
        }

        private static string GetSaveDefaultExtension(bool export = false)
        {
            return export ? "png" : "xml";
        }

        private static List<FileDialogFilter> GetSaveFilters(bool export = false)
        {
            var filters = new List<FileDialogFilter>
            {
                new FileDialogFilter()
                {
                    Name = export ? "Image Files" : "Card Files",
                    Extensions = new List<string>() { export ? "png" : "xml" }
                },

                new FileDialogFilter()
                {
                    Name = "All Files",
                    Extensions = new List<string>() { "*" }
                }
            };

            return filters;
        }
    }
}
