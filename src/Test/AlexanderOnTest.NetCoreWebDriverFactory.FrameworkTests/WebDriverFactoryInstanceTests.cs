// <copyright>
// Copyright 2018 Alexander Dunn
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
using System.Runtime.InteropServices;
using AlexanderonTest.NetCoreWebDriverFactory.Lib.Test;
using AlexanderOnTest.NetCoreWebDriverFactory.DriverOptionsFactory;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AlexanderOnTest.NetCoreWebDriverFactory.FrameworkTests
{
    [TestFixture]
    public class WebDriverFactoryInstanceTests
    {
        private static readonly OSPlatform ThisPlatform = OSPlatform.Windows;
        private static readonly Uri GridUrl = new Uri("http://192.168.0.200:4444/wd/hub");

        private IWebDriver Driver { get; set; }
        private IWebDriverFactory WebDriverFactory { get; set; }
        private IDriverOptionsFactory DriverOptionsFactory { get; set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            Assume.That(() => RuntimeInformation.IsOSPlatform(ThisPlatform));
            DriverOptionsFactory = new DefaultDriverOptionsFactory();
            WebDriverFactory = new FrameworkWebDriverFactory(GridUrl, DriverOptionsFactory);
        }
        
        [Test]
        [TestCase(Browser.Chrome, BrowserVisibility.OnScreen)]
        [TestCase(Browser.Edge, BrowserVisibility.OnScreen)]
        [TestCase(Browser.Firefox, BrowserVisibility.OnScreen)]
        [TestCase(Browser.InternetExplorer, BrowserVisibility.OnScreen)]
        [TestCase(Browser.Chrome, BrowserVisibility.Headless)]
        [TestCase(Browser.Firefox, BrowserVisibility.Headless)]
        public void LocalWebDriverFactoryWorks(Browser browser, BrowserVisibility browserVisibility)
        {
            Driver = WebDriverFactory.GetLocalWebDriver(browser, WindowSize.Hd, browserVisibility == BrowserVisibility.Headless);
            Assertions.AssertThatPageCanBeLoaded(Driver);
        }

        [Test]
        [TestCase(PlatformType.Linux, Browser.Chrome)]
        [TestCase(PlatformType.Linux, Browser.Firefox)]
        [TestCase(PlatformType.Mac, Browser.Chrome)]
        [TestCase(PlatformType.Mac, Browser.Firefox)]
        [TestCase(PlatformType.Mac, Browser.Safari)]
        [TestCase(PlatformType.Windows, Browser.Chrome)]
        [TestCase(PlatformType.Windows, Browser.Edge)]
        [TestCase(PlatformType.Windows, Browser.Firefox)]
        [TestCase(PlatformType.Windows, Browser.InternetExplorer)]
        public void RemoteWebDriverFactoryWorks(PlatformType platformType, Browser browser)
        {
            Driver = WebDriverFactory.GetRemoteWebDriver(browser, platformType);
            Assertions.AssertThatPageCanBeLoaded(Driver);
        }

        [Test]
        [TestCase(WindowSize.Hd, 1366, 768)]
        [TestCase(WindowSize.Fhd, 1920, 1080)]
        public void BrowserIsOfRequestedSize(WindowSize windowSize, int expectedWidth, int expectedHeight)
        {
            Driver = WebDriverFactory.GetLocalWebDriver(Browser.Firefox, windowSize, true);
            Assertions.AssertThatBrowserWindowSizeIsCorrect(Driver, expectedWidth, expectedHeight);
        }

        [Test]
        [TestCase(Browser.Safari)]
        public void RequestingUnsupportedWebDriverThrowsInformativeException(Browser browser)
        {
            Assertions.AssertThatRequestingAnUnsupportedBrowserThrowsCorrectException(Act, browser, ThisPlatform);

            void Act() => WebDriverFactory.GetLocalWebDriver(browser);
        }

        [Test]
        [TestCase(Browser.Edge)]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Safari)]
        public void RequestingUnsupportedHeadlessBrowserThrowsInformativeException(Browser browser)
        {
            Assertions.AssertThatRequestingAnUnsupportedHeadlessBrowserThrowsCorrectException(Act, browser);

            void Act() => WebDriverFactory.GetLocalWebDriver(browser, WindowSize.Hd, true);
        }

        [TearDown]
        public void Teardown()
        {
            Driver?.Quit();
        }
    }
}