using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlBackup
{
    public interface IDirectoryService
    {
        IEnumerable<string> GetFiles(IEnumerable<string> paths, IEnumerable<string> extensions, bool recursive = false);

        Task<IEnumerable<string>> GetFilesAsync(IEnumerable<string> paths, IEnumerable<string> extensions, bool recursive = false);
    }
}