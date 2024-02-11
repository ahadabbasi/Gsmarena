using System.Text.Json;
using System.Windows;
using Gsmarena.WindowsApplication.Models.ViewModels;

namespace Gsmarena.WindowsApplication;

public partial class DatabaseWindow : Window
{
    private DatabaseVM DataBinding { get; }
    public DatabaseWindow(DatabaseVM dataBinding)
    {
        InitializeComponent();
        DataBinding = dataBinding;
        DataContext = DataBinding;
    }

    private async void SaveBtn_OnClick(object sender, RoutedEventArgs e)
    {
        await DataBinding.SaveAsync();
         
        this.Close();
    }
}