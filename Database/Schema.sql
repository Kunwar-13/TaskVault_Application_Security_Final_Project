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