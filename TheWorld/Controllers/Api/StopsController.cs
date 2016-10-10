using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    // Inherit from Controller to make it a controller
    // Class level route is the trip name and its stops
    // Other controller (tripcontroller) gets the trip. Trip name passed in as parameter
    [Route("/api/trips/{tripName}/stops")]
    public class StopsController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<StopsController> _logger;
        private GeoCoordsService _coordsService;

        // Constructor- similar to Trips with repository and a logger for this class
        public StopsController(IWorldRepository repository, 
            ILogger<StopsController> logger,
            GeoCoordsService coordsService)
        {
            // Assign as loval variables --> refactor to auto gen fields
            _repository = repository;
            _logger = logger;
            _coordsService = coordsService;
        }

        // Actions
        // Get - return the stops for a specific trip based on Association. route is the trip name and its stops
        // Other controller (tripcontroller) gets the trip. Trip name passed in as parameter
        [HttpGet("")]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetTripByName(tripName); // GetTripByName is added to repository

                // Configure in startup is needed
                // Map collection of stops to a collectio of stopviewmodelsReturn class of stopview model instead of a raw stop
                return Ok(Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.OrderBy(s => s.Order).ToList()));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get stops: {0}", ex);
            }
            return BadRequest("Failed to get stops");

        }

        [HttpPost("")]
        public async Task<IActionResult> Post(string tripName, [FromBody] StopViewModel vm)
            // accept from the body of page a stopviewmodel
        {
            try
            {
                // If the VM is valid
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);

                    // Look up GeoCodes
                    var result = await _coordsService.GetCoordsAsync(newStop.Name);
                    // Checking properties from service
                    if (!result.Success)
                    {
                        _logger.LogError(result.Message);
                    }
                    else // set the obj in use to the results of the service
                    {
                        // now when go to save changes it will include lat and long
                        newStop.Latitude = result.Latitude;
                        newStop.Longitude = result.Longitude;
                    }

                    // Save to the DB.
                    _repository.AddStop(tripName, newStop);  // Addstop gets added to repository. can use refactoring

                    if (await _repository.SaveChangesAsync())
                    {
                        // Created is result of a post when you successfully created new obj
                        return Created($"/api/trips/{tripName}/stops/{newStop.Name}",
                            Mapper.Map<StopViewModel>(newStop)); // mapper to convert back to a Stop VM  
                    }
                }
            }
            catch (Exception ex)
            {
                    
                _logger.LogError("Failed to save a new Stop: {0}", ex);
            }
            return BadRequest("Failed to save new stop");
        }
    }
}
