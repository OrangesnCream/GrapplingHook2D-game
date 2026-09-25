using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameState : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     [SerializeField] TextMeshProUGUI timerText;
     float elapsedTime;
    [Tooltip("Unique ID for this level. Defaults to the scene name if left empty.")]
    [SerializeField] private string levelId;
    private void Awake()
    {
        if (string.IsNullOrEmpty(levelId))
            levelId = SceneManager.GetActiveScene().name;
    }
    void Start()
    {
        var data = SaveSystem.Current;
        if (data.hasSavedPosition && data.lastSceneName == SceneManager.GetActiveScene().name)
        {
           //code to set player position
        }
    }

    // Update is called once per frame
    void Update()
    {
         elapsedTime+=Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);

        timerText.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
    public void PlayerDeath()
    {
        //trigger for death screen and time stop
    }
    public float? GetBestTime()
    {
        return SaveSystem.Current.TryGetBestTime(levelId, out float best) ? best : (float?)null;
    }
    public void SaveCheckpoint(Vector2 newPosition)
    {
         var data = SaveSystem.Current;
        data.hasSavedPosition = true;
        data.lastSceneName = SceneManager.GetActiveScene().name;
        data.lastPosition = newPosition;
        SaveSystem.Save();
    }
}
