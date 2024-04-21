var plugin = {
    // A simple function that logs a message to the browser console
    SetCookie: function(cookieString) {
		console.log("Cookie string " + UTF8ToString(cookieString));
		window.alert(UTF8ToString(cookieString));
        document.cookie = UTF8ToString(cookieString); 
    },
};

mergeInto(LibraryManager.library, plugin);