using System;
// Message là abstract class, đóng vai trò là khuôn mẫu chung cho mọi loại tin nhắn.
// Các lớp con hiện có: TextMessage và SystemMessage.

public abstract class Message
{
    public string Id {get; }// ID riêng của tin nhắn.
    public User Sender {get; }// Người gửi message.
    public DateTime Timestamp {get; }// Thời điểm message được tạo.

    // Constructor protected: chỉ các lớp kế thừa mới có thể gọi trực tiếp.
    protected Message(string id, User sender)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Message ID cannot be empty.", nameof(id));
        }
        if (sender == null)
        {
            throw new ArgumentNullException(nameof(sender), "Sender cannot be null.");
        }

        Id = id;
        Sender = sender;
        // Dùng UTC để thời gian không phụ thuộc vào múi giờ của máy chạy ứng dụng.
        Timestamp = DateTime.UtcNow;
    }

    // Mỗi loại message có cách tạo nội dung hiển thị riêng.
    // Lớp con bắt buộc override method này (tính đa hình).
    public abstract string GetContent();
 
    
}
