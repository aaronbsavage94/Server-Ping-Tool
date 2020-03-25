using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows;

namespace Ping_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            //New ping object
            Ping ping = new Ping();

            //Blank out results text boxes
            resultsBoxSuccess.Text = "";
            resultsBoxFail.Text = "";
            
            //Count integer used later for server count
            int countInt = 0;

            //Build full string displaying how many servers have been pinged
            counter.Text = "Servers checked: " + countInt.ToString();

            //File path to be read in later
            string path = pathTextBox.Text;

            //Build lists for the two columns in the csv file
            List<string> servers = new List<string>();
            List<string> apps = new List<string>();

                //Try reading in csv from path, splitting on a comma and parsing values into lists
                try
                {
                    using (var reader = new StreamReader(path))
                    {
                        //While not at the end of StreamReader
                        while (!reader.EndOfStream)
                        {
                            //Splitting on comma because csv is comma delimited
                            var line = reader.ReadLine();
                            var values = line.Split(',');

                            //Add values to corresponding list
                            servers.Add(values[0]);
                            apps.Add(values[1]);
                        }
                    }
                }

                //Catch exception, show message box warning user
                catch (Exception ex)
                {
                    string alert = "Could not read file: " + path + "\n" + "Exception message: " + ex.Message;
                    MessageBox.Show(alert);
                }
                
                //Build arguments for asynchronous ping
                byte[] buffer = Encoding.ASCII.GetBytes(".");
                PingOptions options = new PingOptions(10, true);
                AutoResetEvent reset = new AutoResetEvent(false);

                //For all servers in list
                for (int i = 0; i < servers.Count; i++)
                {
                    //Try pinging, checking reply and comparing IPStatus
                    try
                    {
                        //Ping reply to be checked later
                        var reply = await ping.SendPingAsync(servers[i], 1000, buffer, options);

                        //If ping reply is successful, append text to success text box
                        if (reply.Status == IPStatus.Success)
                        {
                            //Success string
                            resultsBoxSuccess.Text += servers[i] + " (OK) - " + apps[i] + "\n";
                            
                            //Increase counter and build string for counter text block
                            countInt++;
                            int lines = File.ReadAllLines(path).Length;
                            counter.Text = "Severs checked: " + countInt.ToString() + " out of " + lines;
                        }

                        //Else, failure logic
                        else
                        {
                            //Append fail text box with below string
                            resultsBoxFail.Text += servers[i] + " (FAILED) - " + apps[i] + "\n";

                            //Increase counter and build string for counter text block
                            countInt++;
                            int lines = File.ReadAllLines(path).Length;
                            counter.Text = "Severs checked: " + countInt.ToString() + " out of " + lines;
                        }
                    }

                    //Catch exceptions, also indicate failure
                    catch
                    {
                        //Append fail text box with below string
                        resultsBoxFail.Text += servers[i] + " (FAILED) - " + apps[i] + "\n";

                        //Increase counter and build string for counter text block
                        countInt++;
                        int lines = File.ReadAllLines(path).Length;
                        counter.Text = "Severs checked: " + countInt.ToString() + " out of " + lines;
                    }
                }
            }

        //Button to duplicate current window
        private void Duplicate_Click(object sender, RoutedEventArgs e)
        {
            //Initialize new MainWindow object and show
            Window window = new MainWindow();
            window.Show();
        }

        //Button for instructions
        private void Instructions_Click(object sender, RoutedEventArgs e)
        {
            //Show messagebox, displaying below instructional string
            string initialAlert = "This tool reads in server names from a csv file and pings them.\nThe first column should have the server name and the second column should have some type of identifier (ie: service name, name of the server, etc.).";
            MessageBox.Show(initialAlert);
        }
    }
}