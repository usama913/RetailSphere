namespace RetailSphere.UI.Clients;

/// <summary>
/// Typed API client. Feature-area methods are added as partial-interface/partial-class
/// pairs (IApiClient.Auth.cs + ApiClient.Auth.cs, IApiClient.Catalog.cs + ApiClient.Catalog.cs, ...)
/// — one HttpClient, partitioned by feature, matching the ServerApiClient.&lt;Feature&gt;.cs
/// pattern already used in Platform.UI, instead of one client per module.
/// </summary>
public partial interface IApiClient;
