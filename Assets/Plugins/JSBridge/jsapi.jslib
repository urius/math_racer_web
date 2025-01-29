mergeInto(LibraryManager.library, {
  Hello: function () {
    console.log("function Hello");
    window.alert("Hello, world!");
  },
  HelloString: function (str) {
    window.alert(UTF8ToString(str));
  },
  SendToJs: function (str) {  
    console.log("calling window.receiveDataFromUnity: " + UTF8ToString(str));
    window.receiveDataFromUnity(UTF8ToString(str));
  }
});