﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

/*--------------------------------------------------------------------------
 * Script Created by: Aiden Nathan.
 *------------------------------------------------------------------------*/

public class GameManager : NetworkBehaviour
{
    #region Variables
    //General
    public bool enemiesDead = true; //Used to determine whether or not all enemies are dead
    public int round = 0; //Counter for the rounds
    public int enemiesPerRound = 8; //Value for determining how many enemies per round will spawn
    public static int enemiesAlive; //Value used to show and keep track of how many enemies are alive
    public static List<Player> playersDead = new List<Player>(); //Used to keep track of all players that have died
    public Transform[] spawnPos; //List of spawn positions for the start of the game and for reviving players
    public bool gameStarted = false;
    public bool gameEnded = false;

    public static GameObject[] players; //Used to determine how many players have connected to the game so that stats can be tracked
    public static string[] playerName = new string[4]; //Used to keep track of players name
    public static int[] playerScore = new int[4]; //Used to keep track of players score
    public static int[] playerKills = new int[4]; //Used to keep track of players kills
    public static int[] playerDowns = new int[4]; //Used to keep track of players downs
    public static int[] playerDeaths = new int[4]; //Used to keep track of players deaths

    //References:
    public EnemySpawner enemySpawner; //Reference for the EnemySpawner
    public HUD hud; //Reference for the HUD
    #endregion

    #region General
    public void Start() //References gameObjects on start
    {
        enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        spawnPos = GameObject.Find("SpawnPos").GetComponentsInChildren<Transform>();
        players = GameObject.FindGameObjectsWithTag("Player");
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUD>();

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && gameStarted == false) //Checks if the game has started and whether or not the host wants to start the game
        {
            StartGame();
        }

        if (gameStarted == true) //All functions tied to the game when out of lobby
        {
            Rounds(); //Used to call upon Rounds
            GetPlayerData(); //Used to call upon GetPlayerData
            if (playersDead.Count == players.Length && !gameEnded) //Used to determine whether or not the game has ended and all players have died
            {
                EndGame();
            }
        }
        Debug.Log("Enemies Alive: " + enemiesAlive);
    }
    #endregion

    #region Start Game
    public void StartGame() //Start Game Function
    {
        players = GameObject.FindGameObjectsWithTag("Player"); //Gathers information to check how many players exist
        for (int i = 0; i < players.Length; i++) //Used to unlock all players functionality (shooting, interaction, etc)
        {
            players[i].GetComponent<Player>().canInteract = true;
        }
        StartCoroutine(BeginCountDown()); //Starts the countdown for the game
    }

    public IEnumerator BeginCountDown() //Used to start the game after a certain amount of time
    {
        yield return new WaitForSeconds(10f);
        gameStarted = true; 
    }
    #endregion

    #region Player Data
    public void GetPlayerData() //Used to keep track of player stats
    {
        for (int i = 0; i < players.Length; i++) //Checks how many players are in the game
        {
            Player playerRef = players[i].GetComponent<Player>(); //private reference for the script
            playerName[i] = "Player " + 1; //Creates and stores the names for each player
            playerScore[i] = playerRef.score; //stores the score of the player into the playerScore array
            playerKills[i] = playerRef.kills; //stores the kills of the player into the playerKills array
            playerDowns[i] = playerRef.downs; //stores the downs of the player into the playerDowns array
            playerDeaths[i] = playerRef.deaths; //stores the deaths of the player into the playerDeaths array
        }
    }
    #endregion

    #region Death System
    public void EndGame() //Used to end the game and show all stats for players
    {
        hud.scoreboard.SetActive(true);
        hud.displayStats();
        StartCoroutine(DeathScreen());
        gameEnded = true;
    }

    public IEnumerator DeathScreen() //Ends the game after x amount of time and sends all players back to the menu
    {
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene(0);
    }
    #endregion

    #region Round Management
    public void Rounds() //Used to moderate rounds
    {
        if (enemiesDead == true && enemySpawner.finishedSpawning == true) //Checks whether or not all enemies are dead and that all enemies have ceased spawning to allow for a new round to start
        {
            round++; //Counts up the round
            if (round > 1) //Checks if the round is above 1 to allow for the difficulty to increase
            {
                StartCoroutine(IncreaseDifficulty()); //Calls upon the co-routine "IncreaseDifficulty"
            }
            CmdRevivePlayers(); //Calls upon the co-routine "RevivePlayers"
            enemySpawner.enemiesToSpawn = enemiesPerRound; //Sets the enemiesToSpawn on the EnemySpawner to the new caculated enemiesPerRound after the difficulty increase
            enemySpawner.enemiesSpawning = true; //Allows the enemies to spawn.
            enemySpawner.finishedSpawning = false; //Ends the round management from the Game Managers end
            enemiesDead = false; //Gateway variable set to false
        }

        if (enemiesAlive == 0 && enemySpawner.finishedSpawning == true && enemiesDead == false) //Checks whether or not all the enemies are dead and that the EnemySpawner has finished spawning
        {
            enemiesDead = true; //Gateway variable set to true
        }
    }

    IEnumerator IncreaseDifficulty() //Used to increase the difficulty of each round
    {
        if (round < 10) //Default difficulty increase
        {
            enemiesPerRound = (int)(enemiesPerRound * 1.4f);
        }
        else if (enemiesPerRound < 350) //Difficulty increase after round 10 and x amount of zombies
        {
            enemiesPerRound = (int)(enemiesPerRound * 1.15f);
        }
        yield return new WaitForEndOfFrame();
    }
    #endregion

    #region Revive Players
    [Command]
    public void CmdRevivePlayers() //Used to revive players at the start of a new round
    {
        for (int i = 0; i < playersDead.Count; i++) //Revives all players within the playersDead list
        {
            StartCoroutine(playersDead[i].Revived(spawnPos[i + 1])); //Used to revive each player
            playersDead.RemoveAt(i); //Removes the entry from the list
        }
    }
    #endregion
}
