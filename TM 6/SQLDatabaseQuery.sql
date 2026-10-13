CREATE DATABASE StudentRegistrationDB;
GO

USE StudentRegistrationDB;
GO

SELECT DB_NAME() AS CurrentDatabase;

--

CREATE TABLE Programs
(
	ProgramId INT IDENTITY(1,1)
	CONSTRAINT PK_Programs PRIMARY KEY,

	ProgramCode VARCHAR(20) NOT NULL
	CONSTRAINT UQ_Programs_ProgramCode UNIQUE,

	ProgramName VARCHAR(100) NOT NULL,

	IsActive BIT NOT NULL
	CONSTRAINT DF_Programs_IsActive DEFAULT 1,

	CreatedAt DATETIME2 NOT NULL
	CONSTRAINT DF_Programs_CreatedAt
	DEFAULT SYSDATETIME()
);
GO

--

INSERT INTO Programs
(
	ProgramCode,
	ProgramName
)
VALUES
	('TI', 'Teknik Informatika'),
	('SI', 'Sistem Informasi'),
	('MNJ', 'Manajemen'),
	('TE', 'Teknik Elektro'),
	('DKV', 'Desain Komunikasi Visual');
GO

SELECT *
FROM Programs;

--

CREATE TABLE Students
(
	StudentId INT IDENTITY(1,1)
		CONSTRAINT PK_Students PRIMARY KEY,
	NIM VARCHAR(20) NOT NULL
		CONSTRAINT UQ_Students_NIM UNIQUE,
	Name VARCHAR(100) NOT NULL,
	ProgramId INT NOT NULL,
	BirthDate DATE NULL,
	Address VARCHAR(250) NULL,
	PhoneNumber VARCHAR(20) NULL,
	CreatedAt DATETIME2 NOT NULL
		CONSTRAINT DF_Students_CreatedAt
		DEFAULT SYSDATETIME(),
	UpdatedAt DATETIME2 NULL,
		CONSTRAINT FK_Students_Programs
		FOREIGN KEY (ProgramId)
		REFERENCES Programs(ProgramId)
);
GO

--

INSERT INTO Students (
    NIM,
    Name,
    ProgramId,
    BirthDate,
    Address,
    PhoneNumber
)
VALUES
(
    '20231001',
    'Ahmad Rizki',
    1,
    '2003-03-12',
    'Jl. Merdeka No. 123, Surabaya',
    '081234567890'
),
(
    '20231002',
    'Siti Aisyah',
    2,
    '2004-07-21',
    'Jl. Diponegoro No. 45, Surabaya',
    '082233445566'
),
(
    '20231003',
    'Budi Santoso',
    1,
    '2003-11-05',
    'Jl. Ahmad Yani No. 10, Surabaya',
    '081355667788'
),
(
    '20231004',
    'Citra Lestari',
    3,
    '2004-01-18',
    'Jl. Darmo No. 20, Surabaya',
    '089900112233'
),
(
    '20231005',
    'Dimas Prayoga',
    4,
    '2003-09-27',
    'Jl. Raya ITS, Surabaya',
    '081288776655'
);
GO

SELECT *
FROM Students;

--

SELECT
    s.StudentId,
    s.NIM,
    s.Name,
    p.ProgramCode,
    p.ProgramName,
    s.BirthDate,
    s.PhoneNumber
FROM Students s
INNER JOIN Programs p
ON s.ProgramId = p.ProgramId;