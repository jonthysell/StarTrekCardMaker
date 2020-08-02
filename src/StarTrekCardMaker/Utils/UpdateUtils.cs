// 
// UpdateUtils.cs
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
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace StarTrekCardMaker.Utils
{
    public static class UpdateUtils
    {
        public static async Task<GitHubReleaseInfo> GetLatestGitHubReleaseInfoAsync(string owner, string repo)
        {
            var releaseInfos = await GetGitHubReleaseInfosAsync(owner, repo);

            return releaseInfos
                .OrderByDescending(info => info.LongVersion)
                .ThenBy(info => info.Name)
                .FirstOrDefault();
        }

        public static async Task<IList<GitHubReleaseInfo>> GetGitHubReleaseInfosAsync(string owner, string repo)
        {
            var releaseInfos = new List<GitHubReleaseInfo>();

            try
            {
                var request = WebRequest.CreateHttp($"https://api.github.com/repos/{owner}/{repo}/releases");
                request.Headers.Add("Accept: application/vnd.github.v3+json");
                request.UserAgent = "Mozilla/5.0";

                using (var response = await request.GetResponseAsync())
                {
                    var responseStream = response.GetResponseStream();
                    var jsonDocument = await JsonDocument.ParseAsync(responseStream);

                    foreach (var releaseObject in jsonDocument.RootElement.EnumerateArray())
                    {
                        string name = releaseObject.GetProperty("name").GetString();
                        string tagName = releaseObject.GetProperty("tag_name").GetString();
                        string htmlUrl = releaseObject.GetProperty("html_url").GetString();
                        bool draft = releaseObject.GetProperty("draft").GetBoolean();
                        bool prerelease = releaseObject.GetProperty("prerelease").GetBoolean();

                        releaseInfos.Add(new GitHubReleaseInfo(name, tagName, htmlUrl, draft, prerelease));
                    }
                }
            }
            catch (Exception) { }

            return releaseInfos;
        }

        public static bool TryParseLongVersion(string s, out ulong result)
        {
            try
            {
                ulong vers = 0;

                string[] parts = s.TrimStart('v').Trim().Split('.');

                for (int i = 0; i < parts.Length; i++)
                {
                    vers |= (ulong.Parse(parts[i]) << ((4 - (i + 1)) * 16));
                }

                result = vers;
                return true;
            }
            catch (Exception) { }

            result = 0;
            return false;
        }

        public class GitHubReleaseInfo
        {
            public readonly string Name;
            public readonly string TagName;
            public readonly Uri HtmlUrl;
            public readonly bool Draft;
            public readonly bool Prerelease;

            public ulong LongVersion
            {
                get
                {
                    if (!_longVersion.HasValue)
                    {
                        TryParseLongVersion(TagName, out ulong result);
                        _longVersion = result;
                    }
                    return _longVersion.Value;
                }
            }
            private ulong? _longVersion;

            public GitHubReleaseInfo(string name, string tagName, string htmlUrl, bool draft, bool prerelease)
            {
                Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
                TagName = tagName?.Trim() ?? throw new ArgumentNullException(nameof(tagName));
                HtmlUrl = new Uri(htmlUrl);
                Draft = draft;
                Prerelease = prerelease;
            }
        }
    }
}
