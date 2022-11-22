using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppSignalRTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection connection;

        public MainWindow()
        {
            InitializeComponent();
            connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:3373/broadcastHub")
            .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }

        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            connection.On<string, string, string>("RecieveRFIDValidationMessage", (topic, identifier, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{topic}: {message}";
                    messagesList.Items.Add(newMessage);
                });
            });

            try
            {
                await connection.StartAsync();
                messagesList.Items.Add("Connection started");
                connectButton.IsEnabled = false;
                sendButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var s = new { topic = "RF_ID_USING_POST", identifier = connection.ConnectionId, data = "RF_ID is match" };
                    client.BaseAddress = new Uri("http://localhost:3373/");
                    var response = client.PostAsJsonAsync("api/communication/NotifyRfidScan", s).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        Console.Write("Success");
                    }
                    else
                        Console.Write("Error");
                }
                await connection.InvokeAsync("NotifyRFIDValidation", "RF_ID_USING_PUSH",
                    this.connection.ConnectionId, messageTextBox.Text);

            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }

        private void clearListButton_Click(object sender, RoutedEventArgs e)
        {
            messagesList.Items.Clear();            
        }
    }
}
