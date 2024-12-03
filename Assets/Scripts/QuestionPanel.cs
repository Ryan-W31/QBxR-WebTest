using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video; // Import Video namespace for VideoPlayer

public class QuestionPanel : MonoBehaviour
{
  public TextMeshProUGUI questionText; // Text component for question text
  public RawImage display; // Use RawImage instead of Image for flexibility
  public VideoPlayer videoPlayer; // VideoPlayer for handling mp4 files
  public Button[] choiceButtons; // References to the UI Buttons for answer choices

  public void SetupQuestion(string problem, object media, string[] choices, int correctAnswer)
  {
    // Set the question text
    if (questionText != null)
    {
      questionText.text = problem;
    }
    else
    {
      Debug.LogError("questionText is not assigned in the inspector.");
    }

    // Determine media type and display accordingly
    display.gameObject.SetActive(false);
    videoPlayer.gameObject.SetActive(false);

    if (media is Sprite sprite)
    {
      // Display static image as Sprite
      display.texture = sprite.texture;
      display.gameObject.SetActive(true);
    }
    else if (media is Texture texture)
    {
      // Display GIF or static image as Texture
      display.texture = texture;
      display.gameObject.SetActive(true);
    }
    else if (media is string videoPath)
    {
      // Play video
      PlayVideo(videoPath);
    }

    // Set up choices
    for (int i = 0; i < choiceButtons.Length; i++)
    {
      if (choiceButtons[i] != null)
      {
        choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
        int choiceIndex = i;
        choiceButtons[i].onClick.RemoveAllListeners();
        choiceButtons[i].onClick.AddListener(() => QuizManager.Instance.OnAnswerSelected(choiceIndex, correctAnswer));
      }
      else
      {
        Debug.LogError("A choiceButton is not assigned in the inspector.");
      }
    }
  }

  private void PlayVideo(string videoPath)
  {
    videoPlayer.url = videoPath; // Set video URL
    videoPlayer.targetTexture = new RenderTexture(1920, 1080, 0); // Set target for RawImage
    display.texture = videoPlayer.targetTexture; // Assign texture to RawImage
    display.gameObject.SetActive(true);
    videoPlayer.gameObject.SetActive(true);
    videoPlayer.Play();
  }
}
