using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using Server;
using System.Text.Json;

namespace Client;

public partial class MainWindow : Window
{
    private IPAddress _ip = IPAddress.Loopback;
    private int _port = 27001;
    private NetworkStream _stream;
    private BinaryReader _binaryReader;
    private BinaryWriter _binaryWriter;
    string response = null!;
    Command command = null!;

    public MainWindow()
    {
        InitializeComponent();
        Combobox.Items.Add("Kill");
        Combobox.Items.Add("Start");
        Starting();
    }


    private void Starting()
    {
        var client = new TcpClient();
        client.Connect(_ip, _port);

        _stream = client.GetStream();
        _binaryReader = new BinaryReader(_stream);
        _binaryWriter = new BinaryWriter(_stream);
    }

    private void BtnRefersh_Click(object sender, RoutedEventArgs e)
    {

        Dispatcher.Invoke(() =>
        {
            ListBox.Items.Clear();
            Task.Run(() =>
            {
                command = new Command { Text = Command.ProsesList, Param = "" };
                _binaryWriter.Write(JsonSerializer.Serialize(command));
                response = _binaryReader.ReadString();
                var processList = JsonSerializer.Deserialize<string[]>(response);
                foreach (var prossesName in processList!)
                {
                    Dispatcher.Invoke(() => ListBox.Items.Add(prossesName));
                }
            });

        });
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var selectedProses = Combobox.SelectedItem;

        if (selectedProses is not null)
        {
            if (selectedProses.ToString() == Command.Start) StratProses();
            else if (selectedProses.ToString() == Command.Kill) KillProsses();

        }
    }

    private void StratProses()
    {
        try
        {
            if (txtBox.Text is not null)
            {
                command = new Command { Text = Command.Start, Param = txtBox.Text };
                _binaryWriter.Write(JsonSerializer.Serialize(command));
                response = _binaryReader.ReadString();
                MessageBox.Show(response);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void KillProsses()
    {
        try
        {
            var selectedValue = ListBox.SelectedItem as string;

            if (selectedValue is not null)
            {
                command = new Command { Text = Command.Kill, Param = selectedValue };
                _binaryWriter.Write(JsonSerializer.Serialize(command));
                response = _binaryReader.ReadString();
                MessageBox.Show(response);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
}