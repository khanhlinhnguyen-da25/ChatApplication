// Message văn bản do một User trong conversation gửi.
// Đây là subtype đơn giản nhất của Message.
public class TextMessage : Message
{

    // Ủy quyền validation chung (id, sender, text, timestamp) cho Message.
    public TextMessage(string id, User sender, string text) : base(id, sender, text)
    {
        if(string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be null or whitespace.", nameof(text));
        }
    }

    // Override điểm mở rộng của Message; text message không cần prefix.
    public override string GetContent()
    {
        return Text;
    }
}