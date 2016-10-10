using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TheWorld.Models
{
    // Class we can execute linq queries against.
    public class WorldContext : DbContext
    {
        private IConfigurationRoot _config;

        // Constructor Injection.    
        // Must also add the param DbContextOPtions and pass to the base class before the OnConfiguing will work correctly
        public WorldContext(IConfigurationRoot config, DbContextOptions options) : base(options)
        {
            // Set config obj to class level vari(field) for use
            _config = config;
        }

        //Take each of the types of Entities we have and expose them as a property of special type DbSet
        // DbSets are starting points for queriable interfaces. Use when query DB directly
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Stop> Stops { get; set; }

        // Override method to set database options. ConString etc.
     
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Add Sql Server package and pass in the connection string from congig.json
            // which was built as a ConfigurationBuilder and added as a Singleton in Startup.cs
            // So it can be used throughout the application
            optionsBuilder.UseSqlServer(_config["ConnectionStrings:WorldContextConnection"]);
        }
    }
}
