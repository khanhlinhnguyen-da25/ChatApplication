// Đại diện cho cuộc trò chuyện riêng giữa đúng hai người dùng.
public class DirectConversation : IConversation
{
    // Các thuộc tính này chỉ được gán khi khởi tạo conversation.
    public string Id { get; }
    public string Name { get; }
    public List<User> Participants { get; }
    public List<Message> Messages { get; }

    // Với mỗi user, lưu những message mà user đó đã đọc.
    private Dictionary<User, HashSet<Message>> readStatus = new Dictionary<User, HashSet<Message>>();

    // Tạo conversation riêng và kiểm tra toàn bộ điều kiện đầu vào ngay từ đầu.
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

        // Ban đầu chưa có message nào được đọc.
        foreach (var participant in participants)
        {
            readStatus[participant] = new HashSet<Message>();
        }

    }

    public void PrintParticipants()
    {
        // In danh sách người tham gia để minh họa dữ liệu của conversation.
        Console.WriteLine($"Participants in Direct Conversation '{Name}':");
        foreach (var participant in Participants)
        {
            Console.WriteLine($"- {participant.Name}");
        }
    }

    public void PrintMessageHistory()
    {
        // GetContent() giúp hiển thị đúng nội dung của mọi loại Message.
        Console.WriteLine($"Message history for Direct Conversation '{Name}':");
        foreach (var message in Messages)
        {
            Console.WriteLine($"[{message.Timestamp}] {message.Sender.Name}: {message.GetContent()}");
        }
    }

    public void AddParticipant(User user)
    {
        // Chat riêng không được vượt quá hai người.
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
        // User mới chưa đọc bất kỳ message nào trong conversation.
        readStatus[user] = new HashSet<Message>();
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

        // Chỉ lưu message sau khi chắc chắn sender thuộc conversation.
        Messages.Add(message);

        // Người gửi không cần nhận notification cho chính message của mình.
        foreach (var participant in Participants.Where(participant => participant != message.Sender))
        {
            participant.ReceiveNotification($"New message from {message.Sender.Name}.");
        }
    }

    public void MarkAsRead(User user, Message message)
    {
        // Hàm dùng chung đảm bảo user và message đều hợp lệ trước khi cập nhật.
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

        // OrdinalIgnoreCase giúp tìm kiếm không phân biệt chữ hoa/chữ thường.
        return Messages
            .Where(message => message.GetContent().Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private void ValidateReadReceiptArguments(User user, Message message)
    {
        // Read receipt chỉ hợp lệ khi user và message thuộc conversation này.
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