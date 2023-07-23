using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApiDemo2.Helper
{
    public class StockDataItem
    {
        [JsonProperty("1. open")]
        public string? Open { get; set; }

        [JsonProperty("2. high")]
        public string? High { get; set; }

        [JsonProperty("3. low")]
        public string? Low { get; set; }

        [JsonProperty("4. close")]
        public string? Close { get; set; }

        [JsonProperty("5. volume")]
        public string? Volume { get; set; }
    }
    public class LastTwoDaysData
    {
        [JsonProperty("Last Two Days Data")]
        public Dictionary<string, StockDataItem>? DateValues { get; set; }
    }

    public class RootObject
    {
        public LastTwoDaysData? LastTwoDays { get; set; }
    }
}

