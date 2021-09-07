using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace CheckUrls
{
    public static class Extensions
    {
        public static void TryAdd(this ConcurrentDictionary<int,List<string>> dictionary, HttpStatusCode code, string url)
        {
            if(!dictionary.ContainsKey((int)code))
            {
                dictionary[(int)code] = new List<string>();
            }

            dictionary[(int)code].Add(url);
        }
    }
}
