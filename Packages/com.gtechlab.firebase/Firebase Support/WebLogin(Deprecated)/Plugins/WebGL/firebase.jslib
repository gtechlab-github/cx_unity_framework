mergeInto(LibraryManager.library, {

     signInWithGoogle: function () {
          FirebaseLib.signInWithGoogle();
     },

     signOutGoogle: function () {
          FirebaseLib.signOut();
     },

     hasAuthSessionGoogle: function () {
          FirebaseLib.getCurrentUser();

          // var user = FirebaseLib.getCurrentUser();
          // if (user) {
          //      var jstr = JSON.stringify(user);
          //      var bufferSize = lengthBytesUTF8(jstr) + 1;
          //      var buffer = _malloc(bufferSize);
          //      stringToUTF8(jstr, buffer, bufferSize);
          //      return buffer;
          // } else {
          //      return null;
          // }
     }
});