// SystemMessage là message do hệ thống tạo ra, ví dụ thông báo một user tham gia nhóm.
public class SystemMessage : Message
{
    public string Text {get; }// Nội dung thông báo của hệ thống.
    public SystemMessage(string id, User sender, string text) : base(id, sender)
    {
        if(string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be null or whitespace.", nameof(text));
        }
        Text=text;
    }

    public override string GetContent()
    {
        // Thêm tiền tố để người đọc phân biệt system message với tin nhắn thường.
        return "[SYSTEM] " + Text;
    }

}