namespace SmartWr.Ipos.Core.Settings
{
    public static class AppKeys
    {
        public static readonly string NameOrConnectionString = "name=IPosDbContext";
        public static readonly string MemoryCacheKey = "ipos_memory_cache";
        public static readonly string EmailTemplateLocation = "~/emailtemplates";
        public static readonly int DefaultCacheTime = 60;

        #region EmailTemplate

        public static readonly string AccountActivationTemplate = "";
        public static readonly string StockReorderTemplate = "";
        public static readonly string RegistrationTemplate = "";
        public static readonly string PasswordResetTemplate = "";
        public static readonly string AccountLockedTemplate = "";

        #endregion EmailTemplate
    }
}