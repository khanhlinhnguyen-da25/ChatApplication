using System;
// Abstract base class cho mọi message trong ứng dụng.
// TextMessage và SystemMessage kế thừa state/validation chung, sau đó override
// GetContent() để tạo cách hiển thị phù hợp với từng loại message.

public abstract class Message
{
    // Id dùng để nhận diện message và chống duplicate trong conversation.
    public string Id {get; }
    // Sender là User thường hoặc SystemUser đối với SystemMessage.
    public User Sender {get; }
    // Lưu UTC để kết quả không phụ thuộc timezone của máy chạy chương trình.
    public DateTime Timestamp {get; }

    // Nội dung gốc dùng chung cho search và các lớp con.
    public string Text {get; }

    // Protected để chỉ lớp message cụ thể mới được tạo thông qua constructor
    // này; Message itself vẫn không thể khởi tạo trực tiếp.
    protected Message(string id, User sender, string text)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Message ID cannot be empty.", nameof(id));
        }
        if (sender == null)
        {
            throw new ArgumentNullException(nameof(sender), "Sender cannot be null.");
        }
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Message text cannot be empty.", nameof(text));
        }

        Id = id;
        Sender = sender;
        Text = text ?? throw new ArgumentNullException(nameof(text), "Text cannot be null.");
        Timestamp = DateTime.UtcNow;
    }

    // Polymorphic operation: conversation không cần biết subtype cụ thể, chỉ
    // gọi GetContent() và runtime chọn implementation của lớp con.
    public abstract string GetContent();
 
    
}
