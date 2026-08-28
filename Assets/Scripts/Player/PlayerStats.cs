
using System.Runtime.CompilerServices;
using UnityEngine;
// handles health and any other player stat and handles the events that happen when a stat reaches some value

public class PlayerStats : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int health=1;
    private int maxHealth;
    private Vector2 spawnLocation;
    private Vector2 firstSpawn;
    public GameObject stateManager;
    [SerializeField] private float damageCooldown = 1f;

    private float nextDamageTime;
    
    void Start()
    {
        //call gamestate manager to set player location, health 
        spawnLocation=gameObject.transform.position;
        firstSpawn=gameObject.transform.position;
        maxHealth=health;
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
        UiHealthUpdate();
       
    }
    public int GetHealth()
    {
        return health;
    }

    public void DamagePlayer(int incomingDamage)
    {
        //player only cares about handling incoming damage
        //damage  amount is decided by the source, rate is decided by player 
        if (Time.time < nextDamageTime)
            return;

        int newHealth=health-incomingDamage;
        nextDamageTime = Time.time + damageCooldown;
         Debug.Log("Player damaged. Health: " + newHealth);
        if (newHealth < 0)
        {
            health=0;
            UiHealthUpdate();
            KillPlayer();
            return;
        }
        health=newHealth;
        UiHealthUpdate();
    } 
    public void KillPlayer()
    {
        // trigger death screen, send message to the game state manager to do this, manager will also store save points
        stateManager.GetComponent<GameState>().PlayerDeath();
        //send player to either the start of the map or to checkpoint 
        gameObject.GetComponent<GrappleMovement>().ReleaseGrapple();//release grapple so that we don't fling ourselves after dying 
        
        gameObject.transform.position=spawnLocation;
        health=maxHealth;
        
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
    private void UiHealthUpdate()
    {
        //Callstatemanager to update UI 
    }
}
