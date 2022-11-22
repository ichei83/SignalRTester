using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SignalRTester
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:3373/broadcastHub")
                //.ConfigureLogging(logging =>
                //{
                //    logging.AddConsole();
                //})
                //.AddMessagePackProtocol()
                .Build();

            await connection.StartAsync();

            Console.WriteLine("Starting connection. Press Ctrl-C to close.");
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, a) =>
            {
                a.Cancel = true;
                cts.Cancel();
            };

            connection.Closed += e =>
            {
                Console.WriteLine("Connection closed with error: {0}", e);

                cts.Cancel();
                return Task.CompletedTask;
            };


            connection.On<string, string>("RecieveRFIDValidationMessage", (user, data) =>
            {
                Console.WriteLine("Message Pushed from server, ConnectionId: " + data + ", message: " + data);
            });

            connection.Reconnecting += error =>
            {
                Console.WriteLine(connection.State == HubConnectionState.Reconnecting);

                // Notify users the connection was lost and the client is reconnecting.
                // Start queuing or dropping messages.

                return Task.CompletedTask;
            };
            //connection.On("marketClosed", () =>
            //{
            //    Console.WriteLine("Market closed");
            //});

            //connection.On("marketReset", () =>
            //{
            //    // We don't care if the market rest
            //});
            while (true)
            { }
            //var channel = await connection.StreamAsChannelAsync<Stock>("StreamStocks", CancellationToken.None);
            //while (await channel.WaitToReadAsync() && !cts.IsCancellationRequested)
            //{
            //    while (channel.TryRead(out var stock))
            //    {
            //        Console.WriteLine($"{stock.Symbol} {stock.Price}");
            //    }
            //}
        }
    }
}
