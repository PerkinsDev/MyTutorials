using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers
{
    public class AuthController : Controller
    {
        private SignInManager<WorldUser> _signInMangager;

        public AuthController(SignInManager<WorldUser> signInManager)  // takes type of user object it does the signing in for
        {
            _signInMangager = signInManager;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Trips", "App");  // Redirect to Trips page so not asked to login again if logged in
            }

            return View();
        }

        // Implement the Login() Action . Post all data to this Action in the form of a view model
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel vm, string returnUrl)  // add returnUrl to make generic and send to other pages
        {
            if (ModelState.IsValid)
            {
                // need to get sign in result using signiNmanager. persistant set true, lock out user if fail set false
                var signInResult = await _signInMangager.PasswordSignInAsync(vm.Username, 
                                                                       vm.Password, 
                                                                       true, false);
                if (signInResult.Succeeded)
                {
                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return RedirectToAction("Trips", "App");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
                else
                {
                    // Adding this model state to get show to user
                  ModelState.AddModelError("", "Username or Password is incorrect");  
                }
            }

            // if not valid return to go back to same login view to try again // also if errors
            return View();  
        }

        public async Task<ActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                // signs out and gets rid of the cookie collection
                await _signInMangager.SignOutAsync();
            }

            return RedirectToAction("Index", "App");
        }
    }
}
