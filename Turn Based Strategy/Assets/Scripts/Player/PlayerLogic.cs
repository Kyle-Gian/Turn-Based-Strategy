using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerLogic : MovementLogic
{
    protected override void LogicToMakeMoves()
    {
        //Check if the current selected unit is moving
        if (!selectedUnit.unitIsMoving)
        {
            //Check if player clicked mouse
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                MoveUnitToPosition();
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                //checks if enemy or player unit has been selected
                UnitSelection();

                //If enemy has been selected attack the enemy
                if (unitToAttack != null)
                {
                    float distToTarget = Vector3.Distance(selectedUnit.transform.position, unitToAttack.transform.position);
                    //Attack If unit is in range
                    if (distToTarget <= selectedUnit.attackRange)
                        selectedUnit.AttackEnemy(unitToAttack.gameObject);
                    
                    unitToAttack = null;
                }
            }
            
        }
        else
            selectedUnit.UnitIsMoving();
        
        if (selectedUnit.UnitMovementComplete)
            UnitFinishedMovement();
            
    }

    private void UnitSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, -1))
            return;

        if (hit.transform.TryGetComponent(out Unit newUnit))
        {
            bool isUnitTarget = true;
            
            //Check if the unit is mine
            for(int unitPos = 0; unitPos < myUnits.Count; unitPos++)
            {
                if (newUnit == myUnits[unitPos])
                {
                    ChangeSelectedUnit(unitPos);
                    isUnitTarget = false;
                }
            }

            //If the selected target is not my unit set the unit to attack
            if (isUnitTarget)
                unitToAttack = newUnit;
        }
    }

}
