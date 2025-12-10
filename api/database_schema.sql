-- Task Manager Database Schema
-- MySQL Database Tables Creation Script

-- Drop tables if they exist (for clean setup)
DROP TABLE IF EXISTS Tasks;
DROP TABLE IF EXISTS Categories;
DROP TABLE IF EXISTS Users;

-- Create Users table
CREATE TABLE Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_username (Username),
    INDEX idx_email (Email)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create Categories table
CREATE TABLE Categories (
    CategoryId INT AUTO_INCREMENT PRIMARY KEY,
    CategoryName VARCHAR(50) NOT NULL,
    Description TEXT,
    Color VARCHAR(7) DEFAULT '#007bff', -- Hex color code for UI display
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_category_name (CategoryName)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create Tasks table
CREATE TABLE Tasks (
    TaskId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CategoryId INT,
    Title VARCHAR(200) NOT NULL,
    Description TEXT,
    Priority ENUM('Low', 'Medium', 'High', 'Urgent') DEFAULT 'Medium',
    Status ENUM('Pending', 'In Progress', 'Completed', 'Cancelled') DEFAULT 'Pending',
    DueDate DATETIME,
    CompletedAt DATETIME NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId) ON DELETE SET NULL,
    INDEX idx_user_id (UserId),
    INDEX idx_category_id (CategoryId),
    INDEX idx_status (Status),
    INDEX idx_priority (Priority),
    INDEX idx_due_date (DueDate)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insert default categories
INSERT INTO Categories (CategoryName, Description, Color) VALUES
('Work', 'Work-related tasks', '#007bff'),
('Personal', 'Personal tasks', '#28a745'),
('Shopping', 'Shopping list items', '#ffc107'),
('Health', 'Health and fitness tasks', '#dc3545'),
('Education', 'Learning and education tasks', '#17a2b8'),
('Other', 'Miscellaneous tasks', '#6c757d');

-- Sample data (optional - can be removed if not needed)
-- Insert a sample user (password: 'password123' - should be hashed in production)
-- Note: In production, passwords should be properly hashed using bcrypt or similar
INSERT INTO Users (Username, Email, PasswordHash) VALUES
('admin', 'admin@example.com', 'hashed_password_here');

-- Insert sample tasks (optional - can be removed if not needed)
-- INSERT INTO Tasks (UserId, CategoryId, Title, Description, Priority, Status, DueDate) VALUES
-- (1, 1, 'Complete project documentation', 'Write comprehensive documentation for the project', 'High', 'Pending', DATE_ADD(NOW(), INTERVAL 7 DAY)),
-- (1, 2, 'Buy groceries', 'Milk, eggs, bread, vegetables', 'Medium', 'Pending', DATE_ADD(NOW(), INTERVAL 2 DAY)),
-- (1, 4, 'Morning workout', '30 minutes cardio and strength training', 'Medium', 'Pending', DATE_ADD(NOW(), INTERVAL 1 DAY));


