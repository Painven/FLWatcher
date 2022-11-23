using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace FlWatcherDesktop.ViewModels;

public class FLParser
{
    const string HOST = "https://www.fl.ru";

    private readonly Timer timer;
    private readonly HttpClient client;
    private readonly Dictionary<string, FLProjectItem> loadedProjects;

    public event EventHandler<FLProjectItem> OnProjectFound;
    public event Action RunStatusChanged;

    bool isRunning;
    public bool IsRunning
    {
        get => isRunning;
        set
        {
            if(isRunning != value)
            {
                isRunning = value;
                RunStatusChanged?.Invoke();
                timer.Enabled = value;
            }
        }
    }

    public TimeSpan Interval
    {
        get => TimeSpan.FromMilliseconds(timer.Interval);
        set => timer.Interval = (int)value.TotalMilliseconds;
    }

    public FLParser()
    {
        loadedProjects = new Dictionary<string, FLProjectItem>();

        timer = new Timer();
        timer.Interval = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
        timer.Elapsed += Timer_Elapsed;

        client = new HttpClient(new HttpClientHandler()
        {
            AllowAutoRedirect = true,
            CookieContainer = new System.Net.CookieContainer(),
            UseCookies = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9");
        client.DefaultRequestHeaders.Add("Cache-control", "no-cache");
        client.DefaultRequestHeaders.Add("pragma", "no-cache");
        client.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
        //client.DefaultRequestHeaders.Add("", "");

    }

    private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        await RefreshNow();
    }

    public async Task RefreshNow()
    {
        var lastProjects = await GetProjects();

        foreach (var p in lastProjects)
        {
            if (!string.IsNullOrWhiteSpace(p.Id) && !loadedProjects.ContainsKey(p.Id))
            {
                OnProjectFound?.Invoke(this, p);
                loadedProjects[p.Id] = p;
            }
        }
    }

    private async Task<FLProjectItem[]> GetProjects()
    {
        try
        {
            var doc = new HtmlDocument();
            var str = await client.GetStringAsync("https://www.fl.ru/projects/");
            doc.LoadHtml(str);

            var projects = doc.DocumentNode.SelectNodes("//div[contains(@id, 'project-item')]")
                .Select(div => new FLProjectItem()
                {
                    Title = div.SelectSingleNode(".//a[@name]")?.InnerText.Trim(),
                    Uri = HOST + div.SelectSingleNode(".//a[@name]")?.GetAttributeValue("href", null),
                    //Description =  div.SelectSingleNode("./div[1]/div[2]/div[1]")?.InnerText.Trim(),
                    Id = div.GetAttributeValue("id", String.Empty),
                    PriceString = div.SelectSingleNode("./div[1]/div[1]")?.InnerText.Trim()
                })
                .ToArray();

            return projects;
        }
        catch
        {
            throw;
        }

        return Enumerable.Empty<FLProjectItem>().ToArray();
    }

    public async Task Start()
    {
        IsRunning = true;
        await RefreshNow();
    }

    public void Stop()
    {
        IsRunning = false;
    }
}
