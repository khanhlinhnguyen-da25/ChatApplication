// Service phụ trách use case tìm kiếm xuyên nhiều conversation.
// ConversationBase chỉ biết search trong chính nó; class này chịu trách nhiệm
// lọc conversation theo participant rồi gộp các kết quả lại.
public sealed class ConversationSearchService
{
    // Snapshot các conversation tại thời điểm service được tạo.
    // IReadOnlyList ngăn service tự ý thay đổi collection của caller.
    private readonly IReadOnlyList<IConversation> conversations;

    // Nhận interface để service hoạt động với cả direct, group hoặc subtype mới.
    public ConversationSearchService(IEnumerable<IConversation> conversations)
    {
        if (conversations == null)
        {
            throw new ArgumentNullException(nameof(conversations));
        }

        // Copy enumerable một lần để tránh phụ thuộc vào collection bên ngoài
        // bị thay đổi trong lúc thực hiện search.
        this.conversations = conversations.ToList();
        if (this.conversations.Any(conversation => conversation == null))
        {
            throw new ArgumentException(
                "Conversation collection cannot contain null values.",
                nameof(conversations));
        }
    }

    public List<Message> SearchMessages(User user, string keyword)
    {
        // Validate tại public API để lỗi input được báo rõ ràng thay vì trả
        // về kết quả rỗng khiến caller tưởng search thành công.
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        if (string.IsNullOrWhiteSpace(keyword))
        {
            throw new ArgumentException(
                "Keyword cannot be null or whitespace.",
                nameof(keyword));
        }

        // Chỉ search conversation mà user thực sự là participant.
        // SelectMany gộp kết quả của tất cả conversation thành một list.
        return conversations
            .Where(conversation => conversation.Participants.Contains(user))
            .SelectMany(conversation => conversation.SearchMessages(keyword))
            .ToList();
    }
}
