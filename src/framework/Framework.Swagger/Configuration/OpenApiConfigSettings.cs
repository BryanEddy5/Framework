namespace HumanaEdge.Webcore.Framework.Swagger.Configuration
{
    /// <summary>
    /// Most of these OpenApiOptions are driven from appsettings.json.
    /// </summary>
    internal sealed class OpenApiConfigSettings
    {
        /// <summary>
        /// Configuration Key - eg node to bind to in appsettings.json.
        /// </summary>
        public const string ConfigSettingsKey = "OpenApi";

        /// <summary>
        /// Non-prod url - second entry in https://swagger.io/docs/specification/api-host-and-base-path/
        /// Default: https://apigw-np.humanaedge.com/api
        /// .
        /// </summary>
        private const string NonProdServerBaseUrl = "https://apigw-np.humanaedge.com/api";

        /// <summary>
        /// Prod url - second entry in https://swagger.io/docs/specification/api-host-and-base-path/
        /// Default: https://apigw.humanaedge.com/api
        /// .
        /// </summary>
        private const string ProductionServerBaseUrl = "https://apigw.humanaedge.com/api";

        /// <summary>
        /// Additional html appended to swagger info.description.
        /// </summary>
        public string AdditionalDescription { get; set; } = "ProvideAdditionalDescriptionValueInAppSettings.json";

        /// <summary>
        /// The document title that appears on the browser tab. Used when configuring SwaggerUI.
        /// </summary>
        /// <example>"cxp".</example>
        public string DocumentTitle { get; set; } = "ProvideDocumentTitleValueInAppSettings.json";

        /// <summary>
        /// Prod url to the deployed service that's the first entry in
        /// https://swagger.io/docs/specification/api-host-and-base-path/
        /// Default: https://apigw-np.humanaedge.com/api
        /// .
        /// </summary>
        public string NonProdServerBaseAndSuffix => $"{NonProdServerBaseUrl}/{ServiceProductSuffix}";

        /// <summary>
        /// Prod url to the deployed service that's the first entry in
        /// https://swagger.io/docs/specification/api-host-and-base-path/
        /// Default: https://apigw-np.humanaedge.com/api
        /// .
        /// </summary>
        public string ProductionServerBaseAndSuffix => $"{ProductionServerBaseUrl}/{ServiceProductSuffix}";

        /// <summary>
        /// URL for the API.md for the service (eg
        /// https://gitlab.humanaedge.com/cxp/ah-cxp-enrollment-entity-service/-/blob/master/API.md).
        /// </summary>
        public string ServiceDocumentationUrl { get; set; } = "ProvideServiceDocumentationUrlValueInAppSettings.json";

        /// <summary>
        /// The suffix that gets appended to 'server.url' values when constructing a path
        /// ie: {server.url}/{ServiceProductSuffix}/{action}
        /// .
        /// </summary>
        /// <example>"cxp".</example>
        public string ServiceProductSuffix { get; set; } = "ProvideServiceProductSuffixValueInAppSettings.json";
    }
}