// 
// Messages.cs
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

using GalaSoft.MvvmLight.Messaging;

namespace StarTrekCardMaker.ViewModels
{
    public abstract class CallbackMessage<T> : MessageBase
    {
        private readonly Action<T> Callback;

        protected CallbackMessage(Action<T> callback = null) : base()
        {
            Callback = callback;
        }

        public void Process(T result)
        {
            Callback?.Invoke(result);
        }
    }

    public class ConfirmationMessage : CallbackMessage<ConfirmationResult>
    {
        public readonly ConfirmationViewModel VM;

        public ConfirmationMessage(string message, Action<ConfirmationResult> callback = null) : base(callback)
        {
            VM = new ConfirmationViewModel(message);
        }
    }

    public class ExceptionMessage : MessageBase
    {
        public readonly ExceptionViewModel VM;

        public ExceptionMessage(Exception exception)
        {
            VM = new ExceptionViewModel(exception ?? throw new ArgumentNullException(nameof(exception)));
        }
    }

    public class AboutMessage : MessageBase
    {
        public readonly AboutViewModel VM;

        public AboutMessage()
        {
            VM = new AboutViewModel();
        }
    }

    public abstract class FileMessage : CallbackMessage<string>
    {
        public readonly string Title;

        public readonly FileType FileType;

        public readonly string ExistingFile;

        public FileMessage(string title, FileType fileType, string existingFile = null, Action<string> callback = null) : base(callback)
        {
            Title = title;
            FileType = fileType;
            ExistingFile = existingFile;
        }
    }

    public class OpenFileMessage : FileMessage
    {
        public OpenFileMessage(string title, FileType fileType, string existingFile = null, Action<string> callback = null) : base(title, fileType, existingFile, callback) { }
    }

    public class SaveFileMessage : FileMessage
    {
        public SaveFileMessage(string title, FileType fileType, string existingFile = null, Action<string> callback = null) : base(title, fileType, existingFile, callback) { }
    }
}
