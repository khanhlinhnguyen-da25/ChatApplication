public class Program
{
    public static void Main(string[] args)
    {
        // Test User class
        //test class User
        User user1 = new User("U001", "Linh");
        User user2 = new User("U002", "Duy");
        User user3 = new User("U003", "Huy");


        Console.WriteLine("   USERS:   ");
        Console.WriteLine($"{user1.Id} - {user1.Name} - Online: {user1.IsOnline}");
        Console.WriteLine($"{user2.Id} - {user2.Name} - Online: {user2.IsOnline}");
        Console.WriteLine($"{user3.Id} - {user3.Name} - Online: {user3.IsOnline}");

        user1.GoOnline();
        user2.GoOnline();
        user3.GoOnline();
        Console.WriteLine("\nAFTER ALL USERS GOES ONLINE:");
        Console.WriteLine($"{user1.Name} - Online: {user1.IsOnline}");
        Console.WriteLine($"{user2.Name} - Online: {user2.IsOnline}");
        Console.WriteLine($"{user3.Name} - Online: {user3.IsOnline}");

        //gửi Notification
        Console.WriteLine("\nNotification test: ");
        user1.ReceiveNotification("You have a new message.");
        user2.ReceiveNotification("You have a new message.");
        user3.ReceiveNotification("You have a new message.");

        //Direct Conversation test
        Console.WriteLine("\nDirect Conversation test");
        DirectConversation directConversation = new DirectConversation("DC001", "Direct Chat", new List<User> { user1, user2 });
        directConversation.PrintParticipants();
        TextMessage textMessage1 = new TextMessage("M001", user1, "Duy nop bao cao OOP tuan 2 chua?");
        TextMessage textMessage2 = new TextMessage("M002", user2, "Chua, tui con dang lam, bao cao tuan 2 hoi kho voi tui.");
        directConversation.SendMessage(textMessage1);
        directConversation.SendMessage(textMessage2);
        Console.WriteLine($"Message sent by {textMessage1.Sender.Name}: {textMessage1.GetContent()}");
        Console.WriteLine($"Message sent by {textMessage2.Sender.Name}: {textMessage2.GetContent()}");
        directConversation.PrintMessageHistory();
        Console.WriteLine($"Has {user2.Name} read the message? {directConversation.HasUserReadMessage(user2, textMessage1)}");
        Console.WriteLine($"Has {user1.Name} read the message? {directConversation.HasUserReadMessage(user1, textMessage2)}");
        Console.WriteLine("Searching for 'bao cao':");
        List<Message> searchResults = directConversation.SearchMessages("bao cao");
        foreach (var message in searchResults)
        {
            Console.WriteLine($"Found message: {message.GetContent()}");
        }

        //Group Conversation test
        Console.WriteLine("\nGroup Conversation test");
        GroupConversation groupConversation = new GroupConversation("GC001", "Group Chat", new List<User> { user1, user2, user3 });
        groupConversation.PrintParticipants();
        TextMessage groupMessage1 = new TextMessage("M003", user1, "Du an Toan Roi Rac cua minh, thay bao lac de luon roi!");
        TextMessage groupMessage2 = new TextMessage("M004", user2, "Oh no, phai viet ke hoach roi lam lai tu dau ha!");
        TextMessage groupMessage3 = new TextMessage("M005", user3, "Minh cung bi roi, thoi lam lai tu dau, khong sao ca!");
        groupConversation.SendMessage(groupMessage1);
        groupConversation.SendMessage(groupMessage2);
        groupConversation.SendMessage(groupMessage3);
        Console.WriteLine($"Message sent by {groupMessage1.Sender.Name}: {groupMessage1.GetContent()}");
        Console.WriteLine($"Message sent by {groupMessage2.Sender.Name}: {groupMessage2.GetContent()}");
        Console.WriteLine($"Message sent by {groupMessage3.Sender.Name}: {groupMessage3.GetContent()}");
        groupConversation.PrintMessageHistory();    
        groupConversation.MarkAsRead(user2, groupMessage1);
        groupConversation.MarkAsRead(user3, groupMessage1);
        groupConversation.MarkAsRead(user1, groupMessage2);
        Console.WriteLine($"Has {user2.Name} read the message \"{groupMessage1.Text}\"? {groupConversation.HasUserReadMessage(user2, groupMessage1)}");
        Console.WriteLine($"Has {user3.Name} read the message \"{groupMessage1.Text}\"? {groupConversation.HasUserReadMessage(user3, groupMessage1)}");
        Console.WriteLine($"Has {user1.Name} read the message \"{groupMessage3.Text}\"? {groupConversation.HasUserReadMessage(user1, groupMessage3)}");
        Console.WriteLine("Searching for 'roi':");
        List<Message> groupSearchResults = groupConversation.SearchMessages("roi");
        foreach (var message in groupSearchResults)
        {
            Console.WriteLine($"Found message: {message.GetContent()}");
        }
    }
}
