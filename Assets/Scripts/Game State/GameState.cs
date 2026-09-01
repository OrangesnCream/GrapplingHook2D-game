using UnityEngine;
using TMPro;
public class GameState : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     [SerializeField] TextMeshProUGUI timerText;
     float elapsedTime;
    void Start()
    {
        
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
}
