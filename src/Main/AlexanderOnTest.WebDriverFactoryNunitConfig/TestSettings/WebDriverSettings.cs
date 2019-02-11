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
using System.IO;
using AlexanderOnTest.NetCoreWebDriverFactory;
using Newtonsoft.Json;
using OpenQA.Selenium;
using static AlexanderOnTest.WebDriverFactoryNunitConfig.TestSettings.Utils;

namespace AlexanderOnTest.WebDriverFactoryNunitConfig.TestSettings
{
    public static class WebDriverSettings
    {
        public static Browser Browser { get; } = GetEnumSettingOrDefault("browserType", Browser.Firefox);

        /// <summary>
        /// Uri of the grid. Configuration Priority:
        /// 1. A value provided in "My Documents/Config_GridUri.json" (Windows) or "/Config_GridUri.json" (Mac / Linux)
        /// 2. The value in an applied .runsettings file
        /// 3. Default (Localhost) grid.
        /// </summary>
        public static Uri GridUri { get; }
            = GetLocalGridUri() ??
              new Uri(GetStringSettingOrDefault("gridUri", "http://localhost:4444/wd/hub"));

        public static bool IsLocal { get; }
            = GetBoolSettingOrDefault("isLocal", true);

        public static PlatformType PlatformType { get; } = GetEnumSettingOrDefault("platform", PlatformType.Windows);

        public static WindowSize WindowSize { get; } = GetEnumSettingOrDefault("windowSize", WindowSize.Hd);

        public static bool Headless { get; } = GetBoolSettingOrDefault("headless", false);

        /// <summary>
        /// Return the Configuration to use - Priority:
        /// 1. A value provided in "My Documents/Config_WebDriver.json" (Windows) or "/Config_WebDriver.json" (Mac / Linux)
        /// 2. The value in an applied .runsettings file
        /// 3. Default values.
        /// </summary>
        public static IWebDriverConfiguration WebDriverConfiguration { get; }
            = GetLocalConfig() ??
              new WebDriverConfiguration
            {
                Browser = Browser,
                IsLocal = IsLocal,
                WindowSize = WindowSize,
                GridUri = GridUri,
                PlatformType = PlatformType,
                Headless = Headless
            };

        private static Uri GetLocalGridUri()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string configFile = Path.Combine(folderPath, "Config_GridUri.json");

            string localGridUriString = null;
            if (!string.IsNullOrEmpty(folderPath) && File.Exists(configFile))
            {
                using (StreamReader file = File.OpenText(configFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    localGridUriString = (string)serializer.Deserialize(file, typeof(string));
                }
            }

            return string.IsNullOrEmpty(localGridUriString) ?
                null :
                new Uri(localGridUriString);
        }

        /// <summary>
        /// Return a WebDriverConfiguration object deserialised from:
        /// "My Documents/Config_WebDriver.json" (Windows) or "/Config_WebDriver.json" (Mac / Linux) if present
        /// </summary>
        /// <returns></returns>
        private static IWebDriverConfiguration GetLocalConfig()
        {
            IWebDriverConfiguration configFromHome = null;

            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string configFile = Path.Combine(folderPath, "Config_WebDriver.json");

            if (!string.IsNullOrEmpty(folderPath) && File.Exists(configFile))
            {
                using (StreamReader file = File.OpenText(configFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    configFromHome = (IWebDriverConfiguration)serializer.Deserialize(file, typeof(WebDriverConfiguration));
                }
            }

            return configFromHome;
        }
    }
}