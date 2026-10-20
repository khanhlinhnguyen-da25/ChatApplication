# Chat Application (Client-Server Simulation)
## Project Description

This project is a simple Chat Application simulation developed using C# and Object-Oriented Programming.
The application will support:
- Users
- Direct conversations
- Group conversations
- Message history
- Online user notifications
- Read receipts
- Different message types
- Message search

The application simulates message delivery in a single process.
Real networking is not implemented.

## OOP Concepts
The project is designed to demonstrate:
- Encapsulation
- Inheritance
- Polymorphism
- Composition
- Observer Pattern

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

## Project Structure
ChatApplication/
- Program.cs
- User.cs
- Message.cs
- TextMessage.cs
- SystemMessage.cs
- IConversation.cs
- ChatApplication.csproj

## How to Run
Open a terminal in the project folder and run: dotnet run

## Current Limitations
- Data is stored in memory only.
- There is no real client-server networking.
- Automated test projects have not been added yet.