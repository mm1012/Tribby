using Microsoft.EntityFrameworkCore;

public class TribbyDbContext : DbContext
{
    public DbSet<User> Users { get; set;}
    public DbSet<Group> Groups { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Share> Shares { get; set; }
    public DbSet<EnumShareType> EnumShareTypes { get; set; }

    public string DbPath { get; } 

    public TribbyDbContext ()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        // Console.WriteLine(path);
        DbPath = Path.Join(path, "tribby.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.Entity<Group>().Property(g => g.Balance)
            .HasConversion<double>()
            .HasColumnType("NUMERIC");
        
        builder.Entity<Transaction>().Property(t => t.Amount)
            .HasConversion<double>()
            .HasColumnType("NUMERIC");

        builder.Entity<Share>().Property(s => s.Amount)
            .HasConversion<double>()
            .HasColumnType("NUMERIC");
    }
}