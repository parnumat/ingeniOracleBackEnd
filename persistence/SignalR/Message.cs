namespace Persistence.SignalR
{
    public class Message
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
        public string TimeStamp { get; set; }
        public int MessageType { get; set; }
    }
}