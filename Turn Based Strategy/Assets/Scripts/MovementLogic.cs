using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementLogic : MonoBehaviour
{
    
    private bool playerTurn = false;
    protected Unit selectedUnit;
    protected Unit unitToAttack;
    public List<Unit> myUnits = new List<Unit>();
    private List<Unit> attackableTargets = new List<Unit>();

    protected Pathfinding pathFinding;
    protected int currentUnitPosInList = 0;
    
    [HideInInspector]public UnityEvent PlayerTurnComplete;
    
    private void Awake()
    {
        pathFinding = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        if (playerTurn)
        {
            LogicToMakeMoves();
        }
    }
    
    protected virtual void LogicToMakeMoves()
    {
        
    }

    public void PlayerTurnStart()
    {
        playerTurn = true;
        StartNewTurn();
    }
    public void PlayerTurnEnd()
    {
        playerTurn = false;
    }
    private void StartNewTurn()
    {
        foreach (var unit in myUnits)
        {
            unit.ResetUnitMovement();
        }

        currentUnitPosInList = 0;
        selectedUnit = myUnits[currentUnitPosInList];
        pathFinding.SetActiveUnit(selectedUnit);
    }

    protected void AddNewUnitToMyList(Unit newUnit)
    {
        myUnits.Add(newUnit);
    }
    
    protected void RemoveUnitFromMyList(Unit unit)
    {
        myUnits.Remove(unit);
    }
    
    protected void MoveUnitToPosition()
    {
        selectedUnit.SetUnitPath(pathFinding.SetPath());

    }

    protected void UnitFinishedMovement()
    {
        if (!CheckForTurnCompletion())
        {
            GetNextUnitInList();
        }
    }
    
    private void GetNextUnitInList()
    {
        if (currentUnitPosInList >= myUnits.Count - 1)
            currentUnitPosInList = 0;
        else
            currentUnitPosInList++;
        
        selectedUnit = myUnits[currentUnitPosInList];
        
        //If the next Unit selected has moved go to next unit
        if (selectedUnit.UnitMovementComplete)
            GetNextUnitInList();
        
        pathFinding.SetActiveUnit(selectedUnit);
    }

    protected void ChangeSelectedUnit(int newUnitPos)
    {
        currentUnitPosInList = newUnitPos;
        selectedUnit = myUnits[currentUnitPosInList];
        pathFinding.SetActiveUnit(selectedUnit);
    }

    private bool CheckForTurnCompletion()
    {
        foreach (var unit in myUnits)
        {
            if (unit.UnitMovementComplete == false)
            {
                return false;
            }
        }
        PlayerTurnComplete.Invoke();
        return true;
    }
}
