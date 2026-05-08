using AttendanceSystemMobile.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AttendanceSystemMobile.Pages;

public partial class InitializePasswordPage : ContentPage
{
    private DatabaseService _db;
    private IServiceProvider _services;
    private string _userEnum;
    private string _userName;
    private string _userEmail;

    public InitializePasswordPage(DatabaseService db, IServiceProvider services, 
                                   string userEnum, string userName, string userEmail)
    {
        InitializeComponent();
        _db = db;
        _services = services;
        _userEnum = userEnum;
        _userName = userName;
        _userEmail = userEmail;

        // Display user info
        UserNameLabel.Text = $"Name: {userName}";
        UserEnumLabel.Text = $"Student ID: {userEnum}";
        UserEmailLabel.Text = $"Email: {userEmail}";
    }
    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        string password = NewPasswordEntry.Text ?? "";
        string confirmPassword = ConfirmPasswordEntry.Text ?? "";

        // Check password strength
        if (!string.IsNullOrEmpty(password))
        {
            var strength = ValidatePasswordStrength(password);
            PasswordStrengthLabel.IsVisible = true;
            PasswordStrengthLabel.Text = strength.Message;
            PasswordStrengthLabel.TextColor = strength.IsValid ? Colors.Green : Colors.Orange;
        }
        else
        {
            PasswordStrengthLabel.IsVisible = false;
        }

        // Enable button only if passwords match and are valid
        bool passwordsMatch = !string.IsNullOrEmpty(password) && password == confirmPassword;
        bool isStrongEnough = ValidatePasswordStrength(password).IsValid;

        SetPasswordButton.IsEnabled = passwordsMatch && isStrongEnough;

        // Show error if passwords don't match (but only if user has typed in confirm field)
        if (!string.IsNullOrEmpty(confirmPassword) && password != confirmPassword)
        {
            ErrorLabel.Text = "Passwords do not match";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            ErrorLabel.IsVisible = false;
        }
    }
    private (bool IsValid, string Message) ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password))
            return (false, "");

        if (password.Length < 8)
            return (false, "Password too short (minimum 8 characters)");

        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasLowerCase = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);

        if (!hasUpperCase)
            return (false, "Add at least one uppercase letter");

        if (!hasLowerCase)
            return (false, "Add at least one lowercase letter");

        if (!hasDigit)
            return (false, "Add at least one number");

        return (true, "Password strength: Strong");
    }
    private async void OnSetPasswordClicked(object sender, EventArgs e)
    {
        string password = NewPasswordEntry.Text?.Trim();
        string confirmPassword = ConfirmPasswordEntry.Text?.Trim();

        // Final validation
        if (string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please enter a password", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match", "OK");
            return;
        }

        var strengthCheck = ValidatePasswordStrength(password);
        if (!strengthCheck.IsValid)
        {
            await DisplayAlert("Weak Password", strengthCheck.Message, "OK");
            return;
        }

        // Disable button to prevent double submission
        SetPasswordButton.IsEnabled = false;
        SetPasswordButton.Text = "Setting password...";

        try
        {
            Debug.WriteLine($"[INIT_PASSWORD] Setting password for user: {_userEnum}");

            // Hash the password
            string hashedPassword = await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(password));

            // Update user's password in database
            bool success = await _db.UpdateUserPasswordAsync(_userEnum, hashedPassword);

            if (success)
            {
                Debug.WriteLine("[INIT_PASSWORD] Password updated successfully");

                await DisplayAlert("Success", 
                    "Your password has been set successfully! Please log in with your new password.", 
                    "OK");

                // Return to login page
                Application.Current.Windows[0].Page = new NavigationPage(
                    _services.GetRequiredService<LoginPage>());
            }
            else
            {
                throw new Exception("Failed to update password in database");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[INIT_PASSWORD] Error: {ex.Message}");

            await DisplayAlert("Error", 
                $"Failed to set password: {ex.Message}", 
                "OK");

            SetPasswordButton.IsEnabled = true;
            SetPasswordButton.Text = "Set Password";
        }
    }
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Cancel", 
            "Are you sure you want to cancel? You won't be able to use the app until you set a password.", 
            "Yes", "No");

        if (confirm)
        {
            Application.Current.Windows[0].Page = new NavigationPage(
                _services.GetRequiredService<LoginPage>());
        }
    }
}