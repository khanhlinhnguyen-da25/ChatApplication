// Đại diện cho cuộc trò chuyện nhóm, có thể có từ hai người trở lên.
public class GroupConversation : IConversation
{
    // Thông tin cơ bản và dữ liệu được quản lý bởi conversation.
    public string Id { get; }
    public string Name { get; }
    public List<User> Participants { get; }
    public List<Message> Messages { get; }

    // Mỗi user có một tập message riêng để theo dõi trạng thái đã đọc.
    private Dictionary<User, HashSet<Message>> readStatus = new Dictionary<User, HashSet<Message>>();

    // Khởi tạo conversation nhóm và loại bỏ dữ liệu người dùng không hợp lệ.
    public GroupConversation(string id, string name, List<User> participants)
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
        if (participants.Any(participant => participant == null))
        {
            throw new ArgumentException(
                "Participants cannot contain null users.",
                nameof(participants));
        }
        if (participants.Distinct().Count() != participants.Count)
        {
            throw new ArgumentException(
                "Group conversation cannot contain duplicate participants.",
                nameof(participants));
        }

        Id = id;
        Name = name;
        Participants = participants;
        Messages = new List<Message>();

        // Mỗi participant bắt đầu với trạng thái chưa đọc message nào.
        foreach (var participant in participants)   
        {
            readStatus[participant] = new HashSet<Message>();
        }
    }

    public void PrintParticipants()
    {
        // In tất cả thành viên hiện tại của nhóm.
        Console.WriteLine($"Participants in Group Conversation '{Name}':");
        foreach (var participant in Participants)
        {
            Console.WriteLine($"- {participant.Name}");
        }
    }

    public void PrintMessageHistory()
    {
        // Hiển thị message theo đúng thứ tự đã được thêm vào danh sách.
        Console.WriteLine($"Message history for Group Conversation '{Name}':");
        foreach (var message in Messages)
        {
            Console.WriteLine($"[{message.Timestamp}] {message.Sender.Name}: {message.GetContent()}");
        }
    }

    public void AddParticipant(User user)
    {
        if(user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        if(Participants.Contains(user))
        {
            throw new InvalidOperationException("User is already a participant of the group conversation.");
        }
        Participants.Add(user);
        // Thành viên mới chưa được đánh dấu đã đọc các message cũ.
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
            throw new InvalidOperationException("Sender is not a participant of the group conversation.");
        }

        // Message chỉ được thêm sau khi xác thực sender thuộc nhóm.
        Messages.Add(message);

        // Gửi notification cho mọi thành viên khác, không gửi lại cho sender.
        foreach (var participant in Participants.Where(participant => participant != message.Sender))
        {
            participant.ReceiveNotification($"New message from {message.Sender.Name}.");
        }
    }

    public void MarkAsRead(User user, Message message)
    {
        // Kiểm tra quan hệ giữa user/message và conversation trước khi ghi nhận.
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

        // Tìm kiếm nội dung của tất cả loại message mà không phân biệt hoa thường.
        return Messages
            .Where(message => message.GetContent().Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private void ValidateReadReceiptArguments(User user, Message message)
    {
        // Không cho phép đánh dấu đọc một message ngoài conversation hiện tại.
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
            throw new InvalidOperationException("User is not a participant of the group conversation.");
        }
        if (!Messages.Contains(message))
        {
            throw new InvalidOperationException("Message does not belong to the group conversation.");
        }
    }
}