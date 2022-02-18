using System.Threading.Tasks;

namespace CIPlatformWebApi.Services.FileProcessor
{
    public interface IFileProcessorService<T>
        where T : new()
    {
        public Task<T> ProcessAsync(string data);
    }
}