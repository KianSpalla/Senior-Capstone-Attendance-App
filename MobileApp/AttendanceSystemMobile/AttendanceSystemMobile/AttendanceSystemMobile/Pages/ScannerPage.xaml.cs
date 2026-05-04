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

        Console.WriteLine($"QR Code Value: {result.Value}");

        if (busy) return;
        busy = true;

        if (!int.TryParse(result.Value, out int eventId))
        {
            busy = false;
            return;
        }

        bool success = await _db.RegisterAttendanceAsync(App.CurrentStudentId, eventId);

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await DisplayAlert(
                success ? "Success" : "Already Checked In",
                success ? "Attendance recorded" : "You already scanned this event",
                "OK");

            busy = false;
        });
    }
}
