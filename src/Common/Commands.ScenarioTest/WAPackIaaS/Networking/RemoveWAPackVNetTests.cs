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
    public class RemoveWAPackVNetTests : CmdletTestNetworkingBase
    {
        public const string cmdletName = "Remove-WAPackVNet";

        [TestInitialize]
        public void TestInitialize()
        {
            // Remove any existing VNet
            this.NetworkingPreTestCleanup();

            // Create a FullVNet
            this.CreateFullVNet();
        }

        [TestMethod]
        [TestCategory("WAPackIaaS-All")]
        [TestCategory("WAPackIaaS-Functional")]
        [TestCategory("WAPackIaaS-Networking")]
        public void RemoveWAPackVNetDefault()
        {
            var vNetToDelete = this.CreatedVNet.First();

            var inputParams = new Dictionary<string, object>()
                {
                    {"VNet", vNetToDelete},
                    {"Force", null},
                    {"PassThru", null}
                };
            var isDeleted = this.InvokeCmdlet(cmdletName, inputParams);
            Assert.AreEqual(1, isDeleted.Count);
            Assert.AreEqual(true, isDeleted.First());

            inputParams = new Dictionary<string, object>()
            {
                {"Name", vNetToDelete.Properties["Name"].Value}
            };
            var deletedVNet = this.InvokeCmdlet(GetVNetCmdletName, inputParams);
            Assert.AreEqual(0, deletedVNet.Count);

            this.CreatedVNet.Remove(vNetToDelete);
        }

        [TestCleanup]
        public void VNetCleanup()
        {
            this.RemoveVNet();
        }
    }
}
