//test class User
User Linh=new User("U001", "Linh");
User An=new User("U002", "An");

Console.WriteLine("   USERS:   ");
Console.WriteLine($"{Linh.Id} - {Linh.Name} - Online: {Linh.IsOnline}");
Console.WriteLine($"{An.Id} - {An.Name} - Online: {An.IsOnline}");

Linh.goOnline();
Console.WriteLine("\nAFTER LINH GOES ONLINE :");
Console.WriteLine($"{Linh.Name} - Online: {Linh.IsOnline}");

//gửi Notification
Console.WriteLine("\n Notification test ");
Linh.ReceiveNotification("You have a new message.");
An.ReceiveNotification("You have a new message.");
//    USERS:   
// U001 - Linh - Online: False
// U002 - An - Online: False

// AFTER LINH GOES ONLINE :
// Linh - Online: True

//  Notification test 
// Notification for Linh: You have a new message.


Console.WriteLine("\nMESSAGE TEST: ");

// Tạo text message
TextMessage message1 = new TextMessage("M001",Linh,"Hello An!");

// Tạo system message
SystemMessage message2 = new SystemMessage("M002",Linh,"An joined the group.");

// In thông tin message
Console.WriteLine($"{message1.Sender.Name}: {message1.GetContent()}");

Console.WriteLine($"{message2.Sender.Name}: {message2.GetContent()}");

// Linh: Hello An!
// Linh: [SYSTEM] An joined the group.

Console.WriteLine("\nPOLYMORPHISM TEST");

// Biến có kiểu Message
// nhưng object thực tế có thể là các class con khác nhau
Message firstMessage = message1;
Message secondMessage = message2;


// Cùng gọi GetContent()
Console.WriteLine(firstMessage.GetContent());
Console.WriteLine(secondMessage.GetContent());
// POLYMORPHISM TEST
// Hello An!
// [SYSTEM]An joined the group.