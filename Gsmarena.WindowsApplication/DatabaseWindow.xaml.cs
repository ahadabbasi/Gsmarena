using System.Windows;
using Gsmarena.WindowsApplication.Models.ViewModels;

namespace Gsmarena.WindowsApplication;

public partial class DatabaseWindow : Window
{
    public DatabaseWindow(DatabaseVM dataBinding)
    {
        InitializeComponent();
        DataContext = dataBinding;
    }

    private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}