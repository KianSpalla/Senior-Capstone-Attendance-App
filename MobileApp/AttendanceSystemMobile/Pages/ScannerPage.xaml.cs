namespace AttendanceSystemMobile.Pages;
using AttendanceSystemMobile.Services;

using ZXing.Net.Maui;

public partial class ScannerPage : ContentPage
{
    private readonly DatabaseService _db;
    bool busy = false;

    public ScannerPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    private async void OnScan(object sender, BarcodeDetectionEventArgs e)
    {
        var result = e.Results.FirstOrDefault();
        if (result == null) return;

        if (busy) return;
        busy = true;

        string eventCode = result.Value?.Trim();

        try
        {
            var ev = await _db.GetEventByCodeAsync(eventCode);

            if (ev == null)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Invalid QR Code", $"Scanned value: \"{eventCode}\"\nThis QR code does not match any event.", "OK");
                    busy = false;
                });
                return;
            }

            bool success = await _db.RegisterAttendanceAsync(App.CurrentStudentId, ev.EventId);

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert(
                    success ? "Checked In" : "Already Checked In",
                    success ? $"You have been checked in to \"{ev.EventName}\"." : "You have already checked in to this event.",
                    "OK");

                busy = false;
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                busy = false;
            });
        }
    }
}
