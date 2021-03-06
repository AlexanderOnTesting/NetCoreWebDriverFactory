﻿// <copyright>
// Copyright 2019 Alexander Dunn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using AlexanderOnTest.NetCoreWebDriverFactory.Utils.Converters;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace AlexanderOnTest.NetCoreWebDriverFactory.Config
{
    /// <summary>
    /// A WebDriver Configuration implementation.
    /// </summary>
    public class WebDriverConfiguration :IWebDriverConfiguration
    {
        private static readonly SizeJsonConverter SizeJsonConverter = new SizeJsonConverter();
        private readonly string description;

        /// <summary>
        /// Generate a new immutable WebDriverConfiguration instance.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="gridUri"></param>
        /// <param name="headless"></param>
        /// <param name="isLocal"></param>
        /// <param name="platformType"></param>
        /// <param name="windowSize"></param>
        /// <param name="windowDefinedSize"></param>
        public WebDriverConfiguration(
            Browser browser = Browser.Firefox, 
            Uri gridUri = null, 
            bool headless = false, 
            bool isLocal = true, 
            PlatformType platformType = PlatformType.Any,
            WindowSize windowSize = WindowSize.Hd,
            Size windowDefinedSize = new Size())
        {
            Browser = browser;
            GridUri = gridUri;
            Headless = headless;
            IsLocal = isLocal;
            PlatformType = platformType;
            WindowSize = (
                    windowSize == WindowSize.Maximise || 
                    windowSize == WindowSize.Maximize || 
                    windowSize == WindowSize.Unchanged) ?
                    windowSize : 
                    WindowSize.Defined;
            WindowDefinedSize = (windowSize == WindowSize.Custom || windowSize == WindowSize.Defined) ?
                windowDefinedSize :
                windowSize.Size();
            description = new StringBuilder()
                .Append($"{base.ToString()}: (")
                .Append($" Browser: {Browser.ToString()} ")
                .Append(Headless ? "headless" : "on screen")
                .Append($", Size: {WindowDefinedSize.Width} x {WindowDefinedSize.Height}, ")
                .Append(IsLocal ? "running locally)" : $"running remotely on {GridUri} on platform: {PlatformType}.)")
                .ToString();
        }

        /// <summary>
        /// Browser type to request.
        /// </summary>
        [DefaultValue(Browser.Firefox)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Browser Browser { get;}

        /// <summary>
        /// Platform to request for a RemoteWebDriver
        /// </summary>
        [DefaultValue(PlatformType.Any)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public PlatformType PlatformType { get;}

        /// <summary>
        /// WindowSize to request
        /// </summary>
        [DefaultValue(WindowSize.Hd)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public WindowSize WindowSize { get;}

        /// <summary>
        /// Actual window size requested (if not Maximize/Maximise or Unchanged)
        /// </summary>
        [JsonConverter(typeof(SizeJsonConverter))]
        public Size WindowDefinedSize { get; }

        /// <summary>
        /// The Uri of the Selenium grid to use for remote calls.
        /// </summary>
        [DefaultValue("https://localhost:4400/wd/grid")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Uri GridUri { get;}

        /// <summary>
        /// Use a local WebDriver.
        /// </summary>
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsLocal { get;}

        /// <summary>
        /// Run headless if available.
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Headless { get; }

        /// <summary>
        /// Return the configuration in a readable form.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return description;
        }

        /// <summary>
        /// Convenience method to Deserialize from Json.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static WebDriverConfiguration DeserializeFromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<WebDriverConfiguration>(jsonString, SizeJsonConverter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this, SizeJsonConverter);
        }
    }
}
