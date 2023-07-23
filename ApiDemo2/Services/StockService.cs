using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ApiDemo2.Helper;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace ApiDemo2.Services
{
    public class StockService
    {
        private const string API_KEY = "P71NRMK3OK63WNZU";
        private const string BASE_URL = "https://www.alphavantage.co/query";

        public async Task<Dictionary<string, dynamic>?> GetDataAsync(string companySymbol)
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

        public async Task<Dictionary<string, JsonElement>> GetStockDataByDateAsync(string companySymbol, DateTime date)
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
                        var result = new Dictionary<string, JsonElement>
                        {
                            {dateString, stockData }
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

        public async Task<Dictionary<string, JsonElement>> GetLastTwoDaysDataAsync(string companySymbol)
        {
            string QUERY_URL = $"{BASE_URL}?function=TIME_SERIES_DAILY&symbol={companySymbol}&apikey={API_KEY}";

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    string json_data = await httpClient.GetStringAsync(QUERY_URL);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var parseData = JsonSerializer.Deserialize<JsonElement>(json_data, options);

                    if (parseData.TryGetProperty("Time Series (Daily)", out JsonElement timeSeriesData))
                    {
                        var lastTwoDaysData = new Dictionary<string, JsonElement>();
                        int count = 0;

                        foreach (var item in timeSeriesData.EnumerateObject())
                        {
                            lastTwoDaysData.Add(item.Name, item.Value);
                            count++;

                            if (count >= 2)
                                break;
                        }

                        return lastTwoDaysData;
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

        public async Task<Dictionary<string, dynamic>> GetLastTwoDaysVarianceAsync(string companySymbol)
        {
            try
            {
                // Get the last two days' data using the existing method
                var lastTwoDaysData = await GetLastTwoDaysDataAsync(companySymbol);

                var lastTwoDaysVariance = new Dictionary<string, dynamic>();

                // Get the last two dates from the dictionary keys
                var dates = lastTwoDaysData.Keys.ToList();
                var lastDay = dates[0];
                var secondLastDay = dates[1];

                // Extract close prices for the last two days and parse them as doubles
                var lastDayClose = double.Parse(lastTwoDaysData[lastDay].GetProperty("4. close").GetString());
                var secondLastDayClose = double.Parse(lastTwoDaysData[secondLastDay].GetProperty("4. close").GetString());

                // Calculate percentage difference
                double difference = lastDayClose - secondLastDayClose;
                double percentageDifference = (difference / secondLastDayClose) * 100.0;

                lastTwoDaysVariance.Add("Date1", lastDay);
                lastTwoDaysVariance.Add("Date2", secondLastDay);
                lastTwoDaysVariance.Add("ClosePrice1", lastDayClose);
                lastTwoDaysVariance.Add("ClosePrice2", secondLastDayClose);
                lastTwoDaysVariance.Add("PercentageDifference", percentageDifference);

                var response = new Dictionary<string, dynamic>
            {
                { "Last Two Days Variance", lastTwoDaysVariance }
            };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch data for the company {companySymbol}" + $"\n {ex.Message}");
            }
        }


    }
}

