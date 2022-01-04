using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private float UnitMaxMoveDistance;
    [HideInInspector] public float unitLeftoverMoveDistance;
    [HideInInspector] public float costOfMovementThisTurn;
    
    public int unitDamagePower;
    public int attackRange;
    public float attackCost;
    
    private UnitMovement unitMovement;
    private List<Vector3> path;

    private float distanceBetweenNodes;
    
    [HideInInspector] public bool unitIsMoving = false;
    [HideInInspector] public bool UnitMovementComplete;
    [HideInInspector] public bool unitHasMovesLeft;


    private void Start()
    {
        UnitMovementComplete = false;
        unitLeftoverMoveDistance = UnitMaxMoveDistance;
        distanceBetweenNodes = FindObjectOfType<Grid>().distanceToNextSphere;
        attackRange = attackRange * (int)distanceBetweenNodes;
        
        unitMovement = GetComponent<UnitMovement>();
        
        unitMovement.OnPathComplete += UnitHasFinishedMovement;
    }

    private void OnDestroy()
    {
        unitMovement.OnPathComplete -= UnitHasFinishedMovement;
    }

    private void UnitHasFinishedMovement()
    {

        unitLeftoverMoveDistance -= costOfMovementThisTurn;
        unitIsMoving = false;
        UnitMovementComplete = true;
        if (unitLeftoverMoveDistance < distanceBetweenNodes)
        {
            unitHasMovesLeft = false;
        }
        
    }

    public void ResetUnitMovement()
    {
        unitLeftoverMoveDistance = UnitMaxMoveDistance;
        UnitMovementComplete = false;
        unitHasMovesLeft = true;
    }

    public void SetUnitPath(List<Vector3> newPath)
    {
        path = newPath;
        unitIsMoving = true;
        UnitMovementComplete = false;
        unitMovement.MoveUnit(path);
    }

    private void SetPathLine()
    {
        
    }

    public void UnitIsMoving()
    {
        unitMovement.MoveUnit(path);
    }

    public void AttackEnemy(GameObject enemy)
    {
        Health enemyHealth = enemy.GetComponent<Health>();
        unitLeftoverMoveDistance -= attackCost;

        enemyHealth.TakeDamage(unitDamagePower);
    }


    private void OnDrawGizmos()
    {
        if (path != null)
        {
            if (path.Count != 0)
            {
                Vector3 nextPos = path[0];
            
                foreach (var pos in path)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(nextPos, pos);
                    nextPos = pos;
                }
            }
        }
    }
}
