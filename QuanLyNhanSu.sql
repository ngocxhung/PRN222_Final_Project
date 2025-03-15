USE [master];
GO
CREATE DATABASE [QuanLyNhanSu];
GO
USE [QuanLyNhanSu];
GO
-- Thêm Role
-- Bảng role
CREATE TABLE role (
    Id INT PRIMARY KEY IDENTITY,
    RoleName NVARCHAR(100) NOT NULL
);

-- Bảng department
CREATE TABLE department (
    Id INT PRIMARY KEY IDENTITY,
    DepartmentName NVARCHAR(255) NOT NULL,
    ParentDepartmentID INT NULL,
    Status NVARCHAR(10) NOT NULL CHECK (Status IN ('Active', 'Inactive'))
);

-- Bảng salary_levels
CREATE TABLE salary_levels (
    Id INT PRIMARY KEY IDENTITY,
    Level INT NOT NULL,
    Daily_Salary DECIMAL(15,2) NOT NULL,
    Monthly_Salary DECIMAL(10,2) NOT NULL
);

-- Bảng positions (Chức vụ)
CREATE TABLE positions (
    Id INT PRIMARY KEY IDENTITY,
    PositionName NVARCHAR(100) NOT NULL UNIQUE
);

-- Bảng users (Có Avatar và liên kết với positions)
CREATE TABLE users (
    Id INT PRIMARY KEY IDENTITY,
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PhoneNumber NVARCHAR(15) NULL,
    Password NVARCHAR(255) NOT NULL,
    DepartmentID INT NULL,
    RoleID INT NULL,
    Status NVARCHAR(10) NOT NULL CHECK (Status IN ('Active', 'Inactive')),
    Salary_Level_Id INT NULL,
    EmploymentType NVARCHAR(20) NOT NULL CHECK (EmploymentType IN ('Full-time', 'Part-time', 'Contract')),
    Gender NVARCHAR(10) NOT NULL CHECK (Gender IN ('Male', 'Female', 'Other')),
    BirthDate DATE NULL,
    Avatar NVARCHAR(255) NULL, -- Ảnh đại diện
    PositionId INT NULL, -- Chức vụ (liên kết bảng positions)
    FOREIGN KEY (DepartmentID) REFERENCES department(Id),
    FOREIGN KEY (RoleID) REFERENCES role(Id),
    FOREIGN KEY (Salary_Level_Id) REFERENCES salary_levels(Id),
    FOREIGN KEY (PositionId) REFERENCES positions(Id)
);

-- Bảng salary_logs
CREATE TABLE salary_logs (
    Id INT PRIMARY KEY IDENTITY,
    Employee_Id INT NOT NULL,
    Valid_Days INT NOT NULL,
    Invalid_Days INT NOT NULL,
    Month INT NOT NULL,
    Year INT NOT NULL,
    Total_Salary DECIMAL(10,2) NOT NULL,
    Processed_By INT NULL,
    Processed_At DATETIME NULL,
    Updated_By INT NULL,
    Updated_At DATETIME NULL,
    Bonus DECIMAL(10,2) NULL,
    Salary_Level NVARCHAR(255) NULL,
    Total_Days INT NOT NULL,
    FOREIGN KEY (Employee_Id) REFERENCES users(Id)
);

-- Bảng leaveconfig
CREATE TABLE leaveconfig (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    Level INT NOT NULL,
    MaxLeaveDays INT NOT NULL,
    UsedLeaveDays INT NOT NULL,
    LeaveYear INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES users(Id)
);

-- Bảng leaverequest
CREATE TABLE leaverequest (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    LeaveDateStart DATE NOT NULL,
    LeaveDateEnd DATE NOT NULL,
    Reason NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(10) NOT NULL CHECK (Status IN ('Pending', 'Approved', 'Rejected')),
    CreatedAt DATETIME NOT NULL,
    ApprovedBy INT NULL,
    ApprovedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES users(Id),
    FOREIGN KEY (ApprovedBy) REFERENCES users(Id)
);

-- Bảng emailconfig
CREATE TABLE emailconfig (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    CheckInTime TIME NOT NULL,
    CheckOutTime TIME NOT NULL,
    LastEditConfig TIMESTAMP NOT NULL,
    FOREIGN KEY (UserId) REFERENCES users(Id)
);

-- Bảng log
CREATE TABLE log (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    Action NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Timestamp DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES users(Id)
);

-- Bảng checkout
CREATE TABLE checkout (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    ActionType NVARCHAR(10) NOT NULL CHECK (ActionType IN ('Check-In', 'Check-Out')),
    CheckInTime DATETIME NULL,
    CheckOutTime DATETIME NULL,
    LogDate DATE NOT NULL,
    Email_Sent TINYINT NOT NULL,
    Status NVARCHAR(10) NOT NULL,
    Reason NVARCHAR(MAX) NULL,
    FOREIGN KEY (UserId) REFERENCES users(Id)
);

-- Bảng employee_data_log
CREATE TABLE employee_data_log (
    Id INT PRIMARY KEY IDENTITY,
    File_Name NVARCHAR(255) NOT NULL,
    Action_Type NVARCHAR(10) NOT NULL CHECK (Action_Type IN ('Insert', 'Update', 'Delete')),
    Action_Time DATETIME NOT NULL,
    PerformedBy INT NOT NULL,
    Total_Records INT NOT NULL,
    FOREIGN KEY (PerformedBy) REFERENCES users(Id)
);

-- Thêm Role
INSERT INTO role (RoleName) VALUES ('Admin'), ('Manager'), ('Employee');

-- Thêm Department
INSERT INTO department (DepartmentName, ParentDepartmentID, Status) 
VALUES ('HR', NULL, 'Active'), ('IT', NULL, 'Active'), ('Finance', NULL, 'Active');

-- Thêm Salary Levels
INSERT INTO salary_levels (Level, Daily_Salary, Monthly_Salary) 
VALUES (1, 100.00, 3000.00), (2, 150.00, 4500.00), (3, 200.00, 6000.00);

-- Thêm Positions
INSERT INTO positions (PositionName) VALUES ('Nhân viên'), ('Quản lý'), ('Giám đốc');

-- Thêm Users (Có PositionId & Avatar)
INSERT INTO users (FullName, Email, PhoneNumber, Password, DepartmentID, RoleID, Status, Salary_Level_Id, EmploymentType, Gender, BirthDate, Avatar, PositionId)
VALUES 
('John Doe', 'john.doe@example.com', '1234567890', 'hashedpassword1', 1, 2, 'Active', 2, 'Full-time', 'Male', '1985-06-15', 'avatars/john_doe.jpg', 2),
('Jane Smith', 'jane.smith@example.com', '0987654321', 'hashedpassword2', 1, 3, 'Active', 1, 'Full-time', 'Female', '1990-02-20', 'avatars/jane_smith.jpg', 1);

-- Thêm Salary Logs
INSERT INTO salary_logs (Employee_Id, Valid_Days, Invalid_Days, Month, Year, Total_Salary, Processed_By, Processed_At, Bonus, Salary_Level, Total_Days)
VALUES (1, 22, 2, 2, 2025, 4500.00, 1, GETDATE(), 200.00, 'Level 2', 24);

-- Thêm Leave Requests
INSERT INTO leaverequest (UserId, LeaveDateStart, LeaveDateEnd, Reason, Status, CreatedAt, ApprovedBy, ApprovedAt)
VALUES (2, '2025-03-10', '2025-03-12', 'Family emergency', 'Approved', GETDATE(), 1, GETDATE());
