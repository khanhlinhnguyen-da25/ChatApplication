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
- Per-user, per-message read receipts
- Online participant notifications
- Case-insensitive message search
- `IConversation` interface
- Console-based functional tests in `Program.cs`

## Project Structure

ChatApplication/
├── Program.cs
├── User.cs
├── Message.cs
├── TextMessage.cs
├── SystemMessage.cs
├── IConversation.cs
├── DirectConversation.cs
├── GroupConversation.cs
├── ChatApplication.csproj
└── README.md
## Week 2 Implementation

In Week 2, the project was extended to simulate communication between users.

The main tasks included:
1. Sending and receiving messages between two users.
2. Storing and displaying message history.
3. Simulating group conversations.
4. Handling invalid messages and unauthorized senders.
5. Testing read receipts, message search, and notifications.

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
- Searching across all conversations belonging to a user has not yet been demonstrated.