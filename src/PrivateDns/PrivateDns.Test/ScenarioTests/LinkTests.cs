// ------------------------------------------------------------------------------------------------
// <copyright file="LinkTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Test.ScenarioTests
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

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestRegistrationLinkCreate()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-RegistrationLinkCreate");
        }
        
        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestLinkAlreadyExistsCreateThrow()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-LinkAlreadyExistsCreateThrow");
        }
        
        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestUpdateLinkRegistrationStatusWithPiping()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-UpdateLinkRegistrationStatusWithPiping");
        }
        
        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestUpdateLinkRegistrationStatusWithPipingResourceId()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-UpdateLinkRegistrationStatusWithPipingResourceId");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestUpdateLinkWithEtagMismatchThrow()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-UpdateLinkWithEtagMismatchThrow");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestUpdateLinkWithEtagMismatchOverwrite()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-UpdateLinkWithEtagMismatchOverwrite");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestUpdateLinkZoneNotExistsThrow()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-UpdateLinkZoneNotExistsThrow");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestUpdateLinkLinkNotExistsThrow()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-UpdateLinkLinkNotExistsThrow");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestUpdateLinkWithNoChangesShouldNotThrow()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-UpdateLinkWithNoChangesShouldNotThrow");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestGetLinkZoneNotExistsThrow()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-GetLinkZoneNotExistsThrow");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestGetLinkLinkNotExistsThrow()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-GetLinkLinkNotExistsThrow");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestRemoveLinkZoneNotExistsShouldNotThrow()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-RemoveLinkZoneNotExistsShouldNotThrow");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestRemoveLinkLinkNotExistsShouldNotThrow()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-RemoveLinkLinkNotExistsShouldNotThrow");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestListLinks()
        {
            PrivateDnsTestsBase.NewInstance.RunPowerShellTest(Logger, "Test-ListLinks");
        }
    }
}
