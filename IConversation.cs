using System.Collections.Generic;
//Interface chung cho các loại Conversation
//DirectConversation và GroupConversation sau này đều sẽ implement interface này

public interface IConversation
{
    string Id{get; }//ID của conversation
    string Name{get; }

    // Danh sách các user tham gia conversation
    List<User> Participants{get; }

    // Danh sách các message trong conversation
    List<Message> Messages{get; }

    // In ra danh sách các user tham gia conversation
    void PrintParticipants();

    // In ra lịch sử message trong conversation
    void PrintMessageHistory();
    
    // Thêm một user vào conversation
    void AddParticipant(User user);

    // Gửi một message vào conversation
    void SendMessage(Message message);

    // Đánh dấu user đã đọc tới message này
    void MarkAsRead(User user, Message message);

    // Kiểm tra user đã đọc message hay chưa
    bool HasUserReadMessage(User user, Message message);

    // Tìm các message có chứa keyword
    List<Message> SearchMessages(string keyword);

}