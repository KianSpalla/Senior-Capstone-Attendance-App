using AttendanceSystemMobile.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Graphics;
using System.Diagnostics;

namespace AttendanceSystemMobile.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly IServiceProvider _services;
    private CircleProgressDrawable _drawable;
    private bool _isRefreshing = false;

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

        // Load data when page appears
        await RefreshDataAsync();
    }

    /// <summary>
    /// Pull-to-refresh handler
    /// </summary>
    private async void OnRefreshing(object sender, EventArgs e)
    {
        Debug.WriteLine("[DASHBOARD] Pull-to-refresh triggered");
        await RefreshDataAsync();
        RefreshViewControl.IsRefreshing = false;
    }

    /// <summary>
    /// Centralized method to refresh all data
    /// </summary>
    private async Task RefreshDataAsync()
    {
        // Prevent concurrent refreshes
        if (_isRefreshing)
        {
            Debug.WriteLine("[DASHBOARD] Refresh already in progress, skipping...");
            return;
        }

        _isRefreshing = true;

        try
        {
            Debug.WriteLine("[DASHBOARD] Refreshing data...");

            // Update static info (doesn't require network)
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                NameLabel.Text = $"Name: {App.CurrentStudentName}";
                ClassLabel.Text = $"Class: {App.CurrentClass}";
            });

            // Fetch data from database (with timeout and retry logic)
            var count = await _db.GetAttendanceCountAsync(App.CurrentStudentId);
            var events = await _db.GetEventsAsync();

            // Update UI on main thread
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                UpdateAttendance(count);
                EventsList.ItemsSource = events;
            });

            Debug.WriteLine($"[DASHBOARD] Refresh complete - {count} attendances, {events.Count} events");
        }
        catch (TimeoutException tex)
        {
            Debug.WriteLine($"[DASHBOARD] Refresh timeout: {tex.Message}");

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Connection Timeout",
                    "The server is taking too long to respond. Please check your internet connection and try again.",
                    "OK");
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[DASHBOARD] Refresh error: {ex.Message}");

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Error",
                    "Failed to load data. Pull down to refresh.",
                    "OK");
            });
        }
        finally
        {
            _isRefreshing = false;
        }
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
