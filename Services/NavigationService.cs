using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace VendingSystemClient.Services
{
    public class NavigationService(ContentControl host)
    {
    private readonly ContentControl _host = host;
    private readonly Stack<UserControl> _stack = new();
    public event Action? NavigationChanged;   
    public bool CanGoBack => _stack.Count > 0;

    public void Navigate(UserControl  page)
    {
         if (_host.Content is UserControl current && current.GetType() == page.GetType())
            return;

        if (_host.Content is UserControl currentPage)
            _stack.Push(currentPage);

        _host.Content = page;
        NavigationChanged?.Invoke();
    }
    public void GoBack()
    {
        if (!CanGoBack)
            return;
        _host.Content = _stack.Pop();
        NavigationChanged?.Invoke();
    }
    }
}