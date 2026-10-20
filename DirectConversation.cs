public class DirectConversation : IConversation
{
    public string Id { get; }
    public string Name { get; }
    public List<User> Participants { get; }
    public List<Message> Messages { get; }

    private Dictionary<User, HashSet<Message>> readStatus = new Dictionary<User, HashSet<Message>>();
    public DirectConversation(string id, string name, List<User> participants)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Conversation ID cannot be empty.", nameof(id));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Conversation name cannot be empty.", nameof(name));
        }
        if (participants == null)
        {
            throw new ArgumentNullException(nameof(participants));
        }
        if (participants.Count != 2)
        {
            throw new ArgumentException(
                "Direct conversation must have exactly two participants.",
                nameof(participants));
        }
        if (participants.Any(participant => participant == null))
        {
            throw new ArgumentException(
                "Participants cannot contain null users.",
                nameof(participants));
        }
        if (participants.Distinct().Count() != participants.Count)
        {
            throw new ArgumentException(
                "Direct conversation cannot contain duplicate participants.",
                nameof(participants));
        }

        Id = id;
        Name = name;
        Participants = participants;
        Messages = new List<Message>();
        foreach (var participant in participants)
        {
            readStatus[participant] = new HashSet<Message>();
        }

    }

    public void PrintParticipants()
    {
        Console.WriteLine($"Participants in Direct Conversation '{Name}':");
        foreach (var participant in Participants)
        {
            Console.WriteLine($"- {participant.Name}");
        }
    }

    public void PrintMessageHistory()
    {
        Console.WriteLine($"Message history for Direct Conversation '{Name}':");
        foreach (var message in Messages)
        {
            Console.WriteLine($"[{message.Timestamp}] {message.Sender.Name}: {message.GetContent()}");
        }
    }

    public void AddParticipant(User user)
    {
        if(Participants.Count >= 2)
        {
            throw new InvalidOperationException("Direct conversation can only have two participants.");
        }

        if(user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        if(Participants.Contains(user))
        {
            throw new InvalidOperationException("User is already a participant of the direct conversation.");
        }
        Participants.Add(user);
        readStatus[user] = new HashSet<Message>(); // Initialize read status for the new participant
    }

    public void SendMessage(Message message)
    {
        if(message == null)
        {
            throw new ArgumentNullException(nameof(message), "Message cannot be null.");
        }
        if(!Participants.Contains(message.Sender))
        {
            throw new InvalidOperationException("Sender is not a participant of the direct conversation.");
        }
        Messages.Add(message);

        foreach (var participant in Participants.Where(participant => participant != message.Sender))
        {
            participant.ReceiveNotification($"New message from {message.Sender.Name}.");
        }
    }

    public void MarkAsRead(User user, Message message)
    {
        ValidateReadReceiptArguments(user, message);
        readStatus[user].Add(message);
    }

    public bool HasUserReadMessage(User user, Message message)
    {
        ValidateReadReceiptArguments(user, message);
        return readStatus[user].Contains(message);
    }

    public List<Message> SearchMessages(string keyword)
    {
        if(string.IsNullOrWhiteSpace(keyword))
        {
            throw new ArgumentException("Keyword cannot be null or whitespace.", nameof(keyword));
        }

        return Messages
            .Where(message => message.GetContent().Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private void ValidateReadReceiptArguments(User user, Message message)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message), "Message cannot be null.");
        }
        if (!Participants.Contains(user))
        {
            throw new InvalidOperationException("User is not a participant of the direct conversation.");
        }
        if (!Messages.Contains(message))
        {
            throw new InvalidOperationException("Message does not belong to the direct conversation.");
        }
    }
}