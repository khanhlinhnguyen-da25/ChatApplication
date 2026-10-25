// Message do hệ thống tạo ra cho các domain event như user joined/left group.
// SystemMessage vẫn nằm trong history như message bình thường nhưng được
// render khác và được delivery theo nhánh system trong ConversationBase.
public class SystemMessage : Message
{
    // Constructor bắt buộc sender là SystemUser để user thường không thể giả
    // mạo một system event.
    public SystemMessage(string id, User sender, string text) : base(id, sender, text)
    {
        if (sender is not SystemUser)
        {
            throw new ArgumentException(
                "SystemMessage sender must be the system identity.",
                nameof(sender));
        }
        if(string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be null or whitespace.", nameof(text));
        }
    }

    public override string GetContent()
    {
        // Prefix giúp người đọc và search result phân biệt system event với
        // nội dung do participant viết.
        return "[SYSTEM] " + Text;
    }

}