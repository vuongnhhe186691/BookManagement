CREATE DATABASE A_PRN212
Use A_PRN212


CREATE TABLE [Author] (
    AuthorId INT PRIMARY KEY IDENTITY(1,1),
    AuthorName NVARCHAR(255) NOT NULL,
    Biography NVARCHAR(MAX) NULL,
    Nationality NVARCHAR(100) NULL,
    BirthDate DATE NULL
);
CREATE TABLE [AuthorImage] (
    ImageId INT PRIMARY KEY IDENTITY(1,1),
    AuthorId INT NOT NULL,
    ImageUrl NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (AuthorId) REFERENCES [Author](AuthorId)
);
CREATE TABLE [User] (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(255) NOT NULL,
	Gender NVARCHAR(50) CHECK (Gender IN ('Male', 'Female', 'Other')),
	Username NVARCHAR(255) NOT NULL,
	Password NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    Phone NVARCHAR(15) UNIQUE,
    Role NVARCHAR(50) NOT NULL CHECK (Role IN ('Admin', 'Librarian', 'Reader')),
);
CREATE TABLE [UserImage] (
    ImageId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    ImageUrl NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES [User](UserId)
);
CREATE TABLE Genre (
    GenreId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL UNIQUE,
    Description NVARCHAR(MAX) NULL
);
CREATE TABLE [GenreImage] (
    ImageId INT PRIMARY KEY IDENTITY(1,1),
    GenreId INT NOT NULL,
    ImageUrl NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (GenreId) REFERENCES [Genre](GenreId)
);
CREATE TABLE [Book] (
    BookId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    AuthorId INT NOT NULL, 
    Publisher NVARCHAR(255),
    GenreId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    Available INT NOT NULL DEFAULT 0,
    FOREIGN KEY (GenreId) REFERENCES Genre(GenreId),
    FOREIGN KEY (AuthorId) REFERENCES Author(AuthorId) 
);
CREATE TABLE [BookImage] (
    ImageId INT PRIMARY KEY IDENTITY(1,1),
    BookId INT NOT NULL,
    ImageUrl NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (BookId) REFERENCES [Book](BookId)
);
CREATE TABLE [BorrowRecord] (
    BorrowId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    BookId INT NOT NULL,
    BorrowDate DATETIME DEFAULT GETDATE(),
    DueDate DATETIME NOT NULL,
    ReturnDate DATETIME NULL,
    Status NVARCHAR(50) CHECK (Status IN ('Not Borrowed', 'Borrowed', 'Returned', 'Overdue')) DEFAULT 'Not Borrowed',
    FOREIGN KEY (UserId) REFERENCES [User](UserId),
    FOREIGN KEY (BookId) REFERENCES Book(BookId)
);
CREATE TABLE [LibraryRule] (
	RuleId int primary key identity(1,1),
	[Rule] varchar (max)
);
CREATE TABLE [Card] (
    CardId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    IssueDate DATETIME DEFAULT GETDATE(),
    ExpiryDate DATETIME NOT NULL,
    Status NVARCHAR(50) CHECK (Status IN ('Active', 'Expired', 'Revoked')) DEFAULT 'Active',
    FOREIGN KEY (UserId) REFERENCES [User](UserId)
);
INSERT INTO [Card] (UserId, ExpiryDate, Status)
VALUES 
    (1, DATEADD(YEAR, 1, GETDATE()), 'Active'),
    (2, DATEADD(YEAR, 2, GETDATE()), 'Active'),
    (3, DATEADD(MONTH, 6, GETDATE()), 'Expired'),
    (4, DATEADD(YEAR, 1, GETDATE()), 'Revoked'),
    (5, DATEADD(YEAR, 3, GETDATE()), 'Active');
INSERT INTO [LibraryRule] ([Rule]) VALUES 
('Readers must return books on time. Late returns will be fined 10,000 VND per day.'),
('Readers must take care of books and materials, and must not tear, lose, or scribble on them.'),
('Each reader is allowed to borrow a maximum of 3 books at the same time.'),
('The maximum borrowing period is 14 days. To extend the borrowing period, readers must register before the due date.'),
('Readers must present a valid library card when borrowing and returning books.'),
('Lost or damaged books must be compensated according to the current value of the book.'),
('Maintain silence and do not make noise in the library reading area.');
INSERT INTO Author (AuthorName, Biography, Nationality, BirthDate) VALUES
('J.K. Rowling', 'British author of the Harry Potter series.', 'British', '1965-07-31'),
('George Orwell', 'British writer known for 1984 and Animal Farm.', 'British', '1903-06-25'),
('Jane Austen', 'English novelist famous for Pride and Prejudice.', 'British', '1775-12-16'),
('Mark Twain', 'American author known for The Adventures of Huckleberry Finn.', 'American', '1835-11-30'),
('Ernest Hemingway', 'American novelist known for The Old Man and the Sea.', 'American', '1899-07-21'),
('Leo Tolstoy', 'Russian writer famous for War and Peace.', 'Russian', '1828-09-09'),
('Fyodor Dostoevsky', 'Russian author of Crime and Punishment.', 'Russian', '1821-11-11'),
('Gabriel García Márquez', 'Colombian writer famous for One Hundred Years of Solitude.', 'Colombian', '1927-03-06'),
('Haruki Murakami', 'Japanese novelist known for Norwegian Wood.', 'Japanese', '1949-01-12');
INSERT INTO [User] (FullName, Gender, Username, Password, Email, Phone, Role) VALUES
('Doan Tien Dat', 'Male', 'dtd', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'admin1@example.com', '0123456789', 'Admin'), -- 123
('Le Thi B', 'Female', 'admin2', 'e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7', 'admin2@example.com', '0901234567', 'Admin'), -- Admin@123
('Tran Thi C', 'Female', 'librarian1', 'e62c6969cd381625af2d3742caf52df58fde8ad3e98260e3cda141e65b670fd5', 'librarian1@example.com', '0987654321', 'Librarian'), -- Lib@1234
('Nguyen Van D', 'Male', 'librarian2', '6d0d1fbeedde8d6f41e2a037f2cf90425bf89ed419da2666814e83773b5024f2', 'librarian2@example.com', '0912345678', 'Librarian'), -- Lib@5678
('Hoang Minh E', 'Male', 'reader1', 'c8d1a2913a78a20a5acbd3d36d697fb541b6e7b50df0b0315c70e4e69a88ce8a', 'reader1@example.com', '0934567890', 'Reader'), -- Reader@123
('Pham Thi F', 'Female', 'reader2', '862063f5b0593dd33fe2488c810f9b26f69fb4a67332990025f4c3de05674fb5', 'reader2@example.com', '0978123456', 'Reader'), -- Reader@456
('Vo Van G', 'Male', 'reader3', '440c89c70d18c30a47f2eb9908cfd68885f7f3412ae7e68b4d8191958874f845', 'reader3@example.com', '0956781234', 'Reader'), -- Reader@789
('Dinh Cong H', 'Male', 'reader4', '519da5a402ed3d155e805d3142aa7d5e2957db00ff697aa6194628fef01ddd27', 'reader4@example.com', '0945678123', 'Reader'), -- Reader@159
('Bui Thi I', 'Female', 'reader5', 'ed6169c7a4b26ad8e1507a97b9f4e77b8fb684a86805f8e2cd911d7addb4e625', 'reader5@example.com', '0923456781', 'Reader'); -- Reader@753
INSERT INTO [Genre] (Name, Description) VALUES
('Fiction', 'Imaginative storytelling.'),
('Non-Fiction', 'Based on real facts.'),
('Science Fiction', 'Futuristic concepts and technology.'),
('Fantasy', 'Magical worlds and creatures.'),
('Mystery', 'Detective and crime stories.'),
('Horror', 'Fear and supernatural elements.'),
('History', 'Documenting past events.'),
('Biography', 'Life stories of real people.');
INSERT INTO [Book] (Title, AuthorId, Publisher, GenreId, Quantity, Available) VALUES
('To Kill a Mockingbird', 3, 'J.B. Lippincott & Co.', 1, 10, 8),
('Great Expectations', 9, 'Chapman & Hall', 1, 12, 10),
('Pride and Prejudice', 3, 'T. Egerton, Whitehall', 1, 15, 13),
('Sapiens: A Brief History of Humankind', 8, 'Harper', 2, 10, 7),
('The Diary of a Young Girl', 1, 'Contact Publishing', 2, 8, 5),
('Foundation', 2, 'Gnome Press', 3, 8, 5),
('1984', 2, 'Secker & Warburg', 3, 10, 6),
('Brave New World', 2, 'Chatto & Windus', 3, 9, 7),
('Harry Potter and the Sorcerer’s Stone', 1, 'Bloomsbury', 4, 20, 18),
('The Lord of the Rings', 7, 'George Allen & Unwin', 4, 15, 12),
('Murder on the Orient Express', 8, 'Collins Crime Club', 5, 10, 6),
('The Hound of the Baskervilles', 3, 'George Newnes', 5, 9, 7),
('The Shining', 5, 'Doubleday', 6, 12, 10),
('It', 5, 'Viking', 6, 10, 8),
('Les Misérables', 5, 'A. Lacroix, Verboeckhoven & Cie.', 7, 8, 5),
('Steve Jobs', 6, 'Simon & Schuster', 8, 10, 8),
('The Da Vinci Code', 6, 'Doubleday', 1, 12, 9),
('The Perks of Being a Wallflower', 3, 'MTV Books', 5, 10, 6),
('The Secret Garden', 6, 'Frederick Warne & Co.', 7, 8, 5),
('The Chronicles of Narnia', 3, 'Geoffrey Bles', 8, 15, 12),
('The Little Prince', 2, 'Reynal & Hitchcock', 2, 10, 8),
('The Time Traveler’s Wife', 9, 'Audrey Niffenegger', 3, 12, 9),
('The Hunger Games', 7, 'Scholastic Press', 1, 15, 11),
('The Maze Runner', 8, 'Delacorte Press', 2, 10, 7),
('Divergent', 1, 'Katherine Tegen Books', 3, 12, 9),
('Educated', 5, 'Random House', 4, 10, 8),
('Becoming', 7, 'Crown', 5, 12, 10),
('The Immortal Life of Henrietta Lacks', 7, 'Crown', 1, 10, 8),
('Into the Wild', 8, 'Knopf', 2, 8, 6),
('The Wright Brothers', 5, 'Simon & Schuster', 3, 10, 7),
('Unbroken', 1, 'Laura Hillenbrand', 4, 12, 9),
('The Night Circus', 3, 'Doubleday', 8, 10, 6),
('The Goldfinch', 1, 'Little, Brown and Company', 5, 10, 8),
('Gone Girl', 4, 'Crown Publishing Group', 5, 12, 10);

