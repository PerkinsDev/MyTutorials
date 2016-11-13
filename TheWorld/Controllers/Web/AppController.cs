﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    // Make sure this is public. Add the INherit from Controller class. Add dependancy in project.json. Or let VS do it for you. Add using Mvc to top
    public class AppController : Controller
    {
        // Get in the constructor but store at class level so we can use it
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private IWorldRepository _repository;
        private ILogger<AppController> _logger;

        // Dependency Injection
        // Using the Controller so when Controller is generated by a request, it Tries to see if it can fullfill what is in the contstructor
        // Instantiates AppController and passes in an implementation of the INterface so code will work. Then can use in other methods here
        // Replaced injection of context to the Interface of context/query repository
        public AppController(IMailService mailService,
            IConfigurationRoot config,
            IWorldRepository repository,
            ILogger<AppController> logger)
        {
            _mailService = mailService;
            _config = config;
            _repository = repository;
            _logger = logger;

        }

        // Need an action ( a method that is going to return a view) returns an Interface called IActionResult
        public IActionResult Index() // No params...just for the home page.
        {
            return View();
        }

        [Authorize]   // check to see if authenitcated. if not attempts to send them to a page to do so
        public IActionResult Trips()
        {
            //try
            //{
            // Get some data. will go to the DB and get list of all the Trips - as those Trip classes(Trip Objects)
            // _context goes out into a DB someplace and converts the line _context.Trips.ToList(); to a query . Changed to interface and repository
            // Will throw error if no DB connected
            // Update: trip was removed- now accessed thorugh api and client-side

            
                // Update: trip was removed- now accessed thorugh api and client-side
                // Pass the data object to the View (List of Trip Objects here)
                return View();     // go find a view, render it, and return it to the user. Need to build actual view
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Failed to get trips in Index page: {ex.Message}");     // Dollar sign interpolation
            //    return Redirect("/error");
            //}
        }

        // Default behavior of an IActionResult is GET. This is a GET
        public IActionResult Contact()
        {
            //throw new InvalidOperationException("Bad things happen to good developers");  // test exceptopn

            return View();
        }

        // POST. if someone posts to the action that is Contact. [HttpPost] - This is the method that should be instantiated.
        // Parameter says that we are Expecting to have the data passed to it as well (with modelbinding)
        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            // Custom validation check and error. Just as example
            if (model.Email.Contains("aol.com"))
            {
                // no value "" enable val sum instead of 1 field
                ModelState.AddModelError("", "We don't support AOL addresses"); 
            }


            // if server side errors..fall through and return them to View
            if (ModelState.IsValid)
            { 
            // Dependency INjection: now we can call the SendMail method. (Name, From,Subject Body, (hard coded name)
            // This shows up in Output Window. config path is the Json value pairs
            _mailService.SendMail(_config["MailSettings:ToAddress"], model.Email, "From TheWorld", model.Message);
                ModelState.Clear(); 
                ViewBag.UserMessage = "Message Sent";
            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
