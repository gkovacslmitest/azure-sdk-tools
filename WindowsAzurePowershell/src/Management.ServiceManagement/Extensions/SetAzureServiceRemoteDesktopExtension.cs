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

namespace Microsoft.WindowsAzure.Management.ServiceManagement.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Security.Cryptography.X509Certificates;
    using WindowsAzure.Management.ServiceManagement.Helpers;
    using WindowsAzure.Management.Utilities.CloudService;
    using WindowsAzure.Management.Utilities.Common;
    using WindowsAzure.ServiceManagement;

    /// <summary>
    /// Set Windows Azure Service Remote Desktop Extension.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "AzureServiceRemoteDesktopExtension"), OutputType(typeof(ManagementOperationContext))]
    public class SetAzureServiceRemoteDesktopExtensionCommand : BaseAzureServiceRemoteDesktopExtensionCmdlet
    {
        public SetAzureServiceRemoteDesktopExtensionCommand()
            : base()
        {
        }

        public SetAzureServiceRemoteDesktopExtensionCommand(IServiceManagement channel)
            : base(channel)
        {
        }

        [Parameter(Position = 0, Mandatory = false, ParameterSetName = "SetExtension", HelpMessage = "Cloud Service Name")]
        [Parameter(Position = 0, Mandatory = false, ParameterSetName = "SetExtensionUsingThumbprint", HelpMessage = "Cloud Service Name")]
        [ValidateNotNullOrEmpty]
        public override string ServiceName
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "SetExtension", HelpMessage = "Production (default) or Staging.")]
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "SetExtensionUsingThumbprint", HelpMessage = "Production (default) or Staging.")]
        [ValidateSet(DeploymentSlotType.Production, DeploymentSlotType.Staging, IgnoreCase = true)]
        public string Slot
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "SetExtension", HelpMessage = "Default All Roles, or specify ones for Named Roles.")]
        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "SetExtensionUsingThumbprint", HelpMessage = "Default All Roles, or specify ones for Named Roles.")]
        [ValidateNotNullOrEmpty]
        public string[] Roles
        {
            get;
            set;
        }

        [Parameter(Position = 3, Mandatory = true, ParameterSetName = "SetExtension", HelpMessage = "Remote Desktop User Name")]
        [Parameter(Position = 3, Mandatory = true, ParameterSetName = "SetExtensionUsingThumbprint", HelpMessage = "Remote Desktop User Name")]
        [ValidateNotNullOrEmpty]
        public string UserName
        {
            get;
            set;
        }

        [Parameter(Position = 4, Mandatory = true, ParameterSetName = "SetExtension", HelpMessage = "Remote Desktop User Password")]
        [Parameter(Position = 4, Mandatory = true, ParameterSetName = "SetExtensionUsingThumbprint", HelpMessage = "Remote Desktop User Password")]
        [ValidateNotNullOrEmpty]
        public string Password
        {
            get;
            set;
        }

        [Parameter(Position = 5, Mandatory = false, ParameterSetName = "SetExtension", HelpMessage = "Remote Desktop User Expiration Date")]
        [Parameter(Position = 5, Mandatory = false, ParameterSetName = "SetExtensionUsingThumbprint", HelpMessage = "Remote Desktop User Expiration Date")]
        [ValidateNotNullOrEmpty]
        public DateTime Expiration
        {
            get;
            set;
        }

        [Parameter(Position = 6, Mandatory = false, ParameterSetName = "SetExtension", HelpMessage = "X509Certificate used to encrypt password.")]
        [ValidateNotNullOrEmpty]
        public X509Certificate2 X509Certificate
        {
            get;
            set;
        }

        [Parameter(Position = 6, Mandatory = true, ParameterSetName = "SetExtensionUsingThumbprint", HelpMessage = "Thumbprint of a certificate used for encryption.")]
        [ValidateNotNullOrEmpty]
        public string Thumbprint
        {
            get;
            set;
        }

        [Parameter(Position = 7, Mandatory = true, ParameterSetName = "SetExtensionUsingThumbprint", HelpMessage = "ThumbprintAlgorithm associated with the Thumbprint.")]
        [ValidateNotNullOrEmpty]
        public string ThumbprintAlgorithm
        {
            get;
            set;
        }

        private string ExpirationStr
        {
            get
            {
                return Expiration.ToString("yyyy-MM-dd");
            }
        }

        private Deployment Deployment
        {
            get;
            set;
        }

        private bool InstallExtension(string extensionId)
        {
            return InstallExtension(extensionId, Thumbprint, ThumbprintAlgorithm);
        }

        private bool InstallExtension(string extensionId, string thumbprint, string thumbprintAlgorithm)
        {
            HostedServiceExtension extension = ExtensionManager.GetExtension(extensionId);
            if (extension != null)
            {
                if (CheckExtensionType(extension))
                {
                    ExtensionManager.DeleteHostedServieExtension(extensionId);
                }
                else
                {
                    WriteExceptionError(new Exception("An extension with ID: " + extensionId
                        + " and Type: " + extension.Type + " already exists. It cannot be overwritten using this command."));
                    return false;
                }
            }

            ExtensionManager.AddHostedServiceExtension(new HostedServiceExtensionInput
            {
                Id = extensionId,
                Thumbprint = thumbprint,
                ThumbprintAlgorithm = thumbprintAlgorithm,
                ProviderNameSpace = ExtensionNameSpace,
                Type = ExtensionType,
                PublicConfiguration = string.Format(PublicConfigurationXmlTemplate.ToString(), UserName, ExpirationStr),
                PrivateConfiguration = string.Format(PrivateConfigurationXmlTemplate.ToString(), Password)
            });
            return true;
        }

        private bool InstallExtension(out ExtensionConfiguration extConfig)
        {
            extConfig = ExtensionManager.NewExtensionConfig(Deployment);

            bool installed = false;
            if (Roles != null && Roles.Any())
            {
                foreach (string roleName in Roles)
                {
                    for (int i = 0; i < ExtensionIdLiveCycleCount && !installed; i++)
                    {
                        string roleExtensionId = string.Format(ExtensionIdTemplate, roleName, Slot, i);
                        if (!ExtensionManager.ExistExtension(extConfig, roleName, roleExtensionId))
                        {
                            if (!string.IsNullOrEmpty(Thumbprint))
                            {
                                installed = InstallExtension(roleExtensionId);
                            }
                            else
                            {
                                HostedServiceExtension existingRoleExtension = ExtensionManager.GetExtension(roleExtensionId);
                                if (existingRoleExtension == null)
                                {
                                    for (int j = 0; j < ExtensionIdLiveCycleCount && existingRoleExtension == null; j++)
                                    {
                                        if (j != i)
                                        {
                                            string otherRoleExtensionId = string.Format(ExtensionIdTemplate, "Default", Slot, j);
                                            existingRoleExtension = ExtensionManager.GetExtension(otherRoleExtensionId);
                                        }
                                    }
                                }

                                if (existingRoleExtension != null)
                                {
                                    installed = InstallExtension(roleExtensionId, existingRoleExtension.Thumbprint, existingRoleExtension.ThumbprintAlgorithm);
                                }
                                else
                                {
                                    installed = InstallExtension(roleExtensionId);
                                }
                            }

                            if (installed)
                            {
                                extConfig = ExtensionManager.RemoveExtension(extConfig, roleName, ExtensionNameSpace, ExtensionType);
                                extConfig = ExtensionManager.AddExtension(extConfig, roleName, roleExtensionId);
                                WriteObject("Setting remote desktop configuration for " + roleName + ".");
                            }
                        }
                    }

                    if (!installed)
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < ExtensionIdLiveCycleCount && !installed; i++)
                {
                    string defaultExtensionId = string.Format(ExtensionIdTemplate, "Default", Slot, i);
                    if (!ExtensionManager.ExistDefaultExtension(extConfig, defaultExtensionId))
                    {
                        if (!string.IsNullOrEmpty(Thumbprint))
                        {
                            installed = InstallExtension(defaultExtensionId);
                        }
                        else
                        {
                            HostedServiceExtension existingDefaultExtension = ExtensionManager.GetExtension(defaultExtensionId);
                            if (existingDefaultExtension == null)
                            {
                                for (int j = 0; j < ExtensionIdLiveCycleCount && existingDefaultExtension == null; j++)
                                {
                                    if (j != i)
                                    {
                                        string otherDefaultExtensionId = string.Format(ExtensionIdTemplate, "Default", Slot, j);
                                        existingDefaultExtension = ExtensionManager.GetExtension(otherDefaultExtensionId);
                                    }
                                }
                            }

                            if (existingDefaultExtension != null)
                            {
                                installed = InstallExtension(defaultExtensionId, existingDefaultExtension.Thumbprint, existingDefaultExtension.ThumbprintAlgorithm);
                            }
                            else
                            {
                                installed = InstallExtension(defaultExtensionId);
                            }
                        }

                        if (installed)
                        {
                            extConfig = ExtensionManager.RemoveDefaultExtension(extConfig, ExtensionNameSpace, ExtensionType);
                            extConfig = ExtensionManager.AddDefaultExtension(extConfig, defaultExtensionId);
                            WriteObject("Setting default remote desktop configuration for all roles.");
                        }
                    }
                }
            }
            return installed;
        }

        private bool ValidateParameters()
        {
            string serviceName;
            ServiceSettings settings = General.GetDefaultSettings(General.TryGetServiceRootPath(CurrentPath()), ServiceName, null, null, null, null, CurrentSubscription.SubscriptionId, out serviceName);

            if (string.IsNullOrEmpty(serviceName))
            {
                WriteExceptionError(new Exception("Invalid service name"));
                return false;
            }
            else
            {
                ServiceName = serviceName;
            }

            if (!IsServiceAvailable(ServiceName))
            {
                WriteExceptionError(new Exception("Service not found: " + ServiceName));
                return false;
            }

            Slot = string.IsNullOrEmpty(Slot) ? DeploymentSlotType.Production : Slot;

            Deployment = Channel.GetDeploymentBySlot(CurrentSubscription.SubscriptionId, ServiceName, Slot);
            if (Deployment == null)
            {
                WriteExceptionError(new Exception(string.Format("Deployment not found in service: {0} and slot: {1}", ServiceName, Slot)));
                return false;
            }

            if (Deployment.ExtensionConfiguration == null)
            {
                Deployment.ExtensionConfiguration = new ExtensionConfiguration
                {
                    AllRoles = new AllRoles(),
                    NamedRoles = new NamedRoles()
                };
            }

            if (Roles != null)
            {
                foreach (string roleName in Roles)
                {
                    if (Deployment.RoleList == null || !Deployment.RoleList.Any(r => r.RoleName == roleName))
                    {
                        WriteExceptionError(new Exception(string.Format("Role: {0} not found in deployment {1} of service {2}.", roleName, Slot, ServiceName)));
                        return false;
                    }
                }
            }

            Expiration = Expiration.Equals(default(DateTime)) ? DateTime.Now.AddMonths(6) : Expiration;

            if (X509Certificate != null)
            {
                var operationDescription = string.Format("{0} - Uploading Certificate: {1}", CommandRuntime, X509Certificate.Thumbprint);
                ExecuteClientActionInOCS(null, operationDescription, s => this.Channel.AddCertificates(s, this.ServiceName, CertUtils.Create(X509Certificate)));
                Thumbprint = X509Certificate.Thumbprint;
                ThumbprintAlgorithm = X509Certificate.SignatureAlgorithm.FriendlyName;
            }
            else if (!string.IsNullOrEmpty(Thumbprint))
            {
                ThumbprintAlgorithm = string.IsNullOrEmpty(ThumbprintAlgorithm) ? "sha1" : ThumbprintAlgorithm;
            }
            else
            {
                Thumbprint = "";
                ThumbprintAlgorithm = string.IsNullOrEmpty(ThumbprintAlgorithm) ? "" : ThumbprintAlgorithm;
            }

            ExtensionManager = new HostedServiceExtensionManager(Channel, CurrentSubscription.SubscriptionId, ServiceName);

            return true;
        }

        private void ChangeDeployment(ExtensionConfiguration extConfig)
        {
            ChangeConfigurationInput changeConfigInput = new ChangeConfigurationInput
            {
                Configuration = Deployment.Configuration,
                ExtendedProperties = Deployment.ExtendedProperties,
                ExtensionConfiguration = Deployment.ExtensionConfiguration = extConfig,
                Mode = "Auto",
                TreatWarningsAsError = false
            };
            ExecuteClientActionInOCS(null, CommandRuntime.ToString(), s => Channel.ChangeConfigurationBySlot(s, ServiceName, Slot, changeConfigInput));
        }

        private void ExecuteCommand()
        {
            ValidateParameters();
            ExtensionConfiguration extConfig = ExtensionManager.NewExtensionConfig();
            InstallExtension(out extConfig);
            ChangeDeployment(extConfig);
        }

        protected override void OnProcessRecord()
        {
            ExecuteCommand();
        }
    }
}
