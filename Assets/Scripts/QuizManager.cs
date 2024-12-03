using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;


public class QuizManager : MonoBehaviour
{
  public static QuizManager Instance;
  public QuestionPanel questionPanel; // Reference to the QuestionPanel script
  public List<Question> questions;
  private int defensiveReadScore = 0;
  private int playRecognitionScore = 0;
  private int currentQuestionIndex = 0;

  // A structure to hold the results
  public class Result
  {
    public int questionId;
    public bool isCorrect;
  }

  // Store results in a list
  private List<Result> results = new List<Result>();

  private string userId;

  // Called from JavaScript to set the user ID
  public void SetUserId(string id)
  {
    userId = id;
  }

  public string GetUserId()
  {
    return userId;
  }

  [System.Serializable]
  public class Question
  {
    public int id;
    public string problem;
    public string media;
    public string[] choices;
    public int correctAnswer;
  }

  private void Awake()
  {
    Instance = this;
  }

  void Start()
  {
    ResetQuizState(); // Reset the quiz state
    StartCoroutine(LoadQuestions());
  }

  // Coroutine to load questions.json from StreamingAssets or external URL
  IEnumerator LoadQuestions()
  {
    string path;

    if (Application.platform == RuntimePlatform.WebGLPlayer)
    {
      // For WebGL, use a relative URL to load the file from StreamingAssets
      path = "StreamingAssets/questions.json";
    }
    else
    {
      // For non-WebGL platforms, use the local file system
      path = "file://" + Application.streamingAssetsPath + "/questions.json";
    }

    // UnityWebRequest to get the content of questions.json
    UnityWebRequest request = UnityWebRequest.Get(path);
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
      // Successfully loaded the file, parse the JSON
      string json = request.downloadHandler.text;
      questions = JsonConvert.DeserializeObject<List<Question>>(json);

      // Shuffle and display the first question
      ShuffleQuestions();
      DisplayNextQuestion();
    }
    else
    {
      Debug.LogError("Error loading questions.json: " + request.error);
    }
  }

  void ShuffleQuestions()
  {
    Debug.Log(questions.Count + " questions loaded.");
    for (int i = 0; i < questions.Count; i++)
    {
      int randomIndex = Random.Range(0, questions.Count);
      Question temp = questions[i];
      questions[i] = questions[randomIndex];
      questions[randomIndex] = temp;
    }
  }

  public void OnAnswerSelected(int choiceIndex, int correctAnswer)
  {
    bool isCorrect = choiceIndex == correctAnswer;
    var currentQuestion = questions[currentQuestionIndex]; // Last shown question
    results.Add(new Result { questionId = currentQuestion.id, isCorrect = isCorrect });

    switch (currentQuestion.id)
    {
      case 1:
        if (isCorrect)
          defensiveReadScore += 1;
        break;
      case 2:
        if (isCorrect)
          defensiveReadScore += 1;
        break;
      case 3:
        if (isCorrect)
          playRecognitionScore += 1;
        break;
      case 4:
        if (isCorrect)
          playRecognitionScore += 1;
        break;
      case 5:
        if (isCorrect)
          defensiveReadScore += 1;
        break;
      case 6:
        if (isCorrect)
          playRecognitionScore += 1;
        break;
      case 7:
        if (isCorrect)
          defensiveReadScore += 1;
        break;
      case 8:
        if (isCorrect)
          playRecognitionScore += 1;
        break;
      case 9:
        if (isCorrect)
          defensiveReadScore += 1;
        break;
      case 10:
        if (isCorrect)
          defensiveReadScore += 1;
        break;
      default:
        break;
    }

    currentQuestionIndex++;
    DisplayNextQuestion();
  }

  public void DisplayNextQuestion()
  {
    if (currentQuestionIndex < questions.Count)
    {
      var question = questions[currentQuestionIndex];
      StartCoroutine(LoadQuestionMedia(question.media, question.problem, question.choices, question.correctAnswer));
    }
    else
    {
      // All questions answered, show the exit screen
      ExitScreen exitScreenScript = FindObjectOfType<ExitScreen>();
      exitScreenScript.ShowExitScreen();
    }
  }

  // Coroutine to load the image for each question from StreamingAssets
  IEnumerator LoadQuestionMedia(string mediaName, string problem, string[] choices, int correctAnswer)
  {
    if (string.IsNullOrEmpty(mediaName))
    {
      Debug.LogError("Media name is null or empty.");
      questionPanel.SetupQuestion(problem, null, choices, correctAnswer); // Proceed with no media
      yield break; // Exit the coroutine
    }

    string mediaPath;

    if (Application.platform == RuntimePlatform.WebGLPlayer)
    {
      // For WebGL, use the relative URL to load the media from StreamingAssets
      mediaPath = "StreamingAssets/" + mediaName;
    }
    else
    {
      // For local testing
      mediaPath = "file://" + Application.streamingAssetsPath + "/" + mediaName;
    }

    // Determine the file extension to handle different media types
    string fileExtension = System.IO.Path.GetExtension(mediaName).ToLower();

    if (fileExtension == ".png" || fileExtension == ".jpg")
    {
      // Handle image files
      UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(mediaPath);
      yield return imageRequest.SendWebRequest();

      if (imageRequest.result == UnityWebRequest.Result.Success)
      {
        Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
        Sprite questionSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        questionPanel.SetupQuestion(problem, questionSprite, choices, correctAnswer);
      }
      else
      {
        Debug.LogError("Error loading image: " + imageRequest.error);
        questionPanel.SetupQuestion(problem, null, choices, correctAnswer); // Set up question without image if loading fails
      }
    }
    else if (fileExtension == ".mp4")
    {
      // Handle MP4 files by passing the media path to play the video
      questionPanel.SetupQuestion(problem, mediaPath, choices, correctAnswer); // Pass mediaPath for video playback
    }
    else
    {
      Debug.LogError("Unsupported file format: " + fileExtension);
      questionPanel.SetupQuestion(problem, null, choices, correctAnswer);
    }
  }


  void ResetQuizState()
  {
    // Reset any variables that might be carried over
    currentQuestionIndex = 0;
    results.Clear();

    // Make sure to hide the exit screen and show the welcome screen or question panel
    ExitScreen exitScreenScript = FindObjectOfType<ExitScreen>();
    if (exitScreenScript != null)
    {
      exitScreenScript.exitScreen.SetActive(false); // Hide the exit screen
    }

    questionPanel.gameObject.SetActive(true); // Show the question panel
  }

  public List<Result> GetResults()
  {
    return results; // Used to get the collected results
  }

  public int GetDefensiveReadScore()
  {
    return defensiveReadScore;
  }

  public int GetPlayRecognitionScore()
  {
    return playRecognitionScore;
  }
}
