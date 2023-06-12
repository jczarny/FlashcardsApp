using FlashcardsApp.Entities;
using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp
{
    public class FlashcardsContext : DbContext
    {

        public FlashcardsContext(DbContextOptions<FlashcardsContext> options)
            : base(options)
        { 
                
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
       
            var users = modelBuilder.Entity<User>();
            users.Property(b => b.Username).IsRequired().HasMaxLength(25);
            users.Property(b => b.PasswordHash).IsRequired();
            users.Property(b => b.PasswordSalt).IsRequired();
            users.HasIndex(b => b.Username).IsUnique();
            users.HasMany(p => p.OwnerDecks).WithOne(t => t.Creator).OnDelete(DeleteBehavior.SetNull);
            users.HasMany(p => p.Statistics).WithOne(t => t.User).OnDelete(DeleteBehavior.Cascade);
            users.HasMany(p => p.RevisionLogs).WithOne(t => t.User).OnDelete(DeleteBehavior.Cascade);
            users.HasMany(p => p.UserDecks).WithOne(t => t.User).OnDelete(DeleteBehavior.Cascade);

            var statistics = modelBuilder.Entity<Statistic>();
            statistics.HasKey(b => new { b.Date, b.UserId });
            statistics.Property(b => b.TotalGuesses).IsRequired();
            statistics.Property(b => b.CorrectGuesses).IsRequired();
            statistics.Property(b => b.WrongGuesses).IsRequired();
            statistics.ToTable(b =>
                b.HasCheckConstraint("CK_TotalGuesses", "[TotalGuesses] = [CorrectGuesses] + [WrongGuesses]"));

            var decks = modelBuilder.Entity<Deck>();
            decks.Property(b => b.Title).IsRequired().HasMaxLength(50);
            decks.Property(b => b.Description).HasMaxLength(500);
            decks.Property(b => b.IsPrivate).IsRequired();
            decks.HasMany(p => p.Cards).WithOne(t => t.Deck).OnDelete(DeleteBehavior.Cascade);
            decks.HasMany(p => p.UserDecks).WithOne(t => t.Deck).OnDelete(DeleteBehavior.NoAction);
            decks.HasMany(p => p.RevisionLogs).WithOne(t => t.Deck).OnDelete(DeleteBehavior.NoAction);

            var cards = modelBuilder.Entity<Card>();
            cards.HasMany(p => p.RevisionLogs).WithOne(t => t.Card).OnDelete(DeleteBehavior.Cascade);

            var userdecks = modelBuilder.Entity<UserDeck>();
            userdecks.HasKey(b => new { b.UserId, b.DeckId });

            var logs = modelBuilder.Entity<RevisionLog>();
            logs.HasKey(b => new { b.UserId, b.DeckId, b.CardId });

            Seed(modelBuilder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<UserDeck> UserDecks { get; set; }

        public void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "Michal15",
                    PasswordHash = new byte[256],
                    PasswordSalt = new byte[256],
                },
                new User
                {
                    Id = 2,
                    Username = "Krzychu7",
                    PasswordHash = new byte[256],
                    PasswordSalt = new byte[256],
                },
                new User
                {
                    Id = 3,
                    Username = "admin",
                    PasswordHash = new byte[256],
                    PasswordSalt = new byte[256],
                });

            modelBuilder.Entity<Statistic>().HasData(
                new Statistic
                {
                    UserId = 1,
                    Date = new DateTime(2008, 5, 1),
                    TotalGuesses = 0,
                    CorrectGuesses = 0,
                    WrongGuesses = 0
                },
                new Statistic
                {
                    UserId = 1,
                    Date = new DateTime(2008, 5, 2),
                    TotalGuesses = 1,
                    CorrectGuesses = 1,
                    WrongGuesses = 0
                },
                new Statistic
                {
                    UserId = 2,
                    Date = new DateTime(2008, 5, 1),
                    TotalGuesses = 1,
                    CorrectGuesses = 0,
                    WrongGuesses = 1
                },
                new Statistic
                {
                    UserId = 2,
                    Date = new DateTime(2008, 5, 2),
                    TotalGuesses = 0,
                    CorrectGuesses = 0,
                    WrongGuesses = 0
                },
                new Statistic
                {
                    UserId = 3,
                    Date = new DateTime(2008, 5, 1),
                    TotalGuesses = 0,
                    CorrectGuesses = 0,
                    WrongGuesses = 0
                },
                new Statistic
                {
                    UserId = 3,
                    Date = new DateTime(2008, 5, 2),
                    TotalGuesses = 1,
                    CorrectGuesses = 0,
                    WrongGuesses = 1
                });

            modelBuilder.Entity<Deck>().HasData(
                new Deck
                {
                    Id = 1,
                    CreatorId = 3,
                    Title = "Polish to English vocab",
                    Description = "Test123",
                    IsPrivate = false
                },
                new Deck
                {
                    Id = 2,
                    CreatorId = 3,
                    Title = "English to Polish vocab",
                    Description = "Test123",
                    IsPrivate = false
                },
                new Deck
                {
                    Id = 3,
                    CreatorId = 3,
                    Title = "German to English vocab",
                    Description = "Test123",
                    IsPrivate = false
                });

            modelBuilder.Entity<Card>().HasData(
                new Card { 
                    Id = 1,
                    DeckId = 1,
                    Front = "Kiełbasa", 
                    Reverse = "Sausage", 
                    Description = "Meat is neat" 
                },
                new Card
                {
                    Id = 2,
                    DeckId = 1,
                    Front = "Niebo",
                    Reverse = "Sky",
                    Description = "Sky is the limit"
                },
                new Card
                {
                    Id = 3,
                    DeckId = 2,
                    Front = "Sausage",
                    Reverse = "Kiełbasa",
                    Description = "Mięso jest spoko"
                },
                new Card
                {
                    Id = 4,
                    DeckId = 2,
                    Front = "Sky",
                    Reverse = "Niebo",
                    Description = "Limitem jest niebo"
                },
                new Card
                {
                    Id = 5,
                    DeckId = 3,
                    Front = "Ja",
                    Reverse = "Yes",
                    Description = "Ja voll!"
                });

            modelBuilder.Entity<RevisionLog>().HasData(
                new RevisionLog
                {
                    UserId = 1,
                    DeckId = 1,
                    CardId = 1,
                    Date = new DateTime(2008, 5, 2),
                    Ease = 2,
                    Stage = 1,
                },
                new RevisionLog
                {
                    UserId = 2,
                    DeckId = 1,
                    CardId = 1,
                    Date = new DateTime(2008, 5, 1),
                    Ease = 2,
                    Stage = 1,
                },
                new RevisionLog
                {
                    UserId = 3,
                    DeckId = 1,
                    CardId = 2,
                    Date = new DateTime(2008, 5, 2),
                    Ease = 2,
                    Stage = 1,
                });

            modelBuilder.Entity<UserDeck>().HasData(
                new UserDeck
                {
                    UserId = 1,
                    DeckId = 1,
                },
                new UserDeck
                {
                    UserId = 2,
                    DeckId = 1,
                },
                new UserDeck
                {
                    UserId = 3,
                    DeckId = 1,
                },
                new UserDeck
                {
                    UserId = 3,
                    DeckId = 2,
                });
        }
    }
}
