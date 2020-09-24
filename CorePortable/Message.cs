namespace SamuS._100to1.CorePortable
{
    public class Message
    {
        public string Command { get; set; }

        public Answer Answer { get; set; }
        public int Retry { get; set; }
        public int Score { get; set; }
        public Team Team { get; set; }
        public string Tag { get; set; }
    }
}
