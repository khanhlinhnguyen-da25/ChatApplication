# Chat Application (Client-Server Simulation)

Course: Object-Oriented Programming (OOP) · Semester 1, 2026-2027 · Instructor: Huỳnh Xuân Phụng

**Team Members:**
- **Nguyễn Trần Khánh Linh** – Student ID: 25110036
- **Nguyễn Lê Khánh Duy** – Student ID: 25110011

> **Important:** message delivery and "real-time" notifications are **simulated
> inside a single process**. The project uses no sockets and no real network.
> How to extend the design to a real client-server system is described in
> [section 8](#8-extending-to-a-real-client-server-architecture).

## 1. Problem Restatement

Build the core object model of a chat application: users, one-to-one and group
conversations, and message history. Requirements:

1. Model `User`, `Conversation` (Direct and Group share one interface) and `Message`.
2. Simulate message delivery: append to the conversation history and notify online participants.
3. Support read receipts: one "last read message" pointer per user per conversation.
4. Support at least one message type beyond plain text (here: `SystemMessage`).
5. Search for messages containing a keyword across all of a user's conversations.

OOP principles to demonstrate: **Polymorphism**, **Composition**,
**Encapsulation** (the project also uses Inheritance and Abstraction).

## 2. Implemented Features

| Requirement | Status | Where |
|---|---|---|
| User, Conversation, Message | Done | `User`, `IConversation`, `ConversationBase`, `Message` |
| Direct and Group behind a shared interface | Done | `DirectConversation`, `GroupConversation` |
| Send message, store history, notify online users | Done | `ConversationBase.SendMessage`, `User.ReceiveNotification` |
| Per-user, per-conversation read receipts | Done | `ConversationBase.MarkAsRead`, `HasUserReadMessage` |
| Non-text message type | Done (`SystemMessage`) | `SystemMessage`, `SystemUser` |
| Search within one conversation | Done | `ConversationBase.SearchMessages` |
| Search across all of a user's conversations | Done | `ConversationSearchService.SearchMessages` |
| Add participant | Done | `ConversationBase.AddParticipant` |
| Leave group / remove participant | Done | `ConversationBase.RemoveParticipant` |
| Input and operation validation | Done | See section 6 |

## 3. Project Structure

```text
ChatApplication/
├── Program.cs                     # Demo scenario and console tests
├── User.cs                        # User, online/offline state, receives notifications
├── SystemUser.cs                  # Singleton system identity that signs SystemMessage
├── Message.cs                     # Abstract base for all messages
├── TextMessage.cs                 # Plain text message
├── SystemMessage.cs               # System event message
├── IConversation.cs               # Shared interface of all conversations
├── ConversationBase.cs            # Shared conversation logic
├── DirectConversation.cs          # One-to-one chat (exactly 2 people, nobody may leave)
├── GroupConversation.cs           # Group chat (at least 2 people)
├── ConversationSearchService.cs   # Search across multiple conversations
├── ChatApplication.csproj
├── output.txt                     # Console test evidence (30 PASS, 0 FAIL)
└── README.md
```

## 4. UML Class Diagram

GitHub renders the `mermaid` block below as a diagram when you view this README
on the repository page.

```mermaid
classDiagram
    direction TB

    class IConversation {
        <<interface>>
        +string Id
        +string Name
        +IReadOnlyList~User~ Participants
        +IReadOnlyList~Message~ Messages
        +AddParticipant(User)
        +RemoveParticipant(User)
        +SendMessage(Message)
        +MarkAsRead(User, Message)
        +HasUserReadMessage(User, Message) bool
        +SearchMessages(string) List~Message~
        +PrintParticipants()
        +PrintMessageHistory()
    }

    class ConversationBase {
        <<abstract>>
        -List~User~ participants
        -List~Message~ messages
        -Dictionary~User, Message~ lastReadMessage
        #ConversationTypeName string*
        #ValidateCanAddParticipant()
        #ValidateCanRemoveParticipant()
    }

    class DirectConversation {
        #ValidateCanAddParticipant()
        #ValidateCanRemoveParticipant()
    }

    class GroupConversation {
        #ValidateCanRemoveParticipant()
    }

    class User {
        +string Id
        +string Name
        +bool IsOnline
        +GoOnline()
        +GoOffline()
        +ReceiveNotification(string)
    }

    class SystemUser {
        +SystemUser Instance$
    }

    class Message {
        <<abstract>>
        +string Id
        +User Sender
        +DateTime Timestamp
        +string Text
        +GetContent() string*
    }

    class TextMessage {
        +GetContent() string
    }

    class SystemMessage {
        +GetContent() string
    }

    class ConversationSearchService {
        +SearchMessages(User, string) List~Message~
    }

    IConversation <|.. ConversationBase : implements
    ConversationBase <|-- DirectConversation
    ConversationBase <|-- GroupConversation
    Message <|-- TextMessage
    Message <|-- SystemMessage
    User <|-- SystemUser

    ConversationBase "1" *-- "0..*" Message : history
    ConversationBase "1" o-- "2..*" User : participants
    Message "*" --> "1" User : sender
    ConversationSearchService "1" o-- "*" IConversation : searches
    ConversationSearchService ..> User : filters by
```

Notation: `<|--` inheritance, `<|..` interface implementation, `*--`
composition, `o--` aggregation, `-->` association, `..>` dependency.

## 5. Self-Explanation

### 5.1 Per requirement

1. **User, Conversation, Message.** `User` holds identity and online state
   (changed only through `GoOnline()` / `GoOffline()`). `IConversation` is the
   shared contract. `Message` is an abstract class carrying the common data
   (Id, Sender, Timestamp, Text).
2. **Delivery and notification.** `ConversationBase.SendMessage` validates the
   message (not null, unique Id, sender is a participant), appends it to the
   history, then calls `ReceiveNotification` on the other participants.
   `User.ReceiveNotification` only shows the notification when the user is online.
3. **Read receipts.** `ConversationBase` keeps `lastReadMessage` (one pointer per
   user). `MarkAsRead` only moves the pointer forward, so reading the third
   message means the first and second are read too. `HasUserReadMessage`
   compares a message's position with the pointer.
4. **New message type.** `SystemMessage` represents events such as "X joined the
   group". Only `SystemUser.Instance` can sign it, so a normal user cannot forge
   a system event.
5. **Search.** `ConversationBase.SearchMessages` searches one conversation
   (case-insensitive). `ConversationSearchService` filters the conversations the
   user takes part in and merges the results.

### 5.2 Per OOP principle

**Polymorphism**
- Where: `IConversation` with `DirectConversation` / `GroupConversation`; the
  hooks `ValidateCanAddParticipant()` and `ValidateCanRemoveParticipant()`; and
  `Message.GetContent()` with `TextMessage` / `SystemMessage`.
- Why it fits: `AddParticipant` and `RemoveParticipant` are the same operation
  with different rules. A direct chat rejects a third person and does not allow
  leaving; a group accepts new people and only forbids leaving when two members
  remain. Using overridden hooks means `ConversationBase` needs no
  `if (this is DirectConversation)` checks, and a new conversation type can be
  added without editing existing code. `ConversationSearchService` only knows
  `IConversation`, so it works with every conversation type.

**Composition**
- Where: `ConversationBase` owns its list of `Message` objects (the history) and
  its list of participant `User` objects.
- Why it fits: a conversation's messages have no meaning without that
  conversation, so the history belongs to it (composition). A `User` exists
  independently and can be in many conversations, so the relationship to
  participants is aggregation. `ConversationSearchService` is built from a list
  of `IConversation` instead of inheriting from anything.

**Encapsulation**
- Where: `lastReadMessage`, `participants` and `messages` are `private` in
  `ConversationBase`; the outside only sees `IReadOnlyList`. `User.IsOnline`
  has a `private set`.
- Why it fits: if outside code could edit the lists or the read pointer, it
  could add a message without the sender check, or move the pointer backwards,
  breaking the rule that a read receipt cannot be undone. Every change must go
  through `SendMessage`, `AddParticipant`, `RemoveParticipant` or `MarkAsRead`,
  so the validation always runs.

**Inheritance and Abstraction (additional)**
- `ConversationBase` collects the shared logic (history, notifications, read
  receipts, search); the subclasses only declare their own rules.
- `Message` is an abstract class with the abstract method `GetContent()`.

### 5.3 Extensibility

- New conversation type (e.g. a broadcast channel): inherit `ConversationBase`
  and override the hooks; no existing class changes.
- New message type (e.g. `ImageMessage`): inherit `Message` and override
  `GetContent()`. Not implemented in this project.
- Acknowledged weakness: `ConversationBase` has quite a few responsibilities
  (history, notifications, read receipts, console printing). A possible
  improvement is extracting a `ReadReceiptTracker` class that
  `ConversationBase` contains.

## 6. Testing and Evidence

Tests are console-based checks implemented in `Program.cs`, not a separate unit-test framework. Each test reports `[PASS]` or `[FAIL]`. The final run produced **30 PASS and 0 FAIL**; the complete console output is saved in [`output.txt`](output.txt).

### 6.1 Functional, OOP, and Validation Tests (21)

| Test | What is checked | Category |
|---|---|---|
| 0 | `TextMessage` and `SystemMessage` inherit from `Message` | Inheritance |
| 0A | User state and read-only conversation collections | Encapsulation |
| 1 | Reject empty message content | Validation |
| 2 | Reject a sender who is not a participant | Validation |
| 3 | Store and display message history in order | Functional |
| 4 | Mark messages as read and check read receipts | Functional |
| 5 | Search messages within a conversation | Functional |
| 6 | Search across a user's conversations | Functional |
| 6A | Exclude conversations the user does not participate in | Validation |
| 6B | Case-insensitive keyword search | Functional |
| 6C | Reject invalid user or search keyword | Validation |
| 7 | Group conversation behavior | Functional |
| 8 | Online/offline notification behavior | Functional |
| 9 | Message polymorphism | OOP |
| 9A | `IConversation` polymorphism | OOP |
| 9B | Leave rules for Direct and Group conversations | OOP / Validation |
| 10 | System message behavior | Functional |
| 11 | Reject a system message with an invalid sender | Validation |
| 12 | Last-read pointer does not move backwards | Validation |
| 13 | Reject whitespace-only message content | Validation |
| 14 | Reject duplicate participants | Validation |

### 6.2 Integration Tests (3)

| Test | Workflow | What is checked |
|---|---|---|
| Integration Test 1 | Direct Chat Workflow | Send message, history, notification, read receipt, search |
| Integration Test 2 | Group Chat Workflow | Participants, text messages, system messages, message history |
| Integration Test 3 | Cross-Conversation Workflow | Search across conversations and exclude unrelated private messages |

### 6.3 Final Verification Tests (6)

| Test | Scenario | Expected behavior |
|---|---|---|
| Final Test 1 | Group Member Leaves | Member removed; old history kept; new messages from the former member rejected |
| Final Test 2 | Duplicate Message Protection | Duplicate message ID rejected; history unchanged |
| Final Test 3 | Read Receipt Consistency | Marking an older message as read does not move the pointer backwards |
| Final Test 4 | Group Member Leave Rule | Leaving succeeds; history remains; former member cannot send new messages |
| Final Test 5 | Direct Conversation Leave Rule | Leaving is rejected; both participants remain |
| Final Test 6 | Minimum Group Participants | A group cannot be reduced below two participants |

Final Tests 1 and 4 intentionally overlap to recheck the group-leave workflow in the final version.

### 6.4 Run and Save Test Evidence

```powershell
dotnet run > output.txt
(Select-String -Path output.txt -Pattern "\[PASS\]").Count
(Select-String -Path output.txt -Pattern "\[FAIL\]").Count
```

**Verified final result:** `30 PASS`, `0 FAIL`. The file `output.txt` contains the actual console output, including the demo scenario and test results.

## 7. Build and Run

Requirement: .NET SDK 10 (`net10.0`).

```powershell
dotnet build
dotnet run
```

To save the output as evidence, run `dotnet run > output.txt`.

The program prints the demo scenario (users, Direct and Group conversations,
history, search, read receipts) and then runs the tests in order.

## 8. Extending to a Real Client-Server Architecture

This section is a description only; it is not implemented.

- **Separate the transport layer:** replace the direct call
  `participant.ReceiveNotification` with an interface such as
  `IMessageTransport` / `INotifier`. The current version is in-process; a real
  version sends over the network.
- **Server:** an ASP.NET Core application (SignalR or WebSocket) that hosts the
  `IConversation` objects. Each connected client is a session bound to a `User`;
  after `SendMessage` succeeds, the server pushes the message to the connections
  of online users.
- **Client:** sends commands (send message, mark as read, leave group) to the
  server and receives pushed events.
- **Protocol:** serialize messages as JSON DTOs; each message carries a `type`
  field so the correct subclass is chosen when decoding.
- **Persistence:** replace the in-memory lists with a repository (for example a
  database) so history survives a restart.
- **Authentication and concurrency:** add login so the server knows who the
  `Sender` is, and use a lock or a per-conversation queue so concurrent messages
  keep a consistent order.
- **Offline messages:** store undelivered notifications and replay them when the
  user comes online.

## 9. Current Limitations

- Data is stored in memory only.
- No real networking; everything runs in one process.
- No automated unit-test project yet (console tests are used instead).
- No `ImageMessage`; the only non-text message type is `SystemMessage`.
- No user registry, so non-existent user IDs are not checked.
- The "left the group" event in the main demo scenario is still a simulated
  system message; the real leave operation is `RemoveParticipant` (see test 9B).
  The system does not automatically generate a `SystemMessage` when someone
  joins or leaves.
- Some test data uses Vietnamese text without diacritics; it has not been
  standardized.

