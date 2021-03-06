//-----------------------------------------------------------------------------
// <auto-generated>
//     This file was generated by the C# SDK Code Generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------


using UnityEngine;
using System.Threading.Tasks;

using Unity.Services.Relay.Apis.Allocations;

using Unity.Services.Relay.Http;
using Unity.Services.Core.Internal;
using Unity.Services.Authentication.Internal;

namespace Unity.Services.Relay
{
    internal class RelayServiceProvider : IInitializablePackage
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            // Pass an instance of this class to Core
            var generatedPackageRegistry =
            CoreRegistry.Instance.RegisterPackage(new RelayServiceProvider());
                // And specify what components it requires, or provides.
            generatedPackageRegistry.DependsOn<IAccessToken>();
;
        }

        public Task Initialize(CoreRegistry registry)
        {
            var httpClient = new HttpClient();

            var accessTokenRelay = registry.GetServiceComponent<IAccessToken>();

            if (accessTokenRelay != null)
            {
                RelayService.Instance =
                    new InternalRelayService(httpClient, registry.GetServiceComponent<IAccessToken>());
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// InternalRelayService
    /// </summary>
    internal class InternalRelayService : IRelayService
    {
        /// <summary>
        /// Constructor for InternalRelayService
        /// </summary>
        /// <param name="httpClient">The HttpClient for InternalRelayService.</param>
        /// <param name="accessToken">The Authentication token for the service.</param>
        public InternalRelayService(HttpClient httpClient, IAccessToken accessToken = null)
        {
            
            AllocationsApi = new AllocationsApiClient(httpClient, accessToken);
            
            Configuration = new Configuration("https://relay-allocations.services.api.unity.com", 10, 4, null);
        }
        
        /// <summary> Instance of IAllocationsApiClient interface</summary>
        public IAllocationsApiClient AllocationsApi { get; set; }
        
        /// <summary> Configuration properties for the service.</summary>
        public Configuration Configuration { get; set; }
    }
}
