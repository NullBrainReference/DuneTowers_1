mergeInto(LibraryManager.library, {

  Hello: function () {
    window.alert("Hello, world!");
  },

  ShowAdv : function () {
    myGameInstance.SendMessage('PauseManager', 'Pause');
    ysdk.adv.showFullscreenAdv({
        callbacks: {
            onOpen: () => {
              console.log('Ad opened, pause.');
              myGameInstance.SendMessage("PauseManager", "PauseForJS");
            },
            onClose: function(wasShown) {
              // some action after close
              myGameInstance.SendMessage('PauseManager', 'UnPause');
              //myGameInstance.SendMessage('RewardCallbacksManager', 'ShowReward')
            },
            onError: function(error) {
              // some action on error
              myGameInstance.SendMessage('PauseManager', 'UnPause');
              //myGameInstance.SendMessage('RewardCallbacksManager', 'ShowReward')
            }
        }
    })
  },

  ShowReward : function() {
    ysdk.adv.showRewardedVideo({
      callbacks: {
          onOpen: () => {
            console.log('Video ad opened, pause.');
            myGameInstance.SendMessage("PauseManager", "PauseForJS");
          },
          onRewarded: () => {
            console.log('Rewarded!');
          },
          onClose: () => {
            console.log('Video ad closed, unpause.');
            myGameInstance.SendMessage("PauseManager", "UnPause");
            myGameInstance.SendMessage('RewardCallbacksManager', 'ShowReward')
          }, 
          onError: (e) => {
            myGameInstance.SendMessage("PauseManager", "UnPause");
            myGameInstance.SendMessage('RewardCallbacksManager', 'ShowReward');
            console.log('Error while open video ad:', e);
          }
      }
    })
  }

});