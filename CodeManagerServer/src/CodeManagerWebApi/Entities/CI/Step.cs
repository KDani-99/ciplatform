namespace CodeManagerWebApi.Entities.CI
{
    public class Step : Entity
    {
        public string Name { get; set; }
        public States State { get; set; }
    }
}