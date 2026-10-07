//TextMessage kế thừa từ Message
// Đây là loại message chứa nội dung văn bản bình thường
public class TextMessage : Message
{
    public string Text {get; }//nội dung của tin nhắn

    //CONSTRUCTOR
    public TextMessage(string id, User sender, string text) : base(id, sender)
    {
        // base(id, sender)
        // gọi constructor của class cha Message
        // để Message xử lý Id, Sender và Timestamp
        Text=text;
    }

    //Message yêu cầu class con phải có GetContent()
    // nên TextMessage phải override method này
    public override string GetContent()
    {
        return Text;
    }


}