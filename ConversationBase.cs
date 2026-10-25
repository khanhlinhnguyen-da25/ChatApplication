using System.Collections.Generic;

// Lớp cơ sở chứa toàn bộ hành vi dùng chung của direct và group conversation.
// Hai lớp con chỉ cần cung cấp quy tắc riêng, ví dụ giới hạn participant của
// DirectConversation, thay vì phải lặp lại logic gửi message và read receipt.
public abstract class ConversationBase : IConversation
{
    // Collection nội bộ có thể thay đổi only through các method của conversation.
    private readonly List<User> participants;
    private readonly List<Message> messages = new();

    // Expose read-only views để caller có thể đọc dữ liệu nhưng không thể
    // gọi Add/Clear trực tiếp và bypass validation của AddParticipant/SendMessage.
    private readonly IReadOnlyList<User> participantView;
    private readonly IReadOnlyList<Message> messageView;

    // Mỗi participant có một last-read pointer riêng.
    // null nghĩa là user chưa đọc message nào trong conversation.
    private readonly Dictionary<User, Message?> lastReadMessage = new();

    // Metadata của conversation được cố định sau khi khởi tạo.
    public string Id { get; }
    public string Name { get; }

    // Interface chỉ expose quyền đọc; dữ liệu thật vẫn được lớp này quản lý.
    public IReadOnlyList<User> Participants => participantView;
    public IReadOnlyList<Message> Messages => messageView;

    // Tên loại conversation được lớp con cung cấp để dùng khi in console.
    protected abstract string ConversationTypeName { get; }

    protected ConversationBase(string id, string name, List<User> participants)
    {
        // Validate ngay tại boundary để conversation không thể tồn tại ở trạng
        // thái không hợp lệ.
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
                "Conversation cannot contain duplicate participants.",
                nameof(participants));
        }

        Id = id;
        Name = name;

        // Copy list đầu vào thay vì giữ reference của caller. Nếu không copy,
        // caller có thể sửa list gốc sau constructor và phá vỡ invariant.
        this.participants = new List<User>(participants);

        // AsReadOnly tạo view gắn với list nội bộ: list vẫn được cập nhật bởi
        // class này, nhưng bên ngoài không thể thay đổi trực tiếp.
        participantView = this.participants.AsReadOnly();
        messageView = messages.AsReadOnly();

        // User mới bắt đầu với trạng thái chưa đọc message nào.
        foreach (var participant in this.participants)
        {
            lastReadMessage[participant] = null;
        }
    }

    public void PrintParticipants()
    {
        Console.WriteLine($"Participants in {ConversationTypeName} '{Name}':");
        foreach (var participant in participants)
        {
            Console.WriteLine($"- {participant.Name}");
        }
    }

    public void PrintMessageHistory()
    {
        Console.WriteLine($"Message history for {ConversationTypeName} '{Name}':");
        foreach (var message in messages)
        {
            Console.WriteLine($"[{message.Timestamp}] {GetSenderDisplayName(message)}: {message.GetContent()}");
        }
    }

    public void AddParticipant(User user)
    {
        // SystemUser chỉ là identity để ký SystemMessage, không phải thành
        // viên thực tế của conversation.
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        if (user is SystemUser)
        {
            throw new InvalidOperationException("The system identity cannot join a conversation.");
        }
        if (participants.Contains(user))
        {
            throw new InvalidOperationException("User is already a participant of the conversation.");
        }

        // Cho lớp con quyết định rule riêng:
        // DirectConversation sẽ reject khi đã đủ hai người; group thì cho phép.
        ValidateCanAddParticipant();
        participants.Add(user);

        // Participant mới chưa đọc các message cũ.
        lastReadMessage[user] = null;
    }

    public void SendMessage(Message message)
    {
        // Message phải hợp lệ trước khi được thêm vào history.
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message), "Message cannot be null.");
        }
        if (messages.Any(existingMessage => existingMessage.Id == message.Id))
        {
            throw new InvalidOperationException(
                $"A message with ID '{message.Id}' already exists in the conversation.");
        }
        // Message thường phải do participant gửi. Ngoại lệ duy nhất là
        // SystemMessage được ký bởi SystemUser và không cần SystemUser nằm
        // trong Participants.
        bool isValidSystemMessage =
            message is SystemMessage && message.Sender is SystemUser;

        if (!isValidSystemMessage && !participants.Contains(message.Sender))
        {
            throw new InvalidOperationException("Sender is not a participant of the conversation.");
        }

        // Chỉ append sau khi tất cả validation đã thành công.
        messages.Add(message);

        // Người gửi không nhận notification của chính mình. SystemUser không
        // nằm trong participants nên system message sẽ notify toàn bộ user
        // thật đang online.
        string senderName = GetSenderDisplayName(message);
        foreach (var participant in participants.Where(participant => !ReferenceEquals(participant, message.Sender)))
        {
            string notification = message is SystemMessage
                ? $"New system message in {Name}."
                : $"New message from {senderName}.";
            participant.ReceiveNotification(notification);
        }
    }

    public void MarkAsRead(User user, Message message)
    {
        ValidateReadReceiptArguments(user, message);

        // Read receipt là con trỏ đơn điệu: chỉ được tiến về phía message mới
        // hơn, không lùi về message cũ. Nhờ vậy đọc message thứ ba đồng nghĩa
        // đã đọc message thứ nhất và thứ hai.
        int currentIndex = GetMessageIndex(lastReadMessage[user]);
        int newIndex = GetMessageIndex(message);
        if (newIndex > currentIndex)
        {
            lastReadMessage[user] = message;
        }
    }

    public bool HasUserReadMessage(User user, Message message)
    {
        ValidateReadReceiptArguments(user, message);
        Message? lastRead = lastReadMessage[user];

        // Message được xem là đã đọc nếu nó đứng trước hoặc đúng tại pointer.
        return lastRead != null &&
               GetMessageIndex(message) <= GetMessageIndex(lastRead);
    }

    public List<Message> SearchMessages(string keyword)
    {
        // Search ở đây chỉ giới hạn trong một conversation. Việc search qua
        // nhiều conversation được ConversationSearchService đảm nhiệm.
        if (string.IsNullOrWhiteSpace(keyword))
        {
            throw new ArgumentException("Keyword cannot be null or whitespace.", nameof(keyword));
        }

        // OrdinalIgnoreCase giúp tìm kiếm không phân biệt hoa thường.
        return messages
            .Where(message => message.GetContent().Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    protected virtual void ValidateCanAddParticipant()
    {
        // GroupConversation dùng rule mặc định. DirectConversation override
        // method này để giới hạn conversation ở hai participant.
    }

    private void ValidateReadReceiptArguments(User user, Message message)
    {
        // Read receipt chỉ hợp lệ khi user là participant và message thuộc
        // đúng conversation hiện tại.
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message), "Message cannot be null.");
        }
        if (!participants.Contains(user))
        {
            throw new InvalidOperationException("User is not a participant of the conversation.");
        }
        if (!messages.Contains(message))
        {
            throw new InvalidOperationException("Message does not belong to the conversation.");
        }
    }

    private static string GetSenderDisplayName(Message message)
    {
        // SystemMessage có SystemUser sender nhưng khi hiển thị cần dùng tên
        // nghiệp vụ "System", không phụ thuộc vào object name nội bộ.
        return message is SystemMessage ? "System" : message.Sender.Name;
    }

    private int GetMessageIndex(Message? message)
    {
        // -1 biểu diễn chưa có last-read pointer.
        return message == null ? -1 : messages.IndexOf(message);
    }
}
