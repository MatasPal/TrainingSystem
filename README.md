# Training System

## Overview
The **Training System** is a C# .NET-based application designed to streamline the management of training sessions, athlete registration, and workout equipment inventory. It is built using RESTful principles, exposing a **REST API** for managing core functionalities. Future versions of the system will introduce additional features such as graphical user interfaces, enhanced notification services, and more.

This project is containerized with **Docker** for easier deployment and uses **PostgreSQL** as the primary database.

## Features
- **REST API**: Current functionality allows for managing users, sessions, and equipment through a RESTful interface.
- **Workout Management**: Create, update, and delete training sessions.
- **Athlete Registration**: Athletes can be registered and assigned to specific workout sessions.
- **Equipment Inventory**: Track and manage the equipment needed for training sessions.
- **PostgreSQL Integration**: Reliable data storage and retrieval.
- **Dockerized Environment**: Simplified deployment using Docker containers.
- **Scalable Architecture**: Designed with future features in mind (e.g., user interfaces, notifications).

## Technologies Used
- **Backend**: C# with .NET 8
- **Database**: PostgreSQL
- **Containerization**: Docker
- **API**: REST

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)
- [PostgreSQL](https://www.postgresql.org/)

### Setup Instructions

1. **Clone the repository**:
   ```bash
   git clone https://github.com/MatasPal/TrainingSystem.git
   cd TrainingSystem
