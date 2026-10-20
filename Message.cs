using System;
//Message là abstract class vì nó chỉ đại diện/ là cái khung là khái niệm chung "tìn nhắn:
//Trong chương trình, ta sẽ tạo các loại tin nhắn cụ thể
//TextMessage
//SystemMessage

public abstract class Message
{
    public string Id {get; }// Id riêng của tin nhắn
    public User Sender {get; }//người gửi message
    public DateTime Timestamp {get; }//thời điểm message được tạo

    //CONSTRUCTOR của Message
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
        Timestamp = DateTime.UtcNow;
    }
        //Mỗi loại Message hiển thị nội dung khác nhau
        //Vì Message không biết loại message cụ thể sẽ hiển thị như thế nào nên method này là abstract
        //các class con BẮT BUỘC phải override
        public abstract string GetContent();
 
    
}
