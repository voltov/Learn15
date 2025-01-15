using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8888/");
        listener.Start();
        Console.WriteLine("Listening for connections on http://localhost:8888/");

        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string resourcePath = GetResourcePath(request.Url.AbsolutePath);
            Console.WriteLine($"Request received for /{resourcePath}");

            if (request.HttpMethod == "POST" && resourcePath == "exit")
            {
                Console.WriteLine("Exit command received. Shutting down listener.");
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Close();
                break;
            }

            switch (resourcePath)
            {
                case "MyName":
                    await HandleMyNameRequest(response);
                    break;
                case "MyNameByHeader":
                    HandleMyNameByHeaderRequest(response);
                    break;
                case "MyNameByCookies":
                    HandleMyNameByCookiesRequest(response);
                    break;
                case "Information":
                    await HandleInformationRequest(response);
                    break;
                case "Success":
                    await HandleSuccessRequest(response);
                    break;
                case "Redirection":
                    await HandleRedirectionRequest(response);
                    break;
                case "ClientError":
                    await HandleClientErrorRequest(response);
                    break;
                case "ServerError":
                    await HandleServerErrorRequest(response);
                    break;
                default:
                    await HandleDefaultRequest(response);
                    break;
            }
        }

        listener.Stop();
    }

    static string GetResourcePath(string absolutePath)
    {
        return absolutePath.Trim('/').Split('/')[0];
    }

    static async Task HandleMyNameRequest(HttpListenerResponse response)
    {
        string responseString = "Volodymyr Voitovych";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.Close();
    }

    static void HandleMyNameByHeaderRequest(HttpListenerResponse response)
    {
        response.AddHeader("X-MyName", "Volodymyr Voitovych");
        response.StatusCode = (int)HttpStatusCode.OK;
        response.Close();
    }

    static void HandleMyNameByCookiesRequest(HttpListenerResponse response)
    {
        Cookie cookie = new Cookie("MyName", "Volodymyr Voitovych");
        response.Cookies.Add(cookie);
        response.StatusCode = (int)HttpStatusCode.OK;
        response.Close();
    }

    static async Task HandleInformationRequest(HttpListenerResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.Accepted;
        response.Close();
    }

    static async Task HandleSuccessRequest(HttpListenerResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.OK;
        response.Close();
    }

    static async Task HandleRedirectionRequest(HttpListenerResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.MultipleChoices;
        response.Close();
    }

    static async Task HandleClientErrorRequest(HttpListenerResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Close();
    }

    static async Task HandleServerErrorRequest(HttpListenerResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.InternalServerError;
        response.Close();
    }

    static async Task HandleDefaultRequest(HttpListenerResponse response)
    {
        string responseString = "Hello World!";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.Close();
    }
}
