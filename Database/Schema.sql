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

-- =============================================================
-- TABLE 4: audit_logs
-- Security event log written by AuditLoggingMiddleware
-- Null user_id means unauthenticated request
-- =============================================================
IF OBJECT_ID('audit_logs', 'U') IS NULL
CREATE TABLE audit_logs (
    id          INT IDENTITY(1,1) PRIMARY KEY,
    user_id     INT           NULL,
    ip_address  NVARCHAR(45)  NOT NULL,
    method      NVARCHAR(10)  NOT NULL,
    path        NVARCHAR(255) NOT NULL,
    status_code INT           NOT NULL,
    event_note  NVARCHAR(255) NULL,
    created_at  DATETIME2     NOT NULL DEFAULT GETDATE(),

    CONSTRAINT fk_logs_user
        FOREIGN KEY (user_id) REFERENCES users(id)
        ON DELETE SET NULL
);
GO

-- =============================================================
-- INDEXES
-- Speed up the most common queries in the app
-- =============================================================
CREATE INDEX idx_tasks_user_id   ON tasks(user_id);
CREATE INDEX idx_task_files_task ON task_files(task_id);
CREATE INDEX idx_audit_logs_user ON audit_logs(user_id);
CREATE INDEX idx_audit_logs_time ON audit_logs(created_at);
GO