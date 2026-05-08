namespace AttendanceSystemMobile.Data
{
    public static class DatabaseConfig
    {
        public const string Server = "attendanceapp-attendanceapp.c.aivencloud.com";
        public const string Port = "21012";
        public const string Database = "attendance_app";
        public const string User = "avnadmin";
        public const string Password = "AVNS_FSRe5gVUEziUMd7a52y";

        public static string GetConnectionString()
        {
            return $"Server={Server};Port={Port};Database={Database};User={User};Password={Password};";
        }
    }
}