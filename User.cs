using System;
//class User đại diện cho một người dùng trong ChatApplication
public class User
{
    //id : dùng để phần biệt các user với nhau
    //get: cho phép người dùng đọc id,  
    //không có set vì: sau khi tạo User thì không cho thay đổi id, mỗi người chỉ có 1 id duy nhất
    //không nên tùy tiện đổi sau khi object đã được tạo
    public string Id{get; }
    public string Name{get;set;}//tên người dùng -> cho phép đọc và thay đổi tên
    public bool IsOnline{get;set;}
    //cho biết user hiện tại đang onl hay off
    //true -> online ; fale ->offline

    //CONSTRUCTOR : được gọi khi tạo một User mới
    public User(string id , string name)
    {
        Id=id;
        Name=name;
        IsOnline=false;//Khi mới tạo User, mặc định cho user offline
    }
    //constructor có nhiệm vụ đảm bảo khi tạo User
    //thì object đã có Id, Name, và có trạng thái online

    //chuyển trạng thái user thành online
    public void goOnline ()
    {
        IsOnline=true;
    }
    //chuyển trạng thái user thành offline
    public void goOffline()
    {
        IsOnline=false;
    }

    //METHOD
    //method này sẽ được gọi khi user nhận thông báo (notification)
    public void ReceiveNotification(string notification)
    {
        //Khi message được gửi ->notify cho những người đang online
        if(IsOnline)
        {
            Console.WriteLine($"Notification for {Name}: {notification}");
        }
    }

}