using AttendanceSystemMobile.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using System.Diagnostics;

namespace AttendanceSystemMobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly IServiceProvider _services;
    private Models.User _currentUser;

    public LoginPage(DatabaseService db, IServiceProvider services)
    {
        InitializeComponent();
        _db = db;
        _services = services;

        NavigationPage.SetHasNavigationBar(this, false);
    }

    /// <summary>
    /// When user presses Enter/Return in E-Number field
    /// </summary>
    private void OnENumberCompleted(object sender, EventArgs e)
    {
        OnNextClicked(sender, e);
    }

    /// <summary>
    /// Step 1: Check if user exists and needs password initialization
    /// </summary>
    private async void OnNextClicked(object sender, EventArgs e)
    {
        string eNumber = ENumberEntry.Text?.Trim();

        // Validation
        if (string.IsNullOrEmpty(eNumber))
        {
            ErrorLabel.Text = "Please enter your E-Number";
            ErrorLabel.IsVisible = true;
            return;
        }

        // Validate E-Number format (optional but recommended)
        if (!System.Text.RegularExpressions.Regex.IsMatch(eNumber, @"^e[0-9]{7}$"))
        {
            ErrorLabel.Text = "Invalid E-Number format (should be e1234567)";
            ErrorLabel.IsVisible = true;
            return;
        }

        // Show loading
        SetLoadingState(true);
        ErrorLabel.IsVisible = false;

        try
        {
            Debug.WriteLine($"[LOGIN] Checking user: {eNumber}");

            // Check if user exists
            _currentUser = await _db.GetUserByEnumAsync(eNumber);

            if (_currentUser == null)
            {
                Debug.WriteLine($"[LOGIN] User not found: {eNumber}");
                ErrorLabel.Text = "E-Number not found. Please check and try again.";
                ErrorLabel.IsVisible = true;
                SetLoadingState(false);
                return;
            }

            Debug.WriteLine($"[LOGIN] User found: {_currentUser.FirstName} {_currentUser.LastName}");

            // Check if password needs initialization
            if (_currentUser.PasswordHash == "NO_WEB_LOGIN")
            {
                Debug.WriteLine("[LOGIN] User needs password initialization");

                // Navigate to password initialization page
                await Navigation.PushAsync(new InitializePasswordPage(
                    _db,
                    _services,
                    _currentUser.Enum,
                    $"{_currentUser.FirstName} {_currentUser.LastName}",
                    _currentUser.Email));

                // Clear the E-Number field for when they return
                ENumberEntry.Text = "";
                SetLoadingState(false);
                return;
            }

            // User exists and has password - show password entry
            Debug.WriteLine("[LOGIN] Proceeding to password entry");
            
            DisplayNameLabel.Text = $"{_currentUser.FirstName} {_currentUser.LastName}";
            DisplayEnumLabel.Text = $"ID: {_currentUser.Enum}";

            // Hide E-Number section, show password section
            EnumSection.IsVisible = false;
            PasswordSection.IsVisible = true;

            // Focus on password field
            PasswordEntry.Focus();

            SetLoadingState(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[LOGIN] Error checking user: {ex.Message}");
            ErrorLabel.Text = "Connection error. Please check your internet and try again.";
            ErrorLabel.IsVisible = true;
            SetLoadingState(false);
        }
    }

    /// <summary>
    /// Step 2: Verify password and log in
    /// </summary>
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string password = PasswordEntry.Text?.Trim();

        // Validation
        if (string.IsNullOrEmpty(password))
        {
            ErrorLabel.Text = "Please enter your password";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (_currentUser == null)
        {
            ErrorLabel.Text = "Session error. Please start over.";
            ErrorLabel.IsVisible = true;
            OnBackClicked(sender, e);
            return;
        }

        // Show loading
        SetLoadingState(true);
        ErrorLabel.IsVisible = false;

        try
        {
            Debug.WriteLine($"[LOGIN] Verifying password for: {_currentUser.Enum}");

            // **CRITICAL FIX: Run BCrypt verification on background thread**
            bool passwordValid = await Task.Run(() =>
            {
                try
                {
                    return BCrypt.Net.BCrypt.Verify(password, _currentUser.PasswordHash);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[LOGIN] BCrypt error: {ex.Message}");
                    return false;
                }
            });

            if (!passwordValid)
            {
                Debug.WriteLine("[LOGIN] Invalid password");
                
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ErrorLabel.Text = "Incorrect password. Please try again.";
                    ErrorLabel.IsVisible = true;
                    PasswordEntry.Text = "";
                    PasswordEntry.Focus();
                });
                
                SetLoadingState(false);
                return;
            }

            // Login successful!
            Debug.WriteLine($"[LOGIN] Successful login for: {_currentUser.Enum}");

            var classes = await _db.GetClassesByStudentAsync(_currentUser.Enum);
            var className = classes.FirstOrDefault()?.ClassName ?? "No Class Assigned";

            // Update on main thread
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                App.CurrentStudentId = _currentUser.Enum;
                App.CurrentStudentName = $"{_currentUser.FirstName} {_currentUser.LastName}";
                App.CurrentClass = className;

                Preferences.Set("StudentId", _currentUser.Enum);
                Preferences.Set("StudentName", $"{_currentUser.FirstName} {_currentUser.LastName}");
                Preferences.Set("StudentClass", className);

                Application.Current.Windows[0].Page = new AppShell();
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[LOGIN] Error during login: {ex.Message}");
            ErrorLabel.Text = "Login error. Please try again.";
            ErrorLabel.IsVisible = true;
            SetLoadingState(false);
        }
    }

    /// <summary>
    /// Back button - return to E-Number entry
    /// </summary>
    private void OnBackClicked(object sender, EventArgs e)
    {
        // Clear current user and password
        _currentUser = null;
        PasswordEntry.Text = "";
        ErrorLabel.IsVisible = false;

        // Show E-Number section, hide password section
        PasswordSection.IsVisible = false;
        EnumSection.IsVisible = true;

        // Focus on E-Number field
        ENumberEntry.Focus();
    }

    /// <summary>
    /// Helper method to manage loading state
    /// </summary>
    private void SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsRunning = isLoading;
        LoadingIndicator.IsVisible = isLoading;

        NextButton.IsEnabled = !isLoading;
        LoginButton.IsEnabled = !isLoading;
        ENumberEntry.IsEnabled = !isLoading;
        PasswordEntry.IsEnabled = !isLoading;
    }

    /// <summary>
    /// Reset page when navigating back from password initialization
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Reset to initial state
        EnumSection.IsVisible = true;
        PasswordSection.IsVisible = false;
        ENumberEntry.Text = "";
        PasswordEntry.Text = "";
        ErrorLabel.IsVisible = false;
        _currentUser = null;

        // Focus on E-Number field
        ENumberEntry.Focus();
    }
}