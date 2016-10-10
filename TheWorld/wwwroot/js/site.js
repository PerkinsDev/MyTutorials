// site.js
// jQuery exposes a single object to the global scope for use anywhere in these javascript files.
// global object is called jQuery (or $)

(function () {

    ////EXAMPLE jQuery
    var ele = $("#username");  // changing the user name of picture with jQuery (enables cross browser in DOM)
    ele.text("Andy Perkins");
    //Real practical example: Show / Hode sidebar by clicking a button
    // optional "$" in vari name to indicate a wrapped set
    var $sidebarAndWrapper = $("#sidebar, #wrapper"); //css and selector. returns a wrapped set of DOM elements. assign to vari
    var $icon = $("#sidebarToggle i.fa");   //jQuery can find this. Id, italic element, with class of fa (font-awesome)

    // uses "on" on the return. usually if only using once
    $("#sidebarToggle").on("click", function () {
        $sidebarAndWrapper.toggleClass("hide-sidebar"); // sets class on /off
        if ($sidebarAndWrapper.hasClass("hide-sidebar")) {
            $icon.removeClass("fa-angle-left");
            $icon.addClass("fa-angle-right");
        } else {
            $icon.addClass("fa-angle-left");
            $icon.removeClass("fa-angle-right");
        }
    });



    ////EXAMPLE jQuery // BELOW IS A REPLACEMENT FOR THE JS BELOW
    //var ele = $("#username");  // changing the user name of picture with jQuery (enables cross browser in DOM)
    //ele.text("Andy Perkins");


    ////// Events   THIS WAY FROM VID DID NOT WORK?????
    ////var main = $("#main");
    ////// 1st param contains data but last param is almost always a callback (callback functions)
    ////main.on("mouseenter", function () {
    ////    main.style = "background-color: #888;";   // assigning actuall css now not js
    ////});

    ////main.on("mouseleave", function () {
    ////    main.style = "";
    ////});

    //// THIS WAY DOES WORK???
    //$("#main").mouseover(function () {
    //    $("#main").css("background-color", "#888");
    //});

    //$("#main").mouseleave(function () {
    //    $("#main").css("background-color", "");
    //});

    //var menuItems = $("ul.menu li a"); //access the dom menu items and set to a vari . 
    ////wire up to each click event of each element
    //menuItems.on("click", function () {
    //    var me = $(this); // look at the object inside this event handler (this)
        
    //    alert(me.text());
    //});
})(); // -() actually executes the full function could supply parameters



// global scope (the window level in js) allows js files to overite each other as they are loaded. 
// to prevent overwriting use functions which provide their own scope (self-executing anonymous function / immediatly invoked function-expression)
// which wrap all code in parenthesis and exe with () at end. OUTSIDSE OF GLOBAL SCOPE

//(function () {
//    var ele = document.getElementById("userName");  // changing the user name of picture
//    ele.innerHTML = "Andy Perkins";

//    var main = document.getElementById("main");
//    // unnamed or anonymous  function assigned to the handler (callback functions)
//    main.onmouseenter = function () {
//        main.style.backgroundColor = "#888";
//    };

//    main.onmouseleave = function () {
//        main.style.backgroundColor = "";
//    };
//})(); // -() actually executes the full function could supply parameters