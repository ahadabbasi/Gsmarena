using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace Gsmarena.Terminal;

public record GsmarenaDevice(
    string Name,
    string Url,
    IDictionary<string, IDictionary<string, IList<string>>> Features);

internal class GsmarenaPhone
{
    private IDocument Document { get; }

    private string? LoudSpeaker { get; }

    public GsmarenaPhone(IDocument document)
    {
        Document = document;

        INode? parent = Document.QuerySelectorAll("[href*=loudspeaker]").Parent("tr").First().NextSibling;

        if (parent is not null)
        {
            LoudSpeaker = parent.Text();
        }
    }
}

public static class Program
{
    public static IList<GsmarenaDevice> Devices { get; } = new List<GsmarenaDevice>();
    private static string BaseAddress => "https://www.gsmarena.com/";

    private static IDictionary<string, string> ValidBrands { get; } = new Dictionary<string, string>()
    {
        { "ALCATEL", string.Empty },
        { "APPLE", string.Empty },
        { "ASUS", string.Empty },
        { "BLU", string.Empty },
        { "HTC", string.Empty },
        { "HUAWEI", string.Empty },
        { "INFINIX", string.Empty },
        { "LENOVO", string.Empty },
        { "LG", string.Empty },
        { "NOKIA", string.Empty },
        { "SONY", string.Empty },
        { "XIAOMI", string.Empty },
        { "ZTE", string.Empty },
        { "SAMSUNG", string.Empty }
    };

    private static string GetFilePathOfAllPhones(string fileName, string fileExtension)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.{1}", fileName, fileExtension));
    }

    private static string GetFilePathOfAllPhones()
    {
        return GetFilePathOfAllPhones("phones", "txt");
    }

    private static async Task BrandsPhonesLink()
    {
        IBrowsingContext context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());

        IDocument document = await context.OpenAsync(request => request.Address(BaseAddress));

        foreach (IElement element in document.QuerySelectorAll(".brandmenu-v2.light.l-box.clearfix ul li"))
        {
            string text = element.Text().ToUpper();

            if (ValidBrands.ContainsKey(text))
            {
                IElement? anchor = element.QuerySelector("a");
                if (anchor is not null)
                    ValidBrands[text] = anchor.GetAttribute("href") ?? string.Empty;
            }
        }

        string filePath = GetFilePathOfAllPhones("brands", "json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (FileStream file = File.Create(filePath))
        {
            byte[] content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ValidBrands, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

            file.Write(content, 0, content.Length);
        }


        Task[] tasks = new Task[ValidBrands.Count];

        int count = 0;

        foreach (KeyValuePair<string, string> brand in ValidBrands)
        {
            tasks[count] = Task.Factory.StartNew(() => GetListOfLink(brand.Value, brand.Key));

            count++;

            if (count % 2 == 0)
            {
                Task.WaitAll(tasks[count - 1], tasks[count - 2]);
            }
            //string url = brand.Value;
        }

        Task.WaitAll(tasks);


        /*
        if (File.Exists(GetFilePathOfAllPhones()))
        {
            File.Delete(GetFilePathOfAllPhones());
        }

        Console.WriteLine(JsonSerializer.Serialize(Phones, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));

        using (FileStream file = File.Create(GetFilePathOfAllPhones()))
        {
            using (TextWriter tw = new StreamWriter(file))
            {
                foreach (string phone in Phones)
                {
                    await tw.WriteLineAsync(phone);
                }
            }
        }
        */
    }

    private static async void GetListOfLink(string url, string brand)
    {
        IList<string> phones = new List<string>();
        IBrowsingContext context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());


        while (!string.IsNullOrEmpty(url))
        {
            Thread.Sleep(TimeSpan.FromSeconds(30));

            url = string.Format("{0}{1}", BaseAddress, url);

            Console.Clear();

            Console.WriteLine(url);
            Console.WriteLine(phones.Count);

            IDocument document = await context.OpenAsync(request => request.Address((url)));

            foreach (IElement element in document.QuerySelectorAll(".makers ul li a"))
            {
                IElement? image = element.QuerySelector("img");
                if (image is not null)
                {
                    string pattern = @"Announced\s\w{3,4}\s(?<Year>\d{4}).";
                    string title = image.GetAttribute("title") ?? string.Empty;
                    if (Regex.IsMatch(title, pattern))
                    {
                        if (int.Parse(Regex.Match(title, pattern).Groups["Year"].Value) == 2010)
                        {
                            url = string.Empty;
                            break;
                        }

                        string line = element.GetAttribute("href") ?? string.Empty;

                        phones.Add(line);
                    }
                }
            }

            if (!string.IsNullOrEmpty(url))
            {
                url = string.Empty;

                IElement? next = document.QuerySelector(".nav-pages a.prevnextbutton[title=\"Next page\"]");
                if (next is not null && next.HasAttribute("href"))
                {
                    url = next.GetAttribute("href") ?? string.Empty;
                }
            }
        }


        string filePath = GetFilePathOfAllPhones(brand, "json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (FileStream file = File.Create(filePath))
        {
            byte[] content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(phones, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

            file.Write(content, 0, content.Length);
        }
    }

    private static async Task FetchPhone(string brand, string line)
    {
        Thread.Sleep(TimeSpan.FromMinutes(5));

        GsmarenaDevice? device = null;

        IDictionary<string, IDictionary<string, IList<string>>> features =
            new Dictionary<string, IDictionary<string, IList<string>>>();

        IBrowsingContext context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());

        if (!string.IsNullOrEmpty(line))
        {
            string url = string.Format("{0}{1}", BaseAddress, line);

            Console.WriteLine(url);

            using (HttpClient client = new HttpClient()
                   {
                       DefaultRequestHeaders =
                       {
                           {
                               "User-Agent",
                               "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
                           },
                           {
                               "Accept",
                               "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
                           }
                       }
                   })

            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    Console.WriteLine(response.StatusCode);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        
                        IDocument document = await context.OpenAsync(request => request.Content(content));

                        string productName = document.QuerySelector(".specs-phone-name-title")?.Text() ?? string.Empty;

                        foreach (IElement table in document.QuerySelectorAll("#specs-list table"))
                        {
                            IElement? scope = table.QuerySelector("[scope=\"row\"]");

                            if (scope is not null)
                            {
                                IDictionary<string, IList<string>> feature = new Dictionary<string, IList<string>>();
                                string lastKey = string.Empty;
                                foreach (IElement row in table.QuerySelectorAll("tr"))
                                {
                                    IElement? name = row.QuerySelector(".ttl");

                                    if (name is not null)
                                    {
                                        string key = name.Text();
                                        if (Regex.IsMatch(key, @"^\s*$"))
                                        {
                                            key = lastKey;
                                        }
                                        else
                                        {
                                            lastKey = key;
                                        }

                                        if (!feature.ContainsKey(key))
                                        {
                                            feature.Add(key, new List<string>());
                                        }

                                        IElement? value = row.QuerySelector(".nfo");

                                        if (value is not null)
                                        {
                                            if (value.InnerHtml.Contains("<br>"))
                                            {
                                                feature[key] = new List<string>(value.InnerHtml.Split("<br>"));
                                            }
                                            else
                                            {
                                                feature[key].Add(value.Text());
                                            }
                                        }
                                    }
                                }

                                features.Add(scope.Text(), feature);
                            }
                        }

                        device = new GsmarenaDevice(productName, line, features);
                    }
                }
            }
        }


        if (device is not null)
        {
            Devices.Add(device);

            string jsonContent = (JsonSerializer.Serialize(device, new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

            Console.WriteLine(jsonContent);

            string filePath =
                GetFilePathOfAllPhones(string.Format("{0}/{1}", brand, device.Url.Replace(BaseAddress, string.Empty)),
                    "json");

            string? directoryPath = Path.GetDirectoryName(filePath);
            if (directoryPath is not null)
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (FileStream file = File.Create(filePath))
                {
                    byte[] fileContent = Encoding.UTF8.GetBytes(jsonContent);
                    file.Write(fileContent, 0, fileContent.Length);
                }
            }
        }
    }

    public static async Task Main(params string[] args)
    {
        
        Task[] tasks = new Task[5];
        int count = 0;
        foreach (string brand in ValidBrands.Keys)
        {
            foreach (string? deviceLink in JsonSerializer.Deserialize<string[]>(
                         await File.ReadAllTextAsync(
                             GetFilePathOfAllPhones(brand, "json")
                         )
                     )
                    )
            {
                tasks[count] = Task.Factory.StartNew(async () => await FetchPhone(brand, deviceLink));


                count++;
                if (count == 5)
                {
                    count = 0;
                    Task.WaitAll(tasks);
                }
                else
                {
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
            }

            Task.WaitAll(tasks);
        }
        

        //await FetchPhone("APPLE", "apple_iphone_15_pro_max-12548.php");

        string jsonContent = (JsonSerializer.Serialize(Devices, new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));

        Console.WriteLine(jsonContent);

        string filePath = GetFilePathOfAllPhones(nameof(Devices), "json");

        if (!File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (FileStream file = File.Create(filePath))
        {
            byte[] fileContent = Encoding.UTF8.GetBytes(jsonContent);
            file.Write(fileContent, 0, fileContent.Length);
        }


        Console.WriteLine("Hello, World!");
    }
}