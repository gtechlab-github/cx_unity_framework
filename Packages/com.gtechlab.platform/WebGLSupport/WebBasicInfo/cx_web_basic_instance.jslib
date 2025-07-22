// Unity WebGL 호환성을 위한 함수 정의
//var UTF8ToString = UTF8ToString || Pointer_stringify;

mergeInto(LibraryManager.library, {
    
    hello: function () {
        window.alert("cxGameInstance Hello, world!");
    },

    setFullScreen : function(on) {
        cxGameInstance.SetFullscreen(on ? 1 : 0);
    },

    closeWindow : function () {
        window.close();
    },
    
    setCookie: function (cname, cvalue, exdays) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        //document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
        document.cookie = UTF8ToString(cname) + "=" + UTF8ToString(cvalue) + expires + ";path=/";

        console.log('set cookie=' + document.cookie);
    },

    getCookie: function (cname) {
        var ret = "";
        //var name = cname + "=";
        var name = UTF8ToString(cname) + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        console.log('get cookie=' + decodedCookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                ret = c.substring(name.length, c.length);
                break;
            }
        }
        var buffer = _malloc(lengthBytesUTF8(ret) + 1);
        writeStringToMemory(ret, buffer);
        return buffer;
    },

    installEventCallback: function () {

        window.cxCallback = function(eventName, jsonParameters) {
            cxGameInstance.SendMessage('cxWebBasicInstance', 'OnEventCallback',eventName, jsonParameters);
        }

        parent.window.cxCallback = function(eventName, jsonParameters) {
            cxGameInstance.SendMessage('cxWebBasicInstance', 'OnEventCallback',eventName, jsonParameters);
        }

        console.log('cxWebInstance: installCallback completed')
    },

    isMobileDevice : function() {
        return UnityWebGLTools.isMobileDevice()
    },

     isIPhoneDevice : function() {
        return UnityWebGLTools.isIPhoneDevice()
    }
});