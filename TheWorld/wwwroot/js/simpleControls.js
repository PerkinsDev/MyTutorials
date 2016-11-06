// simpleControls.js
(function() {
    "use strict";

    angular.module("simpleControls", [])
        .directive("waitCursor", waitCursor);

    // directive returns an object and can define the properties here
    function waitCursor() {
        return {
            // scope = a structre/obj we are binding our waitcurspor to. way to map our new attrib. over to the template
            scope: {
                show: "=displayWhen"   // names don't have to match but show is used in the waitCursor.html template as ng-show="show"
            },
            restrict: "E",
            templateUrl: "/views/waitCursor.html"  // client side so it goes in wwwroot
        };
    }

})();