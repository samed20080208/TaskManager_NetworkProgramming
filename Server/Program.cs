using Server;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var _ip = IPAddress.Loopback;
var _port = 27001;

using var listener = new TcpListener(IPAddress.Loopback, _port);
listener.Start();

Console.WriteLine($"Server Connection : {listener.LocalEndpoint}");

while (true)
{
    var client = listener.AcceptTcpClient();
    var stream = client.GetStream();
    var _binaryReader = new BinaryReader(stream);
    var _binaryWriter = new BinaryWriter(stream);


    while (true)
    {
        var input = _binaryReader.ReadString();
        var command = JsonSerializer.Deserialize<Command>(input);

        if (command is null) continue;
        Console.WriteLine($"{command!.Text}   {command.Param}");



        switch (command.Text)
        {
            case Command.ProsesList:
                var prosses = Process.GetProcesses();
                var prossesNames = JsonSerializer.Serialize(prosses.Select(p => p.ProcessName));
                _binaryWriter.Write(prossesNames);
                break;

            case Command.Start:
                try
                {
                    Process.Start($"{command.Param!.ToLower()}");
                    _binaryWriter.Write($"{command.Param} Runned");
                }
                catch (Exception ex)
                {
                    _binaryWriter.Write(ex.Message);
                }
                break;

            case Command.Kill:
                try
                {
                    var task = Task.Run(() =>
                    {
                        var prosses1 = Process.GetProcesses().ToList().FirstOrDefault(p => p.ProcessName == command.Param!);
                        if (prosses1 is not null)
                        {
                            prosses1.Kill();
                            _binaryWriter.Write($"{prosses1.ProcessName} Killed");
                        }
                        else _binaryWriter.Write("Not Killed");

                    });

                }
                catch (Exception ex)
                {
                    _binaryWriter.Write(ex.Message);
                }
                break;



            default:
                break;
        }
    }
}