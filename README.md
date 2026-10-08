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

## Current Progress - Week 1
Completed:
- Analyzed project requirements
- Identified candidate classes
- Created initial class diagram
- Created C# Console project
- Implemented User class
- Implemented Message abstract class
- Implemented TextMessage
- Implemented SystemMessage
- Created IConversation interface skeleton
- Tested basic Message polymorphism

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
The conversation logic has not been implemented yet.
DirectConversation, GroupConversation, message delivery,
read receipts, search, and automated tests will be implemented in the next development stages.