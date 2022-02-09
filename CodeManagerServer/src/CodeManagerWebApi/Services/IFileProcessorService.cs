using System.Threading.Tasks;

namespace CodeManagerWebApi.Services
{
    public interface IFileProcessorService<T>
        where T : new()
    {
        public Task<T> ProcessAsync(string data, long projectId);
    }
}