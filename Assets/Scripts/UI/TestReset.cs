
using UnityEngine;

public class TestReset : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetPreviousScene()
    {

        if (!SaveSystem.Current.ClearPosition(LastLevelInfo.levelId))
        {
            Debug.Log("No saved checkpoint position found");
        }
    }
}
