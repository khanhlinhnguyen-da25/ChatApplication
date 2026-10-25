using System;

// Domain entity đại diện cho người dùng và trạng thái online của họ.
// User không tự quản lý conversation/message; các trách nhiệm đó thuộc
// ConversationBase để mỗi class có một responsibility rõ ràng.
public class User
{
    // ID là identity bất biến sau khi object được tạo.
    public string Id { get; }
    // Name có thể được quản lý nội bộ trong tương lai nhưng không cho caller
    // tùy ý gán trực tiếp.
    public string Name { get; private set; }
    // private set là phần encapsulation: trạng thái chỉ đổi qua GoOnline/
    // GoOffline thay vì bị sửa trực tiếp từ code bên ngoài.
    public bool IsOnline { get; private set; }

    // Constructor bảo đảm mọi User hợp lệ ngay từ lúc khởi tạo.
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
    // Public behavior để user chủ động chuyển sang online.
    public void GoOnline()
    {
        IsOnline = true;
    }
    // Public behavior để user chủ động chuyển sang offline.
    public void GoOffline()
    {
        IsOnline = false;
    }

    // Notification đang được mô phỏng bằng console. Offline user vẫn nhận
    // được lời gọi nhưng không in ra, thể hiện behavior delivery cơ bản.
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