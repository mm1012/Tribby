using Microsoft.EntityFrameworkCore;

public class TribbyDbContext : DbContext
{
    /// <summary>
    /// Path to the database file on disk.
    /// </summary>
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
}