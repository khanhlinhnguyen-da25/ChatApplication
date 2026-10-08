using System.Collections.Generic;
//Interface chung cho các loại Conversation
//DirectConversation và GroupConversation sau này đều sẽ implement interface này

public interface IConversion
{
    string Id {get; }//ID của conversation
    string Name {get; }

    //Lấy danh sách những user tham gia conversation
    List<User> GetParticipants();

    // Lấy lịch sử tin nhắn của conversation
    List<Message> GetMessageHistory();

    // Thêm một user vào conversation
    void AddParticipant(User user);

    // Gửi một message vào conversation
    void SendMessage(Message message);

    // Đánh dấu user đã đọc tới message này
    void MarkAsRead(User user, Message message);

    // Tìm các message có chứa keyword
    List<Message> SearchMessages(string keyword);

}