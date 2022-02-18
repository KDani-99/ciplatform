using System.Threading.Tasks;

namespace CIPlatformWebApi.Services.File
{
    public interface IFileProcessorService<T>
        where T : new()
    {
        public Task<T> ProcessAsync(string data);
    }
}