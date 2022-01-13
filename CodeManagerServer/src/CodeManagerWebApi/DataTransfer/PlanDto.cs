namespace CodeManagerWebApi.DataTransfer
{
    public class PlanDto
    {
        public string Name { get; set; }
        public int MaxCreatedTeamsPerUser { get; set; }
        public int MaxJoinedTeamsPerUser { get; set; }
    }
}