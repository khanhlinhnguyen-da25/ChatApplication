using System;
// User đại diện cho một người dùng trong ChatApplication.
public class User
{
    // ID không có set để không thể thay đổi sau khi user được tạo.
    public string Id { get; }
    public string Name { get; private set; }
    // Chỉ các method của User được phép thay đổi trạng thái online.
    public bool IsOnline { get; private set; }

    // Constructor đảm bảo user luôn có ID, tên và trạng thái ban đầu hợp lệ.
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
    // Chuyển trạng thái user thành online.
    public void GoOnline()
    {
        IsOnline = true;
    }
    // Chuyển trạng thái user thành offline.
    public void GoOffline()
    {
        IsOnline = false;
    }

    // Hiển thị notification nếu user đang online.
    public void ReceiveNotification(string notification)
    {
        if (string.IsNullOrWhiteSpace(notification))
        {
            throw new ArgumentException("Notification cannot be empty.", nameof(notification));
        }

        if(IsOnline)
        {
            Console.WriteLine($"Notification for {Name}: {notification}");
        }
    }

}