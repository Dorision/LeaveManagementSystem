# Leave Management System

A comprehensive leave management system built with ASP.NET Core that handles employee leave requests, approvals, and management.

## System Flow Diagrams


```mermaid
graph TD
    CEO[Linda Jenkins - CEO]
    M1[Milton Coleman - Manager]
    M2[Colin Horton - Dev Team Lead]
    D1[Ella Jefferson - Developer]
    D2[Earl Craig - Developer]
    D3[Marsha Murphy - Developer]
    D4[Luis Ortega - Developer]
    D5[Faye Dennis - Developer]
    D6[Amy Burns - Developer]
    D7[Darrel Weber - Developer]

    CEO --> M1
    CEO --> M2
    M2 --> D1
    M2 --> D2
    M2 --> D3
    M2 --> D4
    M2 --> D5
    M2 --> D6
    M2 --> D7
```


```mermaid
sequenceDiagram
    participant E as Employee
    participant S as System
    participant M as Manager
    participant HR as HR/Admin

    E->>S: Submit Leave Request
    S->>S: Validate Request
    S->>M: Notify Manager
    
    alt Request Valid
        M->>S: Review Request
        alt Approved
            S->>E: Notify Approval
            S->>HR: Update Leave Balance
        else Rejected
            S->>E: Notify Rejection
        end
    else Invalid Request
        S->>E: Return Validation Errors
    end
```


```mermaid
sequenceDiagram
    participant U as User
    participant A as Auth Controller
    participant S as System
    participant DB as Database

    U->>A: Login/Register Request
    A->>S: Validate Credentials
    S->>DB: Check User Exists
    
    alt Valid Credentials
        DB->>S: User Data
        S->>A: Generate JWT Token
        A->>U: Return Token + User Info
    else Invalid Credentials
        DB->>S: Not Found/Invalid
        S->>A: Error Message
        A->>U: Return Error
    end
```

## Features

1. **User Management**
   - Role-based access control (Admin, Manager, Employee)
   - User hierarchy management
   - Authentication using JWT tokens

2. **Leave Management**
   - Submit leave requests
   - Approve/Reject requests
   - Leave balance tracking
   - Multiple leave types support

3. **Public Holiday Management**
   - Automatic holiday recognition
   - Holiday calendar management
   - Regional holiday support


## Technical Stack

- **Backend**: ASP.NET Core 9.0
- **Database**: SQL Server
- **Authentication**: JWT Bearer
- **Validation**: FluentValidation
- **Logging**: Serilog

## Getting Started

1. Ensure you have .NET 9.0 SDK installed
2. Clone the repository
3. Update the connection string in `appsettings.json`
4. Run database migrations:
   ```bash
   dotnet ef database update
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

## API Endpoints

### Authentication
- POST `/api/Auth/register` - Register new user
- POST `/api/Auth/login` - User login

### Leave Requests
- GET `/api/Leaves` - Get all leaves
- POST `/api/Leaves` - Create leave request
- PUT `/api/Leaves/{id}` - Update leave request
- DELETE `/api/Leaves/{id}` - Delete leave request

### Public Holidays
- GET `/api/PublicHolidays` - Get all public holidays
- POST `/api/PublicHolidays` - Add public holiday
- PUT `/api/PublicHolidays/{id}` - Update public holiday
- DELETE `/api/PublicHolidays/{id}` - Delete public holiday