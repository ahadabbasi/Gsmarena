using System.Windows;
using Gsmarena.WindowsApplication.Models.ViewModels;

namespace Gsmarena.WindowsApplication;

public partial class ExcelWindow : Window
{
    public ExcelWindow(ExcelVM dataBinding)
    {
        InitializeComponent();
        DataContext = dataBinding;
    }

    private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}