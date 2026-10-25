// Conversation chuyên biệt cho trao đổi one-to-one.
// Lớp này kế thừa toàn bộ logic lưu message, notification, search và read
// receipt từ ConversationBase, chỉ giữ lại các rule riêng của direct chat.
public class DirectConversation : ConversationBase
{
    // ConversationBase dùng tên này khi in participant/history ra console.
    protected override string ConversationTypeName => "Direct Conversation";

    // Direct chat luôn phải bắt đầu với đúng hai participant.
    public DirectConversation(string id, string name, List<User> participants)
        : base(id, name, participants)
    {
        if (participants.Count != 2)
        {
            throw new ArgumentException(
                "Direct conversation must have exactly two participants.",
                nameof(participants));
        }
    }

    protected override void ValidateCanAddParticipant()
    {
        // AddParticipant() trong lớp cha gọi hook này trước khi thêm user.
        // Nhờ override, cùng một API có behavior khác nhau ở direct và group.
        if (Participants.Count >= 2)
        {
            throw new InvalidOperationException(
                "Direct conversation can only have two participants.");
        }
    }
}
