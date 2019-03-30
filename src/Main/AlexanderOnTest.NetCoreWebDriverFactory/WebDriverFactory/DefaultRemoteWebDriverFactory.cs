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
using System.Drawing;
using AlexanderOnTest.NetCoreWebDriverFactory.DriverOptionsFactory;
using AlexanderOnTest.NetCoreWebDriverFactory.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;

namespace AlexanderOnTest.NetCoreWebDriverFactory.WebDriverFactory
{
    /// <summary>
    /// Default RemoteWebDriverFactory implementation.
    /// </summary>
    public class DefaultRemoteWebDriverFactory : IRemoteWebDriverFactory
    {
        private static readonly ILog Logger = LogProvider.For<DefaultRemoteWebDriverFactory>();
        private static readonly bool IsDebugEnabled = Logger.IsDebugEnabled();
        
        /// <summary>
        /// Return a DriverFactory instance for use in .NET Core projects.
        /// Try using installedDriverPath = "Path.GetDirectoryName(Assembly.GetCallingAssembly().Location)" when running from .NET core projects.
        /// </summary>
        /// <param name="gridUri"></param>
        /// <param name="driverOptionsFactory"></param>
        public DefaultRemoteWebDriverFactory(IDriverOptionsFactory driverOptionsFactory, Uri gridUri)
        {
            DriverOptionsFactory = driverOptionsFactory;
            GridUri = gridUri ?? new Uri("http://localhost:4444/wd/hub");
        }

        /// <summary>
        /// Return a DriverFactory instance for use in .NET Core projects.
        /// Try using driverPath = new DriverPath(Assembly.GetCallingAssembly()) when testing locally from .NET core projects.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="driverOptionsFactory"></param>
        public DefaultRemoteWebDriverFactory(IDriverOptionsFactory driverOptionsFactory, IWebDriverConfiguration configuration)
            : this(driverOptionsFactory, configuration.GridUri)
        {
        }


        /// <summary>
        /// The DriverOptionsFactory instance to use.
        /// </summary>
        public IDriverOptionsFactory DriverOptionsFactory { get; set; }

        /// <inheritdoc />
        public Uri GridUri { get; set; }

        /// <summary>
        /// Return a RemoteWebDriver instance of the given configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public IWebDriver GetWebDriver(IWebDriverConfiguration configuration)
        {
            if (configuration.IsLocal)
            {
                Exception ex = new ArgumentException("A Local WebDriver Instance cannot be generated by this method.");
                Logger.Fatal("Invalid WebDriver Configuration requested.", ex);
                throw ex;
            }

            return GetWebDriver(
                configuration.Browser,
                configuration.PlatformType,
                configuration.WindowSize,
                configuration.Headless,
                configuration.WindowCustomSize
            );
        }

        /// <summary>
        /// Return a RemoteWebDriver of the given windows size.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="windowSize"></param>
        /// <param name="windowCustomSize"></param>
        /// <returns></returns>
        public virtual IWebDriver GetWebDriver(DriverOptions options, WindowSize windowSize = WindowSize.Hd, Size windowCustomSize = new Size())
        {
            return StaticWebDriverFactory.GetRemoteWebDriver(options, GridUri, windowSize, windowCustomSize);
        }

        /// <summary>
        /// Return a configured RemoteWebDriver of the given browser type with default settings.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="platformType"></param>
        /// <param name="windowSize"></param>
        /// <param name="headless"></param>
        /// <param name="windowCustomSize"></param>
        /// <returns></returns>
        public virtual IWebDriver GetWebDriver(Browser browser, PlatformType platformType = PlatformType.Any, WindowSize windowSize = WindowSize.Hd, bool headless = false, Size windowCustomSize = new Size())
        {
            if (headless && !(browser == Browser.Chrome || browser == Browser.Firefox))
            {
                Exception ex = new ArgumentException($"Headless mode is not currently supported for {browser}.");
                Logger.Fatal("Invalid WebDriver Configuration requested.", ex);
                throw ex;
            }

            switch (browser)
            {
                case Browser.Firefox:
                    return GetWebDriver(DriverOptionsFactory.GetRemoteDriverOptions<FirefoxOptions>(platformType), windowSize, windowCustomSize);

                case Browser.Chrome:
                    return GetWebDriver(DriverOptionsFactory.GetRemoteDriverOptions<ChromeOptions>(platformType), windowSize, windowCustomSize);

                case Browser.InternetExplorer:
                    return GetWebDriver(DriverOptionsFactory.GetRemoteDriverOptions<InternetExplorerOptions>(platformType), windowSize, windowCustomSize);

                case Browser.Edge:
                    return GetWebDriver(DriverOptionsFactory.GetRemoteDriverOptions<EdgeOptions>(platformType), windowSize, windowCustomSize);

                case Browser.Safari:
                    return GetWebDriver(DriverOptionsFactory.GetRemoteDriverOptions<SafariOptions>(platformType), windowSize, windowCustomSize);

                default:
                    Exception ex = new PlatformNotSupportedException($"{browser} is not currently supported.");
                    Logger.Fatal("Invalid WebDriver Configuration requested.", ex);
                    throw ex;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DriverOptionsFactory?.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}