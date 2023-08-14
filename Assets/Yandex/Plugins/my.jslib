mergeInto(LibraryManager.library, {

  Hello: function () {
    window.alert("Hello, world!");
  },

  ShowAdv : function () {
    myGameInstance.SendMessage('PauseManager', 'Pause');
    ysdk.adv.showFullscreenAdv({
        callbacks: {
            onClose: function(wasShown) {
              // some action after close
              myGameInstance.SendMessage('PauseManager', 'UnPause');
            },
            onError: function(error) {
              // some action on error
              myGameInstance.SendMessage('PauseManager', 'UnPause');
            }
        }
    })
  },

  ShowReward : function() {
    ysdk.adv.showRewardedVideo({
      callbacks: {
          onOpen: () => {
            console.log('Video ad open.');
          },
          onRewarded: () => {
            console.log('Rewarded!');
          },
          onClose: () => {
            console.log('Video ad closed.');
            myGameInstance.SendMessage("PauseManager", "UnPause");
          }, 
          onError: (e) => {
            console.log('Error while open video ad:', e);
          }
      }
    })
  }

});