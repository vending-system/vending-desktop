using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VendingSystemClient.Services;

namespace VendingSystemClient.ViewModels;

public partial class LoginViewModel(ApiService api) : ObservableObject
{
    private readonly ApiService _api = api;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private bool _isLoading = false;

    public event Action<string, string>? LoginSuccess;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Введите логин и пароль";
            return;
        }

        IsLoading = true;
        ErrorMessage = "";

        var result = await _api.LoginAsync(Username, Password);

        IsLoading = false;

        if (result == null)
            ErrorMessage = "Неверный логин или пароль";
        else
        {
            _api.SetToken(result.Token);
            LoginSuccess?.Invoke(result.Username, "Администратор"); 
        }
        
    }
}