function getActiveBuildMetaData() {
    var buildName = "release_3";

    var buildUrl = "../Build";
    var loaderUrl = buildUrl + "/" + buildName + ".loader.js";
    var config = {
      dataUrl: buildUrl + "/" + buildName + ".data.unityweb",
      frameworkUrl: buildUrl + "/" + buildName + ".framework.js.unityweb",
      codeUrl: buildUrl + "/" + buildName + ".wasm.unityweb",
      streamingAssetsUrl: "StreamingAssets",
      companyName: "isupgames",
      productName: "market life",
      productVersion: "1.82",
    };

    return {
    	loaderUrl: loaderUrl,
    	config: config
    };
}
