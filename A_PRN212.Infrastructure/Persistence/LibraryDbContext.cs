using A_PRN212.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace A_PRN212.Infrastructure.Persistence
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LibraryRule> LibraryRules { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<AuthorImage> AuthorImages { get; set; }
        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<GenreImage> GenreImages { get; set; }
        public DbSet<UserImage> UserImages { get; set; }

        public LibraryDbContext()
        {
        }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string basePath = File.Exists(Path.Combine(AppContext.BaseDirectory, "appsettings.json"))
                    ? AppContext.BaseDirectory
                    : Directory.GetCurrentDirectory();

                var config = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                string? connectionString = config.GetConnectionString("MyCnn");
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    optionsBuilder.UseSqlServer(connectionString);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .ToTable("Book")
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId);

            modelBuilder.Entity<Author>().ToTable("Author");
            modelBuilder.Entity<Genre>().ToTable("Genre");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<LibraryRule>().ToTable("LibraryRule");

            modelBuilder.Entity<BorrowRecord>()
                .ToTable("BorrowRecord")
                .HasOne(br => br.User)
                .WithMany()
                .HasForeignKey(br => br.UserId);

            modelBuilder.Entity<BorrowRecord>()
                .HasOne(br => br.Book)
                .WithMany()
                .HasForeignKey(br => br.BookId);

            modelBuilder.Entity<Card>().ToTable("Card");
            modelBuilder.Entity<AuthorImage>().ToTable("AuthorImage");
            modelBuilder.Entity<BookImage>().ToTable("BookImage");
            modelBuilder.Entity<GenreImage>().ToTable("GenreImage");
            modelBuilder.Entity<UserImage>().ToTable("UserImage");
        }
    }
}
