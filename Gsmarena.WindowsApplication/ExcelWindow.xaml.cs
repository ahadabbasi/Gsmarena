using System.Windows;
using Gsmarena.WindowsApplication.Models.ViewModels;

namespace Gsmarena.WindowsApplication;

public partial class ExcelWindow : Window
{
    private  ExcelVM DataBinding { get; }
    public ExcelWindow(ExcelVM dataBinding)
    {
        DataBinding = dataBinding;
        InitializeComponent();
        DataContext = DataBinding;
    }

    private async void SaveBtn_OnClick(object sender, RoutedEventArgs e)
    {
        await DataBinding.SaveAsync();
        Close();
    }
}