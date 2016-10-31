//tripsController.js
(function() {

    "use strict";

    // angular.module with 1 param gets the existing module created before
    // Create a controller, it's name, and the function that will actually hold the body of the code
    // returns an object on which we can add our controller
    angular.module("app-trips")
        .controller("tripsController", tripsController);

    // function that is the actual tripsController (body of code)
    function tripsController($http) {
        
     	// expose some data from the controller (could use this which calls and return a new tripController and returns a prototypical class)
        // declaing this as vm to be more descriptive as a view model
        var vm = this;

		// construct an array of objects called vm.trips and add properties
        vm.trips = [

            //{
            //    name: "US Trip",
            //    created: new Date()
            //},
            //{
            //    name: "World Trip",
            //    created: new Date()
            //}
        ];

        // represent an object that is going to accept all the data about a new trip
        vm.newTrip = {};   // object literal for obj with no props

        // create member of the vm that represent non-functional. way to show, if that data exists (if failure below)
        vm.errorMessage = "";
        vm.isBusy = true;

        // using angular service to talk to server. injected into the function at runtime much like ASP>Net constructor injection
        // call (i.e. get, post) is matched to the server verb. match up
        $http.get("/api/trips")
            // a promise object is returned ( object where we can immediately call "then" to specifiy the callback for success and failure
            
            .then(function(response) {
                // Success. include the respoinse as parameter which returns object that has information. 
                // data is the payload we got from the server. Has already been converted from Json to an obj for us
                // shortcut for a foreach (angular object copy
                    angular.copy(response.data, vm.trips); 
                },
                function (error) {
                    // failure
                    vm.errorMessage = "Failed to load data: " + error;
                })
             // fluent syntax: (.something on the entire function (http.get etc)
             // elegant way instead of writing vm.isBusy = false once in each function
        .finally(function() {   
            vm.isBusy = false;
            });



        //called when user click on the add trip btn. set with ng-submit on form tag with function name
        vm.addTrip = function () {

            vm.isBusy = true;
            // clear everytime so we don't end up withan old error mess. stuck in there
            vm.errorMessage = "";

            $http.post("/api/trips", vm.newTrip)
                .then(function( response) {
                    // Success
                    vm.trips.push(response.data);
                    vm.newTrip = {};  // clear the form
                    },
                    function() {
                        // failure
                        vm.errorMessage = "Failed to save new trip";
                    })
                .finally(function() {
                    vm.isBusy = false;
                });


            // Removed: OLD
            //// add a new trip to our object, push to collection. Angular auto adds to page as add new trips in text box. Not sent to DB yet.
            //vm.trips.push({ name: vm.newTrip.name, created: new Date() });
            //// makes newTrip a blank object. tells browser the data is gone so clear the form
            //vm.newTrip = {};
        };
    }

})();