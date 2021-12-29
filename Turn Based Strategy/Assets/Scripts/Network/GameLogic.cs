using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLogic : MonoBehaviour
{
    public static GameLogic instance;
    [SerializeField]private List<PlayerLogic> players;
    private int playerIndex = 0;
    private PlayerLogic currentPlayerLogic;
    private int currentPlayerTurn;
    private Coroutine playerTurnCo;
    private float turnTimer = 90;
    private bool timerComplete = false;
    [SerializeField] private GameObject unitPrefab;
    

    private void Start()
    {
        foreach (var player in players)
        {
            player.PlayerTurnComplete.AddListener(PlayerTurnCompleted);
        }
        
        currentPlayerLogic = players[0].GetComponent<PlayerLogic>();
        currentPlayerLogic.PlayerTurnStart();
        playerTurnCo = StartCoroutine(PlayerTurnTimer());

    }
    
    public void PlayerTurnCompleted()
    {
        NextPlayersTurn();
        //Check if the player finished before turn timer completed
        if (!timerComplete && playerTurnCo != null)
        {
            //Stop the Coroutine
            StopCoroutine(playerTurnCo);
        }
        currentPlayerLogic.PlayerTurnEnd();

        currentPlayerLogic = players[playerIndex].GetComponent<PlayerLogic>();
        
        currentPlayerLogic.PlayerTurnStart();
        timerComplete = false;
        playerTurnCo = StartCoroutine(PlayerTurnTimer());
    }

    IEnumerator PlayerTurnTimer()
    {
        yield return new WaitForSeconds(turnTimer);
        timerComplete = true;
        if (playerIndex >= players.Count)
        {
            playerIndex = 0;
        }
        else
        {
            playerIndex++;
        }
        PlayerTurnCompleted();
    }

    private void NextPlayersTurn()
    {
        if (playerIndex >= players.Count - 1)
        {
            playerIndex = 0;
        }
        else
        {
            playerIndex++;
        }
    }
}
