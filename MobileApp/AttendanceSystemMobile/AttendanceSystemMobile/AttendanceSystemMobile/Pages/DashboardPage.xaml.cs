using AttendanceSystemMobile.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Graphics;

namespace AttendanceSystemMobile.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly IServiceProvider _services;
    private CircleProgressDrawable _drawable;

    public DashboardPage(DatabaseService db, IServiceProvider services)
    {
        InitializeComponent();
        _db = db;
        _services = services;

        _drawable = new CircleProgressDrawable();
        _drawable.Progress = 0f;
        ProgressCircle.Drawable = _drawable;

        ProgressCircle.Invalidate();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _drawable.Progress = 0f;

        NameLabel.Text = $"Name: {App.CurrentStudentName}";
        ClassLabel.Text = $"Class: {App.CurrentClass}";

        var count = await _db.GetAttendanceCountAsync(App.CurrentStudentId);
        UpdateAttendance(count);

        EventsList.ItemsSource = await _db.GetEventsAsync();
    }

    public void UpdateAttendance(int attended)
    {
        attended = Math.Clamp(attended, 0, 3);

        AttendanceLabel.Text = $"{attended}/3 events";

        _drawable.Progress = attended / 3f;

        ProgressCircle.Invalidate();
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        App.CurrentStudentId = null;
        App.CurrentStudentName = null;
        App.CurrentClass = null;

        Preferences.Remove("StudentId");
        Preferences.Remove("StudentName");
        Preferences.Remove("StudentClass");

        Application.Current.Windows[0].Page = new NavigationPage(_services.GetRequiredService<LoginPage>());
    }



}
