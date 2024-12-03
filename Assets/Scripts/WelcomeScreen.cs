using UnityEngine;
using UnityEngine.UI;

public class WelcomeScreen : MonoBehaviour
{
  public GameObject welcomeScreen;  // Reference to the WelcomeScreen canvas
  public GameObject questionPanel;  // Reference to the QuestionPanel canvas

  void Start()
  {
    // Find the start button and set up the click listener
    Button startButton = welcomeScreen.GetComponentInChildren<Button>();
    startButton.onClick.AddListener(StartQuiz);
  }

  public void StartQuiz()
  {
    // Hide the welcome screen
    welcomeScreen.SetActive(false);
    // Show the first question
    questionPanel.SetActive(true);
    // Start the quiz by showing the first question
    QuizManager.Instance.DisplayNextQuestion();
  }
}
