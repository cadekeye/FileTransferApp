using FileTransferApp.Interfaces;
using Serilog;
using System.Collections.Generic;
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
            var files = new DirectoryInfo(sourceDir)
                .GetFiles("*", SearchOption.AllDirectories)
                .GroupBy(f => f.Extension).Select(s => s);

            List<Task> taskList = new List<Task>();

            files.ToList().ForEach(file =>
            {
                file.ToList().ForEach(f =>
                {
                    taskList.Add(MoveFile(DestDir, f));
                });
            });

            await Task.WhenAll(taskList);
        }

        private Task MoveFile(string DestDir, FileInfo f)
        {
            return Task.Run(() =>
            {
                var filePath = $@"{DestDir}\{f.Name}";

                if (!File.Exists(filePath))
                {
                    Log.Information($"Starting moving file {f.Name} to {DestDir}");
                    f.MoveTo(filePath);
                }
            });
        }
    }
}