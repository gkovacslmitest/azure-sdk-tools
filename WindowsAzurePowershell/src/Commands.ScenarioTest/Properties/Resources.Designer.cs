﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Commands.ScenarioTest.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.WindowsAzure.Commands.ScenarioTest.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;TEST_CSM_ORGID_AUTHENTICATION&quot;:&quot;SubscriptionId=0a496442-635f-4974-bda4-2d339b9a6b3c;BaseUri=https://api-dogfood.resources.windows-int.net/;UserId=admin@aad18.ccsctp.net;AADAuthEndpoint=https://login.windows-ppe.net/&quot;,
        ///  &quot;TEST_ORGID_AUTHENTICATION&quot;:&quot;SubscriptionId=0a496442-635f-4974-bda4-2d339b9a6b3c;BaseUri=https://management-preview.core.windows-int.net/&quot;,
        ///}.
        /// </summary>
        internal static string CsmTestDummy {
            get {
                return ResourceManager.GetString("CsmTestDummy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;PublishData&gt;
        ///  &lt;PublishProfile
        ///    SchemaVersion=&quot;2.0&quot;
        ///    PublishMethod=&quot;AzureServiceManagementAPI&quot;&gt;
        ///    &lt;!-- this subscriptions contains same subscription id used for recording, and a dummy certificate--&gt;
        ///    &lt;Subscription
        ///      ServiceManagementUrl=&quot;https://management.core.windows.net&quot;
        ///      Id=&quot;2c224e7e-3ef5-431d-a57b-e71f4662e3a6&quot;
        ///      Name=&quot;Node CLI Test&quot;
        ///      ManagementCertificate=&quot;MIIC8jCCAdqgAwIBAgIQEtPvu8+LaLVIs/Kjh+uqSDANBgkqhkiG9w0BAQUFADAXMRUw [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RdfeTestDummy {
            get {
                return ResourceManager.GetString("RdfeTestDummy", resourceCulture);
            }
        }
    }
}
