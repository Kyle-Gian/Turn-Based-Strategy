using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerLogic : MonoBehaviour
{
    private bool playerTurn = false;
    private Unit selectedUnit;
    public List<Unit> myUnits = new List<Unit>();
    private Pathfinding pathFinding;
    private int currentUnitInList = 0;
    public UnityEvent PlayerTurnComplete;

    private void Start()
    {
        pathFinding = GetComponent<Pathfinding>();
        
        if (selectedUnit == null)
        {
            selectedUnit = myUnits[0];
            pathFinding.SetActiveUnit(selectedUnit);
        }
    }

    public void PlayerTurnStart()
    {
        playerTurn = true;
    }
    public void PlayerTurnEnd()
    {
        playerTurn = false;
    }

    private void Update()
    {
        if (playerTurn)
        {
            //Check if the current selected unit is moving
            if (!selectedUnit.unitIsMoving)
            {
                //Check if player clicked mouse
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (!UnitSelectionChange())
                        MoveUnitToPosition();
                }
            }
            else
            {
                selectedUnit.UnitIsMoving();
            }
        
            if (selectedUnit.UnitMovementComplete)
            {
                UnitFinishedMovement();
            }
        }

    }

    private bool UnitSelectionChange()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, -1))
            return false;

        if (hit.transform.TryGetComponent(out Unit newUnit))
        {
            foreach (var unit in myUnits)
            {
                if (newUnit == unit)
                {
                    selectedUnit = newUnit;
                    break;
                }
            }
            return true;
        }

        return false;
    }

    public void AddNewUnitToMyList(Unit newUnit)
    {
        myUnits.Add(newUnit);
    }
    
    public void RemoveUnitFromMyList(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void MoveUnitToPosition()
    {
        selectedUnit.SetUnitPath(pathFinding.SetPath());

    }

    private void UnitFinishedMovement()
    {
        if (!CheckForTurnCompletion())
        {
            pathFinding.SetActiveUnit(selectedUnit);
            GetNextUnitInList();
        }
        else
            PlayerTurnEnd();
    }

    private void GetNextUnitInList()
    {
        if (currentUnitInList >= myUnits.Count)
            currentUnitInList = 0;
        else
            currentUnitInList++;
        
        selectedUnit = myUnits[currentUnitInList];
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
