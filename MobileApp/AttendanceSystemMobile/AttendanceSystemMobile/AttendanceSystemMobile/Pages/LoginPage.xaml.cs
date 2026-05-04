namespace AttendanceSystemMobile.Pages;
using AttendanceSystemMobile.Services;
using Microsoft.Maui.Storage;


public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _db;

    public LoginPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;

        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var user = await _db.LoginAsync(ENumberEntry.Text, PasswordEntry.Text);

        if (user != null)
        {
            var classes = await _db.GetClassesByStudentAsync(user.Enum);
            var className = classes.FirstOrDefault()?.ClassName ?? "";

            App.CurrentStudentId = user.Enum;
            App.CurrentStudentName = $"{user.FirstName} {user.LastName}";
            App.CurrentClass = className;

            Preferences.Set("StudentId", user.Enum);
            Preferences.Set("StudentName", $"{user.FirstName} {user.LastName}");
            Preferences.Set("StudentClass", className);

            Application.Current.Windows[0].Page = new AppShell();
        }
        else
        {
            ErrorLabel.Text = "Invalid login";
            ErrorLabel.IsVisible = true;
        }
    }
}
