namespace CodeManager.Data.Entities
{
    public class Variable : Entity
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsSecret { get; set; }
        public Project Project { get; set; }
    }
}