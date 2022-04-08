using FileTransferApp.Interfaces;
using Serilog;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileTransferApp
{
    public class HelperService : IHelperService
    {
        public bool DirectoryExist(string sPath)
        {
            return Directory.Exists(sPath);
        }

        public async Task ProcessFilesTransfer(string sourceDir, string DestDir)
        {
            if (!DirectoryExist(sourceDir))
            {
                Log.Error($"ERROR: Source Dir - {sourceDir} does not exist");
                return;
            }

            if (!DirectoryExist(DestDir))
            {
                Log.Error($"ERROR: Destination Dir - {DestDir} does not exist");
                return;
            }

            var files = new DirectoryInfo(sourceDir)
                .GetFiles("*", SearchOption.AllDirectories)
                .GroupBy(f => f.Extension).Select(s => s);

            foreach (var file in files)
            {
                Parallel.ForEach(file, f =>
                {
                    var filePath = $@"{DestDir}\{f.Name}";

                    if (!File.Exists(filePath))
                    {
                        Log.Information($"Starting moving file {f.Name} to {DestDir}");
                        f.MoveTo(filePath);
                    }
                });
            }

            await Task.CompletedTask;
        }
    }
}