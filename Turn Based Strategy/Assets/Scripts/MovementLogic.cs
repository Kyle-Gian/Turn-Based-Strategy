using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementLogic : MonoBehaviour
{
    [SerializeField] private DisplayInformation displayInformation;

    private bool playerTurn = false;
    protected Unit selectedUnit;
    protected Unit unitToAttack;
    public List<Unit> myUnits = new List<Unit>();
    private List<Unit> attackableTargets = new List<Unit>();
    protected List<Vector3> currentUnitPath;

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
        displayInformation.SetActivePlayerText(gameObject.name);
        ChangeSelectedUnit(0);

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
        currentUnitPath = pathFinding.SetPath();
        selectedUnit.SetUnitPath(currentUnitPath);
        selectedUnit.costOfMovementThisTurn = pathFinding.pathCost;
    }

    protected void UnitFinishedMovement()
    {
        //If the unit still has moves left reset pathfinding start node pos
        if (selectedUnit.unitHasMovesLeft)
        {
            displayInformation.SetMoveCostText(selectedUnit.unitLeftoverMoveDistance);

            pathFinding.SetActiveUnit(selectedUnit);
            selectedUnit.UnitMovementComplete = false;
            return;
        }
        
        //check if all units in my units have completed all their moves
        if (!CheckForTurnCompletion())
            GetNextUnitInList();
        
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
        
        ChangeSelectedUnit(currentUnitPosInList);

    }

    protected void ChangeSelectedUnit(int newUnitPos)
    {
        currentUnitPosInList = newUnitPos;
        selectedUnit = myUnits[currentUnitPosInList];
        pathFinding.SetActiveUnit(selectedUnit);
        displayInformation.SetMoveCostText(selectedUnit.unitLeftoverMoveDistance);
        displayInformation.SetCurrentActiveUnitText(selectedUnit.gameObject.name);

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
