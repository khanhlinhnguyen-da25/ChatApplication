// Entry point và executable specification của project.
// Main tạo một in-process scenario hoàn chỉnh, sau đó chạy các test console
// cho validation, inheritance, encapsulation, polymorphism, search, delivery
// notification và read receipt. Đây không phải unit-test framework; kết quả
// được thể hiện bằng [PASS]/[FAIL] để dễ demo trong buổi bảo vệ.
public class Program
{
    public static void Main(string[] args)
    {
        // Tạo domain data dùng chung cho các scenario bên dưới.
        // Các object được giữ trong một process để mô phỏng client/server
        // communication mà không cần networking thật.
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

        // Notification chỉ được in ra nếu user đang online. Đây là phần mô
        // phỏng delivery layer, không phải message được lưu trong history.
        Console.WriteLine("\nNotification test: ");
        user1.ReceiveNotification("You have a new message.");
        user2.ReceiveNotification("You have a new message.");
        user3.ReceiveNotification("You have a new message.");

        
        // TEST 0: Kiểm tra tính kế thừa.
        // TextMessage và SystemMessage đều kế thừa từ Message.
        Console.WriteLine("\n---TEST 0: INHERITANCE---");

        Message inheritedTextMessage =
            new TextMessage("M000", user1, "Inherited text message.");

        Message inheritedSystemMessage =
            new SystemMessage("SM000", SystemUser.Instance, "Inherited system event.");

        Console.WriteLine("Input: One TextMessage and one SystemMessage.");
        Console.WriteLine("Action: Check their base types, properties, and GetContent().");
        Console.WriteLine("Expected: Both message types inherit from Message.");

        // Kiểm tra tính kế thừa và các thuộc tính chung
        bool inheritanceWorks =
            inheritedTextMessage is Message &&
            inheritedTextMessage is TextMessage &&
            inheritedSystemMessage is Message &&
            inheritedSystemMessage is SystemMessage &&
            inheritedTextMessage.Id == "M000" &&
            inheritedTextMessage.Sender == user1 &&
            inheritedTextMessage.Timestamp != default &&
            inheritedSystemMessage.GetContent() == "[SYSTEM] Inherited system event.";

        // Hiển thị kết quả thực tế
        Console.WriteLine("\nActual:");
        Console.WriteLine("TextMessage base type: " +
            inheritedTextMessage.GetType().BaseType?.Name);
        Console.WriteLine("SystemMessage base type: " +
            inheritedSystemMessage.GetType().BaseType?.Name);

        if (inheritanceWorks)
        {
            Console.WriteLine("[PASS] TextMessage and SystemMessage inherit from Message.");
        }
        else
        {
            Console.WriteLine("[FAIL] Message inheritance validation failed.");
        }


        
        // TEST 0A: Kiểm tra tính đóng gói.
        // User thay đổi trạng thái qua method.
        // Conversation không cho sửa trực tiếp collection.
        Console.WriteLine("\n---TEST 0A: ENCAPSULATION---");

        Console.WriteLine("Input: A User and a DirectConversation.");
        Console.WriteLine("Action: Change online status and attempt direct collection modification.");
        Console.WriteLine("Expected: User state changes through methods; collection modification is rejected.");

        User encapsulatedUser = new User("U000", "Encapsulated User");

        bool initialOffline = !encapsulatedUser.IsOnline;

        encapsulatedUser.GoOnline();
        bool onlineAfterMethod = encapsulatedUser.IsOnline;

        encapsulatedUser.GoOffline();
        bool offlineAfterMethod = !encapsulatedUser.IsOnline;

        // Tạo conversation và tin nhắn dùng cho các test tiếp theo
        Console.WriteLine("\nDirect Conversation test");

        IConversation directConversation = new DirectConversation(
            "DC001",
            "Direct Chat",
            new List<User> { user1, user2 });

        directConversation.PrintParticipants();

        Message textMessage1 = new TextMessage(
            "M001", user1, "Duy nop bao cao OOP tuan 2 chua?");

        Message textMessage2 = new TextMessage(
            "M002", user2, "Chua, tui con dang lam, bao cao tuan 2 hoi kho voi tui.");

        directConversation.SendMessage(textMessage1);
        directConversation.SendMessage(textMessage2);

        // Kiểm tra collection chỉ cho phép đọc từ bên ngoài
        bool readOnlyCollections =
            directConversation.Participants is IReadOnlyList<User> &&
            directConversation.Messages is IReadOnlyList<Message>;

        bool collectionMutationRejected = false;

        try
        {
            ((IList<Message>)directConversation.Messages).Add(
                new TextMessage("M009", user1, "This mutation must be rejected."));
        }
        catch (NotSupportedException)
        {
            collectionMutationRejected = true;
        }

        // Hiển thị kết quả thực tế
        Console.WriteLine("\nActual:");
        Console.WriteLine("Initial offline: " + initialOffline);
        Console.WriteLine("Online after GoOnline(): " + onlineAfterMethod);
        Console.WriteLine("Offline after GoOffline(): " + offlineAfterMethod);
        Console.WriteLine("Read-only collections: " + readOnlyCollections);
        Console.WriteLine("Direct modification rejected: " + collectionMutationRejected);
        Console.WriteLine("Total messages: " + directConversation.Messages.Count);

        if (initialOffline &&
            onlineAfterMethod &&
            offlineAfterMethod &&
            readOnlyCollections &&
            collectionMutationRejected &&
            directConversation.Messages.Count == 2)
        {
            Console.WriteLine("[PASS] User state and conversation collections are encapsulated.");
        }
        else
        {
            Console.WriteLine("[FAIL] Encapsulation validation failed.");
        }


        
        // TEST 1: Kiểm tra tin nhắn rỗng.
        Console.WriteLine("\n---TEST 1: EMPTY MESSAGE---");

        Console.WriteLine("Sender: Linh");
        Console.WriteLine("Input message: \"\"");
        Console.WriteLine("Action: Create a TextMessage with empty content.");
        Console.WriteLine("Expected: Reject empty message.");

        try
        {
            // Thử tạo tin nhắn rỗng
            Message emptyMessage = new TextMessage("M100", user1, "");

            Console.WriteLine("\nActual: Empty message was accepted.");
            Console.WriteLine("[FAIL] Empty message validation failed.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("\nActual: Empty message was rejected.");
            Console.WriteLine("Reason: " + ex.Message);
            Console.WriteLine("[PASS] Empty message was rejected correctly.");
        }

        
        // TEST 2: Kiểm tra người gửi không thuộc conversation.
        Console.WriteLine("\n---TEST 2: INVALID USER---");

        Console.WriteLine("Conversation: Direct Chat");
        Console.WriteLine("Participants: Linh, Duy");
        Console.WriteLine("Invalid sender: Huy");
        Console.WriteLine("Input message: Hello Linh and Duy!");
        Console.WriteLine("Action: Huy attempts to send a message.");
        Console.WriteLine("Expected: Reject the message and keep history unchanged.");

        // Lưu số lượng tin nhắn trước khi kiểm thử
        int messageCountBefore = directConversation.Messages.Count;

        try
        {
            Message invalidMessage = new TextMessage(
                "M101", user3, "Hello Linh and Duy!");

            directConversation.SendMessage(invalidMessage);

            Console.WriteLine("\nActual: Message was accepted.");
            Console.WriteLine("[FAIL] Invalid sender was allowed.");
        }
        catch (InvalidOperationException ex)
        {
            // Kiểm tra history không bị thay đổi
            int messageCountAfter = directConversation.Messages.Count;

            Console.WriteLine("\nActual: Message was rejected.");
            Console.WriteLine("Reason: " + ex.Message);
            Console.WriteLine("Message count before: " + messageCountBefore);
            Console.WriteLine("Message count after: " + messageCountAfter);

            if (messageCountAfter == messageCountBefore)
            {
                Console.WriteLine("[PASS] Invalid sender was rejected and history remained unchanged.");
            }
            else
            {
                Console.WriteLine("[FAIL] Message history changed after rejection.");
            }
        }


        
        // TEST 3: Kiểm tra lịch sử tin nhắn.
        Console.WriteLine("\n---TEST 3: MESSAGE HISTORY---");

        Console.WriteLine("Conversation: Direct Chat");
        Console.WriteLine("Input: Two messages from Linh and Duy.");
        Console.WriteLine("Action: Linh sends a message, then Duy replies.");
        Console.WriteLine("Expected: 2 messages stored with correct senders, content, and order.");

        bool correctCount = directConversation.Messages.Count == 2;
        bool correctOrder = false;
        bool correctContent = false;

        if (correctCount)
        {
            // Kiểm tra thứ tự người gửi
            correctOrder =
                directConversation.Messages[0].Sender == user1 &&
                directConversation.Messages[1].Sender == user2;

            // Kiểm tra nội dung tin nhắn
            correctContent =
                directConversation.Messages[0].GetContent() == textMessage1.GetContent() &&
                directConversation.Messages[1].GetContent() == textMessage2.GetContent();
        }

        // In kết quả thực tế
        Console.WriteLine("\nActual:");
        Console.WriteLine("Total messages: " + directConversation.Messages.Count);

        foreach (Message message in directConversation.Messages)
        {
            Console.WriteLine("- " + message.Sender.Name + ": " + message.GetContent());
        }

        if (correctCount && correctOrder && correctContent)
        {
            Console.WriteLine("\n[PASS] Messages were stored with correct count, senders, content, and order.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Message history is incorrect.");
        }

        
        
        // TEST 4: Kiểm tra trạng thái đọc tin nhắn.
        Console.WriteLine("\n---TEST 4: READ RECEIPT---");

        Console.WriteLine("Conversation: Direct Chat");
        Console.WriteLine("User: " + user2.Name);
        Console.WriteLine("Message: " + textMessage1.GetContent());
        Console.WriteLine("Action: Duy marks Linh's message as read.");
        Console.WriteLine("Expected: Read status changes from False to True.");

        // Kiểm tra trước khi đọc
        bool beforeRead =
            directConversation.HasUserReadMessage(user2, textMessage1);

        // Đánh dấu đã đọc
        directConversation.MarkAsRead(user2, textMessage1);

        // Kiểm tra sau khi đọc
        bool afterRead =
            directConversation.HasUserReadMessage(user2, textMessage1);

        Console.WriteLine("\nActual:");
        Console.WriteLine("Before MarkAsRead(): " + beforeRead);
        Console.WriteLine("After MarkAsRead(): " + afterRead);

        if (!beforeRead && afterRead)
        {
            Console.WriteLine("\n[PASS] Read status changed from False to True.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Read status was not updated correctly.");
        }



        // TEST 5: Search trong một conversation.
        // Cross-conversation search được kiểm tra riêng ở TEST 6.
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

        // Tạo group bằng IConversation để tiếp tục chứng minh polymorphism.
        // SystemMessage được gửi trước các TextMessage để event hệ thống xuất
        // hiện trong history và notify participant online.
        Console.WriteLine("\nGroup Conversation test");
        IConversation groupConversation = new GroupConversation(
            "GC001",
            "Group Chat",
            new List<User> { user1, user2 });
        SystemMessage groupCreatedMessage = new SystemMessage(
            "SM001",
            SystemUser.Instance,
            "Group Chat was created."
        );
        groupConversation.SendMessage(groupCreatedMessage);
        groupConversation.AddParticipant(user3);
        groupConversation.SendMessage(new SystemMessage(
            "SM002",
            SystemUser.Instance,
            "Huy was added to the group."
        ));
        groupConversation.SendMessage(new SystemMessage(
            "SM003",
            SystemUser.Instance,
            "Huy joined the group."
        ));
        groupConversation.SendMessage(new SystemMessage(
            "SM004",
            SystemUser.Instance,
            "Duy left the group (simulated event)."
        ));
        groupConversation.PrintParticipants();
        Message groupMessage1 = new TextMessage("M003", user1, "Du an Toan Roi Rac bao cao cua minh, thay bao lac de luon roi!");
        Message groupMessage2 = new TextMessage("M004", user2, "Oh no, phai viet ke hoach roi lam lai tu dau ha!");
        Message groupMessage3 = new TextMessage("M005", user3, "Minh cung bi roi, thoi lam lai tu dau, khong sao ca!");
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

        
        
       // TEST 6: Tìm kiếm tin nhắn trên nhiều conversation.
Console.WriteLine("\n---TEST 6: CROSS-CONVERSATION SEARCH---");

string crossKeyword = "bao cao";

Console.WriteLine("User: " + user1.Name);
Console.WriteLine("Input keyword: " + crossKeyword);
Console.WriteLine("Action: Search across all conversations Linh participates in.");
Console.WriteLine("Expected: Find 3 messages containing the keyword.");

// Tạo service tìm kiếm trên hai conversation
ConversationSearchService searchService =
    new ConversationSearchService(new IConversation[]
    {
        directConversation,
        groupConversation
    });

// Thực hiện tìm kiếm
List<Message> crossSearchResults =
    searchService.SearchMessages(user1, crossKeyword);

// In kết quả thực tế
Console.WriteLine("\nActual: Found " + crossSearchResults.Count + " messages:");

foreach (Message message in crossSearchResults)
{
    Console.WriteLine("- " + message.Sender.Name + ": " + message.GetContent());
}

        // Kiểm tra đủ 3 tin nhắn mong đợi từ hai conversation
        if (crossSearchResults.Count == 3 &&
            crossSearchResults.Contains(textMessage1) &&
            crossSearchResults.Contains(textMessage2) &&
            crossSearchResults.Contains(groupMessage1))
        {
            Console.WriteLine("\n[PASS] Search found messages across both conversations.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Cross-conversation search results are incorrect.");
        }


        
        // TEST 6A: Không trả về message từ conversation user không tham gia.
        Console.WriteLine("\n---TEST 6A: SEARCH PARTICIPATION FILTER---");

        IConversation unrelatedConversation = new DirectConversation(
            "DC002",
            "Duy and Huy Chat",
            new List<User> { user2, user3 });

        Message unrelatedMessage = new TextMessage(
            "M006",
            user2,
            "Bao cao noi bo cua Duy va Huy.");

        unrelatedConversation.SendMessage(unrelatedMessage);

        ConversationSearchService searchWithUnrelatedConversation =
            new ConversationSearchService(new IConversation[]
            {
                directConversation,
                groupConversation,
                unrelatedConversation
            });

        Console.WriteLine("User: " + user1.Name);
        Console.WriteLine("Input keyword: bao cao");
        Console.WriteLine("Additional conversation: Duy and Huy Chat");
        Console.WriteLine("Action: Search including a conversation Linh did not join.");
        Console.WriteLine("Expected: Find 3 messages and exclude Duy and Huy Chat.");

        // Thực hiện tìm kiếm
        List<Message> filteredResults =
            searchWithUnrelatedConversation.SearchMessages(user1, "bao cao");

        Console.WriteLine("\nActual: Found " + filteredResults.Count + " messages:");
        Console.WriteLine("Unrelated message included: " +
            filteredResults.Contains(unrelatedMessage));

        if (filteredResults.Count == 3 &&
            !filteredResults.Contains(unrelatedMessage))
        {
            Console.WriteLine("\n[PASS] Search excluded conversations the user did not join.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Search participation filtering is incorrect.");
        }


        
        // TEST 6B: Tìm kiếm không phân biệt chữ hoa, chữ thường.
        Console.WriteLine("\n---TEST 6B: CROSS-CONVERSATION CASE-INSENSITIVE SEARCH---");

        Console.WriteLine("User: " + user1.Name);
        Console.WriteLine("Input keyword: BAO CAO");
        Console.WriteLine("Action: Search using uppercase letters.");
        Console.WriteLine("Expected: Find the same 3 messages as keyword 'bao cao'.");

        // Thực hiện tìm kiếm bằng chữ hoa
        List<Message> caseInsensitiveResults =
            searchWithUnrelatedConversation.SearchMessages(user1, "BAO CAO");

        // In kết quả thực tế
        Console.WriteLine("\nActual: Found " + caseInsensitiveResults.Count + " messages:");

        foreach (Message message in caseInsensitiveResults)
        {
            Console.WriteLine("- " + message.Sender.Name + ": " + message.GetContent());
        }

        if (caseInsensitiveResults.Count == 3 &&
            caseInsensitiveResults.Contains(textMessage1) &&
            caseInsensitiveResults.Contains(textMessage2) &&
            caseInsensitiveResults.Contains(groupMessage1))
        {
            Console.WriteLine("\n[PASS] Cross-conversation search is case-insensitive.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Cross-conversation case-insensitive search failed.");
        }


        
        // TEST 6C: Kiểm tra validation đầu vào của cross-conversation search.
        Console.WriteLine("\n---TEST 6C: CROSS-CONVERSATION SEARCH VALIDATION---");

        Console.WriteLine("Input 1: Blank keyword (spaces only).");
        Console.WriteLine("Input 2: Null user.");
        Console.WriteLine("Action: Search using invalid inputs.");
        Console.WriteLine("Expected: Reject both inputs with appropriate exceptions.");

        bool blankKeywordRejected = false;
        bool nullUserRejected = false;

        // Kiểm tra từ khóa chỉ chứa khoảng trắng
        try
        {
            searchService.SearchMessages(user1, "   ");
        }
        catch (ArgumentException ex)
        {
            blankKeywordRejected = true;
            Console.WriteLine("\nBlank keyword rejected: " + ex.Message);
        }

        // Kiểm tra user null
        try
        {
            searchService.SearchMessages(null!, "bao cao");
        }
        catch (ArgumentNullException ex)
        {
            nullUserRejected = true;
            Console.WriteLine("Null user rejected: " + ex.Message);
        }

        Console.WriteLine("\nActual:");
        Console.WriteLine("Blank keyword rejected: " + blankKeywordRejected);
        Console.WriteLine("Null user rejected: " + nullUserRejected);

        if (blankKeywordRejected && nullUserRejected)
        {
            Console.WriteLine("\n[PASS] Cross-conversation search validates its inputs.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Cross-conversation search validation failed.");
        }


        
        // TEST 7: Kiểm tra Group Conversation.
        // Kiểm tra thành viên và thứ tự các tin nhắn văn bản.
        Console.WriteLine("\n---TEST 7: GROUP CONVERSATION---");

        Console.WriteLine("Conversation: Group Chat");
        Console.WriteLine("Participants: Linh, Duy, Huy");
        Console.WriteLine("Action: Each participant sends one text message.");
        Console.WriteLine("Expected: 3 participants and 3 text messages stored in order.");

        // Kiểm tra số lượng và thành viên trong nhóm
        bool correctParticipants =
            groupConversation.Participants.Count == 3 &&
            groupConversation.Participants.Contains(user1) &&
            groupConversation.Participants.Contains(user2) &&
            groupConversation.Participants.Contains(user3);

        // Lấy riêng các TextMessage, không tính SystemMessage
        List<Message> groupTextMessages = new List<Message>();

        foreach (Message message in groupConversation.Messages)
        {
            if (message is TextMessage)
            {
                groupTextMessages.Add(message);
            }
        }

        // Kiểm tra số lượng và thứ tự người gửi
        bool correctGroupMessages =
            groupTextMessages.Count == 3 &&
            groupTextMessages[0].Sender == user1 &&
            groupTextMessages[1].Sender == user2 &&
            groupTextMessages[2].Sender == user3;

        // Hiển thị kết quả thực tế
        Console.WriteLine("\nActual:");
        Console.WriteLine("Total participants: " + groupConversation.Participants.Count);

        foreach (User user in groupConversation.Participants)
        {
            Console.WriteLine("- " + user.Name);
        }

        Console.WriteLine("Total text messages: " + groupTextMessages.Count);

        foreach (Message message in groupTextMessages)
        {
            Console.WriteLine("- " + message.Sender.Name + ": " + message.GetContent());
        }

        // Đánh giá kết quả
        if (correctParticipants && correctGroupMessages)
        {
            Console.WriteLine("\n[PASS] Group participants and text messages are correct.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Group conversation validation failed.");
        }

        
       
        // TEST 8: Kiểm tra notification khi user offline.
        Console.WriteLine("\n---TEST 8: OFFLINE NOTIFICATION---");

        Console.WriteLine("User: " + user2.Name);
        Console.WriteLine("Input: Duy goes offline.");
        Console.WriteLine("Action: Send a notification to Duy.");
        Console.WriteLine("Expected: No notification is displayed.");

        // Chuyển Duy sang trạng thái offline
        user2.GoOffline();

        // Lưu Console output ban đầu
        System.IO.TextWriter originalOutput = Console.Out;
        System.IO.StringWriter capturedOutput = new System.IO.StringWriter();

        try
        {
            // Tạm thời lưu nội dung in ra Console
            Console.SetOut(capturedOutput);

            // Gọi đúng phương thức trong User.cs
            user2.ReceiveNotification("You have a new message.");
        }
        finally
        {
            // Khôi phục Console output ban đầu
            Console.SetOut(originalOutput);
        }

        // Kiểm tra notification có được in ra không
        bool noNotification =
            string.IsNullOrWhiteSpace(capturedOutput.ToString());

        Console.WriteLine("\nActual:");
        Console.WriteLine("Online status: " + user2.IsOnline);
        Console.WriteLine("Notification displayed: " + !noNotification);

        if (!user2.IsOnline && noNotification)
        {
            Console.WriteLine("\n[PASS] No notification was displayed for offline user.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Offline notification validation failed.");
        }


        
        // TEST 9: Kiểm tra tính đa hình.
        // Biến kiểu Message tham chiếu đến object TextMessage.
        Console.WriteLine("\n---TEST 9: POLYMORPHISM---");

        Console.WriteLine("Input: A TextMessage with content 'Hello from Linh!'.");
        Console.WriteLine("Action: Store TextMessage in a Message variable and call GetContent().");
        Console.WriteLine("Expected: GetContent() returns 'Hello from Linh!'.");

        // Tạo object TextMessage nhưng khai báo bằng kiểu Message
        Message polymorphicMessage =
            new TextMessage("M200", user1, "Hello from Linh!");

        // Kiểm tra kiểu object và nội dung trả về
        Console.WriteLine("\nActual:");
        Console.WriteLine("Message type: " + polymorphicMessage.GetType().Name);
        Console.WriteLine("Message content: " + polymorphicMessage.GetContent());

        if (polymorphicMessage is TextMessage &&
            polymorphicMessage.GetContent() == "Hello from Linh!")
        {
            Console.WriteLine("\n[PASS] Polymorphism works correctly.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Polymorphism validation failed.");
        }


        
        // TEST 9A: Kiểm tra tính đa hình thông qua IConversation.
        Console.WriteLine("\n---TEST 9A: IConversation POLYMORPHISM---");

        Console.WriteLine("Input: DirectConversation and GroupConversation.");
        Console.WriteLine("Action: Access both conversations through IConversation.");
        Console.WriteLine("Expected: Both conversation types implement IConversation.");

        // Sử dụng interface để tham chiếu đến hai loại conversation
        IConversation directTest = directConversation;
        IConversation groupTest = groupConversation;

        // Kiểm tra kiểu object thực tế
        Console.WriteLine("\nActual:");
        Console.WriteLine("Direct conversation type: " + directTest.GetType().Name);
        Console.WriteLine("Group conversation type: " + groupTest.GetType().Name);

        // Kiểm tra cả hai object đều triển khai IConversation
        if (directTest is DirectConversation &&
            groupTest is GroupConversation)
        {
            Console.WriteLine("\n[PASS] Both conversation types can be accessed through IConversation.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] IConversation polymorphism validation failed.");
        }

        // TEST 9B: RemoveParticipant (rời nhóm) khác nhau theo subtype.
        Console.WriteLine("\n---TEST 9B: LEAVE CONVERSATION---");

        Console.WriteLine("Input: A direct chat and a group chat with three members.");
        Console.WriteLine("Action: Try to remove participants through IConversation.");
        Console.WriteLine("Expected: Direct rejects leaving; Group allows it until two members remain; history is preserved.");

        User leaveUser1 = new User("U004", "Mai");
        User leaveUser2 = new User("U005", "Nam");
        User leaveUser3 = new User("U006", "Lan");
        IConversation directAsInterface = new DirectConversation(
            "DC003",
            "Leave Direct Chat",
            new List<User> { leaveUser1, leaveUser2 });
        IConversation groupAsInterface = new GroupConversation(
            "GC002",
            "Leave Group Chat",
            new List<User> { leaveUser1, leaveUser2, leaveUser3 });

        bool directLeaveRejected = false;
        try
        {
            directAsInterface.RemoveParticipant(leaveUser2);
        }
        catch (InvalidOperationException)
        {
            directLeaveRejected = true;
        }

        Message leaverMessage = new TextMessage("M009", leaveUser3, "I will leave soon.");
        groupAsInterface.SendMessage(leaverMessage);
        groupAsInterface.MarkAsRead(leaveUser3, leaverMessage);
        groupAsInterface.RemoveParticipant(leaveUser3);
        groupAsInterface.SendMessage(new SystemMessage(
            "SM009", SystemUser.Instance, "Lan left the group."));

        bool leaverRejectedAfterLeaving = false;
        try
        {
            groupAsInterface.SendMessage(new TextMessage("M010", leaveUser3, "Still here?"));
        }
        catch (InvalidOperationException)
        {
            leaverRejectedAfterLeaving = true;
        }

        bool groupMinimumRejected = false;
        try
        {
            groupAsInterface.RemoveParticipant(leaveUser2);
        }
        catch (InvalidOperationException)
        {
            groupMinimumRejected = true;
        }

        if (directLeaveRejected &&
            leaverRejectedAfterLeaving &&
            groupMinimumRejected &&
            !groupAsInterface.Participants.Contains(leaveUser3) &&
            groupAsInterface.Messages.Contains(leaverMessage))
        {
            Console.WriteLine("[PASS] Leave rules differ by conversation type and history is preserved.");
        }
        else
        {
            Console.WriteLine("[FAIL] Leave conversation behavior is incorrect.");
        }

        // TEST 10: SystemMessage vừa là Message subtype vừa là domain event.
        Console.WriteLine("\n---TEST 10: SYSTEM MESSAGE---");

        Console.WriteLine("Conversation: Group Chat");
        Console.WriteLine("Input: A system event message.");
        Console.WriteLine("Action: Send a SystemMessage to Group Chat.");
        Console.WriteLine("Expected: SystemMessage is stored with the correct sender and content.");

        // Tạo SystemMessage với sender là System
        Message systemTestMessage = new SystemMessage(
            "SM100",
            SystemUser.Instance,
            "A system event was recorded."
        );

        // Gửi tin nhắn hệ thống vào Group Chat
        groupConversation.SendMessage(systemTestMessage);

        // Kiểm tra tin nhắn có được lưu vào history không
        bool systemMessageStored =
            groupConversation.Messages.Contains(systemTestMessage);

        // Kiểm tra sender và nội dung
        bool correctSystemMessage =
            systemTestMessage is SystemMessage &&
            systemTestMessage.Sender == SystemUser.Instance &&
            systemTestMessage.GetContent() ==
                "[SYSTEM] A system event was recorded.";

        // Hiển thị kết quả thực tế
        Console.WriteLine("\nActual:");
        Console.WriteLine("Sender: " + systemTestMessage.Sender.Name);
        Console.WriteLine("Content: " + systemTestMessage.GetContent());
        Console.WriteLine("Stored in history: " + systemMessageStored);

        if (systemMessageStored && correctSystemMessage)
        {
            Console.WriteLine("\n[PASS] SystemMessage was stored correctly.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] SystemMessage validation failed.");
        }


        // TEST 11: Kiểm tra người gửi không hợp lệ của SystemMessage.
        // Chỉ SystemUser.Instance mới được tạo SystemMessage.
        Console.WriteLine("\n---TEST 11: INVALID SYSTEM MESSAGE SENDER---");

        Console.WriteLine("Input sender: " + user1.Name);
        Console.WriteLine("Input message: Invalid system event.");
        Console.WriteLine("Action: Create a SystemMessage using a regular user.");
        Console.WriteLine("Expected: Reject the message because the sender is not System.");

        try
        {
            // Thử tạo SystemMessage với sender là Linh
            Message invalidSystemMessage = new SystemMessage(
                "SM101",
                user1,
                "Invalid system event."
            );

            Console.WriteLine("\nActual: Invalid system message was accepted.");
            Console.WriteLine("[FAIL] Invalid system message sender was allowed.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("\nActual: Invalid system message was rejected.");
            Console.WriteLine("Reason: " + ex.Message);
            Console.WriteLine("[PASS] SystemMessage sender validation succeeded.");
        }

        
        // TEST 12: Kiểm tra cơ chế last-read pointer.
        // Đánh dấu tin nhắn mới nhất là đã đọc thì các tin trước cũng được đọc.
        Console.WriteLine("\n---TEST 12: LAST-READ POINTER---");

        Console.WriteLine("Conversation: Last Read Test");
        Console.WriteLine("User: " + user2.Name);
        Console.WriteLine("Input: 3 messages sent in order.");
        Console.WriteLine("Action: Duy marks the latest message as read.");
        Console.WriteLine("Expected: All 3 messages are marked as read.");

        // Tạo conversation riêng để kiểm thử
        IConversation readTestConversation = new DirectConversation(
            "DC003",
            "Last Read Test",
            new List<User> { user1, user2 }
        );

        // Tạo 3 tin nhắn theo thứ tự
        Message earlierMessage = new TextMessage(
            "M301", user1, "First message.");

        Message middleMessage = new TextMessage(
            "M302", user1, "Second message.");

        Message latestMessage = new TextMessage(
            "M303", user1, "Third message.");

        // Gửi 3 tin nhắn vào conversation
        readTestConversation.SendMessage(earlierMessage);
        readTestConversation.SendMessage(middleMessage);
        readTestConversation.SendMessage(latestMessage);

        // Kiểm tra trạng thái trước khi đánh dấu đã đọc
        bool initiallyUnread =
            !readTestConversation.HasUserReadMessage(user2, earlierMessage) &&
            !readTestConversation.HasUserReadMessage(user2, middleMessage) &&
            !readTestConversation.HasUserReadMessage(user2, latestMessage);

        // Đánh dấu tin nhắn mới nhất là đã đọc
        readTestConversation.MarkAsRead(user2, latestMessage);

        // Kiểm tra trạng thái sau khi đánh dấu
        bool earlierRead =
            readTestConversation.HasUserReadMessage(user2, earlierMessage);

        bool middleRead =
            readTestConversation.HasUserReadMessage(user2, middleMessage);

        bool latestRead =
            readTestConversation.HasUserReadMessage(user2, latestMessage);

        // In kết quả thực tế
        Console.WriteLine("\nActual:");
        Console.WriteLine("All messages initially unread: " + initiallyUnread);
        Console.WriteLine("Earlier message read: " + earlierRead);
        Console.WriteLine("Middle message read: " + middleRead);
        Console.WriteLine("Latest message read: " + latestRead);

        // PASS khi cả 3 tin nhắn đều được đánh dấu đã đọc
        if (initiallyUnread && earlierRead && middleRead && latestRead)
        {
            Console.WriteLine("\n[PASS] Read receipts use a monotonic last-read pointer.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Last-read pointer validation failed.");
        }
        
        // TEST 13: Kiểm tra tin nhắn chỉ chứa khoảng trắng.
        Console.WriteLine("\n---TEST 13: WHITESPACE MESSAGE VALIDATION---");

        Console.WriteLine("Sender: " + user1.Name);
        Console.WriteLine("Input message: \"   \"");
        Console.WriteLine("Action: Create a TextMessage containing only spaces.");
        Console.WriteLine("Expected: Reject the message with ArgumentException.");

        try
        {
            // Thử tạo tin nhắn chỉ chứa khoảng trắng
            Message whitespaceMessage = new TextMessage(
                "M130", user1, "   ");

            Console.WriteLine("\nActual: Whitespace message was accepted.");
            Console.WriteLine("[FAIL] Whitespace message validation failed.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("\nActual: Whitespace message was rejected.");
            Console.WriteLine("Reason: " + ex.Message);
            Console.WriteLine("[PASS] Whitespace message was rejected correctly.");
        }
        
        
        // TEST 14: Kiểm tra việc thêm thành viên trùng vào Group Chat.
        Console.WriteLine("\n---TEST 14: DUPLICATE PARTICIPANT VALIDATION---");

        Console.WriteLine("Input: Linh is already a participant.");
        Console.WriteLine("Action: Add Linh to Group Chat again.");
        Console.WriteLine("Expected: Reject the duplicate and keep participants unchanged.");

        int participantCountBefore = groupConversation.Participants.Count;
        bool duplicateRejected = false;

        try
        {
            groupConversation.AddParticipant(user1);
        }
        catch (InvalidOperationException ex)
        {
            duplicateRejected = true;
            Console.WriteLine("\nActual: " + ex.Message);
        }

        int participantCountAfter = groupConversation.Participants.Count;

        Console.WriteLine("Participants before: " + participantCountBefore);
        Console.WriteLine("Participants after: " + participantCountAfter);

        if (duplicateRejected &&
            participantCountBefore == participantCountAfter)
        {
            Console.WriteLine("\n[PASS] Duplicate participant was rejected correctly.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Duplicate participant validation failed.");
        }


        // INTEGRATION TEST 1: Kiểm tra luồng Direct Chat hoàn chỉnh.
        Console.WriteLine("\n---INTEGRATION TEST 1: DIRECT CHAT WORKFLOW---");

        Console.WriteLine("Input: Linh sends 'Hello Duy, integration test!' to Duy.");
        Console.WriteLine("Action: Send, notify, read, and search for the message.");
        Console.WriteLine("Expected: Message stored, notification received, read status updated, and search successful.");

        // Tạo user và conversation riêng, không ảnh hưởng test cũ
        User integrationLinh = new User("UI01", "Linh");
        User integrationDuy = new User("UI02", "Duy");

        integrationLinh.GoOnline();
        integrationDuy.GoOnline();

        IConversation integrationConversation = new DirectConversation(
            "DC_INT01",
            "Integration Direct Chat",
            new List<User> { integrationLinh, integrationDuy }
        );

        Message integrationMessage = new TextMessage(
            "M_INT01",
            integrationLinh,
            "Hello Duy, integration test!"
        );

        // Bước 1: Gửi tin nhắn và kiểm tra notification
        System.IO.TextWriter integrationOriginalOutput = Console.Out;
        System.IO.StringWriter integrationCapturedOutput =
            new System.IO.StringWriter();

        try
        {
            Console.SetOut(integrationCapturedOutput);
            integrationConversation.SendMessage(integrationMessage);
        }
        finally
        {
            Console.SetOut(integrationOriginalOutput);
        }

        bool integrationNotification =
            integrationCapturedOutput.ToString().Contains(
                "Notification for Duy: New message from Linh.");

        // Bước 2: Kiểm tra tin nhắn được lưu đúng vào history
        bool integrationHistory =
            integrationConversation.Messages.Count == 1 &&
            integrationConversation.Messages[0] == integrationMessage;

        // Bước 3: Kiểm tra trạng thái đọc trước và sau MarkAsRead()
        bool integrationUnreadBefore =
            !integrationConversation.HasUserReadMessage(
                integrationDuy, integrationMessage);

        integrationConversation.MarkAsRead(
            integrationDuy, integrationMessage);

        bool integrationReadAfter =
            integrationConversation.HasUserReadMessage(
                integrationDuy, integrationMessage);

        // Bước 4: Tìm lại tin nhắn bằng từ khóa
        List<Message> integrationSearchResults =
            integrationConversation.SearchMessages("integration");

        bool integrationSearch =
            integrationSearchResults.Count == 1 &&
            integrationSearchResults.Contains(integrationMessage);

        // Hiển thị kết quả thực tế
        Console.WriteLine("\nActual:");
        Console.WriteLine("Message stored: " + integrationHistory);
        Console.WriteLine("Notification received: " + integrationNotification);
        Console.WriteLine("Initially unread: " + integrationUnreadBefore);
        Console.WriteLine("Read after MarkAsRead(): " + integrationReadAfter);
        Console.WriteLine("Message found by search: " + integrationSearch);

        if (integrationHistory &&
            integrationNotification &&
            integrationUnreadBefore &&
            integrationReadAfter &&
            integrationSearch)
        {
            Console.WriteLine("\n[PASS] Direct chat workflow completed successfully.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Direct chat workflow validation failed.");
        }

        // INTEGRATION TEST 2: Kiểm tra luồng Group Chat hoàn chỉnh.
        Console.WriteLine("\n---INTEGRATION TEST 2: GROUP CHAT WORKFLOW---");

        Console.WriteLine("Input: Linh, Duy and Huy join a group conversation.");
        Console.WriteLine("Action: Create group, add Huy, send system and text messages.");
        Console.WriteLine("Expected: 3 participants and 2 messages stored in order.");

        // Tạo các user riêng để không ảnh hưởng test trước
        User groupLinh = new User("GI01", "Linh");
        User groupDuy = new User("GI02", "Duy");
        User groupHuy = new User("GI03", "Huy");

        // Tạo group ban đầu gồm Linh và Duy
        IConversation integrationGroup = new GroupConversation(
            "GC_INT01",
            "Integration Group Chat",
            new List<User> { groupLinh, groupDuy }
        );

        // Thêm Huy vào nhóm
        integrationGroup.AddParticipant(groupHuy);

        // Tạo thông báo hệ thống
        Message integrationSystemMessage = new SystemMessage(
            "SM_INT01",
            SystemUser.Instance,
            "Huy joined the group."
        );

        integrationGroup.SendMessage(integrationSystemMessage);

        // Linh gửi tin nhắn vào nhóm
        Message integrationGroupMessage = new TextMessage(
            "M_INT02",
            groupLinh,
            "Hello everyone!"
        );

        integrationGroup.SendMessage(integrationGroupMessage);

        // Kiểm tra danh sách thành viên
        bool integrationGroupParticipants =
            integrationGroup.Participants.Count == 3 &&
            integrationGroup.Participants.Contains(groupLinh) &&
            integrationGroup.Participants.Contains(groupDuy) &&
            integrationGroup.Participants.Contains(groupHuy);

        // Kiểm tra số lượng và thứ tự tin nhắn
        bool integrationGroupHistory =
            integrationGroup.Messages.Count == 2 &&
            integrationGroup.Messages[0] == integrationSystemMessage &&
            integrationGroup.Messages[1] == integrationGroupMessage;

        // Kiểm tra nội dung SystemMessage
        bool integrationSystemContent =
            integrationSystemMessage.GetContent() ==
                "[SYSTEM] Huy joined the group.";

        // Kiểm tra người gửi TextMessage
        bool integrationGroupSender =
            integrationGroupMessage.Sender == groupLinh;

        // Hiển thị kết quả thực tế
        Console.WriteLine("\nActual:");
        Console.WriteLine("Participants correct: " + integrationGroupParticipants);
        Console.WriteLine("Message history correct: " + integrationGroupHistory);
        Console.WriteLine("System message correct: " + integrationSystemContent);
        Console.WriteLine("Text message sender correct: " + integrationGroupSender);

        if (integrationGroupParticipants &&
            integrationGroupHistory &&
            integrationSystemContent &&
            integrationGroupSender)
        {
            Console.WriteLine("\n[PASS] Group chat workflow completed successfully.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Group chat workflow validation failed.");
        }


        // INTEGRATION TEST 3: Kiểm tra tìm kiếm trên nhiều conversation.
        Console.WriteLine("\n---INTEGRATION TEST 3: CROSS-CONVERSATION WORKFLOW---");

        Console.WriteLine("Input: 3 conversations containing the keyword 'project report'.");
        Console.WriteLine("Action: Linh searches across all conversations.");
        Console.WriteLine("Expected: Find 2 messages and exclude the private conversation.");

        // Tạo các user riêng cho integration test
        User searchLinh = new User("SI01", "Linh");
        User searchDuy = new User("SI02", "Duy");
        User searchHuy = new User("SI03", "Huy");

        // Conversation 1: Linh và Duy
        IConversation searchDirect = new DirectConversation(
            "DC_INT03",
            "Direct Chat",
            new List<User> { searchLinh, searchDuy }
        );

        // Conversation 2: Linh, Duy và Huy
        IConversation searchGroup = new GroupConversation(
            "GC_INT03",
            "Group Chat",
            new List<User> { searchLinh, searchDuy, searchHuy }
        );

        // Conversation 3: Chỉ có Duy và Huy
        IConversation searchPrivate = new DirectConversation(
            "DC_INT04",
            "Private Chat",
            new List<User> { searchDuy, searchHuy }
        );

        // Tạo 3 tin nhắn có cùng từ khóa
        Message searchMessage1 = new TextMessage(
            "M_INT03",
            searchLinh,
            "Project report in direct chat."
        );

        Message searchMessage2 = new TextMessage(
            "M_INT04",
            searchHuy,
            "Project report in group chat."
        );

        Message searchMessage3 = new TextMessage(
            "M_INT05",
            searchDuy,
            "Project report is confidential."
        );

        // Gửi tin nhắn vào từng conversation
        searchDirect.SendMessage(searchMessage1);
        searchGroup.SendMessage(searchMessage2);
        searchPrivate.SendMessage(searchMessage3);

        // Tạo service tìm kiếm trên cả 3 conversation
        ConversationSearchService integrationSearchService =
            new ConversationSearchService(new IConversation[]
            {
                searchDirect,
                searchGroup,
                searchPrivate
            });

        // Linh tìm kiếm tin nhắn
        List<Message> integrationResults =
            integrationSearchService.SearchMessages(
                searchLinh, "project report");

        // Kiểm tra kết quả tìm kiếm
        bool integrationCorrectCount =
            integrationResults.Count == 2;

        bool integrationCorrectMessages =
            integrationResults.Contains(searchMessage1) &&
            integrationResults.Contains(searchMessage2);

        bool integrationPrivacyFilter =
            !integrationResults.Contains(searchMessage3);

        // Hiển thị kết quả thực tế
        Console.WriteLine("\nActual:");
        Console.WriteLine("Messages found: " + integrationResults.Count);
        Console.WriteLine("Correct messages found: " + integrationCorrectMessages);
        Console.WriteLine("Private message excluded: " + integrationPrivacyFilter);

        if (integrationCorrectCount &&
            integrationCorrectMessages &&
            integrationPrivacyFilter)
        {
            Console.WriteLine("\n[PASS] Cross-conversation workflow completed successfully.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Cross-conversation workflow validation failed.");
        }


    }
}
