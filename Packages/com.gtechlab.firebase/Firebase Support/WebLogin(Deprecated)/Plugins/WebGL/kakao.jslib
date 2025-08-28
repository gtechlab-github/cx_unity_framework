mergeInto(LibraryManager.library, {

    signInWithKakao: function () {
         KakaoLib.signInWithKakao();
    },

    signOutKakao: function () {
        KakaoLib.signOut();
    },

    hasAuthSessionKakao: function () {
        KakaoLib.getCurrentUser();

        // var user = KakaoLib.getCurrentUser();
        //  if (user) {
        //       var jstr = JSON.stringify(user);
        //       var bufferSize = lengthBytesUTF8(jstr) + 1;
        //       var buffer = _malloc(bufferSize);
        //       stringToUTF8(jstr, buffer, bufferSize);
        //       return buffer;
        //  } else {
        //       return null;
        //  }
    }
});