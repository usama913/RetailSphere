using System.Net.Http.Json;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient(HttpClient httpClient) : IApiClient
{
    private const string BasePath = "api/v1";

    private async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string relativeUrl, TRequest body, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"{BasePath}/{relativeUrl}", body, cancellationToken);
        return await ReadEnvelopeAsync<TResponse>(response, cancellationToken);
    }

    private async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string relativeUrl, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"{BasePath}/{relativeUrl}", cancellationToken);
        return await ReadEnvelopeAsync<TResponse>(response, cancellationToken);
    }

    private static async Task<ApiResponse<TResponse>> ReadEnvelopeAsync<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>(cancellationToken);

        return envelope ?? ApiResponse<TResponse>.Fail(new ApiError
        {
            Code = "Client.EmptyResponse",
            Message = $"The server returned an empty response (HTTP {(int)response.StatusCode}).",
        });
    }
}
