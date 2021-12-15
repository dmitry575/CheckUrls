using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CheckUrls
{
    class CheckService
    {
        /// <summary>
        /// Max threads for parsing pages
        /// </summary>
        private const int MaxThreads = 5;

        /// <summary>
        /// Options from user
        /// </summary>
        private readonly Options _options;

        /// <summary>
        /// Result of searching url
        /// </summary>
        private readonly ConcurrentDictionary<int, List<string>> _results = new ConcurrentDictionary<int, List<string>>();

        /// <summary>
        /// Queue of task of working searching
        /// </summary>
        private readonly TaskQueue _taskQueue;

        private readonly HttpClient _httpClient;

        public CheckService(Options options)
        {
            var httpClientHandler = new HttpClientHandler() { AllowAutoRedirect = false };
            _httpClient = new HttpClient(httpClientHandler)
            {
                MaxResponseContentBufferSize = 1_000_000
            };

            _options = options;
            _taskQueue = new TaskQueue(MaxThreads);
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            _httpClient.DefaultRequestHeaders.Add("Referer", "https://yandex.ru");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");
        }

        /// <summary>
        /// Checking urls
        /// </summary>
        /// <returns></returns>
        public async Task CheckAsync()
        {
            if (!CheckParams())
            {
                return;
            }

            Write($"try to checking urls: {_options.FileName}");

            List<string> listUrl = await GetUrlsAsync();
            ClearResult();

            var tasks = new Task[listUrl.Count];

            for (int i = 0; i < listUrl.Count; i++)
            {
                var url = listUrl[i].Trim();
                tasks[i] = _taskQueue.Enqueue(async () => await GetAsync(url));
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Cleaning results
        /// </summary>
        private void ClearResult()
        {
            _results.Clear();
            foreach (var code in _results.Keys)
            {
                _results[code] = new List<string>();
            }
        }

        /// <summary>
        /// Reqeuest to page and find url in tag a
        /// </summary>
        /// <param name="url">Url needs parsing</param>
        private async Task GetAsync(string url)
        {
            try
            {
                Write($"starting get: {url}");

                var res = await _httpClient.GetAsync(url);
                _results.TryAdd(res.StatusCode, url);

                Write($"result:  {url}{(res.StatusCode == HttpStatusCode.Moved ? ", " + res.Headers.Location : string.Empty)}, {res}");
            }
            catch (HttpRequestException e)
            {
                Write($"error get url, http error: {url}, {e.Message}");

                _results.TryAdd(e.StatusCode ?? HttpStatusCode.NotFound, url);
            }
            catch (Exception e)
            {
                Write($"error get url: {url}, {e.Message}");
                _results.TryAdd(HttpStatusCode.NotFound, url);
            }
        }

        /// <summary>
        /// Check some params in Options
        /// </summary>
        private bool CheckParams()
        {
            if (!File.Exists(_options.FileName))
            {
                Write($"File not exists: {_options.FileName}");
                return false;
            }

            return true;
        }

        private async Task<List<string>> GetUrlsAsync() => (await File.ReadAllLinesAsync(_options.FileName)).ToList();

        private void Write(string str)
        {
            if (_options.Verbose)
            {
                Console.WriteLine(str);
            }
        }

        public void PrintResult()
        {
            var sb = new StringBuilder();
            foreach (var res in _results)
            {
                sb.Append(res.Key);
                sb.AppendFormat(" - {0}", res.Value.Count);
                sb.Append("\r\n");
                if (res.Key != 200)
                {
                    AddToSb(res.Value, sb);
                }
            }

            File.WriteAllText(_options.OutName, sb.ToString());
            Write("");
            Write(sb.ToString());
        }

        private void AddToSb(List<string> urls, StringBuilder sb)
        {
            foreach (string url in urls)
            {
                sb.Append("\t");
                sb.Append(url);
                sb.Append("\r\n");
            }
        }
    }
}