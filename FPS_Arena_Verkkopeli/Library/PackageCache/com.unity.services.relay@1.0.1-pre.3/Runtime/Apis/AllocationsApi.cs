//-----------------------------------------------------------------------------
// <auto-generated>
//     This file was generated by the C# SDK Code Generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------


using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Relay.Models;
using Unity.Services.Relay.Http;
using Unity.Services.Authentication.Internal;
using Unity.Services.Relay.Allocations;

namespace Unity.Services.Relay.Apis.Allocations
{
    /// <summary>
    /// Interface for the AllocationsApiClient
    /// </summary>
    internal interface IAllocationsApiClient
    {
            /// <summary>
            /// Async Operation.
            /// Create Allocation.
            /// </summary>
            /// <param name="request">Request object for CreateAllocation.</param>
            /// <param name="operationConfiguration">Configuration for CreateAllocation.</param>
            /// <returns>Task for a Response object containing status code, headers, and Models.AllocateResponseBody object.</returns>
            /// <exception cref="Unity.Services.Relay.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
            Task<Response<Models.AllocateResponseBody>> CreateAllocationAsync(CreateAllocationRequest request, Configuration operationConfiguration = null);

            /// <summary>
            /// Async Operation.
            /// Create Join Code.
            /// </summary>
            /// <param name="request">Request object for CreateJoincode.</param>
            /// <param name="operationConfiguration">Configuration for CreateJoincode.</param>
            /// <returns>Task for a Response object containing status code, headers, and Models.JoinCodeResponseBody object.</returns>
            /// <exception cref="Unity.Services.Relay.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
            Task<Response<Models.JoinCodeResponseBody>> CreateJoincodeAsync(CreateJoincodeRequest request, Configuration operationConfiguration = null);

            /// <summary>
            /// Async Operation.
            /// Join Relay.
            /// </summary>
            /// <param name="request">Request object for JoinRelay.</param>
            /// <param name="operationConfiguration">Configuration for JoinRelay.</param>
            /// <returns>Task for a Response object containing status code, headers, and Models.JoinResponseBody object.</returns>
            /// <exception cref="Unity.Services.Relay.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
            Task<Response<Models.JoinResponseBody>> JoinRelayAsync(JoinRelayRequest request, Configuration operationConfiguration = null);

            /// <summary>
            /// Async Operation.
            /// List relay regions.
            /// </summary>
            /// <param name="request">Request object for ListRegions.</param>
            /// <param name="operationConfiguration">Configuration for ListRegions.</param>
            /// <returns>Task for a Response object containing status code, headers, and Models.RegionsResponseBody object.</returns>
            /// <exception cref="Unity.Services.Relay.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
            Task<Response<Models.RegionsResponseBody>> ListRegionsAsync(ListRegionsRequest request, Configuration operationConfiguration = null);

    }

    ///<inheritdoc cref="IAllocationsApiClient"/>
    internal class AllocationsApiClient : BaseApiClient, IAllocationsApiClient
    {
        private IAccessToken _accessToken;
        private const int _baseTimeout = 10;
        private Configuration _configuration;
        /// <summary>
        /// Accessor for the client configuration object. This returns a merge
        /// between the current configuration and the global configuration to
        /// ensure the correct combination of headers and a base path (if it is
        /// set) are returned.
        /// </summary>
        public Configuration Configuration
        {
            get {
                // We return a merge between the current configuration and the
                // global configuration to ensure we have the correct
                // combination of headers and a base path (if it is set).
                Configuration globalConfiguration = new Configuration("https://relay-allocations.services.api.unity.com", 10, 4, null);
                if (RelayService.Instance != null)
                {
                    globalConfiguration = RelayService.Instance.Configuration;
                }
                return Configuration.MergeConfigurations(_configuration, globalConfiguration);
            }
        }
        
        /// <summary>
        /// AllocationsApiClient Constructor.
        /// </summary>
        /// <param name="httpClient">The HttpClient for AllocationsApiClient.</param>
        /// <param name="accessToken">The Authentication token for the client.</param>
        /// <param name="configuration"> AllocationsApiClient Configuration object.</param>
        public AllocationsApiClient(IHttpClient httpClient,
            IAccessToken accessToken,
            Configuration configuration = null) : base(httpClient)
        {
            // We don't need to worry about the configuration being null at
            // this stage, we will check this in the accessor.
            _configuration = configuration;

            _accessToken = accessToken;
        }


        /// <summary>
        /// Async Operation.
        /// Create Allocation.
        /// </summary>
        /// <param name="request">Request object for CreateAllocation.</param>
        /// <param name="operationConfiguration">Configuration for CreateAllocation.</param>
        /// <returns>Task for a Response object containing status code, headers, and Models.AllocateResponseBody object.</returns>
        /// <exception cref="Unity.Services.Relay.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
        public async Task<Response<Models.AllocateResponseBody>> CreateAllocationAsync(CreateAllocationRequest request,
            Configuration operationConfiguration = null)
        {
            var statusCodeToTypeMap = new Dictionary<string, System.Type>() { {"201", typeof(Models.AllocateResponseBody)   },{"400", typeof(Models.ErrorResponseBody)   },{"401", typeof(Models.ErrorResponseBody)   },{"403", typeof(Models.ErrorResponseBody)   },{"500", typeof(Models.ErrorResponseBody)   },{"503", typeof(Models.ErrorResponseBody)   } };
            
            // Merge the operation/request level configuration with the client level configuration.
            var finalConfiguration = Configuration.MergeConfigurations(operationConfiguration, Configuration);

            var response = await HttpClient.MakeRequestAsync("POST",
                request.ConstructUrl(finalConfiguration.BasePath),
                request.ConstructBody(),
                request.ConstructHeaders(_accessToken, finalConfiguration),
                finalConfiguration.RequestTimeout ?? _baseTimeout);

            var handledResponse = ResponseHandler.HandleAsyncResponse<Models.AllocateResponseBody>(response, statusCodeToTypeMap);
            return new Response<Models.AllocateResponseBody>(response, handledResponse);
        }


        /// <summary>
        /// Async Operation.
        /// Create Join Code.
        /// </summary>
        /// <param name="request">Request object for CreateJoincode.</param>
        /// <param name="operationConfiguration">Configuration for CreateJoincode.</param>
        /// <returns>Task for a Response object containing status code, headers, and Models.JoinCodeResponseBody object.</returns>
        /// <exception cref="Unity.Services.Relay.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
        public async Task<Response<Models.JoinCodeResponseBody>> CreateJoincodeAsync(CreateJoincodeRequest request,
            Configuration operationConfiguration = null)
        {
            var statusCodeToTypeMap = new Dictionary<string, System.Type>() { {"200", typeof(Models.JoinCodeResponseBody)   },{"201", typeof(Models.JoinCodeResponseBody)   },{"400", typeof(Models.ErrorResponseBody)   },{"401", typeof(Models.ErrorResponseBody)   },{"403", typeof(Models.ErrorResponseBody)   },{"500", typeof(Models.ErrorResponseBody)   } };
            
            // Merge the operation/request level configuration with the client level configuration.
            var finalConfiguration = Configuration.MergeConfigurations(operationConfiguration, Configuration);

            var response = await HttpClient.MakeRequestAsync("POST",
                request.ConstructUrl(finalConfiguration.BasePath),
                request.ConstructBody(),
                request.ConstructHeaders(_accessToken, finalConfiguration),
                finalConfiguration.RequestTimeout ?? _baseTimeout);

            var handledResponse = ResponseHandler.HandleAsyncResponse<Models.JoinCodeResponseBody>(response, statusCodeToTypeMap);
            return new Response<Models.JoinCodeResponseBody>(response, handledResponse);
        }


        /// <summary>
        /// Async Operation.
        /// Join Relay.
        /// </summary>
        /// <param name="request">Request object for JoinRelay.</param>
        /// <param name="operationConfiguration">Configuration for JoinRelay.</param>
        /// <returns>Task for a Response object containing status code, headers, and Models.JoinResponseBody object.</returns>
        /// <exception cref="Unity.Services.Relay.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
        public async Task<Response<Models.JoinResponseBody>> JoinRelayAsync(JoinRelayRequest request,
            Configuration operationConfiguration = null)
        {
            var statusCodeToTypeMap = new Dictionary<string, System.Type>() { {"200", typeof(Models.JoinResponseBody)   },{"400", typeof(Models.ErrorResponseBody)   },{"401", typeof(Models.ErrorResponseBody)   },{"403", typeof(Models.ErrorResponseBody)   },{"404", typeof(Models.ErrorResponseBody)   },{"500", typeof(Models.ErrorResponseBody)   } };
            
            // Merge the operation/request level configuration with the client level configuration.
            var finalConfiguration = Configuration.MergeConfigurations(operationConfiguration, Configuration);

            var response = await HttpClient.MakeRequestAsync("POST",
                request.ConstructUrl(finalConfiguration.BasePath),
                request.ConstructBody(),
                request.ConstructHeaders(_accessToken, finalConfiguration),
                finalConfiguration.RequestTimeout ?? _baseTimeout);

            var handledResponse = ResponseHandler.HandleAsyncResponse<Models.JoinResponseBody>(response, statusCodeToTypeMap);
            return new Response<Models.JoinResponseBody>(response, handledResponse);
        }


        /// <summary>
        /// Async Operation.
        /// List relay regions.
        /// </summary>
        /// <param name="request">Request object for ListRegions.</param>
        /// <param name="operationConfiguration">Configuration for ListRegions.</param>
        /// <returns>Task for a Response object containing status code, headers, and Models.RegionsResponseBody object.</returns>
        /// <exception cref="Unity.Services.Relay.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
        public async Task<Response<Models.RegionsResponseBody>> ListRegionsAsync(ListRegionsRequest request,
            Configuration operationConfiguration = null)
        {
            var statusCodeToTypeMap = new Dictionary<string, System.Type>() { {"200", typeof(Models.RegionsResponseBody)   },{"400", typeof(Models.ErrorResponseBody)   },{"401", typeof(Models.ErrorResponseBody)   },{"403", typeof(Models.ErrorResponseBody)   },{"500", typeof(Models.ErrorResponseBody)   } };
            
            // Merge the operation/request level configuration with the client level configuration.
            var finalConfiguration = Configuration.MergeConfigurations(operationConfiguration, Configuration);

            var response = await HttpClient.MakeRequestAsync("GET",
                request.ConstructUrl(finalConfiguration.BasePath),
                request.ConstructBody(),
                request.ConstructHeaders(_accessToken, finalConfiguration),
                finalConfiguration.RequestTimeout ?? _baseTimeout);

            var handledResponse = ResponseHandler.HandleAsyncResponse<Models.RegionsResponseBody>(response, statusCodeToTypeMap);
            return new Response<Models.RegionsResponseBody>(response, handledResponse);
        }

    }
}