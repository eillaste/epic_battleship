using System;
using Domain;
//using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace DAL
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Game> Games { get; set; } = default!;
        public DbSet<GameState> GameStates { get; set; } = default!;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        {
            
        }
        /*private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(
            builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Information)
                    .AddConsole();
            }
        );*/
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                //.UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"
                        Server=barrel.itcollege.ee,1533;
                        User Id=student;
                        Password=Student.Bad.password.0;
                        Database=erik.illaste_hwdbdemo1;
                        MultipleActiveResultSets=true;
                        "
                );
        }
    }
}