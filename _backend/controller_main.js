init(tryEstablishConnection);

function onUnityInstanceLoaded(unityInstance) {
	window.receiveDataFromUnity = receiveDataFromUnityHandler;
    window.unityInstance = unityInstance;
	tryEstablishConnection();
}

function tryEstablishConnection() {
	if (window.vkParams != null && window.unityInstance != null) {
		let is_ios = (/iPhone|iPad|iPod/i.test(navigator.userAgent));
		let data = {
			platform: "vk",
			is_ios_direct: is_ios,
			viewer_id: window.vkParams.viewer_id
		};
		sendCommandToUnity('SetVkPlatformData', data);

		proceedWithPermissions("friends", getAppFriendsSequence);
	}
}

function getAppFriendsSequence() {
	callVKApi("friends.getAppUsers", null, r => {
		if (r.response != null && r.response.length > 0) 
		{
			var appFriendsIds = r.response;
			callVKApi("users.get", { user_ids:appFriendsIds.join(","), fields: "first_name,last_name,photo_50,photo_100,online,last_seen", order:"random" }, r => {
				var appFriends = [];
				if (r.response != null && r.response.length > 0)
				{
					appFriends = r.response;
					for (var i = 0; i < appFriends.length; i++) {
						appFriends[i].is_app = true;
					}
				}
				sendCommandToUnity('SetVkFriendsData', appFriends);
			});
		} else {			
			sendCommandToUnity('SetVkFriendsData', []);
		}
	});
}

function getUsersData(userIdsStr) {
	callVKApi("users.get", { user_ids:userIdsStr, fields: "first_name,last_name,photo_50,photo_100,online,last_seen", order:"random" }, 
		r => sendCommandToUnity('GetUsersDataResponse', r.response));
}

function sendCommandToUnity(command, data) {
    window.unityInstance.SendMessage('JsBridge', 'JsCommandMessage', JSON.stringify({command:command, data:data}));
}

function receiveDataFromUnityHandler(data) {
	console.log("OnReceiveDataFromUnity!, data:" + data);
	var parsedData = JSON.parse(data);
	processCommandFromUnity(parsedData.command, parsedData.payload);
}

function processCommandFromUnity(command, payload) {
	switch(command) {
		case "NotifyInactiveFriend":
			sendSocialRequest(payload.uid, "Давно не видно тебя в магазинчике. Возвращайся! А я зайду в гости :)");
		break;
		case "InviteFriends":
			showInviteBox();
		break;
		case "BuyMoney":
			window.chargedProductId = payload.product;
			showOrderBox(payload.product, onOrderBoxCallback);
		break;
		case "LevelUp":
			sendRequest(
				"https://holybit.space/marketVK/unity/vk/VKDataReceiver.php?command=push_level&id=" + window.vkParams.viewer_id + "&level=" + payload.level + "&time=" + (new Date()).getTime());
		break;
		case "PostNewLevel":
			postNewLevel(payload.level);
		break;
		case "PostOfflineRevenue":
			postOfflineRevenue(payload.hours, payload.minutes, payload.revenue);
		break;
		case "ShowAds":
			showNativeAds(result =>	sendCommandToUnity('ShowAdsResult', { is_success: result }));
		break;
		case "GetUsersData":
			getUsersData(payload.uids.join(","));
		break;
	}
}

function onOrderBoxCallback(result) {
	sendCommandToUnity('BuyVkMoneyResult', { is_success:result.is_success, is_user_cancelled:result.is_user_cancelled, order_id:window.chargedProductId });
}

function sendRequest(url, callback = null) {
	const http = new XMLHttpRequest();
	http.open("GET", url);
	http.send();
	if (callback != null) {		
		http.onload = () => callback(http.responseText);
	}
}

function postNewLevel(level) {
	var str = "Прокачал свой магазин до " + level + "-го уровня!\nДогоняйте!";
	switch(level) {
	    case 2:
			str = "Еще чуть-чуть, и стану супермаркетом!\nВторой уровень - это вам не палатка с шаурмой :)";
		break;
		case 3:
			str = "Бизнес растет как на дрожжах!\nТретий уровень - уже появляются постоянные клиенты!";
		break;
		case 4:
			str = "Конкуренты? Не, не слышали :)\nМой магазин уже на четвертом уровне!";
		break;
		case 5:
			str = "Магазинчик пятого уровня!\nКогда-нибудь видели что-то подобное? :)";
		break;
		case 6:
			str = "Это уже серьёзный бизнес!\nМой магазин уже на шестом уровне!";
		break;
		case 7:
			str = "Спасибо друзьям, клиентам и бизнес партнерам!\nМы сделали это! Седьмой уровень!";
		break;
		case 8:
			str = "Мне всегда говорили, что я талантливый руководитель!\nМой магазин развился уже до восьмого уровня!";
		break;
		case 9:
			str = "Этот супермаркет знают все вокруг!\nМой магазинчик настолько популярен, что добрался до девятого уровня!";
		break;
		case 10:
			str = "Это давно уже торговый центр, а не магазинчик :)\nПриглашаю всех посетить мой магазин десятого уровня!";
		break;
		case 11:
			str = "Все ходят за покупками только сюда!\nПриглашаю всех посетить мой магазин одиннадцатого уровня!";
		break;
		case 12:
			str = "У меня талант быть директором магазина!\nМой супермаркет уже на 12 уровне!";
		break;
	}
	wallPost(str, () => sendCommandToUnity('VkWallPostSuccess'));
}

function postOfflineRevenue(hours, minutes, revenue) {
	var timePassedStr = ((hours >= 1) ? (parseInt(hours) + " ч.") : (minutes +" мин."));
	wallPost("Мой магазинчик принес мне " + revenue + "$ всего за " + timePassedStr + "!\nА ваш магазинчик так сможет? :)", () => sendCommandToUnity('VkWallPostSuccess'));
}

//{"api_url":"https://api.vk.com/api.php","api_id":"4995114","api_settings":"8455","viewer_id":"48982",
//"viewer_type":"0","sid":"2dcd4aafa1575def2e4c9d3c5629e00b824b7e113482d224f0eb22746b7431dea9ef606faee73e269c9ea",
//"secret":"94836c36ab","access_token":"e1c0e3e7ad550705f2325d465a0c2e22448fd174a38ca546dc89881787d0f717e53eb19c267925144fed0",
//"user_id":"0","is_app_user":"1","language":"0","parent_language":"0","is_secure":"1","stats_hash":"0f6038c9a92a05c47d",
//"group_id":"0","ads_app_id":"4995114_6d2e2a13c4ce8e81d7","access_token_settings":"notify,friends,photos,menu,wall",
//"referrer":"unknown","lc_name":"2d629194","platform":"web","is_widescreen":"0",
//"whitelist_scopes":"friends,photos,video,stories,pages,status,notes,wall,docs,groups,stats,market,ads,notifications",
//"group_whitelist_scopes":"stories,photos,app_widget,messages,wall,docs,manage",
//"auth_key":"fbe8f7b5f4668d430a71b6608b1d4306","timestamp":"1629136547","sign":"ybAJ4yRuKLawG4c2TWOu3KJQFUM1LdFNpZT5KRUyXcM",
//"sign_keys":"access_token,access_token_settings,ads_app_id,api_id,api_settings,api_url,auth_key,group_id,group_whitelist_scopes,is_app_user,is_secure,is_widescreen,language,lc_name,parent_language,platform,referrer,secret,sid,stats_hash,timestamp,user_id,viewer_id,viewer_type,whitelist_scopes",
//"hash":""}

function init(callback) {
	vkBridge
	  .send('VKWebAppInit')
	  .then(data => {
	  	var queryDict = {};
	  	location.search.substr(1).split("&").forEach(function(item) {queryDict[item.split("=")[0]] = item.split("=")[1]});
	 	window.vkParams = queryDict;

	  	callback();
	  	var viewerId = window.vkParams["viewer_id"];
	  	processNotifications(viewerId);
	  })
	  .catch(error => {
		console.log(JSON.stringify(error));
	  });
}

function processNotifications(viewerId) {
	var path = "https://holybit.space/marketVK/unity/vk/";
	sendRequest(path + "VKNotificationsProcessor.php?command=reset&id="+viewerId, function (result) {
		console.log("Notification reset response:" + result);
		sendRequest(path + "VKNotificator.php?command=notify&id="+viewerId, function(notificationResult) {
			console.log("Notify response:" + notificationResult);
		})
	});
}

function callVKApi(methodName, params, callback) {
	if (params == null) params = {};

	var accessToken = window.access_token != null ? window.access_token : window.vkParams.access_token;
	params.v = "5.131";
	params.access_token = accessToken;
	vkBridge.send("VKWebAppCallAPIMethod", {"method": methodName, "request_id": new Date().toISOString(), "params": params})
	.then(data => {
		if(callback != null) {
			callback(data);
		}
	})
	.catch(error => {
		console.log(JSON.stringify(error));
	});
}
function showInviteBox() {
	vkBridge.send("VKWebAppShowInviteBox", {})
         //.then(data => alert(data.success))
		.catch(error => {
			console.log(JSON.stringify(error));
		});
}
function showOrderBox(item, callback) {
	vkBridge.send("VKWebAppShowOrderBox", {type:"item",item:item})
        .then(data => callback({ is_success:true }))
        .catch(error => {
        	var isUserCancelled = (error != null && error.error_data != null && error.error_data.error_code == 4);
        	callback({ is_success:false, is_user_cancelled:isUserCancelled });
        });
}
function wallPost(message, successCallback) {
	proceedWithPermissions("wall",() => wallPostInternal(message, successCallback));
}
function wallPostInternal(message, successCallback) {
	vkBridge.send("VKWebAppShowWallPostBox", {"message": message, "attachments":"https://vk.com/app4995114_48982", "v":"5.131"})
		.then(data => {
				if (data != null && data.post_id != null) {					
					successCallback();		
				}
			})
		.catch(error => {
			alert(JSON.stringify(error));
			console.log(JSON.stringify(error));
		});
}
function proceedWithPermissions(permissionsStr, callback) {
	var appId = parseInt(window.vkParams.api_id);
	var splittedPermissions = permissionsStr.split(",");
	vkBridge.send("VKWebAppGetAuthToken", {"app_id": appId, "scope": permissionsStr})
		.then(data => {
			var allPermissions = true;
			if(data.scope != null) {				
				for (var i = 0; i < splittedPermissions.length; i++) {
					allPermissions &= data.scope.indexOf(splittedPermissions[i]) >= 0;
				}
			}

			if (allPermissions == true) {
				window.access_token = data.access_token;
				callback();
			}
		})
		.catch(error => {
			console.log(JSON.stringify(error));
		});;
}
function showNativeAds(callback) {
	vkBridge.send("VKWebAppShowNativeAds", { ad_format:"reward", use_waterfall: true })
	.then(data => (data != null && data.result === true) ? callback(true) : callback(false))
	.catch(error => callback(false));
}
function sendSocialRequest(friendUId, message, callback = null) {
	vkBridge.send("VKWebAppShowRequestBox", {uid:parseInt(friendUId), message: message, requestKey:"default_key"})
        .then(data => { if (callback != null) callback(data.success); })
        .catch(error => { if (callback != null) callback(false); });
}