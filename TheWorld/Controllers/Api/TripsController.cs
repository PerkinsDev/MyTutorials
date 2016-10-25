using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    // Base route for entire class. Methods can now be "" because base roue is set
    //adding another level in methods below APPENDS to base route [HttpGet("/foo")] == api/trips/foo
    [Authorize]
    [Route("api/trips")]
    public class TripsController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<TripsController> _logger;

        // Have system inject an ILogger for our class to log errors. Pass in Class name
        public TripsController(IWorldRepository repository, ILogger<TripsController> logger)
        {
            _repository = repository;
            _logger = logger;  // with logger obj. it now knows which class it is being logged from 
        }

        [HttpGet("")]
        public IActionResult Get()
        //public JsonResult Get() replace with IActionResult so we can get errors. Will need to be returned as Json though
        {
            try
            {
                var results = _repository.GetTripsByUsername(User.Identity.Name);   // change from GetAllTrips to get individual user. Get the current users uksername by drilling down. Can use to query the trips for that user
                // Add Mapping to this method (also maps collections as needed here)
                // Tell it to return (expect a collection) of IEnumerable - (change from Ok(_repository.GetAllTrips());)
                return Ok(Mapper.Map<IEnumerable<TripViewModel>>(results));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all Trips: {ex}");  // dev log: $ and {ex} put it all together

                return BadRequest("Error occurred");  // send back generic message to user
            }
        }

        // Implement POST. Match the Post method and the URI. Ok returns a status code 200 (ok)
        // FromBody attribute (to the body of post) to tell it to model bind data coming in with post to this object
        // by trying to match up the name of properties of json to the properties of this object
        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody] TripViewModel theTrip)  // wrap the action in async
        {
            if (ModelState.IsValid)
            {
                // Save to Database
                var newTrip = Mapper.Map<Trip>(theTrip);

                // Makes assumption that user is authenticated to get this far so we can use the username to post to DB
                newTrip.UserName = User.Identity.Name;

                // need to set in startup.cs because it assumes they are already mapped (but not)
                _repository.AddTrip(newTrip); // AddTrip needs to be built in the repository

                if (await _repository.SaveChangesAsync())
                {
                    return Created($"api/trips/ {theTrip.Name}", Mapper.Map<TripViewModel>(newTrip));
                }
            }

            return BadRequest("Failed to save changes to the database");


        }
    }
}
