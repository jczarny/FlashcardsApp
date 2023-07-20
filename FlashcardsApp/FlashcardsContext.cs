using FlashcardsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp
{
    /// <summary>
    /// Entity framework database context. \
    /// Creates Database, models relationships and seeds with example data.
    /// </summary>
    public class FlashcardsContext : DbContext
    {

        public FlashcardsContext(DbContextOptions<FlashcardsContext> options)
            : base(options)
        {
            //this.Database.EnsureDeleted();
            this.Database.EnsureCreated();
        }

        /// <summary>
        /// Creates entities, models relationships and puts restrictions as maxLength.
        /// </summary>
        /// <param name="modelBuilder"></param>
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
            decks.HasMany(p => p.UserDecks).WithOne(t => t.Deck).OnDelete(DeleteBehavior.Cascade);
            decks.HasMany(p => p.RevisionLogs).WithOne(t => t.Deck);

            var cards = modelBuilder.Entity<Card>();
            cards.HasMany(p => p.RevisionLogs).WithOne(t => t.Card);

            var userdecks = modelBuilder.Entity<UserDeck>();
            userdecks.HasKey(b => new { b.UserId, b.DeckId });

            var logs = modelBuilder.Entity<RevisionLog>();
            logs.HasKey(b => new { b.UserId, b.DeckId, b.CardId });

            Seed(modelBuilder);
        }

        /// <summary>
        /// Entity containing all user credentials.
        /// </summary>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// Entity containing user's achievments (Currently empty, work in progress).
        /// </summary>
        public DbSet<Statistic> Statistics { get; set; }
        /// <summary>
        /// Entity containing information about decks made by users.
        /// </summary>
        public DbSet<Deck> Decks { get; set; }
        /// <summary>
        /// Entity containing information about cards within decks.
        /// </summary>
        public DbSet<Card> Cards { get; set; }
        /// <summary>
        /// Weak entity modeling owning deck by user.
        /// </summary>
        public DbSet<UserDeck> UserDecks { get; set; }

        /// <summary>
        /// Seeds database with example data.
        /// </summary>
        /// <param name="modelBuilder"></param>
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
                    Username = "Admin111",
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
                new Card
                {
                    Id = 1,
                    DeckId = 1,
                    Front = "Kiełbasa",
                    Reverse = "Sausage",
                    Description = "Sausage is tasty"
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
                    Description = "Yes, i do like sunflowers"
                },
                new Card
                {
                    Id = 6,
                    DeckId = 1,
                    Front = "Rok",
                    Reverse = "Year",
                    Description = "This year I turn 18!"
                },
                new Card
                {
                    Id = 7,
                    DeckId = 1,
                    Front = "Dziś",
                    Reverse = "Today",
                    Description = "Today at 6:15 we have a meeting"
                },
                new Card
                {
                    Id = 8,
                    DeckId = 1,
                    Front = "Iść",
                    Reverse = "Go",
                    Description = "walk on a road"
                },
                new Card
                {
                    Id = 9,
                    DeckId = 1,
                    Front = "Śmiać się",
                    Reverse = "Laugh",
                    Description = "Laugh at a picture"
                },
                new Card
                {
                    Id = 10,
                    DeckId = 1,
                    Front = "Daleko",
                    Reverse = "Far",
                    Description = "My younger brother lives far away, but he comes home for Christmas."
                },
                new Card
                {
                    Id = 11,
                    DeckId = 1,
                    Front = "Piękny",
                    Reverse = "Beautiful",
                    Description = "She is beautiful."
                },
                new Card
                {
                    Id = 12,
                    DeckId = 1,
                    Front = "Brzydki",
                    Reverse = "Ugly",
                    Description = "Ugly face"
                },
                new Card
                {
                    Id = 13,
                    DeckId = 1,
                    Front = "Blisko",
                    Reverse = "Near",
                    Description = "I live near the university."
                },
                new Card
                {
                    Id = 14,
                    DeckId = 1,
                    Front = "Wino",
                    Reverse = "Wine",
                    Description = "Glass of wine."
                },
                new Card
                {
                    Id = 15,
                    DeckId = 1,
                    Front = "Herbata",
                    Reverse = "Tea",
                    Description = "Would you like a cup of tea?"
                },
                new Card
                {
                    Id = 16,
                    DeckId = 1,
                    Front = "Kawa",
                    Reverse = "Coffee",
                    Description = "The coffee pot is full of coffee."
                },
                new Card
                {
                    Id = 17,
                    DeckId = 1,
                    Front = "Wołowina",
                    Reverse = "Beef",
                    Description = "Beef for the main course."
                },
                new Card
                {
                    Id = 18,
                    DeckId = 2,
                    Front = "Year",
                    Reverse = "Rok",
                    Description = "W tym roku będę miał 18 lat!"
                },
                new Card
                {
                    Id = 19,
                    DeckId = 2,
                    Front = "Today",
                    Reverse = "Dziś",
                    Description = "Dziś o 6:15 mamy spotkanie."
                },
                new Card
                {
                    Id = 20,
                    DeckId = 2,
                    Front = "Go",
                    Reverse = "Iść",
                    Description = "Iść po drodze."
                },
                new Card
                {
                    Id = 21,
                    DeckId = 2,
                    Front = "Laugh",
                    Reverse = "Śmiać się",
                    Description = "Śmiać się z samego siebie."
                },
                new Card
                {
                    Id = 22,
                    DeckId = 1,
                    Front = "Far",
                    Reverse = "Daleko",
                    Description = "Mój młodszy brat mieszka daleko stąd, ale przyjeżdża na te święta."
                },
                new Card
                {
                    Id = 23,
                    DeckId = 1,
                    Front = "Beautiful",
                    Reverse = "Piękny",
                    Description = "Ona jest piękna."
                },
                new Card
                {
                    Id = 24,
                    DeckId = 1,
                    Front = "Ugly",
                    Reverse = "Brzydki",
                    Description = "Brzydka zagrywka."
                },
                new Card
                {
                    Id = 25,
                    DeckId = 1,
                    Front = "Near",
                    Reverse = "Blisko",
                    Description = "Mieszkam blisko uniwersytetu."
                },
                new Card
                {
                    Id = 26,
                    DeckId = 1,
                    Front = "Wine",
                    Reverse = "Wino",
                    Description = "Czerwone wino jest lepsze niż białe."
                },
                new Card
                {
                    Id = 27,
                    DeckId = 1,
                    Front = "Tea",
                    Reverse = "Herbata",
                    Description = "Czy chciałbyś szklankę herbaty?"
                },
                new Card
                {
                    Id = 28,
                    DeckId = 1,
                    Front = "Coffee",
                    Reverse = "Kawa",
                    Description = "Poproszę kawę z dwoma łyżkami cukru."
                },
                new Card
                {
                    Id = 29,
                    DeckId = 1,
                    Front = "Beef",
                    Reverse = "Wołowina",
                    Description = "Wołowina na danie główne."
                },
                new Card
                {
                    Id = 30,
                    DeckId = 3,
                    Front = "Woche",
                    Reverse = "Week",
                    Description = " There are seven days in one week, and four full weeks in a month."
                },
                new Card
                {
                    Id = 31,
                    DeckId = 3,
                    Front = "Gestern",
                    Reverse = "Yesterday",
                    Description = "We met yesterday afternoon."
                },
                new Card
                {
                    Id = 32,
                    DeckId = 3,
                    Front = "Können",
                    Reverse = "Can",
                    Description = "Can jump over."
                },
                new Card
                {
                    Id = 33,
                    DeckId = 3,
                    Front = "Benutzen",
                    Reverse = "Use",
                    Description = "The person is using a computer to write an e-mail."
                },
                new Card
                {
                    Id = 34,
                    DeckId = 3,
                    Front = "Lachen",
                    Reverse = "Laugh",
                    Description = "The couple laughs at the picture."
                },
                new Card
                {
                    Id = 35,
                    DeckId = 3,
                    Front = "Machen",
                    Reverse = "Make",
                    Description = "The chef makes orange juice."
                },
                new Card
                {
                    Id = 36,
                    DeckId = 3,
                    Front = "Sehen",
                    Reverse = "See",
                    Description = "The tourists see the sunset."
                },
                new Card
                {
                    Id = 37,
                    DeckId = 3,
                    Front = "Weit",
                    Reverse = "Far",
                    Description = "The woman is looking at something far away."
                },
                new Card
                {
                    Id = 38,
                    DeckId = 3,
                    Front = "Gut",
                    Reverse = "Good",
                    Description = "Vegetables are good for you."
                },
                new Card
                {
                    Id = 39,
                    DeckId = 3,
                    Front = "Hässlich",
                    Reverse = "Ugly",
                    Description = "Why is his face so ugly."
                },
                new Card
                {
                    Id = 40,
                    DeckId = 3,
                    Front = "Schwierig",
                    Reverse = "Difficult",
                    Description = "I was given a very difficult task, but i managed to do it."
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
