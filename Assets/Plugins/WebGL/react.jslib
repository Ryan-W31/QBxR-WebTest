mergeInto(LibraryManager.library, {
  navigateToProfile: function () {
    window.dispatchReactUnityEvent("navigateToProfile");
  },
  sendScore: function (score, defensiveReadScore, playRecognitionScore, isComplete) {
    window.dispatchReactUnityEvent("sendScore", score, defensiveReadScore, playRecognitionScore, isComplete);
  },
});