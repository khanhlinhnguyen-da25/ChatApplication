//SystemMessage cũng là một loại Message.
public class SystemMessage : Message
{
    public string Text {get; }//nội dung thông báo của hệ thống
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
        return "[SYSTEM] " + Text;
    }

}
// POLYMORPHISM:
// Cả 2 đều có GetContent()
// Nhưng: 
// TextMessage
// GetContent()
// -> "Hello An"

// SystemMessage
// GetContent()
// -> "[SYSTEM] An joined the group."