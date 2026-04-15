-- ============================================================
-- SECU2000 - Application Security Project
-- TaskVault Database Schema
-- Authors: Ayushpreet (8982295)
-- ============================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'taskvault')
    CREATE DATABASE taskvault;
GO

USE taskvault;
GO

-- =============================================================
-- TABLE 1: users
-- Stores all registered users with roles
-- =============================================================
IF OBJECT_ID('users', 'U') IS NULL
CREATE TABLE users (
    id            INT IDENTITY(1,1) PRIMARY KEY,
    username      NVARCHAR(50)  NOT NULL UNIQUE,
    email         NVARCHAR(100) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    role          NVARCHAR(10)  NOT NULL DEFAULT 'User'
                      CONSTRAINT chk_users_role CHECK (role IN ('User', 'Admin')),
    is_active     BIT           NOT NULL DEFAULT 1,
    created_at    DATETIME2     NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================================
-- TABLE 2: tasks
-- Core task data, always scoped to an owner (user_id)
-- =============================================================
IF OBJECT_ID('tasks', 'U') IS NULL
CREATE TABLE tasks (
    id          INT IDENTITY(1,1) PRIMARY KEY,
    user_id     INT            NOT NULL,
    title       NVARCHAR(100)  NOT NULL,
    description NVARCHAR(MAX)  NULL,
    status      NVARCHAR(10)   NOT NULL DEFAULT 'Pending'
                    CONSTRAINT chk_tasks_status CHECK (status IN ('Pending', 'InProgress', 'Done')),
    created_at  DATETIME2      NOT NULL DEFAULT GETDATE(),
    updated_at  DATETIME2      NOT NULL DEFAULT GETDATE(),

    CONSTRAINT fk_tasks_user
        FOREIGN KEY (user_id) REFERENCES users(id)
        ON DELETE CASCADE
);
GO

-- =============================================================
-- TABLE 3: task_files
-- Files uploaded and attached to a specific task
-- =============================================================
IF OBJECT_ID('task_files', 'U') IS NULL
CREATE TABLE task_files (
    id            INT IDENTITY(1,1) PRIMARY KEY,
    task_id       INT           NOT NULL,
    original_name NVARCHAR(255) NOT NULL,
    stored_name   NVARCHAR(255) NOT NULL UNIQUE,
    file_size     INT           NOT NULL,
    mime_type     NVARCHAR(100) NOT NULL,
    uploaded_at   DATETIME2     NOT NULL DEFAULT GETDATE(),

    CONSTRAINT fk_files_task
        FOREIGN KEY (task_id) REFERENCES tasks(id)
        ON DELETE CASCADE
);
GO