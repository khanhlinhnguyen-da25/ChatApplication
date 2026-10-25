// Conversation dành cho từ hai participant trở lên.
// Logic dùng chung nằm trong ConversationBase; group chỉ bổ sung invariant
// tối thiểu hai thành viên và dùng rule mặc định khi thêm thành viên.
public class GroupConversation : ConversationBase
{
    // Nhãn hiển thị khi ConversationBase in history/participant.
    protected override string ConversationTypeName => "Group Conversation";

    // Group cần ít nhất hai participant để đúng bản chất của group chat.
    public GroupConversation(string id, string name, List<User> participants)
        : base(id, name, participants)
    {
        if (participants.Count < 2)
        {
            throw new ArgumentException(
                "Group conversation must have at least two participants.",
                nameof(participants));
        }
    }
}
