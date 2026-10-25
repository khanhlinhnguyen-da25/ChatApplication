using System.Collections.Generic;

// Hợp đồng chung cho mọi loại conversation.
// Code phía client, ví dụ Program và ConversationSearchService, chỉ cần biết
// interface này thay vì phụ thuộc vào DirectConversation hay GroupConversation.
// Đây là abstraction giúp thể hiện polymorphism và dễ thêm conversation subtype.

public interface IConversation
{
    // Identity và tên hiển thị không được sửa sau khi khởi tạo.
    string Id{get; }// ID duy nhất của conversation.
    string Name{get; }// Tên hiển thị của conversation.

    // Chỉ cho phép đọc collection. Việc thêm participant phải đi qua
    // AddParticipant để validation của conversation luôn được thực thi.
    IReadOnlyList<User> Participants{get; }

    // History theo thứ tự message được gửi. Caller không được tự thêm message;
    // phải dùng SendMessage để kiểm tra sender và message ID.
    IReadOnlyList<Message> Messages{get; }

    // Hiển thị participant phục vụ simulation console.
    void PrintParticipants();

    // Hiển thị toàn bộ message history, bao gồm TextMessage và SystemMessage.
    void PrintMessageHistory();
    
    // Thêm participant và khởi tạo read state cho participant đó.
    void AddParticipant(User user);

    // Validate, lưu message và notify các participant online.
    void SendMessage(Message message);

    // Di chuyển last-read pointer của user tới message được chỉ định.
    void MarkAsRead(User user, Message message);

    // Kiểm tra message có nằm trước hoặc tại last-read pointer hay không.
    bool HasUserReadMessage(User user, Message message);

    // Search trong một conversation; search nhiều conversation do
    // ConversationSearchService đảm nhiệm.
    List<Message> SearchMessages(string keyword);

}