// Module.CallManagedCallback_1 = function (callbackId, jsonString) {
//   Module.ccall(
//     'JSCallbackWithString', // C#에서 정의한 콜백 함수 이름
//     null, // 반환 타입 (null이면 반환값 없음)
//     ['number', 'string'], // 전달 인수 타입 (number: int, string: 직렬화된 JSON)
//     [callbackId, jsonString] // 전달할 값
//   );
// };

// Module.CallManagedCallback_2 = function (callbackId, jsonString) {

//     var buffer = stringToNewUTF8(jsonString);
//      {{{ makeDynCall('vii', 'JSCallbackWithString') }}} (callbackId, buffer);
//       _free(buffer);
// };

// Module.CallManagedFallback_1 = function (callbackId, jsonString) {
//   Module.ccall(
//     "JSFallbackWithString", // C#에서 정의한 콜백 함수 이름
//     null, // 반환 타입 (null이면 반환값 없음)
//     ["number", "string"], // 전달 인수 타입 (number: int, string: 직렬화된 JSON)
//     [callbackId, jsonString] // 전달할 값
//   );
// };

Module.onRuntimeInitialized = function() {
    console.log("WebAssembly module initialized.");

  Module.CallManagedCallback = function (callbackId, jsonString) {
    var param = {
      callbackId: callbackId,
      jsonString: jsonString,
    };

    window.unityInstance.SendMessage(
      "cxWebCallbackInstance",
      "JSCallbackWithString",
      JSON.stringify(param)
    );
  };

  Module.CallManagedFallback = function (callbackId, jsonString) {
    var param = {
      callbackId: callbackId,
      jsonString: jsonString,
    };

    window.unityInstance.SendMessage(
      "cxWebCallbackInstance",
      "JSFallbackWithString",
      JSON.stringify(param)
    );
  };
}
