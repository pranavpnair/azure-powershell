// ------------------------------------------------------------------------------------------------
// <copyright file="ZoneTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Test.ScenarioTests
{
    using Microsoft.Azure.ServiceManagement.Common.Models;
    using Microsoft.WindowsAzure.Commands.ScenarioTest;
    using Xunit;

    public class ZoneTests : PrivateDnsTestsBase
    {
        public XunitTracingInterceptor _logger;

        public ZoneTests(Xunit.Abstractions.ITestOutputHelper output)
        {
            _logger = new XunitTracingInterceptor(output);
            XunitTracingInterceptor.AddToContext(_logger);
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneCrud()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneCrud");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneCrudTrimsDot()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneCrudTrimsDot");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneCrudWithPiping()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneCrudWithPiping");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneSetUsingResourceId()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneSetUsingResourceId");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneRemoveUsingResourceId()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneRemoveUsingResourceId");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneCrudWithPipingTrimsDot()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneCrudWithPipingTrimsDot");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneList()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneList");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneListSubscription()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneListSubscription");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneNewAlreadyExists()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneNewAlreadyExists");
        }

        [Fact()]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneSetEtagMismatch()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneSetEtagMismatch");
        }

        [Fact()]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneSetNotFound()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneSetNotFound");
        }

        [Fact()]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneRemoveEtagMismatch()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneRemoveEtagMismatch");
        }

        [Fact()]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestZoneRemoveNotFound()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(_logger, "Test-ZoneRemoveNonExisting");
        }
    }
}
