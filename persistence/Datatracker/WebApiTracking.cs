using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Persistence.SignalR;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
// using Persistence.Helpers;
using testAspOracle01.persistence.Helpers;

namespace Persistence.DataTracker
{
    public class WebApiTracking
    {
        public static string TrackingUrl;
        private static HubConnection hubConnection;

        // private readonly IHubContext<TrackingHub> _hub;
        //  public WebApiTracking(IHubContext<TrackingHub> hub)
        // {
        //     _hub = hub;
        // }

        public static string SetUpSignalRTracking()
        {
            if (hubConnection != null)
            {
                if (hubConnection.State == HubConnectionState.Connected)
                    return "OK";
            }

            //HttpTransportType transportType = HttpTransportType.None;
            var url = TrackingUrl;
            // Create a handler that accepts custom (untrusted) certificates
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                    // Validate the cert here and return true if it's correct.
                    // If this is a development app, you could just return true always
                    // In production you should ALWAYS either use a trusted cert or check the thumbprint of the cert matches one you expect.
                }
            };

            WebApiTracking.hubConnection = new HubConnectionBuilder()
                .WithUrl(url, options =>
                {
                    // Register the custom handler above and also configure WebSockets
                    options.HttpMessageHandlerFactory = _ => handler;
                    options.WebSocketConfiguration = sockets =>
                    {
                        sockets.RemoteCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) =>
                        {
                            return true;
                            // You have to repeat your cert validation code here. Feel free to use a helper method!
                        });
                    };
                })
                .Build();
                
            // WebApiTracking.hubConnection = transportType == HttpTransportType.None ?
            //    new HubConnectionBuilder()
            //    .WithUrl(url)
            //    .Build() :
            //    new HubConnectionBuilder()
            //    .WithUrl(url, transportType)
            //    .Build();

            try
            {
                hubConnection.StartAsync().Wait();
                return "OK";
            }
            catch (Exception ex)
            {
                Console.WriteLine("WebApiTracking.SetUpSignalRTracking", ex.ToString());
                return ex.ToString();
            }
        }

        public static async Task<string> SendAsync(string groupName, Message m)
        {
            string status = WebApiTracking.SetUpSignalRTracking();
            if (status != "OK")
                return status;
            await WebApiTracking.hubConnection.InvokeAsync("AddToGroup", groupName);
            await WebApiTracking.hubConnection.InvokeAsync("SendMessage", groupName, m);
            await WebApiTracking.hubConnection.InvokeAsync("RemoveFromGroup", groupName);

            return status;
        }

        public static async Task SendToSignalRWithTimeUsedAsync(User user, string header, string body, string footer, Stopwatch stopwatch, DateTime timeStart, DateTime timeEnd)
        {
            if (string.IsNullOrEmpty(user.TrackingStatus))
                return;

            if (user.TrackingStatus != "T")
                return;
            if (WebApiTracking.SetUpSignalRTracking()  != "OK")
                return;
            string trackMessage = string.Empty;

            TimeSpan ts = stopwatch.Elapsed;
            trackMessage += System.Environment.NewLine
                + " Time : " + timeStart.ToString() + " => " + timeEnd.ToString()
                + System.Environment.NewLine
                + " Time used : " + String.Format("{0:00} Hours {1:00} Minutes {2:00} Seconds {3:00} Milliseconds",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10)
                + System.Environment.NewLine;


            Message m = new Message();
            m.Header = header;
            m.Body = body;
            m.Footer = footer;
            m.TimeStamp = trackMessage;

            await WebApiTracking.hubConnection.InvokeAsync("AddToGroup", user.UserId);
            await WebApiTracking.hubConnection.InvokeAsync("SendMessage", user.UserId, m);
            await WebApiTracking.hubConnection.InvokeAsync("RemoveFromGroup", user.UserId);
        }

        public static void SendToSignalRWithTimeUsed(User user, string header, string body, string footer, Stopwatch stopwatch, DateTime timeStart, DateTime timeEnd)
        {
            if (string.IsNullOrEmpty(user.TrackingStatus))
                return;

            if (user.TrackingStatus != "T")
                return;
            if (WebApiTracking.SetUpSignalRTracking() != "OK")
                return;
            string trackMessage = string.Empty;

            TimeSpan ts = stopwatch.Elapsed;
            trackMessage += System.Environment.NewLine
                + " Time : " + timeStart.ToString() + " => " + timeEnd.ToString()
                + System.Environment.NewLine
                + " Time used : " + String.Format("{0:00} Hours {1:00} Minutes {2:00} Seconds {3:00} Milliseconds",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10)
                + System.Environment.NewLine;


            Message m = new Message();
            m.Header = header;
            m.Body = body;
            m.Footer = footer;
            m.TimeStamp = trackMessage;

            WebApiTracking.hubConnection.InvokeAsync("AddToGroup", user.UserId);
            WebApiTracking.hubConnection.InvokeAsync("SendMessage", user.UserId, m);
            WebApiTracking.hubConnection.InvokeAsync("RemoveFromGroup", user.UserId);
        }

    }

    public enum TrackingTypeEnum
    {
        HEADER,
        SUCCESS,
        ERROR,
        WARNING,
        CONTENT
    }
}