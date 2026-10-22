// TextMessage kế thừa từ Message và chứa nội dung văn bản thông thường.
public class TextMessage : Message
{
    public string Text {get; }// Nội dung của tin nhắn.

    // Constructor truyền ID và sender lên Message để lớp cha xử lý phần dùng chung.
    public TextMessage(string id, User sender, string text) : base(id, sender)
    {
        if(string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be null or whitespace.", nameof(text));
        }
        Text=text;
    }

    // Trả về nội dung văn bản khi conversation cần hiển thị message.
    public override string GetContent()
    {
        return Text;
    }


}