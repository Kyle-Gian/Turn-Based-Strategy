using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLogic : NetworkBehaviour
{
    public static GameLogic instance;
    private List<NetworkIdentity> playersInGame = new List<NetworkIdentity>();
    private PlayerLogic currentPlayerLogic;
    private int currentPlayerTurn;
    private Coroutine playerTurnCo;
    private float turnTimer = 90;
    private bool timerComplete = false;

    private void Start()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }
    
    public void AddPlayerToGameList(NetworkIdentity player)
    {
        playersInGame.Add(player);
    }
    public void RemovePlayerFromGameList(NetworkIdentity player)
    {
        playersInGame.Remove(player);
    }

    public void OnGameStart()
    {
        currentPlayerTurn = Random.Range(0,playersInGame.Count);

        currentPlayerLogic = playersInGame[currentPlayerTurn].GetComponent<PlayerLogic>();
        
        currentPlayerLogic.PlayerTurnStart();
        StartCoroutine(PlayerTurnTimer());
    }

    public void PlayerTurnCompleted()
    {
        //Check if the player finished before turn timer completed
        if (!timerComplete)
        {
            //Stop the Coroutine
            StopCoroutine(playerTurnCo);
        }
        currentPlayerLogic.PlayerTurnEnd();
        
        //Check who's turn it is next
        if (currentPlayerTurn >= playersInGame.Count)
        {
            currentPlayerTurn = 0;
        }
        else
        {
            currentPlayerTurn++;
        }
        
        currentPlayerLogic = playersInGame[currentPlayerTurn].GetComponent<PlayerLogic>();
        
        currentPlayerLogic.PlayerTurnStart();
        timerComplete = false;
        playerTurnCo = StartCoroutine(PlayerTurnTimer());
    }

    IEnumerator PlayerTurnTimer()
    {
        yield return new WaitForSeconds(turnTimer);
        timerComplete = true;
        PlayerTurnCompleted();
    }
    
    
}
