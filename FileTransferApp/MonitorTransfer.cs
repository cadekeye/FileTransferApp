using FileTransferApp.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace FileTransferApp
{
    public class MonitorTransfer
    {
        private readonly IBackgroundQueue _taskQueue;
        private readonly CancellationToken _cancellationToken;
        private readonly IHelperService _helperService;

        public MonitorTransfer(IBackgroundQueue taskQueue,
            IHostApplicationLifetime hostApplicationLifeTime,
            IHelperService helperService)
        {
            _taskQueue = taskQueue;
            _cancellationToken = hostApplicationLifeTime.ApplicationStopping;
            _helperService = helperService;
        }

        public void StartMonitorTransfer(string[] args)
        {
            Log.Information("MonitorAsync transfer is starting ...");

            Task.Run(async () => await MonitorAsync(args));
        }

        private async ValueTask MonitorAsync(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("***********************  COMMANDS **************************************");
            Console.WriteLine("********   Add a new pair of source and destinationfolders: enter 'a'");
            Console.WriteLine("*******             Exit: enter 'x'  ****************************");
            Console.WriteLine("*************************************************************************");

            while (!_cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(2000);

                var inputCommand = ReadUserCommand();

                if (inputCommand.Key == ConsoleKey.X)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Shutting down please wait...");
                    Environment.Exit(0);
                }
                else if (inputCommand.Key == ConsoleKey.A)
                {
                    var userEntry = ReadUserEntry();

                    var rootCommand = new RootCommand
                    {
                        new Option<string>("-s", description: "source folder"),
                        new Option<string>("-d", description:"destination folder")
                    };

                    rootCommand.Description = "Console App for file transfer.";
                    rootCommand.Handler = CommandHandler.Create<string, string>((arg1, arg2) =>
                    {
                        var allEntries = userEntry.Split(' ');

                        if (ValidateEntry(allEntries))
                        {
                            Execute(allEntries[1], allEntries[3]);
                        }
                    });

                    await rootCommand.InvokeAsync(args);
                }
                else
                {
                    Console.WriteLine("");
                    Log.Error("Error: Wrong command received.");
                }
            }
        }

        private ConsoleKeyInfo ReadUserCommand()
        {
            Console.WriteLine("");
            Console.WriteLine("What will you like to do? Please enter appropriate command 'a' to add new pair, 'x' to exit: ");

            return Console.ReadKey();
        }

        private string ReadUserEntry()
        {
            Console.WriteLine("");
            Console.WriteLine("**************************************************************************************");
            Console.WriteLine(@"Please supply the source Dir and Destination Dir in the format -s [\source] -d [\dest]: ");
            Console.WriteLine("**************************************************************************************");

            return Console.ReadLine();
        }

        private bool ValidateEntry(string[] allEntries)
        {
            if (allEntries.Length != 4)
            {
                Log.Error($"Wrong number of argument supplied");
                return false;
            }

            if (string.IsNullOrEmpty(allEntries[1]))
            {
                Log.Error("ERROR: Invalid entry for source folder.");
                return false;
            }

            if (string.IsNullOrEmpty(allEntries[3]))
            {
                Log.Error("ERROR: Invalid entry for destination folder.");
                return false;
            }

            if (!_helperService.DirectoryExist(allEntries[1]))
            {
                Log.Error($"Error: Source directory '{allEntries[1]}' supplied is not a valid folder");
                return false;
            }

            if (!_helperService.DirectoryExist(allEntries[3]))
            {
                Log.Error($"Error: Destination directory '{allEntries[3]}' supplied is not a valid folder ");
                return false;
            }

            return true;
        }

        private Task Execute(string source, string dest)
        {
            Log.Information($"args: {source} - {dest}");

            if (!string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(dest))
            {
                _taskQueue.QueueTask(async token =>
                {
                    await _helperService.ProcessFilesTransfer(source, dest);
                });
            }
            else
            {
                Log.Error("Error: Wrong entry made");
            }

            return Task.CompletedTask;
        }
    }
}