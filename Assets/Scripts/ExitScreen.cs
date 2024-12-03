using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class ExitScreen : MonoBehaviour
{
  public GameObject exitScreen;
  public GameObject questionPanel;

  [DllImport("__Internal")]
  private static extern void navigateToProfile();

  [DllImport("__Internal")]
  private static extern void sendScore(int score, int defensiveReadScore, int playRecognitionScore, bool isComplete);

  public void OnFinishButton()
  {
    RestartQuiz();

#if UNITY_WEBGL == true && UNITY_EDITOR == false
    navigateToProfile();
#endif
  }

  public void ShowExitScreen()
  {
    questionPanel.SetActive(false);
    exitScreen.SetActive(true);

    // Once the quiz is completed, send the results to the server
    List<QuizManager.Result> results = QuizManager.Instance.GetResults();
    int score = results.FindAll(result => result.isCorrect).Count;
    int defensiveReadScore = QuizManager.Instance.GetDefensiveReadScore();
    int playRecognitionScore = QuizManager.Instance.GetPlayRecognitionScore();

#if UNITY_WEBGL == true && UNITY_EDITOR == false
    sendScore(score, defensiveReadScore, playRecognitionScore, true);
#endif
  }

  public void RestartQuiz()
  {
    // Reload the current scene to reset the quiz
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }
}
