// app-trips.js
// main angularJS routing system
// take out of global scope with immediately invoking function expression
(function () {

    "use strict";
    // angular.module with 2 params creates the actual module which we use in other js files
    // where we define a packet of code for angular. Creates a module that we can then specify in the trips where to get this code
    // need to tell this main module to expect and inject our simple controls.js file to access our custom directives /array is how dependencies are managed
    // ng-route is the name of the module. Once added we can config cliet side routes. 
    // do this by handling the config method of the module objecct that is returned
    // takes a callback function for when configuration needs to happen. 
    // Use param injection to include prop $routeProvider
    angular.module("app-trips", ["simpleControls", "ngRoute"])  // watch for syntax variation from ngRoute here to ng-route
        .config(function ($routeProvider) {

            //when: 1st param: client side route we're looking for
            $routeProvider.when("/", 
            {
                controller: "tripsController", // define a controller for that indiviaual view
                controllerAs: "vm", // alias for databinding
                templateUrl: "/views/tripsView.html" // path to the html that represent that actual view });
            });

            // 2nd route
            //:tripName represents a variable in the tripEditor that we will pull out of the page to be used as the name for the trip
            $routeProvider.when("/editor/:tripName", {
                controller: "tripEditorController",
                controllerAs: "vm",
                templateUrl: "/views/tripEditorView.html"
            });

            // if none of routes match: otherwise
            $routeProvider.otherwise({ redirectTo: "/" });

        });

})();