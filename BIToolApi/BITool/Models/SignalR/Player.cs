namespace BITool.Models.SignalR
{
    public class Player
    {
        public int GameId { get; set; }
        public string Name { get; set; }
        public DateTime JoinedOn { get; set; }
        public bool IsHost { get; set; }
    }
}