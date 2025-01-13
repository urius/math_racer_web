mergeInto(LibraryManager.library, {
      JsSetTimeout: function (timeout, callback) {
          var buffer = stringToNewUTF8("test");
          setTimeout(function () {   
              {{{ makeDynCall('vi', 'callback') }}} (buffer);
               _free(buffer);
          }, timeout);
      },
      HostNewConnection: async function (callback, onChannelOpenCallback, onChannelCloseCallback, onMessageReceivedCallback) {
            console.log("host...,  iceServers.len: " + window.p2pIceServers.length);
            
            const onChannelOpen = (label) => { 
                console.log(`Connection ready! channelLabel: ` + label);
                const labelCStr = stringToNewUTF8(label);
                {{{ makeDynCall('vi', 'onChannelOpenCallback') }}} (labelCStr);
                 _free(labelCStr);
            }
            const onChannelClose = (label) => {
                console.log(`Connection closed, channelLabel: ` + label);
                const labelCStr = stringToNewUTF8(label);
                {{{ makeDynCall('vi', 'onChannelCloseCallback') }}} (labelCStr);
                _free(labelCStr);               
            }
            const onMessageReceived = (label, message) => {
                console.log(`New incoming message from ${label}: ${message}`);
                const labelCStr = stringToNewUTF8(label);
                const messageCStr = stringToNewUTF8(message);
                {{{ makeDynCall('vii', 'onMessageReceivedCallback') }}} (labelCStr, messageCStr);  
                _free(labelCStr);
                _free(messageCStr);
            }            
            const callbackAction = (channelLabel, description) => {
                const channelLabelCStr = stringToNewUTF8(channelLabel);
                const descriptionCStr = stringToNewUTF8(description);
                {{{ makeDynCall('vii', 'callback') }}} (channelLabelCStr, descriptionCStr);
                _free(channelLabelCStr);
                _free(descriptionCStr);
            }
            
            const p2pContext = await window.p2pCreatePeerConnection({
              iceServers: window.p2pIceServers,
              onMessageReceived,
              onChannelOpen,
              onChannelClose
            });

            const localDescriptionAdv_b64 = btoa(p2pContext.localDescription);
            console.log("host localDescription_b64: " + localDescriptionAdv_b64);

            callbackAction(p2pContext.channelLabel, localDescriptionAdv_b64);
      },
      JoinConnection: async function (connectionDescription, callback, onChannelOpenCallback, onChannelCloseCallback, onMessageReceivedCallback) {
            const descriptionAdv64Str = Pointer_stringify(connectionDescription);
            
            console.log("join...,  hostDescription: " + descriptionAdv64Str);
            
            const onChannelOpen = (label) => { 
                console.log(`Connection ready! channelLabel: ` + label);
                const labelCStr = stringToNewUTF8(label);
                {{{ makeDynCall('vi', 'onChannelOpenCallback') }}} (labelCStr);
                _free(labelCStr);
            }
            const onChannelClose = (label) => {
                console.log(`Connection closed, channelLabel: ` + label);                
                const labelCStr = stringToNewUTF8(label);
                {{{ makeDynCall('vi', 'onChannelCloseCallback') }}} (labelCStr);     
                _free(labelCStr);       
            }
            const onMessageReceived = (label, message) => {
                console.log(`New incoming message from ${label}: ${message}`);                
                const labelCStr = stringToNewUTF8(label);
                const messageCStr = stringToNewUTF8(message);
                {{{ makeDynCall('vii', 'onMessageReceivedCallback') }}} (labelCStr, messageCStr);  
                _free(labelCStr);
                _free(messageCStr);
            }
            const callbackAction = (channelLabel, description) => {
                const channelLabelCStr = stringToNewUTF8(channelLabel);
                const descriptionCStr = stringToNewUTF8(description);
                {{{ makeDynCall('vii', 'callback') }}} (channelLabelCStr, descriptionCStr);
                _free(channelLabelCStr);
                _free(descriptionCStr);
            }
            
            const remoteDescriptionAvd = JSON.parse(atob(descriptionAdv64Str));
            const hostChannelLabel = remoteDescriptionAvd.channelLabel;

            console.log(`remoteDescriptionAvd: ` + JSON.stringify(remoteDescriptionAvd));

            const p2pContext = await window.p2pCreatePeerConnection({
              remoteDescription: JSON.stringify(remoteDescriptionAvd.description),
              channelLabel: hostChannelLabel,
              iceServers: window.p2pIceServers,
              onMessageReceived,
              onChannelOpen,
              onChannelClose
            });

            const localDescriptionAdv64 = btoa(p2pContext.localDescription);
            console.log("joined! localDescription_b64: " + localDescriptionAdv64);
            
            callbackAction(p2pContext.channelLabel, localDescriptionAdv64);
      },
      CompleteHostConnection: function (joinedPeerDescription64) {
            const joinedPeerDescription64Str = Pointer_stringify(joinedPeerDescription64);
            
            console.log("host2 (complete host)...");

            answerDescriptionAdv = JSON.parse(atob(joinedPeerDescription64Str));
            const channelLabel = answerDescriptionAdv.channelLabel;
            const answerDescription = JSON.stringify(answerDescriptionAdv.description);

            const targetContext = window.p2pContexts.find(context => context.channelLabel === channelLabel);

            if (targetContext) {
                console.log("host2.setAnswerDescription to channelLabel: " + channelLabel);
                targetContext.setAnswerDescription(answerDescription);
            } else {
                console.error("host2.setAnswerDescription unable to find context with channelLabel: " + channelLabel);
            }
      },
      SendToAll: function (msg) {
           const msgStr = Pointer_stringify(msg);
           for (let i = 0; i < window.p2pContexts.length; i++) {
              p2pContexts[i].sendMessage(msgStr);
           }
      },
      SendTo: function (channelLabel, msg) {      
           const channelLabelStr = Pointer_stringify(channelLabel);
           const msgStr = Pointer_stringify(msg);
           
           for (let i = 0; i < window.p2pContexts.length; i++) {
               if (p2pContexts[i].channelLabel === channelLabelStr) {
                    p2pContexts[i].sendMessage(msgStr);
               };
           }
      },
      Close: function (channelLabel) {        
        const channelLabelStr = Pointer_stringify(channelLabel);   
        
        console.log("closing " + channelLabelStr);
                     
        for (let i = 0; i < window.p2pContexts.length; i++) {
          const context = p2pContexts[i];
          if (context.channelLabel === channelLabelStr) {
            context.peerConnection.close();
            return;
          }
        }        
        console.error("close() unable to find context with channelLabel: " + channelLabelStr);
      },
      SetupIceServerUrl: function(url) {
         window.p2pIceServers = [
             { urls: Pointer_stringify(url), }
         ];
      },
      InitLib: function () {
          if (window.p2pIceServers == null) {
                window.p2pIceServers = [
                    { urls: 'stun:stun.l.google.com:19302' }
                ];
          }          
                                 
          window.p2pCreatePeerConnection = 
              async function (props) {          
                 console.log("p2pCreatePeerConnection() props: " + JSON.stringify(props));
                 
                 const iceServers = props.iceServers;
                 const remoteDescription = props.remoteDescription;
                 const onChannelOpen = props.onChannelOpen;
                 const onMessageReceived = props.onMessageReceived;
                 const onChannelClose = props.onChannelClose;
                 
                 console.log("p2pCreatePeerConnection() iceServers: " + props.iceServers);
                 console.log("p2pCreatePeerConnection() JSON.stringify(props.iceServers): " + JSON.stringify(iceServers));
               
                 if (window.p2pContextsInfo == null) {
                    window.p2pContextsInfo = { totalContextsCreated:0 };
                 }
                 if (window.p2pContexts == null) {
                    window.p2pContexts = [];
                 }
               
                 const channelLabel = props.channelLabel != null ? props.channelLabel : 'P2P_CHANNEL_LABEL_' + window.p2pContextsInfo.totalContextsCreated;
                 console.log("channelLabel: " + channelLabel);
               
                 const p2pContext = { channelLabel };
                 p2pContexts.push(p2pContext);
                 window.p2pContextsInfo.totalContextsCreated++;
               
                 const peerConnection = new RTCPeerConnection({ iceServers });
                 p2pContext.peerConnection = peerConnection;
                 let channelInstance;
                 console.log("peerConnection: " + peerConnection + " iceServers.length: " + iceServers.length);
               
                 function setupChannelAsAHost() {
                        // console.log("setupChannelAsAHost");
                   
                       try {
                         channelInstance = peerConnection.createDataChannel(channelLabel);
                         console.log("channelInstance id: " + channelInstance.id + " label: " + channelInstance.label);
                   
                         channelInstance.onopen = function () {
                            //console.log("onChannelOpen channelInstance id: " + channelInstance.id + " label: " + channelInstance.label);
                           onChannelOpen(p2pContext.channelLabel);
                         };
                   
                         channelInstance.onmessage = function (event) {
                           onMessageReceived(p2pContext.channelLabel, event.data);
                         };
                   
                         channelInstance.onclose = function () {
                           onChannelClose(p2pContext.channelLabel);
                           removeContext(p2pContext);
                         };
                       } catch (e) {
                         console.error('No data channel (peerConnection)', e);
                       }
                 }
               
                 async function createOffer() {
                        // console.log("createOffer, peerConnection: " + peerConnection);
                   
                       const description = await peerConnection.createOffer();
                       //console.log("description: " + JSON.stringify(description));
                   
                       peerConnection.setLocalDescription(description);
                 }
               
                 function setupChannelAsASlave() {
                        peerConnection.ondatachannel = function ({ channel }) {
                         channelInstance = channel;
                         //console.log("channelInstance id: " + channelInstance.id + " label: " + channelInstance.label);
                   
                         channelInstance.onopen = function () {
                           onChannelOpen(p2pContext.channelLabel);
                         };
                   
                         channelInstance.onmessage = function (event) {
                           onMessageReceived(p2pContext.channelLabel, event.data);
                         };
                   
                         channelInstance.onclose = function () {
                           onChannelClose(p2pContext.channelLabel);
                           removeContext(p2pContext);
                         };
                       };
                 }
               
                 async function createAnswer(remoteDescription) {
                       await peerConnection.setRemoteDescription(JSON.parse(remoteDescription));
                       const description = await peerConnection.createAnswer();
                       peerConnection.setLocalDescription(description);
                 }
               
                 function setAnswerDescription(answerDescription) {
                        peerConnection.setRemoteDescription(JSON.parse(answerDescription));
                 }
               
                 function sendMessage(message) {
                       if (channelInstance && channelInstance.readyState === "open") {
                         channelInstance.send(message);
                       }
                 }
               
                 function removeContext(p2pContext) {
                        const p2pContexts = window.p2pContexts;
                        for (let i = 0; i < p2pContexts.length; i++) {
                          if (p2pContexts[i] == p2pContext) {
                            p2pContexts.splice(i, 1);
                            console.log("p2pContext " + p2pContext.channelLabel + " removed");
                            return;
                          }
                        }
                       
                        console.log("Unable to remove p2pContext " + p2pContext.channelLabel);
                 }
                  
               
                 return new Promise((resolve) => {
                       peerConnection.onicecandidate = function (e) {
                         //console.log("onicecandidate  e:" + e + "\n e.candidate: " + e.candidate + "\n peerConnection.localDescription: \n" + JSON.stringify(peerConnection.localDescription));
                   
                         if (e.candidate === null && peerConnection.localDescription) {
                           peerConnection.localDescription.sdp = peerConnection.localDescription.sdp.replace('b=AS:30', 'b=AS:1638400');
                   
                            const descriptionAdv = { description: peerConnection.localDescription, channelLabel: p2pContext.channelLabel};
                            //console.log("descriptionAdv: " + JSON.stringify(descriptionAdv));
                            p2pContext.localDescription = JSON.stringify(descriptionAdv);
                            p2pContext.setAnswerDescription = setAnswerDescription;
                            p2pContext.sendMessage = sendMessage;
                   
                            resolve(p2pContext);
                         }
                       };
                   
                       if (!remoteDescription) {
                             setupChannelAsAHost();
                             createOffer();
                       } else {
                             setupChannelAsASlave();
                             createAnswer(remoteDescription);
                       }
                 });
          }
          
          console.log("InitLib complete");
      }     
});