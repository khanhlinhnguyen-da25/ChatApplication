using System;
//class User đại diện cho một người dùng trong ChatApplication
public class User
{
    //id : dùng để phần biệt các user với nhau
    //get: cho phép người dùng đọc id,  
    //không có set vì: sau khi tạo User thì không cho thay đổi id, mỗi người chỉ có 1 id duy nhất
    //không nên tùy tiện đổi sau khi object đã được tạo
    public string Id { get; }
    public string Name { get; private set; }
    public bool IsOnline { get; private set; }
    //cho biết user hiện tại đang onl hay off
    //true -> online ; fale ->offline

    //CONSTRUCTOR : được gọi khi tạo một User mới
    public User(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(id));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("User name cannot be empty.", nameof(name));
        }

        Id = id;
        Name = name;
        IsOnline = false;
    }
    //constructor có nhiệm vụ đảm bảo khi tạo User
    //thì object đã có Id, Name, và có trạng thái online

    //chuyển trạng thái user thành online
    public void GoOnline()
    {
        IsOnline = true;
    }
    //chuyển trạng thái user thành offline
    public void GoOffline()
    {
        IsOnline = false;
    }

    //METHOD
    //method này sẽ được gọi khi user nhận thông báo (notification)
    public void ReceiveNotification(string notification)
    {
        if (string.IsNullOrWhiteSpace(notification))
        {
            throw new ArgumentException("Notification cannot be empty.", nameof(notification));
        }

        //Khi message được gửi ->notify cho những người đang online
        if(IsOnline)
        {
            Console.WriteLine($"Notification for {Name}: {notification}");
        }
    }

}