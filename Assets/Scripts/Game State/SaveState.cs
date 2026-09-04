using UnityEngine;

public class SaveState : MonoBehaviour
{
    //this one will read and write to a save file, if the file is not found we make it. \
    //also need a delete and a reset
    //this info will be sent to game state or any other script that calls it 
    //might add other functions for more specific functionality 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool WriteSave(/*what we want to write into the save*/)
    {
        //check for null=false
        //check if the save file exists 
        //if not then make the file 
        //add entry into the file, checks will be done by other functions
        //return  true if succesfull
        return false;
    }
    //replace void with the object type we are returning 
    public void ReadSave(/*what we want to read from the save, default returns everything?*/)
    {
        //check for null
        //check if the save file exists 
        //if not then make the file 
        //search the file for the object
        //if not found 
        //return null;
        //else return object

    }
    public bool DeleteSave(/*what we want to delete*/)
    {
        //check for null=false
        //search the file for the object
        //if not found 
        return false;
        //else 
        //delete it 
        return true;

    }
    public void ResetSave()
    {
        //deletes save file 
        //makes new file
    }

}
