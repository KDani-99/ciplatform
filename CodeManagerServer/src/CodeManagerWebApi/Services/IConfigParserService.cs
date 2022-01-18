namespace CodeManagerWebApi.Services
{
    public interface IConfigParserService<T>
    {
        public T Parse(string data);
    }
}