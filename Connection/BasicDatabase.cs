using familyMart.Model;
using Microsoft.EntityFrameworkCore;

namespace familyMart.Connection;

public class BasicDatabase : DbContext
{
    public BasicDatabase(DbContextOptions<BasicDatabase> dbContextOptions)
        : base(dbContextOptions)
    {
    }


    public DbSet<Employee> Employees { get; set; }
    public DbSet<Absen> Absens { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base.OnModelCreating(modelBuilder);
    }
}