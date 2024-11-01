using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var daysOfWeek = new Dictionary<int, string>
{
    { 1, "日曜日" },
    { 2, "月曜日" },
    { 3, "火曜日" },
    { 4, "水曜日" },
    { 5, "木曜日" },
    { 6, "金曜日" },
    { 7, "土曜日" }
};

foreach (var day in daysOfWeek)
{
    Console.WriteLine($"{day.Key}: {day.Value}");
}

Console.Write("曜日の番号を選択してください: ");
int selectedId = int.Parse(Console.ReadLine());

if (daysOfWeek.TryGetValue(selectedId, out string selectedDay))
{
    Console.WriteLine($"選択した曜日: {selectedDay}");
}
else
{
    Console.WriteLine("ID が正しくありません");
}


var httpClient = new HttpClient();
string airportsApiUrl = "http://localhost:5000/airports"; // Replace with the actual API URL

HttpResponseMessage responseAirports = await httpClient.GetAsync(airportsApiUrl);
if (responseAirports.IsSuccessStatusCode)
{
    var responseContent = await responseAirports.Content.ReadAsStringAsync();
    var airportData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(responseContent);

    if (airportData != null)
    {
        Console.WriteLine($"ID:\t Name:");
        foreach (var airport in airportData)
        {
            Console.WriteLine($"{airport["id"]}\t{airport["name"]}");
        }
    }
    else
    {
        Console.WriteLine("飛行場データの読み取りでエラーが発生しました");
    }
}
else
{
    Console.WriteLine("飛行場データの読み取りでエラーが発生しました");
}

Console.Write("飛行場 ID を選択してください: ");
string selectedAirportId = Console.ReadLine();


var apiUrl = "http://localhost:5000/predict"; // Replace with the actual API URL


Console.WriteLine("AirportId: " + selectedAirportId);
Console.WriteLine("DayOfWeek: " + selectedId);

var apiUrlFul = apiUrl + $"?day_of_week={selectedId}&airport_id={selectedAirportId}";
Console.WriteLine("API URL: " + apiUrlFul);

HttpResponseMessage response = await httpClient.GetAsync(apiUrlFul);

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
