using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Gsmarena.WindowsApplication.Models;
using Gsmarena.WindowsApplication.Models.Entities;
using Gsmarena.WindowsApplication.Models.ViewModels;
using IronXL;
using Microsoft.Win32;

namespace Gsmarena.WindowsApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected MainVM DataBinding { get; }
        
        public MainWindow()
        {
            
            InitializeComponent();
            DataBinding = new MainVM();
            DataContext = DataBinding;
        }

        private float ConvertMmToIn(float entry)
        {
            return entry / 25.4f;
        }

        private double ScreenSizeToBodySizeRatio(float width, float height, float display, float resolutionX,
            float resolutionY)
        {
            width = ConvertMmToIn(width);
            height = ConvertMmToIn(height);
            
            float aspectScreen = Math.Max(resolutionX, resolutionY) / Math.Min(resolutionX, resolutionY);
            float deviceArea = width * height;
            double screenX =  WidthFromDiagonal(display, aspectScreen);
            double screenY = HeightFromDiagonal(display, aspectScreen);
            double screenArea = screenX * screenY;
            return Math.Ceiling((screenArea / deviceArea) * 100);
        }
        
        private double WidthFromDiagonal(float diagonal, float aspect) {
            return diagonal * Math.Sin(Math.Atan(aspect));
        }

        private double HeightFromDiagonal(float diagonal, float aspect) {
            return diagonal * Math.Cos(Math.Atan(aspect));
        }

        private void ChooseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel file (*.xlsx) | *.xlsx;";
            
            bool? resultOfShowDialog = fileDialog.ShowDialog();

            if (resultOfShowDialog is not null && resultOfShowDialog == true)
            {
                WorkBook book = WorkBook.LoadExcel(fileDialog.FileName);
                WorkSheet sheet = book.WorkSheets.First();
                
                int count = 2;

                while (!string.IsNullOrWhiteSpace(sheet[$"A{count}"].ToString()))
                {
                    Device device = new Device()
                    {
                        Url = sheet[$"B{count}"].ToString(),
                        Name = sheet[$"C{count}"].ToString(),
                        Networks = sheet[$"D{count}"].ToString().Split("/"),
                        OperationSystem = sheet[$"S{count}"].ToString(),
                        CpuModel = sheet[$"U{count}"].ToString(),
                        CountOfThread = sheet[$"V{count}"].ToString(),
                        DisplaySize = float.Parse(sheet[$"M{count}"].ToString())
                    };

                    string pattern = @"\d+";
                    string value = sheet[$"AS{count}"].ToString();

                    if (Regex.IsMatch(value, pattern))
                    {
                        device.BatteryCapacity = int.Parse(Regex.Match(value, pattern).Value);
                    }

                    value = sheet[$"I{count}"].ToString();
                    pattern = @"\d+";
                    if (Regex.IsMatch(value, pattern))
                    {
                        device.Weight = float.Parse(Regex.Match(value, pattern).Value);
                    }

                    pattern = @"\d+(.\d+(.\d+)?)?";
                    value = sheet[$"T{count}"].ToString();

                    if (Regex.IsMatch(value, pattern))
                    {
                        device.OperationSystemVersion = Regex.Match(value, pattern).Value;
                    }

                    Match match;
                    
                    value = sheet[$"H{count}"].ToString();
                    pattern = @"(?<Width>\d+(.\d+)?)\sx\s(?<Height>\d+(.\d+)?)\sx\s(?<Depth>\d+(.\d+)?)";
                    
                    if (Regex.IsMatch(value, pattern))
                    {
                        match = Regex.Match(value, pattern);

                        device.Dimension = new Dimension()
                        {
                            Width = float.Parse(match.Groups["Width"].ToString()),
                            Height = float.Parse(match.Groups["Height"].ToString()),
                            Depth = float.Parse(match.Groups["Depth"].ToString())
                        };
                    }
                    
                    /*
                    value = sheet[$"O{count}"].ToString();

                    if (Regex.IsMatch(value, pattern))
                    {
                        device.DisplayRatio = float.Parse(Regex.Match(value, pattern).Value);
                    }
                    else
                    {
                        
                    }
                    */
                    match = Regex.Match(
                        sheet[$"P{count}"].ToString(),
                        @"(?<X>\d+(.\d+)?)\sx\s(?<Y>\d+(.\d+)?)\spixels"
                    );
                    device.DisplayRatio = float.Parse(string.Format("{0:0.00}", ScreenSizeToBodySizeRatio(
                        device.Dimension.Width,
                        device.Dimension.Height,
                        device.DisplaySize,
                        float.Parse(match.Groups["X"].Value),
                        float.Parse(match.Groups["Y"].Value)
                    )));

                    pattern = @"\(~(?<PerInch>\d+(.\d+)?).*\)";
                    value = sheet[$"P{count}"].ToString();

                    if (Regex.IsMatch(value, pattern))
                    {
                        device.PixelPerInch = float.Parse(Regex.Match(value, pattern).Groups["PerInch"].Value);
                    }

                    pattern = @"(?<Year>\d{4})-(?<Month>\d{2})";
                    value = sheet[$"F{count}"].ToString();
                    
                    if (!Regex.IsMatch(value, pattern))
                    {
                        value = sheet[$"E{count}"].ToString();
                    }

                    if (Regex.IsMatch(value, pattern))
                    {
                        match = Regex.Match(value, pattern);

                        device.ReleaseDate = new DateTime(
                            int.Parse(match.Groups["Year"].Value),
                            int.Parse(match.Groups["Month"].Value),
                            1
                        );
                    }

                    pattern =
                        @"(?<Internal>(?<SizeOfInternal>\d+(.\d+)?)(?<UnitOfInternal>\w+))\s(?<Ram>(?<SizeOfRam>\d+(.\d+)?)(?<UnitOfRam>\w+)\sRAM)";
                    value = sheet[$"Y{count}"].ToString();
                    if (Regex.IsMatch(value, pattern))
                    {
                        device.Memories = Regex.Matches(value, pattern).Select(matchItem => new MemoryCapacity()
                        {
                            SizeOfInternal = float.Parse(matchItem.Groups["SizeOfInternal"].Value),
                            UnitOfInternal = matchItem.Groups["UnitOfInternal"].Value,
                            SizeOfRam = float.Parse(matchItem.Groups["SizeOfRam"].Value),
                            UnitOfRam = matchItem.Groups["UnitOfRam"].Value
                        });
                    }
                    else
                    {
                        pattern = @"(?<Ram>(?<SizeOfRam>\d+(.\d+)?)(?<UnitOfRam>\w+)\sRAM)";
                        
                        if (Regex.IsMatch(value, pattern))
                        {
                            match = Regex.Match(value, pattern);
                            device.Memories = Regex.Matches(
                                value.Replace(match.Value, string.Empty),
                                @"(?<Internal>(?<SizeOfInternal>\d+(.\d+)?)(?<UnitOfInternal>\w+))"
                            ).Select(internalMatch => new MemoryCapacity()
                            {
                                SizeOfInternal = float.Parse(internalMatch.Groups["SizeOfInternal"].Value),
                                UnitOfInternal = internalMatch.Groups["UnitOfInternal"].Value,
                                SizeOfRam = float.Parse(match.Groups["SizeOfRam"].Value),
                                UnitOfRam = match.Groups["UnitOfRam"].Value
                            });
                        }
                        else
                        {
                            device.Memories = Regex.Matches(
                                value,
                                @"(?<Internal>(?<SizeOfInternal>\d+(.\d+)?)(?<UnitOfInternal>\w+))"
                            ).Select(internalMatch => new MemoryCapacity()
                            {
                                SizeOfInternal = float.Parse(internalMatch.Groups["SizeOfInternal"].Value),
                                UnitOfInternal = internalMatch.Groups["UnitOfInternal"].Value,
                                SizeOfRam = null,
                                UnitOfRam = string.Empty
                            });
                        }
                    }

                    pattern = @"(?<Pixel>\d+(.\d+)?)\sMP(.*\((?<Type>[\w\s]+)\))?";
                    value = sheet[$"AA{count}"].ToString();

                    IList<Camera> cameras = new List<Camera>();

                    if (Regex.IsMatch(value, pattern))
                    {
                        foreach (Match matchItem in Regex.Matches(value, pattern))
                        {
                            Camera camera = new Camera()
                            {
                                Pixel = float.Parse(matchItem.Groups["Pixel"].Value),
                                Position = CameraPosition.Back
                            };
                            
                            foreach (Group group in matchItem.Groups)
                            {
                                if (group.Name == "Pixel")
                                {
                                    continue;
                                }

                                camera.Type = group.Value;
                            }
                            
                            cameras.Add(camera);
                        }
                    }
                    
                    value = sheet[$"AE{count}"].ToString();
                    
                    if (Regex.IsMatch(value, pattern))
                    {
                        foreach (Match matchItem in Regex.Matches(value, pattern))
                        {
                            Camera camera = new Camera()
                            {
                                Pixel = float.Parse(matchItem.Groups["Pixel"].Value),
                                Position = CameraPosition.Front
                            };
                            
                            foreach (Group group in matchItem.Groups)
                            {
                                if (group.Name == "Pixel")
                                {
                                    continue;
                                }

                                camera.Type = group.Value;
                            }
                            
                            cameras.Add(camera);
                        }
                    }

                    device.Cameras = cameras;
                    
                    value = sheet[$"K{count}"].ToString();

                    device.Technologies = new[] { "Full-size", "Mini-SIM", "Micro-SIM", "Nano-SIM", "eSIM" }
                        .Where(simType => value.ToLower().Contains(simType.ToLower()));
                    
                    DataBinding.AddDevice(device);
                    count++;
                    //Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DataBinding.Delete(DataBinding.Choose);
        }
    }
}