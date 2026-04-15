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