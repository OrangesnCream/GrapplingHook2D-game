
using UnityEngine;
// handles health and any other player stat and handles the events that happen when a stat reaches some value

public class PlayerStats : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int health=1;
    private Vector2 spawnLocation;
    private Vector2 firstSpawn;
    public GameObject stateManager;
    
    void Start()
    {
        //call gamestate manager to set player location, health 
        spawnLocation=gameObject.transform.position;
        firstSpawn=gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetHealth(int newHealth)
    {
        //for setting the health of the player in game state manager 
        if (newHealth >= 0)
        {
             health=newHealth;
        }
       
    }
    public int GetHealth()
    {
        return health;
    }

    public void DamagePlayer(int incomingDamage)
    {
        //player only cares about handling incoming damage
        //damage rate and amount is decided by the source
        int newHealth=health-incomingDamage;
        if (newHealth < 0)
        {
            health=0;
            KillPlayer();
            return;
        }
    } 
    public void KillPlayer()
    {
        // trigger death screen, send message to the game state manager to do this, manager will also store save points
        stateManager.GetComponent<GameState>().PlayerDeath();
        //send player to either the start of the map or to checkpoint 

        gameObject.transform.position=spawnLocation;
    }
    public void SetSpawnLocation(Vector2 newSpawn)
    {
        spawnLocation=newSpawn;
        
    }
    public Vector2 GetFirstSpawn()
    {
        return firstSpawn;
    }
    public void ResetPlayer()
    {
        gameObject.transform.position=firstSpawn;
    }
}
