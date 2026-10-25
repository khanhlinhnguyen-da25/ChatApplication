# Chat Application (Client-Server Simulation)

## Project Description

This project is a simple Chat Application simulation developed using C# and Object-Oriented Programming.

The application supports:
- Users
- Direct conversations
- Group conversations
- Message history
- Online user notifications
- Read receipts
- Different message types
- Message search

The application simulates message delivery in a single process. Real networking is not implemented.

## OOP Concepts

The project is designed to demonstrate:
- Encapsulation
- Inheritance
- Polymorphism
- Abstraction
- Composition

## Current Progress

Implemented:
- User management and online status
- Direct and group conversations
- Participant validation
- Message history and message validation
- Text and system messages
- Per-user monotonic last-read message pointers
- Online participant notifications
- Case-insensitive message search
- `IConversation` interface
- Shared `ConversationBase` implementation for common conversation behavior
- Dedicated `SystemUser` identity for system-generated messages
- Cross-conversation message search for a user
- Per-user last-read message pointers
- Explicit inheritance, encapsulation, and polymorphism tests
- Console-based functional tests in `Program.cs`

## Project Structure

ChatApplication/
├── Program.cs
├── User.cs
├── Message.cs
├── TextMessage.cs
├── SystemMessage.cs
├── SystemUser.cs
├── IConversation.cs
├── ConversationBase.cs
├── DirectConversation.cs
├── GroupConversation.cs
├── ChatApplication.csproj
└── README.md

`ConversationBase` contains the behavior shared by both conversation types:
participant management, message history, notifications, read receipts, and
message search. `DirectConversation` adds the two-participant rule, while
`GroupConversation` uses the default participant rule. Both remain usable
through `IConversation` to demonstrate polymorphism.

The application code consumes conversations through the `IConversation` type
wherever it only needs shared behavior. For example, `Program.Main` creates
concrete `DirectConversation` and `GroupConversation` objects on the right-hand
side but stores them in `IConversation` variables. The `: IConversation`
declarations remain on the concrete classes because they define which classes
implement the contract; the `Program` declarations demonstrate substitutability
and runtime polymorphism.

`SystemMessage` uses the dedicated `SystemUser.Instance` identity. The system
identity is not a conversation participant; `ConversationBase.SendMessage`
allows it only for a validated `SystemMessage`, while ordinary messages still
require a participant sender. System messages are stored in history and notify
online participants. The simulation remains in-process and does not implement
real networking.

The sample program demonstrates group-created, user-added, user-joined, and
user-left system events. The user-left event is currently a simulated history
event because the model does not yet expose a participant-removal operation;
it does not silently remove a participant.

The console test suite makes the OOP evidence explicit:

- `TEST 0` verifies that `TextMessage` and `SystemMessage` inherit shared
  `Message` state while overriding `GetContent()`.
- `TEST 0A` verifies that `User` state changes only through `GoOnline()` and
  `GoOffline()`, and that conversation collections reject external mutation.
- `TEST 9A` stores both direct and group conversations in `IConversation`
  variables and verifies their different participant rules through the same
  interface.

`ConversationSearchService` searches every conversation containing the requested
user and combines the matching messages. Read receipts are stored as one
last-read message pointer per participant; marking a later message as read also
marks all earlier messages as read, and attempting to move the pointer backward
does not undo an existing receipt.
## Week 2 Implementation

In Week 2, the project was extended to simulate communication between users.

The main tasks included:
1. Sending and receiving messages between two users.
2. Storing and displaying message history.
3. Simulating group conversations.
4. Handling invalid messages and unauthorized senders.
5. Testing read receipts, message search, and notifications.
## Week 3 – Testing and Validation

### Overview
During Week 3, we reviewed and extended the console-based tests to verify the Chat Application's functionality, OOP principles, and error handling.

### Features Tested
- Direct and group conversations
- Message sending and message history
- Online/offline notifications
- Read receipts and last-read pointer
- TextMessage and SystemMessage
- Message searching across conversations
- Input validation and exception handling
- Inheritance, encapsulation, and polymorphism

### Test Cases
The application includes 21 console-based test cases, covering:
- Functional testing
- Negative testing and edge cases
- OOP validation
- Integration testing

### Integration Test – Direct Chat Workflow
The integration test verifies the following workflow:

1. Linh sends a message to Duy.
2. The message is stored in conversation history.
3. Duy receives a notification while online.
4. Duy marks the message as read.
5. The message can be found using keyword search.

### Testing Results
All implemented test cases were executed using the console application.

The integration test passed all five checks:
- Message stored: True
- Notification received: True
- Initially unread: True
- Read after MarkAsRead(): True
- Message found by search: True

Result: PASS
## How to Run

1. Open the project folder in VS Code.
2. Open the terminal using `Ctrl + \``.
3. Run the following command: dotnet run

The program will display:
- User information and online status
- Direct conversation simulation
- Group conversation simulation
- Message history and search results
- Read receipts
- Test results

## Current Limitations
- Data is stored in memory only.
- There is no real client-server networking.
- Automated unit test projects have not been added yet.
- The current tests do not verify nonexistent user IDs through a user registry.