//tripsController.js
(function() {

    "use strict";

    // angular.module with 1 param gets the existing module created before
    // Create a controller, it's name, and the function that will actually hold the body of the code
    // returns an object on which we can add our controller
    angular.module("app-trips")
        .controller("tripsController", tripsController);

    // function that is the actual tripsController (body of code)
    function tripsController() {
        
     	// expose some data from the controller (could use this which calls and return a new tripController and returns a prototypical class)
        // declaing this as vm to be more descriptive as a view model
        var vm = this;

		// construct an array of objects called vm.trips and add properties
        vm.trips = [
            {
                name: "US Trip",
                created: new Date()
            },
            {
                name: "World Trip",
                created: new Date()
            }
        ];

        // represent an object that is going to accept all the data about a new trip
        vm.newTrip = {};   // object literal for obj with no props

        //called when user click on the add trip btn. set with ng-submit on form tag with function name
        vm.addTrip = function () {
            // add a new trip to our object, push to collection. Angular auto adds to page as add new trips in text box. Not sent to DB yet.
            vm.trips.push({ name: vm.newTrip.name, created: new Date() });
            // makes newTrip a blank object. tells browser the data is gone so clear the form
            vm.newTrip = {};
        };
    }

})();