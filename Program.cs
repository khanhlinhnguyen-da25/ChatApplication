public class Program
{
    public static void Main(string[] args)
    {
        // Tạo dữ liệu mẫu để minh họa các chức năng của ứng dụng.
        User user1 = new User("U001", "Linh");
        User user2 = new User("U002", "Duy");
        User user3 = new User("U003", "Huy");


        Console.WriteLine("USERS:");
        Console.WriteLine($"{user1.Id} - {user1.Name} - Online: {user1.IsOnline}");
        Console.WriteLine($"{user2.Id} - {user2.Name} - Online: {user2.IsOnline}");
        Console.WriteLine($"{user3.Id} - {user3.Name} - Online: {user3.IsOnline}");

        user1.GoOnline();
        user2.GoOnline();
        user3.GoOnline();
        Console.WriteLine("\n---AFTER ALL USERS GOES ONLINE---");
        Console.WriteLine($"{user1.Name} - Online: {user1.IsOnline}");
        Console.WriteLine($"{user2.Name} - Online: {user2.IsOnline}");
        Console.WriteLine($"{user3.Name} - Online: {user3.IsOnline}");

        // Notification chỉ được in ra nếu user đang online.
        Console.WriteLine("\nNotification test: ");
        user1.ReceiveNotification("You have a new message.");
        user2.ReceiveNotification("You have a new message.");
        user3.ReceiveNotification("You have a new message.");

        // Minh họa conversation riêng: chỉ có đúng hai participant.
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

        // TEST 1: Kiểm tra tin nhắn rỗng
        Console.WriteLine("\n---TEST 1: EMPTY MESSAGE---");

        Console.WriteLine("Sender: Linh");
        Console.WriteLine("Input message: \"\"");
        Console.WriteLine("Expected: Reject empty message.");

        try
        {
            // Thử tạo tin nhắn rỗng
            TextMessage emptyMessage = new TextMessage("M100", user1, "");

            Console.WriteLine("\nActual: Empty message was accepted.");
            Console.WriteLine("[FAIL] Empty message validation failed.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("\nActual: Empty message was rejected.");
            Console.WriteLine("Reason: " + ex.Message);
            Console.WriteLine("[PASS] Empty message was rejected correctly.");
        }

        // TEST 2: Kiểm tra người gửi không thuộc cuộc trò chuyện
        Console.WriteLine("\n---TEST 2: INVALID USER---");

        Console.WriteLine("Conversation: Direct Chat");
        Console.WriteLine("Participants: Linh, Duy");
        Console.WriteLine("Invalid sender: Huy");
        Console.WriteLine("Input message: Hello Linh and Duy!");
        Console.WriteLine("Expected: Reject the message because Huy is not a participant.");

        try
        {
            // Huy không thuộc cuộc trò chuyện
            TextMessage invalidMessage = new TextMessage(
                "M101", user3, "Hello Linh and Duy!"
            );

            directConversation.SendMessage(invalidMessage);

            Console.WriteLine("\nActual: Message was accepted.");
            Console.WriteLine("[FAIL] Invalid sender was allowed.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("\nActual: Message was rejected.");
            Console.WriteLine("Reason: " + ex.Message);
            Console.WriteLine("[PASS] Invalid sender was rejected correctly.");
        }

        // TEST 3: Kiểm tra lịch sử tin nhắn
        Console.WriteLine("\n---TEST 3: MESSAGE HISTORY---");

        Console.WriteLine("Conversation: Direct Chat");
        Console.WriteLine("Action: Linh sends a message, then Duy replies.");
        Console.WriteLine("Expected: 2 messages stored in sending order.");

        Console.WriteLine("\nActual:");
        Console.WriteLine("Total messages: " + directConversation.Messages.Count);

        // In các tin nhắn thực tế đã lưu
        foreach (Message message in directConversation.Messages)
        {
            Console.WriteLine("- " + message.Sender.Name + ": " + message.GetContent());
        }

        // Kiểm tra số lượng và thứ tự
        if (directConversation.Messages.Count == 2 &&
            directConversation.Messages[0] == textMessage1 &&
            directConversation.Messages[1] == textMessage2)
        {
            Console.WriteLine("\n[PASS] Messages were stored in the correct order.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Message history is incorrect.");
        }

        // TEST 4: Kiểm tra trạng thái đọc tin nhắn
        Console.WriteLine("\n---TEST 4: READ RECEIPT---");

        Console.WriteLine("Conversation: Direct Chat");
        Console.WriteLine("User: " + user2.Name);
        Console.WriteLine("Message: " + textMessage1.GetContent());
        Console.WriteLine("Expected: Read status changes from False to True.");

        // Trạng thái trước khi đọc
        bool beforeRead = directConversation.HasUserReadMessage(user2, textMessage1);

        Console.WriteLine("\nBefore MarkAsRead(): " + beforeRead);

        // Đánh dấu đã đọc
        directConversation.MarkAsRead(user2, textMessage1);

        // Trạng thái sau khi đọc
        bool afterRead = directConversation.HasUserReadMessage(user2, textMessage1);

        Console.WriteLine("After MarkAsRead(): " + afterRead);

        // Kiểm tra kết quả
        if (beforeRead == false && afterRead == true)
        {
            Console.WriteLine("\n[PASS] Read status changed from False to True.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Read status was not updated correctly.");
        }

       // TEST 5: Kiểm tra chức năng tìm kiếm tin nhắn
        Console.WriteLine("\n---TEST 5: SEARCH MESSAGE---");

        string keyword = "bao cao";

        Console.WriteLine("Conversation: Direct Chat");
        Console.WriteLine("Input keyword: " + keyword);
        Console.WriteLine("Expected: Find 2 messages containing the keyword.");

        // Thực hiện tìm kiếm
        List<Message> testSearchResults =
            directConversation.SearchMessages(keyword);

        // In kết quả thực tế
        Console.WriteLine("\nActual: Found " + testSearchResults.Count + " messages:");

        foreach (Message message in testSearchResults)
        {
            Console.WriteLine("- " + message.Sender.Name + ": " + message.GetContent());
        }

        // So sánh với kết quả mong đợi
        if (testSearchResults.Count == 2 &&
            testSearchResults.Contains(textMessage1) &&
            testSearchResults.Contains(textMessage2))
        {
            Console.WriteLine("\n[PASS] Both expected messages were found.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Search results are incorrect.");
        }

        // Minh họa conversation nhóm với nhiều participant.
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
        // TEST 6: Kiểm tra cuộc trò chuyện nhóm
        Console.WriteLine("\n---TEST 6: GROUP CONVERSATION---");

        Console.WriteLine("Conversation: Group Chat");
        Console.WriteLine("Participants: Linh, Duy, Huy");
        Console.WriteLine("Action: Each participant sends one message.");
        Console.WriteLine("Expected: 3 participants and 3 messages stored in order.");

        // In danh sách thành viên thực tế
        Console.WriteLine("\nActual participants:");
        foreach (User user in groupConversation.Participants)
        {
            Console.WriteLine("- " + user.Name);
        }

        // In lịch sử tin nhắn thực tế
        Console.WriteLine("\nActual messages:");
        foreach (Message message in groupConversation.Messages)
        {
            Console.WriteLine("- " + message.Sender.Name + ": " + message.GetContent());
        }

        // Kiểm tra thành viên và thứ tự tin nhắn
        if (groupConversation.Participants.Count == 3 &&
            groupConversation.Participants.Contains(user1) &&
            groupConversation.Participants.Contains(user2) &&
            groupConversation.Participants.Contains(user3) &&
            groupConversation.Messages.Count == 3 &&
            groupConversation.Messages[0] == groupMessage1 &&
            groupConversation.Messages[1] == groupMessage2 &&
            groupConversation.Messages[2] == groupMessage3)
        {
            Console.WriteLine("\n[PASS] Group participants and messages are correct.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Group participants or messages are incorrect.");
        }
        // TEST 7: Kiểm tra thông báo khi user offline
        Console.WriteLine("\n---TEST 7: OFFLINE NOTIFICATION---");

        Console.WriteLine("User: Duy");
        Console.WriteLine("Input: Duy goes offline.");
        Console.WriteLine("Action: Send notification to Duy.");
        Console.WriteLine("Expected: No notification is displayed.");

        // Chuyển Duy sang offline
        user2.GoOffline();

        // Lưu Console hiện tại
        System.IO.TextWriter originalOutput = Console.Out;

        // Tạo nơi lưu nội dung Console tạm thời
        System.IO.StringWriter output = new System.IO.StringWriter();

        try
        {
            // Chuyển đầu ra Console vào StringWriter
            Console.SetOut(output);

            // Thử gửi thông báo cho Duy
            user2.ReceiveNotification("You have a new message.");
        }
        finally
        {
            // Khôi phục Console, kể cả khi xảy ra lỗi
            Console.SetOut(originalOutput);
        }

        // Lấy nội dung notification thực tế
        string actualNotification = output.ToString();

        Console.WriteLine("\nActual online status: " + user2.IsOnline);
        Console.WriteLine("Actual notification: " +
            (actualNotification.Length == 0 ? "(none)" : actualNotification));

        // Kiểm tra kết quả
        if (user2.IsOnline == false && actualNotification.Length == 0)
        {
            Console.WriteLine("[PASS] No notification was displayed for offline user.");
        }
        else
        {
            Console.WriteLine("[FAIL] Offline notification test failed.");
        }
        // Cho Duy online trở lại
        user2.GoOnline();


        // TEST 8: Kiểm tra tính đa hình
        Console.WriteLine("\n---TEST 8: POLYMORPHISM---");

        Console.WriteLine("Action: Store a TextMessage object in a Message variable.");
        Console.WriteLine("Input: Hello from Linh!");
        Console.WriteLine("Expected: GetContent() returns the text message content.");

        // Tạo đối tượng TextMessage nhưng khai báo bằng kiểu Message
        Message polymorphicMessage =
            new TextMessage("M102", user1, "Hello from Linh!");

        // Gọi phương thức thông qua biến kiểu Message
        string actualContent = polymorphicMessage.GetContent();

        Console.WriteLine("\nActual message type: " +
            polymorphicMessage.GetType().Name);

        Console.WriteLine("Actual content: " + actualContent);

        // So sánh kết quả
        if (actualContent == "Hello from Linh!" &&
            polymorphicMessage is TextMessage)
        {
            Console.WriteLine("[PASS] Polymorphism works correctly.");
        }
        else
        {
            Console.WriteLine("[FAIL] Polymorphism test failed.");
        }
    }
}
