using System.Threading.Tasks;

namespace FileTransferApp.Interfaces
{
    public interface IHelperService
    {
        Task ProcessFilesTransfer(string sourceDir, string DestDir);

        bool DirectoryExist(string sPath);
    }
}