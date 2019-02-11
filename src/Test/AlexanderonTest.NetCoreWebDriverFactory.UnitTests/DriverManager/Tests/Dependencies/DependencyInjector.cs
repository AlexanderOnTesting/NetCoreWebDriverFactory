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
using System.Reflection;
using AlexanderOnTest.NetCoreWebDriverFactory;
using AlexanderonTest.NetCoreWebDriverFactory.UnitTests.Settings;
using AlexanderOnTest.WebDriverFactoryNunitConfig.TestSettings;
using Microsoft.Extensions.DependencyInjection;

namespace AlexanderonTest.NetCoreWebDriverFactory.UnitTests.DriverManager.Tests.Dependencies
{
    internal static class DependencyInjector
    {
        public static IServiceProvider GetServiceProvider()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton(WebDriverSettings.WebDriverConfiguration);

            services.AddSingleton(new DriverPath(Assembly.GetExecutingAssembly()));

            // Allow a setting to override the default FakeWebDriveFactory
            Type webDriverType = TestSettings.UseRealWebDriver
                ? typeof(DefaultWebDriverFactory)
                : typeof(FakeWebDriverFactory);

            services.AddTransient(typeof(IWebDriverFactory), webDriverType);

            services.AddTransient(typeof(IWebDriverManager), typeof(WebDriverManager));
            
            return services.BuildServiceProvider();
        }
    }
}