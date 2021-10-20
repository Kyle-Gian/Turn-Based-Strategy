using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private bool playerTurn = false;

    private List<Unit> myUnits = new List<Unit>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerTurnStart()
    {
        playerTurn = true;
    }
    public void PlayerTurnEnd()
    {
        playerTurn = false;
    }
}
