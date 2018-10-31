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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AlexanderOnTest.NetCoreWebDriverFactory.MacOsTests
{
    [TestFixture]
    public class WebDriverFactoryInstanceTests
    {
        private IWebDriver Driver { get; set; }
        private readonly OSPlatform thisPlatform = OSPlatform.OSX;
        private string DriverPath => Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
        private IWebDriverFactory WebDriverFactory { get; set; }
        private IDriverOptionsFactory DriverOptionsFactory { get; set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            Assume.That(() => RuntimeInformation.IsOSPlatform(thisPlatform));
            WebDriverFactory = new DefaultWebDriverFactory();
            DriverOptionsFactory = new DefaultDriverOptionsFactory();
        }

        [Test]
        [TestCase(Browser.Firefox)]
        [TestCase(Browser.Chrome)]
        [TestCase(Browser.Safari)]
        public void LocalWebDriverCanBeLaunchedAndLoadExampleDotCom(Browser browser)
        {
            Driver = WebDriverFactory.GetLocalWebDriver(browser, browser == Browser.Safari ? null : DriverPath);
            Driver.Url = "https://example.com/";
            Driver.Title.Should().Be("Example Domain");
        }

        [Test]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Edge)]
        public void RequestingUnsupportedWebDriverThrowsInformativeException(Browser browser)
        {
            Action act = () => WebDriverFactory.GetLocalWebDriver(browser, DriverPath);
            act.Should()
                .Throw<PlatformNotSupportedException>($"because {browser} is not supported on {thisPlatform}.")
                .WithMessage("*is only available on*");
        }

        [Test]
        [TestCase(Browser.Firefox)]
        [TestCase(Browser.Chrome)]
        public void HeadlessBrowsersCanBeLaunched(Browser browser)
        {
            Driver = WebDriverFactory.GetLocalWebDriver(browser, DriverPath, true);
            Driver.Url = "https://example.com/";
            Driver.Title.Should().Be("Example Domain");
        }

        [Test]
        [TestCase(Browser.Edge)]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Safari)]
        public void RequestingUnsupportedHeadlessBrowserThrowsInformativeException(Browser browser)
        {
            Action act = () => WebDriverFactory.GetLocalWebDriver(browser, DriverPath, true);
            act.Should()
                .ThrowExactly<ArgumentException>($"because headless mode is not supported on {browser}.")
                .WithMessage($"Headless mode is not currently supported for {browser}.");
        }

        [Test]
        public void HdBrowserIsOfRequestedSize()
        {
            Driver = WebDriverFactory.GetLocalWebDriver(DriverOptionsFactory.GetFirefoxOptions(true), DriverPath, WindowSize.Hd);

            Assert.Multiple(() =>
            {
                Size size = Driver.Manage().Window.Size;
                size.Width.Should().Be(1366);
                size.Height.Should().Be(768);
            });
        }

        [Test]
        public void FhdBrowserIsOfRequestedSize()
        {
            Driver = WebDriverFactory.GetLocalWebDriver(DriverOptionsFactory.GetFirefoxOptions(true), DriverPath, WindowSize.Fhd);

            Assert.Multiple(() =>
            {
                Size size = Driver.Manage().Window.Size;
                size.Height.Should().Be(1080);
                size.Width.Should().Be(1920);
            });
        }

        [Test]
        [Explicit]
        [TestCase(Browser.Firefox)]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Edge)]
        [TestCase(Browser.Chrome)]
        [TestCase(Browser.Safari)]
        public void RemoteWebDriverCanBeLaunchedAndLoadExampleDotCom(Browser browser)
        {
            Driver = WebDriverFactory.GetRemoteWebDriver(browser, new Uri("http://192.168.0.200:4444/wd/hub"), PlatformType.Windows);
            Driver.Url = "https://example.com/";
            Driver.Title.Should().Be("Example Domain");
        }


        [TearDown]
        public void Teardown()
        {
            Driver?.Quit();
        }
    }
}
