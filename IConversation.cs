using System.Collections.Generic;
// Interface chung cho các loại conversation.
// DirectConversation và GroupConversation cùng triển khai các hành vi này.

public interface IConversation
{
    string Id{get; }// ID duy nhất của conversation.
    string Name{get; }// Tên hiển thị của conversation.

    // Danh sách các user đang tham gia conversation.
    List<User> Participants{get; }

    // Các message được gửi trong conversation, theo thứ tự gửi.
    List<Message> Messages{get; }

    // In ra danh sách các user tham gia conversation.
    void PrintParticipants();

    // In ra lịch sử message trong conversation.
    void PrintMessageHistory();
    
    // Thêm một user vào conversation.
    void AddParticipant(User user);

    // Gửi message và xử lý các notification liên quan.
    void SendMessage(Message message);

    // Đánh dấu một user đã đọc message cụ thể.
    void MarkAsRead(User user, Message message);

    // Kiểm tra trạng thái đã đọc của một user với một message.
    bool HasUserReadMessage(User user, Message message);

    // Tìm các message có nội dung chứa keyword.
    List<Message> SearchMessages(string keyword);

}