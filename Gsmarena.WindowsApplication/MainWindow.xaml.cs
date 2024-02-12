﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Gsmarena.WindowsApplication.Models.DataTransfers;
using Gsmarena.WindowsApplication.Models.Entities;
using Gsmarena.WindowsApplication.Models.Repositories;
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
        protected DatabaseVM DatabaseDataBinding { get; }
        protected ExcelVM ExcelDataBinding { get; }
        private IEnumerable<string> DeviceTypes { get; }

        private string TwoDigitsPattern
        {
            get { return "{0:0.00}"; }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataBinding = new MainVM();
            DataContext = DataBinding;
            DatabaseDataBinding = DatabaseVM.Read();
            DeviceTypes = Enum.GetNames(typeof(DeviceType));
            ExcelDataBinding = ExcelVM.Read();
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
            double screenX = WidthFromDiagonal(display, aspectScreen);
            double screenY = HeightFromDiagonal(display, aspectScreen);
            double screenArea = screenX * screenY;
            return Math.Ceiling((screenArea / deviceArea) * 100);
        }

        private double WidthFromDiagonal(float diagonal, float aspect)
        {
            return diagonal * Math.Sin(Math.Atan(aspect));
        }

        private double HeightFromDiagonal(float diagonal, float aspect)
        {
            return diagonal * Math.Cos(Math.Atan(aspect));
        }

        private async void ChooseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            chooseBtn.IsEnabled = !chooseBtn.IsEnabled;
            excelBtn.IsEnabled = !excelBtn.IsEnabled;
            
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Excel file (*.xlsx) | *.xlsx;";

                bool? resultOfShowDialog = fileDialog.ShowDialog();

                if (resultOfShowDialog is not null && resultOfShowDialog == true)
                {

                    IronXL.License.LicenseKey =
                        "IRONSUITE.AHADVIRUS.YAHOO.COM.31387-B369359C47-JD63M-HT6GVXI73LR3-FC54E3N6QHAF-I7KTX3T4CZDD-5N6PTWAKSSB3-INGFEU2TIEOX-OK4AMEN5XAA3-7BCVB6-T77RZZPOU6CLUA-DEPLOYMENT.TRIAL-3XJKDT.TRIAL.EXPIRES.12.MAR.2024";

                    await Task.Run(() =>
                    {
                        WorkBook book = WorkBook.LoadExcel(fileDialog.FileName);
                        WorkSheet sheet = book.WorkSheets.First();

                        int count = 2;

                        while (!string.IsNullOrWhiteSpace(sheet[$"A{count}"].ToString()))
                        {
                            try
                            {
                                Device device = new Device()
                                {
                                    Type = Enum.Parse<DeviceType>(sheet[$"{ExcelDataBinding.Type}{count}"]
                                        .ToString()),
                                    Brand = sheet[$"{ExcelDataBinding.Brand}{count}"]
                                        .ToString(),
                                    Url = sheet[$"{ExcelDataBinding.Url}{count}"]
                                        .ToString(), //sheet[$"B{count}"].ToString(),
                                    Name = sheet[$"{ExcelDataBinding.Name}{count}"]
                                        .ToString(), // sheet[$"C{count}"].ToString(),
                                    Networks = sheet[$"{ExcelDataBinding.Networks}{count}"].ToString()
                                        .Split("/"), // sheet[$"D{count}"].ToString().Split("/"),
                                    OperationSystem =
                                        sheet[$"{ExcelDataBinding.OperationSystem}{count}"]
                                            .ToString(), // sheet[$"S{count}"].ToString(),
                                    CpuModel = sheet[$"{ExcelDataBinding.CpuModel}{count}"]
                                        .ToString(), // sheet[$"U{count}"].ToString(),
                                    CountOfThread =
                                        sheet[$"{ExcelDataBinding.CountOfThread}{count}"]
                                            .ToString(), //  sheet[$"V{count}"].ToString(),
                                    DisplaySize =
                                        float.Parse(sheet[$"{ExcelDataBinding.DisplaySize}{count}"]
                                            .ToString()), //  float.Parse(sheet[$"M{count}"].ToString())
                                };

                                if (device.Type != DeviceType.Phone || device.DisplaySize >= 7)
                                {
                                    device.Type = DeviceType.Tablet;
                                }

                                string pattern = @"\d+";
                                string value = sheet[$"{ExcelDataBinding.BatteryCapacity}{count}"]
                                    .ToString(); //   sheet[$"AS{count}"].ToString();

                                if (Regex.IsMatch(value, pattern))
                                {
                                    device.BatteryCapacity = int.Parse(Regex.Match(value, pattern).Value);
                                }

                                value = sheet[$"{ExcelDataBinding.Weight}{count}"]
                                    .ToString(); // sheet[$"I{count}"].ToString();
                                pattern = @"\d+";
                                if (Regex.IsMatch(value, pattern))
                                {
                                    device.Weight = float.Parse(Regex.Match(value, pattern).Value);
                                }

                                pattern = @"\d+(.\d+(.\d+)?)?";
                                value = sheet[$"{ExcelDataBinding.OperationSystemVersion}{count}"]
                                    .ToString(); // sheet[$"T{count}"].ToString();

                                if (Regex.IsMatch(value, pattern))
                                {
                                    device.OperationSystemVersion = Regex.Match(value, pattern).Value;
                                }

                                Match match;

                                value = sheet[$"{ExcelDataBinding.Dimension}{count}"]
                                    .ToString(); // sheet[$"H{count}"].ToString();
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


                                value = sheet[$"{ExcelDataBinding.DisplayRatio}{count}"]
                                    .ToString(); // sheet[$"O{count}"].ToString();

                                if (Regex.IsMatch(value, pattern))
                                {
                                    device.DisplayRatio = float.Parse(Regex.Match(value, pattern).Value);
                                }
                                else
                                {
                                    match = Regex.Match(
                                        sheet[$"{ExcelDataBinding.Resolution}{count}"]
                                            .ToString(), //sheet[$"P{count}"].ToString(),
                                        @"(?<X>\d+(.\d+)?)\sx\s(?<Y>\d+(.\d+)?)\spixels"
                                    );
                                    device.DisplayRatio = float.Parse(string.Format(TwoDigitsPattern,
                                        ScreenSizeToBodySizeRatio(
                                            device.Dimension.Width,
                                            device.Dimension.Height,
                                            device.DisplaySize,
                                            float.Parse(match.Groups["X"].Value),
                                            float.Parse(match.Groups["Y"].Value)
                                        )));
                                }

                                pattern = @"\(~(?<PerInch>\d+(.\d+)?).*\)";
                                value = sheet[$"{ExcelDataBinding.Resolution}{count}"]
                                    .ToString(); // sheet[$"P{count}"].ToString();

                                if (Regex.IsMatch(value, pattern))
                                {
                                    device.PixelPerInch =
                                        float.Parse(Regex.Match(value, pattern).Groups["PerInch"].Value);
                                }

                                pattern = @"(?<Year>\d{4})-(?<Month>\d{2})";
                                value = sheet[$"{ExcelDataBinding.ReleaseDate}{count}"]
                                    .ToString(); // sheet[$"F{count}"].ToString();

                                if (!Regex.IsMatch(value, pattern))
                                {
                                    value = sheet[$"{ExcelDataBinding.AnnounceDate}{count}"]
                                        .ToString(); // sheet[$"E{count}"].ToString();
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
                                value = sheet[$"{ExcelDataBinding.MemoryInternal}{count}"]
                                    .ToString();
                                if (Regex.IsMatch(value, pattern))
                                {
                                    device.Memories = Regex.Matches(value, pattern).Select(matchItem =>
                                        new MemoryCapacity()
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
                                value = sheet[$"{ExcelDataBinding.MainCamera}{count}"]
                                    .ToString();

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

                                value = sheet[$"{ExcelDataBinding.SelfieCamera}{count}"]
                                    .ToString();

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

                                value = sheet[$"{ExcelDataBinding.Technology}{count}"].ToString();

                                device.Technologies = new[] { "Full-size", "Mini-SIM", "Micro-SIM", "Nano-SIM", "eSIM" }
                                    .Where(simType => value.ToLower().Contains(simType.ToLower()))
                                    .ToArray();

                                value = sheet[$"{ExcelDataBinding.Price}{count}"]
                                    .ToString();
                                pattern = @"\d+(.\d+)?";
                                if (Regex.Matches(value, pattern).Count == 1)
                                {
                                    match = Regex.Match(value, pattern);
                                    device.Price = float.Parse(match.Value);
                                }
                                else
                                {
                                    foreach (KeyValuePair<string, float> priceUnit in new Dictionary<string, float>()
                                             {
                                                 { "EUR", 1.0f }, { "USD", 1.3f }, { "GBP", 2.5f }, { "INR", 10.58f }
                                             })
                                    {
                                        pattern = @$"\'{priceUnit.Key}\'\:\s\'(?<Price>\d+(.\d+)?)\'";
                                        if (Regex.IsMatch(value, pattern))
                                        {
                                            match = Regex.Match(value, pattern);
                                            device.Price = float.Parse(match.Groups["Price"].Value) * priceUnit.Value;
                                            break;
                                        }
                                    }
                                }

                                DataBinding.AddDevice(device);
                                Debug.WriteLine("\n{0}\n", count);
                                Thread.Sleep(20);
                            }
                            catch (Exception exception)
                            {
                                Debug.WriteLine(exception.Message);
                            }

                            count++;
                            
                        }
                        
                        book.Close();

                    });

                    
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
            
            chooseBtn.IsEnabled = !chooseBtn.IsEnabled;
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //if (DataBinding.Choose != null)
                //DataBinding.Delete(DataBinding.Choose);
        }

        private void DatabaseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DatabaseWindow databaseWindow = new DatabaseWindow(DatabaseDataBinding);
            databaseWindow.ShowDialog();
        }

        private async void SaveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = !saveBtn.IsEnabled;
            
            ApplicationContext context = new ApplicationContext(DatabaseDataBinding.ConnectionString);

            foreach (Device device in DataBinding)
            {
                try
                {
                    int brandId = await context.Brand.GetIdByNameAsync(device.Brand) ??
                                  await context.Brand.SaveAsync(device.Brand);

                    int operationId = await context.OperationSystem.GetIdByNameAsync(device.OperationSystem) ??
                                      await context.OperationSystem.SaveAsync(device.OperationSystem);

                    int deviceId = await context.Device.SaveAsync(new DeviceDTO()
                    {
                        Url = device.Url,
                        Name = device.Name,
                        BrandId = brandId,
                        Weight = decimal.Parse(string.Format(TwoDigitsPattern, device.Weight)),
                        BatteryCapacity = device.BatteryCapacity,
                        CountOfThread = device.CountOfThread,
                        CpuModel = device.CpuModel,
                        Type = device.Type.ToString().ToUpper(),
                        DisplaySize = decimal.Parse(string.Format(TwoDigitsPattern, device.DisplaySize)),
                        DisplayRatio = decimal.Parse(string.Format(TwoDigitsPattern, device.DisplayRatio)),
                        PixelPerInch = decimal.Parse(string.Format(TwoDigitsPattern, device.PixelPerInch)),
                        ReleaseDate = device.ReleaseDate ?? DateTime.Now,
                        Price = decimal.Parse(string.Format(TwoDigitsPattern, device.Price)),
                        OperationSystemId = operationId,
                        OperationSystemVersion = device.OperationSystemVersion
                    });

                    foreach (string network in device.Networks)
                    {
                        try
                        {
                            int networkId = await context.Network.GetIdByNameAsync(network) ??
                                            await context.Network.SaveAsync(network);

                            await context.Network.SaveConnectToDeviceAsync(deviceId, networkId);
                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine(exception.Message);
                        }
                    }

                    foreach (string technology in device.Technologies)
                    {
                        try
                        {
                            int technologyId = await context.Technology.GetIdByNameAsync(technology) ??
                                               await context.Technology.SaveAsync(technology);

                            await context.Technology.SaveConnectToDeviceAsync(deviceId, technologyId);
                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine(exception.Message);
                        }
                    }

                    if (device.Dimension != null)
                    {
                        try
                        {
                            await context.Dimension.SaveAsync(new DimensionDTO()
                            {
                                DeviceId = deviceId,
                                Depth = decimal.Parse(string.Format(TwoDigitsPattern, device.Dimension.Depth)),
                                Width = decimal.Parse(string.Format(TwoDigitsPattern, device.Dimension.Width)),
                                Height = decimal.Parse(string.Format(TwoDigitsPattern, device.Dimension.Height))
                            });
                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine(exception.Message);
                        }
                    }

                    foreach (Camera camera in device.Cameras)
                    {
                        try
                        {
                            await context.Camera.SaveAsync(new CameraDTO()
                            {
                                DeviceId = deviceId,
                                Pixel = decimal.Parse(string.Format(TwoDigitsPattern, camera.Pixel)),
                                Type = camera.Type,
                                Position = camera.Position.ToString().ToUpper()
                            });
                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine(exception.Message);
                        }
                    }

                    foreach (MemoryCapacity memory in device.Memories)
                    {
                        try
                        {
                            await context.Memory.SaveAsync(new MemoryDTO()
                            {
                                DeviceId = deviceId,
                                MemorySize = decimal.Parse(string.Format(TwoDigitsPattern, memory.SizeOfInternal)),
                                MemoryUnit = memory.UnitOfInternal,
                                RamSize = memory.SizeOfRam != null
                                    ? decimal.Parse(string.Format(TwoDigitsPattern, memory.SizeOfRam))
                                    : null,
                                RamUnit = memory.SizeOfRam != null ? memory.UnitOfRam : null
                            });
                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine(exception.Message);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                }
            }

            DataBinding.Clear();
            
            saveBtn.IsEnabled = !saveBtn.IsEnabled;
            excelBtn.IsEnabled = !excelBtn.IsEnabled;
        }


        private async void CreateTablesBtn_OnClick(object sender, RoutedEventArgs e)
        {
            createTablesBtn.IsEnabled = !createTablesBtn.IsEnabled;
            
            ApplicationContext context = new ApplicationContext(DatabaseDataBinding.ConnectionString);

            try
            {
                await context.CreateTable();
                MessageBox.Show("Table has been created");
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
            
            createTablesBtn.IsEnabled = !createTablesBtn.IsEnabled;
        }

        private void ExcelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ExcelWindow window = new ExcelWindow(ExcelDataBinding);

            window.ShowDialog();
        }
    }
}