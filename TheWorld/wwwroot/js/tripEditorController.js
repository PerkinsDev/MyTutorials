// tripEditorController.js
(function() {
    "use strict";

    angular.module("app-trips")
        .controller("tripEditorController", tripEditorController);

    // include  a param inside the route function called $routePOarams
    // allows us to add tripName to controller or viewmodel
    // extends angular routeParams with whatever props it found in our route
    function tripEditorController($routeParams, $http) {
        var vm = this;

        vm.tripName = $routeParams.tripName;
        vm.stops = [];
        vm.errorMessage = "";
        vm.isBusy = true;

        $http.get("/api/trips/" + vm.tripName + "/stops")
            .then(function(response) {
                //success
                angular.copy(response.data, vm.stops);
                _showMap(vm.stops);
            }, function (err) {
                // failure
                vm.errorMessage = "Failed to load stops";
            })
        .finally(function() {
                vm.isBusy = false;
            });
    }

    // underscore shows that it is a private function only goint ot be used in this file
    // take the stops we have and draw them on the map
    function _showMap(stops) {
        // check that it exists and is not empty (valid)
        if (stops && stops.length > 0) {
            
            var mapStops = _.map(stops, function(item) {
                return {
                    lat: item.latitude,
                    long: item.longitude,
                    info: item.name
                };
            });  // the _ is the variable that contains the entire underscore lib. used for underscore to allow global scope. map if function of u.s. to map types

            // Show map using our travelmap api. Takes an obj literal that takes some overrides
            travelMap.createMap({
                stops: mapStops,
                selector: "#map",       // id w/ css selector
                currentStop: 1,         // which stop to high;ight
                initialZoom: 3          // view of entire world
            });
        }
        
    }

})();