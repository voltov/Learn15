using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        using HttpClient client = new HttpClient(new HttpClientHandler { UseCookies = false });
        string url = "http://localhost:8888/";

        // Send a GET request to the root URL
        HttpResponseMessage response = await client.GetAsync(url);
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"GET Response: {responseBody}");

        // Send a GET request to the MyName URL
        response = await client.GetAsync(url + "MyName/");
        responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"GET MyName Response: {responseBody}");

        // Send requests to URLs and output response codes
        await SendRequestAndPrintResponseCode(client, url + "Information/");
        await SendRequestAndPrintResponseCode(client, url + "Success/");
        await SendRequestAndPrintResponseCode(client, url + "Redirection/");
        await SendRequestAndPrintResponseCode(client, url + "ClientError/");
        await SendRequestAndPrintResponseCode(client, url + "ServerError/");

        // Send a GET request to the MyNameByHeader URL
        response = await client.GetAsync(url + "MyNameByHeader/");
        if (response.Headers.TryGetValues("X-MyName", out var values))
        {
            string myName = string.Join(",", values);
            Console.WriteLine($"GET MyNameByHeader Response Header X-MyName: {myName}");
        }

        // Send a GET request to the MyNameByCookies URL
        response = await client.GetAsync(url + "MyNameByCookies/");
        if (response.Headers.TryGetValues("Set-Cookie", out var cookieValues))
        {
            foreach (var cookieValue in cookieValues)
            {
                Console.WriteLine($"GET MyNameByCookies Response Cookie: {cookieValue}");
            }
        }

        // Send a POST request to exit the listener
        HttpContent content = new StringContent("", Encoding.UTF8, "application/json");
        response = await client.PostAsync(url + "exit", content);
        Console.WriteLine($"POST Response: {response.StatusCode}");
    }

    static async Task SendRequestAndPrintResponseCode(HttpClient client, string url)
    {
        HttpResponseMessage response = await client.GetAsync(url);
        Console.WriteLine($"GET {url} Response Status Code: {response.StatusCode}");
    }
}
