using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VendingSystemClient.Models;
using VendingSystemClient.Models.DTO;

namespace VendingSystemClient.Services;

public class ApiService(HttpClient http)
{
    private readonly HttpClient _http = http;
    private const string Base = "http://localhost:5070/api";

    public void SetToken(string token)
    {
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<AuthResponse?> LoginAsync(string username, string password)
{
    try
    {
        var response = await _http.PostAsJsonAsync($"{Base}/auth/login",
            new LoginRequest { Username = username, Password = password });
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<AuthResponse>(): null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Login exception: {ex.Message}");
        return null;
    }
}
    public async Task<MashineStats?> GetMashineStatsAsync()
    {
        try { return await _http.GetFromJsonAsync<MashineStats>($"{Base}/machines/stats"); }
        catch { return null; }
    }

    public async Task<SalesSummary?> GetSalesSummaryAsync()
    {
        try { return await _http.GetFromJsonAsync<SalesSummary>($"{Base}/sales/summary"); }
        catch { return null; }
    }

    public async Task<List<SalesDynamic>> GetSalesDynamicsAsync(int days = 10)
    {
        try { return await _http.GetFromJsonAsync<List<SalesDynamic>>($"{Base}/sales/sales-dynamics?days={days}") ?? []; }
        catch { return []; }
    }

    public async Task<PaginatedResponse<Machine>?> GetMachinesAsync(int page = 1, int limit = 20)
    {
        try
        {
            return await _http.GetFromJsonAsync<PaginatedResponse<Machine>>($"{Base}/machines?page={page}&limit={limit}");
        }
        catch { return null; }
    }

    public async Task DeleteMachineAsync(int id)
    {
        await _http.DeleteAsync($"{Base}/machines/{id}");
    }

    public async Task UnlinkModemAsync(int id)
    {
        await _http.PatchAsync($"{Base}/machines/{id}/unlink-modem", null);
    }
    public void CleanToken()
    {
        _http.DefaultRequestHeaders.Authorization = null;
    }
    
    public async Task<Machine?> CreateMachineAsync(CreateMachineDto dto)
{
   try
    {
        var response = await _http.PostAsJsonAsync($"{Base}/machines", dto);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"CreateMachine error {response.StatusCode}: {error}");
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Machine>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{ex.Message}");
        return null;
    }
}
public async Task<bool> UpdateMachineAsync(int id, CreateMachineDto dto)
{
    try
    {
        var response = await _http.PutAsJsonAsync($"{Base}/machines/{id}", dto);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"UpdateMachine error {response.StatusCode}: {error}");
            return false;
        }
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"UpdateMachine exception: {ex.Message}");
        return false;
    }
}
public async Task<PaginatedResponse<CompanyDto>?> GetCompaniesAsync(int page = 1, int limit = 20, string? search = null)
{
    try
    {
        var url = $"{Base}/companies?page={page}&limit={limit}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&searchName={Uri.EscapeDataString(search)}";
        return await _http.GetFromJsonAsync<PaginatedResponse<CompanyDto>>(url);
    }
    catch { return null; }
}

public async Task<RealtimeData?> GetMachineRealtimeAsync(int machineId)
{
    try
    {
        return await _http.GetFromJsonAsync<RealtimeData>(
            $"{Base}/machines/{machineId}/realtime");
    }
    catch { return null; }
}
}