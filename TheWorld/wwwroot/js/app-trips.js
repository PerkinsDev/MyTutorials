// app-trips.js
// take out of global scope with immediately invoking function expression
(function () {

    "use strict";
    // angular.module with 2 params creates the actual module which we use in other js files
    // where we define a packet of code for angular. Creates a module that we can then specify in the trips where to get this code
    // need to tell this main module to expect and inject our simple controls.js file to access our custom directives /array is how dependencies are managed
    angular.module("app-trips", ["simpleControls"]); 

})();