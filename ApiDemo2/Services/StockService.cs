using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiDemo2.Services
{
    public class StockService
    {
        private const string API_KEY = "P71NRMK3OK63WNZU";
        private const string BASE_URL = "https://www.alphavantage.co/query";

        public async Task<Dictionary<string, dynamic>> GetDataAsync(string companySymbol)
        {
            string QUERY_URL = $"{BASE_URL}?function=TIME_SERIES_DAILY&symbol={companySymbol}&apikey={API_KEY}";

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    string json_data = await httpClient.GetStringAsync(QUERY_URL);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var parseData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json_data, options);
                    return parseData;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to fetch data for the company {companySymbol}" + $"\n {ex.Message}");
                }
            }
        }

        public async Task<Dictionary<string, dynamic>> GetStockDataByDateAsync(string companySymbol, DateTime date)
        {
            string dateString = date.ToString("yyyy-MM-dd");
            string QUERY_URL = $"{BASE_URL}?function=TIME_SERIES_DAILY&symbol={companySymbol}&apikey={API_KEY}";

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    string json_data = await httpClient.GetStringAsync(QUERY_URL);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var parseData = JsonSerializer.Deserialize<JsonElement>(json_data, options);

                    if (parseData.TryGetProperty("Time Series (Daily)", out JsonElement timeSeriesData) && timeSeriesData.TryGetProperty(dateString, out JsonElement stockData))
                    {
                        var result = new Dictionary<string, dynamic>
                        {
                            { dateString, JsonSerializer.Deserialize<Dictionary<string, dynamic>>(stockData.GetRawText(), options) }
                        };
                        return result;
                    }
                    else
                    {
                        throw new Exception($"No data available for the company {companySymbol} on {dateString}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to fetch data for the company {companySymbol}" + $"\n {ex.Message}");
                }
            }
        }

        public async Task<Dictionary<string, dynamic>> GetLastTwoDaysDataAsync(string companySymbol)
        {
            string QUERY_URL = $"{BASE_URL}?function=TIME_SERIES_DAILY&symbol={companySymbol}&apikey={API_KEY}";

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    string json_data = await httpClient.GetStringAsync(QUERY_URL);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var parseData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json_data, options);

                    if (parseData.TryGetValue("Time Series (Daily)", out var timeSeriesData))
                    {
                        var lastTwoDaysData = new Dictionary<string, Dictionary<string, dynamic>>();
                        int count = 0;

                        foreach (var item in timeSeriesData.EnumerateObject())
                        {
                            lastTwoDaysData.Add(item.Name, JsonSerializer.Deserialize<Dictionary<string, dynamic>>(item.Value.GetRawText(), options));
                            count++;

                            if (count >= 2)
                                break;
                        }

                        var response = new Dictionary<string, dynamic>
                        {
                            { "Meta Data", parseData["Meta Data"] },
                            { "Time Series (Daily)", lastTwoDaysData }
                        };

                        return response;
                    }
                    else
                    {
                        throw new Exception("Time Series (Daily) data not found in API response.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to fetch data for the company {companySymbol}" + $"\n {ex.Message}");
                }
            }
        }

    }
}

