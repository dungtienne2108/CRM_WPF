namespace CRM.Shared.Constants
{
    public static class AppConstants
    {
        public const string ApplicationName = "CRM System";
        public const string Version = "1.0.0";

        public static class DateFormats
        {
            public const string Default = "dd/MM/yyyy";
            public const string DateTime = "dd/MM/yyyy HH:mm";
            public const string DateTimeFull = "dd/MM/yyyy HH:mm:ss";
        }

        public static class Roles
        {
            //public const string Administrator = "Administrator";
            //public const string Manager = "Manager";
            //public const string SalesRep = "SalesRep";
            //public const string User = "User";
        }

        public static class Pagination
        {
            public const int DefaultPageSize = 20;
            public const int MaxPageSize = 100;
        }
    }
}
