# MG-DistributedLoggingSystem-BackEnd

### Screenshots
![Screenshot 2025-01-14 072831](https://github.com/user-attachments/assets/b97aff8d-b1f4-453d-bcec-3b2aa3ab1474)
![Screenshot 2025-01-14 073108](https://github.com/user-attachments/assets/63f3e947-f10a-418e-aca4-94396762078a)
![Screenshot_14-1-2025_7222_localhost](https://github.com/user-attachments/assets/538c5909-b041-4682-81ef-9aa2a52bf914)
![Screenshot_14-1-2025_72336_localhost](https://github.com/user-attachments/assets/ab5ac19a-fb13-4ca3-b3ec-b9e407352277)
![Screenshot_14-1-2025_72412_localhost](https://github.com/user-attachments/assets/80ea97b2-8f60-4a68-a9c7-6492d11c65e7)
![Screenshot_14-1-2025_7626_localhost](https://github.com/user-attachments/assets/6424e651-6895-4547-a33d-07c65985c751)
![Screenshot_14-1-2025_7715_localhost](https://github.com/user-attachments/assets/e3c57b23-771e-473a-a480-1bfc410e9987)
![Screenshot_14-1-2025_7739_localhost](https://github.com/user-attachments/assets/f7d79fd1-faaa-4ab3-a7e2-865a8e21c19b)
![Screenshot_14-1-2025_7139_localhost](https://github.com/user-attachments/assets/ef5bdc02-e54c-4eb5-bc53-5c426d6a1b03)
![Screenshot_14-1-2025_7165_localhost](https://github.com/user-attachments/assets/da99477d-3f85-463d-967c-35d5b03df8c7)
![Screenshot_14-1-2025_71630_localhost](https://github.com/user-attachments/assets/df1e118d-4b4c-4f19-8d58-220d29fba0c7)
![Screenshot_14-1-2025_71649_localhost](https://github.com/user-attachments/assets/253201cf-99ca-4b9b-853a-56855baf6a50)
![Screenshot_14-1-2025_7179_localhost](https://github.com/user-attachments/assets/f707c700-e974-4629-9a48-91699119d0d0)
![Screenshot_14-1-2025_71811_localhost](https://github.com/user-attachments/assets/8b5a1af5-8047-4399-bb04-b7045904f442)
![Screenshot_14-1-2025_71824_localhost](https://github.com/user-attachments/assets/09d23fc2-7da2-4d92-9ead-20972c8dce20)
![Screenshot_14-1-2025_71838_localhost](https://github.com/user-attachments/assets/ee263ef0-d755-45ad-800c-a7d99235e44b)
![Screenshot_14-1-2025_71926_localhost](https://github.com/user-attachments/assets/376c4616-2f34-48ed-b6cd-d1966a6ae12e)
![Screenshot_14-1-2025_72043_localhost](https://github.com/user-attachments/assets/ed128b3b-3ca2-4434-a253-f24efab7d90a)
![Screenshot_14-1-2025_72144_localhost](https://github.com/user-attachments/assets/745da0e4-6850-4646-89cc-526262ef9f5d)


## Technologies Used

### Back-End
- **ASP.NET Core**: The framework used to build the API service, providing a high-performance and scalable environment.
- **SQL Server**: Database used to persist logs, supporting advanced querying and scalability.
- **Amazon S3**: Used for scalable cloud storage to store log entries, ensuring reliability and durability.
- **Local File System**: For simple log storage and management directly on the server.
- **Message Queue (Optional)**: For asynchronous processing of logs across distributed systems.
- **JWT Authentication**: Secure API access with token-based authentication, ensuring data protection.

### Front-End
- **Angular**: A powerful front-end framework for building dynamic, responsive UIs that allow users to manage logs efficiently.

### Other Tools
- **Git**: Version control system used to manage project code and collaborate.

## Features & Highlights

- **Seamless Log Management**: Easily store, retrieve, and filter logs from multiple backends, making it ideal for distributed systems.
- **Dynamic Backend Switching**: The system can automatically switch between Amazon S3, SQL Server, or local storage without requiring any changes to the application code.
- **Scalable Architecture**: Designed to handle logs from any size system, whether it's a small application or a large distributed microservices architecture.
- **User-Friendly Front-End**: The Angular application provides a simple yet powerful UI to visualize and filter logs, making log management easy and intuitive.
- **Security**: With JWT authentication, only authorized users can access the logs, protecting your data from unauthorized access.
- **Flexible Metadata**: Track and filter logs based on service, log level, and timestamp, providing deep insights into your system's health.
- **Pagination and Filtering**: Efficiently handle large sets of log data with pagination, while offering fine-grained control over which logs to display based on user input.
- **Error Handling**: User-friendly error messages guide users in case of failed requests or invalid inputs, ensuring a smooth experience.
- **Extensibility**: The system can be easily extended with additional backends or features without requiring major changes to the core codebase.

## Why This Project?

This project aims to simplify the complexity of logging in distributed systems. Logs are an essential part of understanding system performance, troubleshooting issues, and maintaining security. With its flexible backend integration, modern front-end interface, and focus on scalability, the Distributed Logging System provides a robust solution for managing logs in any environment. Whether you're managing a small application or a large-scale distributed system, this tool is designed to make log management easier, more secure, and more efficient.

```markdown
# Distributed Logging System

## Overview

The Distributed Logging System is a unified application designed to handle log entries for a distributed system. It provides APIs to store and retrieve log entries across multiple backends, such as:

- Amazon S3-Compatible Storage
- Database Table
- Local File System
- Message Queue

The system also includes an Angular front-end to enable efficient log management and visualization.

## Features

### Back-End (API Service)

#### Store Log Entries:

- **Endpoint**: `POST /v1/logs`
- **Request Format**:

```json
{
    "service": "name_of_the_service",
    "level": "info | warning | error",
    "message": "The log message",
    "timestamp": "2023-01-22T21:37:55Z"
}
```

- **Description**: Validates fields and dynamically selects the storage backend.

#### Retrieve Log Entries:

- **Endpoint**: `GET /v1/logs`
- **Query Parameters**:
  - `service` (optional)
  - `level` (optional)
  - `start_time` (optional)
  - `end_time` (optional)

- **Response Format**:

```json
[
    {
        "service": "name_of_the_service",
        "level": "info",
        "message": "The log message",
        "timestamp": "2023-01-22T21:37:55Z"
    }
]
```

#### Authentication:

- Bearer token authentication secures all API endpoints.

#### Dynamic Backend Configuration:

- Seamlessly switch between storage backends (e.g., S3, database, local file system).

#### Metadata Tracking:

- Store and query metadata such as service, level, timestamp, and backend type.

### Front-End (Angular Application)

#### Log List View:

- Displays log entries in a tabular format.
- Columns include service, level, message, and timestamp.

#### Filtering Options:

- Filters logs by service, level, start_time, and end_time.

#### Details View:

- Shows detailed information for individual log entries.

#### Pagination:

- Handles large log datasets efficiently.

#### Error Handling:

- Displays user-friendly error messages for failed requests or invalid inputs.

## Project Structure

### Back-End

- **Endpoints**:
  - `POST /v1/logs`: Stores logs.
  - `GET /v1/logs`: Retrieves logs.

- **Backends Supported**:
  - **Amazon S3-Compatible Storage**: Uses HTTP client for integration.
  - **Database Storage**: Scalable and ensures data integrity.
  - **Local File System**: Configurable path for log storage.
  - **Message Queue (Optional)**: Allows distributed processing of logs.

### Front-End

- Built with Angular.
- Provides a dynamic user interface for managing logs.

## Setup Instructions

### Back-End

1. Clone the repository.
2. Install dependencies:

   ```bash
   npm install
   ```

3. Set up environment variables for backend configurations.
4. Run the application:

   ```bash
   npm start
   ```

### Front-End

1. Navigate to the frontend directory.
2. Install Angular dependencies:

   ```bash
   npm install
   ```

3. Run the Angular application:

   ```bash
   ng serve
   ```

## License

This project is licensed under the MIT License.
```

You can now add this file to the root directory of your GitHub repository.
