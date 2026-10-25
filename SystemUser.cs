// Identity nội bộ đại diện cho hệ thống khi tạo SystemMessage.
// SystemUser không phải participant thật của conversation.
public sealed class SystemUser : User
{
    // Constructor private bảo đảm bên ngoài không tạo thêm system identity.
    private SystemUser() : base("SYSTEM", "System")
    {
        GoOnline();
    }

    // Singleton identity được dùng thống nhất để ConversationBase có thể
    // kiểm tra chính xác message có thật sự do hệ thống tạo hay không.
    public static SystemUser Instance { get; } = new SystemUser();
}
