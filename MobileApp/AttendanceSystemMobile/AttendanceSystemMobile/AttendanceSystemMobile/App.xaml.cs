using AttendanceSystemMobile.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;

namespace AttendanceSystemMobile
{
    public partial class App : Application
    {
        public static string? CurrentStudentId;
        public static string? CurrentStudentName;
        public static string? CurrentClass;

        private readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            _services = services;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            string studentId;
            try
            {
                studentId = Preferences.Get("StudentId", "");
            }
            catch
            {
                // Stored value was saved as int in a previous version — clear it
                Preferences.Remove("StudentId");
                Preferences.Remove("StudentName");
                Preferences.Remove("StudentClass");
                studentId = "";
            }

            if (!string.IsNullOrEmpty(studentId))
            {
                CurrentStudentId = studentId;
                CurrentStudentName = Preferences.Get("StudentName", "");
                CurrentClass = Preferences.Get("StudentClass", "");

                return new Window(new AppShell());
            }
            else
            {
                return new Window(new NavigationPage(_services.GetRequiredService<LoginPage>()));
            }
        }
    }
}
