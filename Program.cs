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

        // TEST 0: INHERITANCE
        // Chứng minh TextMessage/SystemMessage kế thừa state chung của Message
        // nhưng vẫn có implementation GetContent riêng.
        Console.WriteLine("\n---TEST 0: INHERITANCE---");

        Message inheritedTextMessage =
            new TextMessage("M000", user1, "Inherited text message.");
        Message inheritedSystemMessage =
            new SystemMessage("SM000", SystemUser.Instance, "Inherited system event.");

        bool inheritanceWorks =
            inheritedTextMessage is Message &&
            inheritedTextMessage is TextMessage &&
            inheritedSystemMessage is Message &&
            inheritedSystemMessage is SystemMessage &&
            inheritedTextMessage.Id == "M000" &&
            inheritedTextMessage.Sender == user1 &&
            inheritedTextMessage.Timestamp != default &&
            inheritedSystemMessage.GetContent() == "[SYSTEM] Inherited system event.";

        Console.WriteLine("TextMessage base type: " +
            inheritedTextMessage.GetType().BaseType?.Name);
        Console.WriteLine("SystemMessage base type: " +
            inheritedSystemMessage.GetType().BaseType?.Name);

        if (inheritanceWorks)
        {
            Console.WriteLine("[PASS] TextMessage and SystemMessage inherit shared Message state and behavior.");
        }
        else
        {
            Console.WriteLine("[FAIL] Message inheritance behavior is incorrect.");
        }

        // TEST 0A: ENCAPSULATION
        // Chứng minh state User và conversation collections không bị sửa trực
        // tiếp từ bên ngoài; thay đổi phải đi qua public method hợp lệ.
        Console.WriteLine("\n---TEST 0A: ENCAPSULATION---");
        User encapsulatedUser = new User("U000", "Encapsulated User");
        bool initialOffline = !encapsulatedUser.IsOnline;
        encapsulatedUser.GoOnline();
        bool onlineAfterMethod = encapsulatedUser.IsOnline;
        encapsulatedUser.GoOffline();
        bool offlineAfterMethod = !encapsulatedUser.IsOnline;

        Console.WriteLine("Online state changed through GoOnline/GoOffline: " +
            (initialOffline && onlineAfterMethod && offlineAfterMethod));

        // Dùng biến kiểu IConversation thay vì kiểu concrete để chương trình
        // phụ thuộc abstraction. Runtime object thực tế vẫn là DirectConversation.
        Console.WriteLine("\nDirect Conversation test");
        IConversation directConversation = new DirectConversation(
            "DC001",
            "Direct Chat",
            new List<User> { user1, user2 });
        directConversation.PrintParticipants();
        Message textMessage1 = new TextMessage("M001", user1, "Duy nop bao cao OOP tuan 2 chua?");
        Message textMessage2 = new TextMessage("M002", user2, "Chua, tui con dang lam, bao cao tuan 2 hoi kho voi tui.");
        directConversation.SendMessage(textMessage1);
        directConversation.SendMessage(textMessage2);

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

        // TEST 1: Kiểm tra tin nhắn rỗng.
        // Đây là validation ở Message constructor, trước khi message tới
        // conversation.
        Console.WriteLine("\n---TEST 1: EMPTY MESSAGE---");

        Console.WriteLine("Sender: Linh");
        Console.WriteLine("Input message: \"\"");
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
        // Conversation phải từ chối message unauthorized và không append history.
        Console.WriteLine("\n---TEST 2: INVALID USER---");

        Console.WriteLine("Conversation: Direct Chat");
        Console.WriteLine("Participants: Linh, Duy");
        Console.WriteLine("Invalid sender: Huy");
        Console.WriteLine("Input message: Hello Linh and Duy!");
        Console.WriteLine("Expected: Reject the message because Huy is not a participant.");

        try
        {
            // Huy không thuộc cuộc trò chuyện
            Message invalidMessage = new TextMessage(
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

        // TEST 3: Kiểm tra composition/message history.
        // Conversation sở hữu collection Message và phải giữ đúng thứ tự gửi.
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

        // TEST 4: Kiểm tra read receipt cơ bản.
        // MarkAsRead thay đổi trạng thái nội bộ, caller chỉ quan sát qua query.
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

        // TEST 6: Search toàn bộ conversation của một user.
        // Service phải tự lọc conversation theo participant trước khi gộp kết quả.
        Console.WriteLine("\n---TEST 6: CROSS-CONVERSATION SEARCH---");
        ConversationSearchService searchService =
            new ConversationSearchService(new IConversation[]
            {
                directConversation,
                groupConversation
            });
        List<Message> crossConversationResults =
            searchService.SearchMessages(user1, "bao cao");

        Console.WriteLine("User: " + user1.Name);
        Console.WriteLine("Keyword: bao cao");
        Console.WriteLine("Actual matches: " + crossConversationResults.Count);
        foreach (Message message in crossConversationResults)
        {
            Console.WriteLine("- " + message.GetContent());
        }

        if (crossConversationResults.Count == 3 &&
            crossConversationResults.Contains(textMessage1) &&
            crossConversationResults.Contains(textMessage2) &&
            crossConversationResults.Contains(groupMessage1))
        {
            Console.WriteLine("[PASS] Search found messages across all user conversations.");
        }
        else
        {
            Console.WriteLine("[FAIL] Cross-conversation search returned incorrect results.");
        }

        // TEST 6A: Không trả về message từ conversation user không tham gia
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
        List<Message> filteredResults =
            searchWithUnrelatedConversation.SearchMessages(user1, "bao cao");

        Console.WriteLine("Additional conversation does not include user: " + user1.Name);
        Console.WriteLine("Actual matches after adding unrelated conversation: " + filteredResults.Count);

        if (filteredResults.Count == 3 &&
            !filteredResults.Contains(unrelatedMessage))
        {
            Console.WriteLine("[PASS] Search excluded conversations the user did not join.");
        }
        else
        {
            Console.WriteLine("[FAIL] Search participation filtering is incorrect.");
        }

        // TEST 6B: Tìm kiếm không phân biệt hoa thường trên nhiều conversation
        Console.WriteLine("\n---TEST 6B: CROSS-CONVERSATION CASE-INSENSITIVE SEARCH---");
        List<Message> caseInsensitiveResults =
            searchWithUnrelatedConversation.SearchMessages(user1, "BAO CAO");

        if (caseInsensitiveResults.Count == 3 &&
            caseInsensitiveResults.Contains(textMessage1) &&
            caseInsensitiveResults.Contains(textMessage2) &&
            caseInsensitiveResults.Contains(groupMessage1))
        {
            Console.WriteLine("[PASS] Cross-conversation search is case-insensitive.");
        }
        else
        {
            Console.WriteLine("[FAIL] Cross-conversation case-insensitive search failed.");
        }

        // TEST 6C: Validation đầu vào của cross-conversation search
        Console.WriteLine("\n---TEST 6C: CROSS-CONVERSATION SEARCH VALIDATION---");
        bool blankKeywordRejected = false;
        bool nullUserRejected = false;
        try
        {
            searchService.SearchMessages(user1, "   ");
        }
        catch (ArgumentException ex)
        {
            blankKeywordRejected = true;
            Console.WriteLine("Blank keyword rejected: " + ex.Message);
        }

        try
        {
            searchService.SearchMessages(null!, "bao cao");
        }
        catch (ArgumentNullException ex)
        {
            nullUserRejected = true;
            Console.WriteLine("Null user rejected: " + ex.Message);
        }

        if (blankKeywordRejected && nullUserRejected)
        {
            Console.WriteLine("[PASS] Cross-conversation search validates its inputs.");
        }
        else
        {
            Console.WriteLine("[FAIL] Cross-conversation search validation failed.");
        }

        // TEST 7: Kiểm tra behavior riêng của GroupConversation và composition.
        Console.WriteLine("\n---TEST 7: GROUP CONVERSATION---");

        Console.WriteLine("Conversation: Group Chat");
        Console.WriteLine("Participants: Linh, Duy, Huy");
        Console.WriteLine("Action: Each participant sends one message.");
        Console.WriteLine("Expected: 3 participants and 3 text messages stored in order.");

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
        List<Message> groupTextMessages = groupConversation.Messages
            .Where(message => message is TextMessage)
            .ToList();

        if (groupConversation.Participants.Count == 3 &&
            groupConversation.Participants.Contains(user1) &&
            groupConversation.Participants.Contains(user2) &&
            groupConversation.Participants.Contains(user3) &&
            groupTextMessages.Count == 3 &&
            groupTextMessages[0] == groupMessage1 &&
            groupTextMessages[1] == groupMessage2 &&
            groupTextMessages[2] == groupMessage3)
        {
            Console.WriteLine("\n[PASS] Group participants and messages are correct.");
        }
        else
        {
            Console.WriteLine("\n[FAIL] Group participants or messages are incorrect.");
        }
        // TEST 8: Offline delivery.
        // User offline không hiển thị notification nhưng flow gửi message vẫn
        // không bị exception.
        Console.WriteLine("\n---TEST 8: OFFLINE NOTIFICATION---");

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


        // TEST 9: Polymorphism giữa Message và các lớp con.
        Console.WriteLine("\n---TEST 9: POLYMORPHISM---");

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

        // TEST 9A: Polymorphism qua IConversation.
        // Cùng một contract nhưng Direct/Group có participant rule khác nhau.
        Console.WriteLine("\n---TEST 9A: IConversation POLYMORPHISM---");

        User polymorphicUser1 = new User("U004", "Mai");
        User polymorphicUser2 = new User("U005", "Nam");
        User polymorphicUser3 = new User("U006", "Lan");
        IConversation directAsInterface = new DirectConversation(
            "DC003",
            "Interface Direct Chat",
            new List<User> { polymorphicUser1, polymorphicUser2 });
        IConversation groupAsInterface = new GroupConversation(
            "GC002",
            "Interface Group Chat",
            new List<User> { polymorphicUser1, polymorphicUser2 });

        directAsInterface.SendMessage(new TextMessage(
            "M007",
            polymorphicUser1,
            "Direct message through IConversation."));
        groupAsInterface.SendMessage(new TextMessage(
            "M008",
            polymorphicUser1,
            "Group message through IConversation."));

        bool directParticipantRuleRejected = false;
        try
        {
            directAsInterface.AddParticipant(polymorphicUser3);
        }
        catch (InvalidOperationException)
        {
            directParticipantRuleRejected = true;
        }

        groupAsInterface.AddParticipant(polymorphicUser3);

        bool interfacePolymorphismWorks =
            directAsInterface is DirectConversation &&
            groupAsInterface is GroupConversation &&
            directAsInterface.Messages.Count == 1 &&
            groupAsInterface.Messages.Count == 1 &&
            directParticipantRuleRejected &&
            groupAsInterface.Participants.Contains(polymorphicUser3) &&
            directAsInterface.Participants is IReadOnlyList<User> &&
            directAsInterface.Messages is IReadOnlyList<Message>;

        if (interfacePolymorphismWorks)
        {
            Console.WriteLine("[PASS] Direct and Group conversations behave polymorphically through IConversation.");
        }
        else
        {
            Console.WriteLine("[FAIL] IConversation polymorphism behavior is incorrect.");
        }

        // TEST 9B: RemoveParticipant (rời nhóm) khác nhau theo subtype.
        Console.WriteLine("\n---TEST 9B: LEAVE CONVERSATION---");
        bool directLeaveRejected = false;
        try
        {
            directAsInterface.RemoveParticipant(polymorphicUser2);
        }
        catch (InvalidOperationException)
        {
            directLeaveRejected = true;
        }

        Message leaverMessage = new TextMessage("M009", polymorphicUser3, "I will leave soon.");
        groupAsInterface.SendMessage(leaverMessage);
        groupAsInterface.MarkAsRead(polymorphicUser3, leaverMessage);
        groupAsInterface.RemoveParticipant(polymorphicUser3);
        groupAsInterface.SendMessage(new SystemMessage(
            "SM009", SystemUser.Instance, "Lan left the group."));

        bool leaverRejectedAfterLeaving = false;
        try
        {
            groupAsInterface.SendMessage(new TextMessage("M010", polymorphicUser3, "Still here?"));
        }
        catch (InvalidOperationException)
        {
            leaverRejectedAfterLeaving = true;
        }

        bool groupMinimumRejected = false;
        try
        {
            groupAsInterface.RemoveParticipant(polymorphicUser2);
        }
        catch (InvalidOperationException)
        {
            groupMinimumRejected = true;
        }

        if (directLeaveRejected &&
            leaverRejectedAfterLeaving &&
            groupMinimumRejected &&
            !groupAsInterface.Participants.Contains(polymorphicUser3) &&
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

        Message polymorphicSystemMessage = new SystemMessage(
            "SM005",
            SystemUser.Instance,
            "A system event was recorded."
        );
        groupConversation.SendMessage(polymorphicSystemMessage);

        bool systemMessageStored =
            groupConversation.Messages.Contains(polymorphicSystemMessage) &&
            polymorphicSystemMessage.GetContent() == "[SYSTEM] A system event was recorded.";

        Console.WriteLine("System message sender: " + polymorphicSystemMessage.Sender.Name);
        Console.WriteLine("System message content: " + polymorphicSystemMessage.GetContent());

        if (systemMessageStored && !groupConversation.Participants.Contains(SystemUser.Instance))
        {
            Console.WriteLine("[PASS] SystemMessage was stored through Message polymorphism.");
        }
        else
        {
            Console.WriteLine("[FAIL] SystemMessage handling is incorrect.");
        }

        Console.WriteLine("\n---TEST 11: INVALID SYSTEM MESSAGE SENDER---");

        try
        {
            SystemMessage invalidSystemMessage = new SystemMessage(
                "SM006",
                user1,
                "This message must not use a normal user sender."
            );

            Console.WriteLine("Actual: Invalid system message was accepted.");
            Console.WriteLine("[FAIL] SystemMessage sender validation failed.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Actual: Invalid system message was rejected.");
            Console.WriteLine("Reason: " + ex.Message);
            Console.WriteLine("[PASS] SystemMessage sender validation succeeded.");
        }

        // TEST 12: Last-read pointer.
        // Đọc message mới hơn phải bao phủ message cũ; đọc lùi không được
        // reset trạng thái đã đọc.
        Console.WriteLine("\n---TEST 12: LAST-READ POINTER---");

        groupConversation.MarkAsRead(user3, groupMessage3);
        groupConversation.MarkAsRead(user3, groupMessage1);

        bool earlierMessageRead =
            groupConversation.HasUserReadMessage(user3, groupMessage1);
        bool middleMessageRead =
            groupConversation.HasUserReadMessage(user3, groupMessage2);
        bool latestMessageRead =
            groupConversation.HasUserReadMessage(user3, groupMessage3);

        Console.WriteLine($"Earlier message read: {earlierMessageRead}");
        Console.WriteLine($"Middle message read: {middleMessageRead}");
        Console.WriteLine($"Latest message read: {latestMessageRead}");

        if (earlierMessageRead && middleMessageRead && latestMessageRead)
        {
            Console.WriteLine("[PASS] Read receipts use a monotonic last-read pointer.");
        }
        else
        {
            Console.WriteLine("[FAIL] Last-read pointer behavior is incorrect.");
        }
    }
}
