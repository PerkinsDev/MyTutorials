using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TheWorld.Models
{
    // Repository gives a single place of entry for all the queries (although some may be in other places)
    // Can be mocked-up to test the controller against this repository or a fake rep.
    // Convert to an Interface - ctr + .   : Interface. Might fail to put in proper place. If at root, move to Models dir
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Implemented from the INterface. Still not saving to DB. Just pushing into the context as a new obj


        public void AddTrip(Trip trip)
        {
            _context.Add(trip);
        }

        // Implemented from the INterface. Still not saving to DB. Just pushing into the context as a new obj
        public void AddStop(string tripName, Stop newStop, string username)
        {
            // get the actual trip. make sure trip belongs to the logged in user
            var trip = GetUserTripByName(tripName, username);

            if (trip != null)   // if exists
            {
                // Need both to happen since stop is a related entity to be saved correctly
                trip.Stops.Add(newStop); // add new stop, but just setting the Foreign Key to trips.Stops
                _context.Stops.Add(newStop);    // actual push to add to EF as a new obj
            }
        }

        // New method to return a collection of Trips
        public IEnumerable<Trip> GetAllTrips()
        {
            _logger.LogInformation("Getting all trips from the database");
            return _context.Trips.ToList();
        }



        // Include = Eager load collection of stops (as a property) and add it to the trip when it is returned
        public Trip GetTripByName(string tripName)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .FirstOrDefault(t => t.Name == tripName);
        }

        public IEnumerable<Trip> GetTripsByUsername(string name)
        {
            //Add where clause
            return _context
                .Trips
                .Include(t => t.Stops)
                .Where(t => t.UserName == name)
                .ToList();
        }

        public Trip GetUserTripByName(string tripName, string username)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .FirstOrDefault(t => t.Name == tripName && t.UserName == username);

            // Video way. It appears FirstOrDefault takes a where clause
            //return _context.Trips
            //   .Include(t => t.Stops)
            //   .Where(t => t.Name == tripName && t.UserName == username)
            //   .FirstOrDefault();
        }




        // Actual Save to DB
        // returns a task that wraps a bool so can use async
        public async Task<bool> SaveChangesAsync()
        {
            // want operation to be awaited but want the result as bool to compare
            // wrapped in bool because SaveChangesAsync return integer of rows affected
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
