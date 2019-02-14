// ------------------------------------------------------------------------------------------------
// <copyright file="LinkTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Tests.ScenarioTests
{
    using Microsoft.Azure.ServiceManagement.Common.Models;
    using Microsoft.WindowsAzure.Commands.ScenarioTest;
    using Xunit;

    public class LinkTests : PrivateDnsTestsBase
    {
        public XunitTracingInterceptor Logger;

        public LinkTests(Xunit.Abstractions.ITestOutputHelper output)
        {
            Logger = new XunitTracingInterceptor(output);
            XunitTracingInterceptor.AddToContext(Logger);
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestLinkCrud()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-LinkCrud");
        }
        /*
        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneCrudTrimsDot()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneCrudTrimsDot");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneCrudWithPiping()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneCrudWithPiping");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneSetUsingResourceId()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneSetUsingResourceId");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneRemoveUsingResourceId()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneRemoveUsingResourceId");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneCrudWithPipingTrimsDot()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneCrudWithPipingTrimsDot");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneList()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneList");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneListSubscription()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneListSubscription");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneNewAlreadyExists()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneNewAlreadyExists");
        }

        [Fact()]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneSetEtagMismatch()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneSetEtagMismatch");
        }

        [Fact()]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneSetNotFound()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneSetNotFound");
        }

        [Fact()]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneRemoveEtagMismatch()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneRemoveEtagMismatch");
        }

        [Fact()]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneRemoveNotFound()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ZoneRemoveNonExisting");
        }
        */
    }
}
