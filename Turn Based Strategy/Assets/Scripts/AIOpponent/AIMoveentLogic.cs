using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIMoveentLogic : MovementLogic
{
    private List<Unit> enemyUnits = new List<Unit>();
    private Unit closestEnemyUnit = null;
    private void Start()
    {
        enemyUnits = FindObjectOfType<PlayerLogic>().myUnits;
    }

    protected override void LogicToMakeMoves()
    {
        //Check if the current selected unit is moving
        if (!selectedUnit.unitIsMoving)
        {
            //Check if player clicked mouse
        }
        else
            selectedUnit.UnitIsMoving();
        
        if (selectedUnit.UnitMovementComplete)
            UnitFinishedMovement();
            
    }

    private void MakeMoveDecision()
    {
        
    }
    private void CheckClosestEnemyHealth()
    {
        
    }
    private void FindClosestEnemy()
    {
        closestEnemyUnit = enemyUnits[0];
        
        Vector3 posOfSelectedUnit = selectedUnit.transform.position;
        float distToClosestEnemy = Vector3.Distance(posOfSelectedUnit, closestEnemyUnit.transform.position);
        
        foreach (var enemyUnit in enemyUnits)
        {
            float distanceToEnemy = Vector3.Distance(posOfSelectedUnit, enemyUnit.transform.position);

            if (distanceToEnemy < distToClosestEnemy)
            {
                closestEnemyUnit = enemyUnit;
            }
        }
    }
    private bool CanPlayerEnemyAttackNextTurn()
    {
        return false;
    }
    
    private bool DoesEnemyHaveLineOfSight(Vector3 posToCheck)
    {
        Vector3 currentUnitPos = selectedUnit.transform.position;
        
        foreach (var enemyUnit in enemyUnits)
        {
            Vector3 dirToEnemyUnit = (posToCheck - currentUnitPos).normalized;

            if (Physics.Raycast(currentUnitPos, dirToEnemyUnit))
            {
                return true;
            }
        }

        return false;
    } 
}
