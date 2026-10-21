-- Jalankan di SQL Server Management Studio pada database yang akan digunakan.
IF DB_ID('QuizAppDb') IS NULL CREATE DATABASE QuizAppDb;
GO
USE QuizAppDb;
GO

IF OBJECT_ID('dbo.UserAnswers','U') IS NULL
CREATE TABLE dbo.UserAnswers (
    UserAnswerId INT IDENTITY(1,1) PRIMARY KEY,
    AttemptId INT NOT NULL,
    QuestionId INT NOT NULL,
    SelectedOptionId INT NOT NULL,
    EarnedWeight DECIMAL(5,2) NOT NULL DEFAULT 0
);
IF OBJECT_ID('dbo.UserQuizAttempts','U') IS NULL
CREATE TABLE dbo.UserQuizAttempts (
    AttemptId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    QuizId INT NOT NULL,
    StartedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    FinishedAt DATETIME2 NULL,
    TotalEarnedWeight DECIMAL(6,2) NOT NULL DEFAULT 0,
    FinalPercentage DECIMAL(5,2) NOT NULL DEFAULT 0
);
IF OBJECT_ID('dbo.QuestionOptions','U') IS NULL
CREATE TABLE dbo.QuestionOptions (
    OptionId INT IDENTITY(1,1) PRIMARY KEY,
    QuestionId INT NOT NULL,
    OptionLabel CHAR(1) NOT NULL,
    OptionText NVARCHAR(MAX) NOT NULL,
    IsCorrect BIT NOT NULL DEFAULT 0
);
IF OBJECT_ID('dbo.Questions','U') IS NULL
CREATE TABLE dbo.Questions (
    QuestionId INT IDENTITY(1,1) PRIMARY KEY,
    QuizId INT NOT NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    Weight DECIMAL(5,2) NOT NULL CHECK (Weight > 0),
    ImagePath NVARCHAR(500) NULL,
    OrderNumber INT NOT NULL DEFAULT 1
);
IF OBJECT_ID('dbo.Quizzes','U') IS NULL
CREATE TABLE dbo.Quizzes (
    QuizId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,
    DurationMinutes INT NOT NULL DEFAULT 30,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
IF OBJECT_ID('dbo.Users','U') IS NULL
CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Student',
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
-- Foreign keys are added after all tables exist.
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_Questions_Quizzes')
    ALTER TABLE dbo.Questions ADD CONSTRAINT FK_Questions_Quizzes FOREIGN KEY (QuizId) REFERENCES dbo.Quizzes(QuizId) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_Options_Questions')
    ALTER TABLE dbo.QuestionOptions ADD CONSTRAINT FK_Options_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_Attempts_Users')
    ALTER TABLE dbo.UserQuizAttempts ADD CONSTRAINT FK_Attempts_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_Attempts_Quizzes')
    ALTER TABLE dbo.UserQuizAttempts ADD CONSTRAINT FK_Attempts_Quizzes FOREIGN KEY (QuizId) REFERENCES dbo.Quizzes(QuizId);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_UserAnswers_Attempts')
    ALTER TABLE dbo.UserAnswers ADD CONSTRAINT FK_UserAnswers_Attempts FOREIGN KEY (AttemptId) REFERENCES dbo.UserQuizAttempts(AttemptId) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_UserAnswers_Questions')
    ALTER TABLE dbo.UserAnswers ADD CONSTRAINT FK_UserAnswers_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_UserAnswers_Options')
    ALTER TABLE dbo.UserAnswers ADD CONSTRAINT FK_UserAnswers_Options FOREIGN KEY (SelectedOptionId) REFERENCES dbo.QuestionOptions(OptionId);
GO

-- Demo account: student / student123
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username='student')
    INSERT INTO dbo.Users (Username, PasswordHash, FullName, Role)
    VALUES ('student', '703B0A3D6AD75B649A28ADDE7D83C6251DA457549263BC7FF45EC709B0A8448B', 'Demo Student', 'Student');
-- The app demo password marker above is replaced below with the SHA-256 hash of student123.
UPDATE dbo.Users SET PasswordHash='703B0A3D6AD75B649A28ADDE7D83C6251DA457549263BC7FF45EC709B0A8448B' WHERE Username='student';
GO
