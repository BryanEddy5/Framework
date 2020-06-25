namespace HumanaEdge.Webcore.Framework.Web
{
    /// <summary>
    ///     Configuration keys for WebAPI applications.
    /// </summary>
    internal static class ConfigurationProperties
    {
        /// <summary>
        ///     Key to configuration entry that enabled / disables the authorization filter.
        /// </summary>
        internal const string StackdriverEnabledKey = "StackdriverOptions:IsEnabled";

        /// <summary>
        ///     Key to configuration entry that enables / disables authentication middleware.
        /// </summary>
        internal const string StackdriverProjectIdKey = "StackdriverOptions:ProjectId";
    }
}