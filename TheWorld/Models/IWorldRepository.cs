using System.Collections.Generic;
using System.Threading.Tasks;


namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetTripsByUsername(string username);
        Trip GetTripByName(string tripName);
        Trip GetUserTripByName(string tripName, string username);


        // Could actually stored ViewModels in Repository also. Some people Do. Judgement call
        // takes an actual trip object. Sends trip obj to the underlying context. Void Doesn't need curly braces
        void AddTrip(Trip trip);
        void AddStop(string tripName, Stop newStop, string username);

        // async to db to reduce user lag. bool allows to test if T/F and then save
        // bool that returns a task...does not like async modifier keyword
        Task<bool> SaveChangesAsync();

       
    }
}