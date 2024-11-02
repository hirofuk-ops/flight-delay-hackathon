// 動作確認用のコード
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var daysOfWeek = GetDaysOfWeek();
        DisplayDaysOfWeek(daysOfWeek);

        int selectedId = GetSelectedDayId();
        if (!daysOfWeek.TryGetValue(selectedId, out string selectedDay))
        {
            Console.WriteLine("ID が正しくありません");
            return;
        }

        Console.WriteLine($"選択した曜日: {selectedDay}");

        var airportData = await FetchAirportData();
        if (airportData == null)
        {
            Console.WriteLine("飛行場データの読み取りでエラーが発生しました");
            return;
        }

        DisplayAirportData(airportData);

        string selectedAirportId = GetSelectedAirportId();
        await FetchAndDisplayPrediction(selectedId, selectedAirportId);
    }

    static Dictionary<int, string> GetDaysOfWeek()
    {
        return new Dictionary<int, string>
        {
            { 1, "月曜日" },
            { 2, "火曜日" },
            { 3, "水曜日" },
            { 4, "木曜日" },
            { 5, "金曜日" },
            { 6, "土曜日" },
            { 7, "日曜日" }
        };
    }

    static void DisplayDaysOfWeek(Dictionary<int, string> daysOfWeek)
    {
        foreach (var day in daysOfWeek)
        {
            Console.WriteLine($"{day.Key}: {day.Value}");
        }
    }

    static int GetSelectedDayId()
    {
        Console.Write("曜日の番号を選択してください: ");
        return int.Parse(Console.ReadLine());
    }

    static async Task<List<Dictionary<string, string>>> FetchAirportData()
    {
        using var httpClient = new HttpClient();
        string airportsApiUrl = "http://localhost:5000/airports";
        HttpResponseMessage response = await httpClient.GetAsync(airportsApiUrl);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(responseContent);
        }

        return null;
    }

    static void DisplayAirportData(List<Dictionary<string, string>> airportData)
    {
        Console.WriteLine($"ID:\t Name:");
        foreach (var airport in airportData)
        {
            Console.WriteLine($"{airport["id"]}\t{airport["name"]}");
        }
    }

    static string GetSelectedAirportId()
    {
        Console.Write("飛行場 ID を選択してください: ");
        return Console.ReadLine();
    }

    static async Task FetchAndDisplayPrediction(int selectedId, string selectedAirportId)
    {
        using var httpClient = new HttpClient();
        string apiUrl = "http://localhost:5000/predict";
        string apiUrlFull = $"{apiUrl}?day_of_week={selectedId}&airport_id={selectedAirportId}";

        Console.WriteLine("API URL: " + apiUrlFull);

        HttpResponseMessage response = await httpClient.GetAsync(apiUrlFull);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var prediction = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

            if (prediction != null && prediction.TryGetValue("delay", out string delay) && prediction.TryGetValue("certainty", out string certainty))
            {
                Console.WriteLine($"遅延予想(%): {double.Parse(delay) * 100}%");
                Console.WriteLine($"確実性(%): {double.Parse(certainty) * 100}%");
            }
            else
            {
                Console.WriteLine("遅延予測データの読み取り時にエラーが発生しました");
            }
        }
        else
        {
            Console.WriteLine("遅延予測時にエラーが発生しました");
        }
    }
}
