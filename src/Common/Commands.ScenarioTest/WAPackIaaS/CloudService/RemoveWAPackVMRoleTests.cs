﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Commands.ScenarioTest.WAPackIaaS.FunctionalTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class RemoveWAPackVMRoleTests : CmdletTestCloudServiceBase
    {
        public const string cmdletName = "Remove-WAPackVMRole";

        [TestInitialize]
        public void TestInitialize()
        {
            // Remove any existing VMRoles/CloudService
            this.VMRolePreTestCleanup();
            this.CloudServicePreTestCleanup();
        }

        [TestMethod]
        [TestCategory("WAPackIaaS-All")]
        [TestCategory("WAPackIaaS-Functional")]
        [TestCategory("WAPackIaaS-CloudService")]
        public void RemoveWAPackQuickCreateVMRole()
        {
            this.CreateVMRoleFromQuickCreate();
            var vmRoleToDelete = this.CreatedVMRolesFromQuickCreate.First();

            var inputParams = new Dictionary<string, object>()
            {
                {"VMRole", vmRoleToDelete},
                {"Force", null},
                {"PassThru", null}
            };
            var isDeleted = this.InvokeCmdlet(cmdletName, inputParams);
            Assert.AreEqual(1, isDeleted.Count);
            Assert.AreEqual(true, isDeleted.First());

            inputParams = new Dictionary<string, object>()
            {
                {"Name", vmRoleToDelete.Properties["Name"].Value}
            };
            var deletedCloudService = this.InvokeCmdlet(GetVMRoleCmdletName, inputParams, NonExistantResourceExceptionMessage);
            Assert.AreEqual(0, deletedCloudService.Count);

            this.CreatedVMRolesFromQuickCreate.Remove(vmRoleToDelete);
        }

        [TestMethod]
        [TestCategory("WAPackIaaS-All")]
        [TestCategory("WAPackIaaS-Functional")]
        [TestCategory("WAPackIaaS-CloudService")]
        public void RemoveWAPackVMRoleOnExistingCloudService()
        {
            this.CreateVMRoleFromCloudService();
            var vmRoleToDelete = this.CreatedVMRolesFromCloudService.First();

            var inputParams = new Dictionary<string, object>()
            {
                {"VMRole", vmRoleToDelete},
                {"CloudServiceName", this.CreatedCloudServices.First().Properties["Name"].Value},
                {"Force", null},
                {"PassThru", null}
            };
            var isDeleted = this.InvokeCmdlet(cmdletName, inputParams);
            Assert.AreEqual(1, isDeleted.Count);
            Assert.AreEqual(true, isDeleted.First());

            inputParams = new Dictionary<string, object>()
            {
                {"Name", vmRoleToDelete.Properties["Name"].Value}
            };
            var deletedCloudService = this.InvokeCmdlet(GetVMRoleCmdletName, inputParams, NonExistantResourceExceptionMessage);
            Assert.AreEqual(0, deletedCloudService.Count);

            this.CreatedVMRolesFromCloudService.Remove(vmRoleToDelete);
        }

        [TestCleanup]
        public void VMRoleCleanup()
        {
            this.RemoveVMRoles();
            this.RemoveCloudServices();
        }
    }
}
