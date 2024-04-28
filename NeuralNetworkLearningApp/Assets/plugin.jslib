var plugin = {
    // A simple function that logs a message to the browser console
    SetCookie: function(cookieString) {
		console.log("Cookie string " + UTF8ToString(cookieString));
        document.cookie = UTF8ToString(cookieString);
		
		
		console.log(document.getElementsByName('fun')[0].value);
		//CheckQuestionnairVisibility();
    },
};



mergeInto(LibraryManager.library, plugin);