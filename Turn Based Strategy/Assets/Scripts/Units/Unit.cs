using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private float UnitMaxMoveDistance;
    [HideInInspector] public float unitCurrentMoveDistance;
    
    public int unitDamagePower;
    public int attackRange;
    
    private UnitMovement unitMovement;
    private List<Vector3> path;
    private Vector3 unitStartPos = Vector3.zero;

    private float distanceBetweenNodes;
    
    [HideInInspector] public bool unitIsMoving = false;
    [HideInInspector] public bool UnitMovementComplete;
    [HideInInspector] public bool unitHasMovesLeft;


    private void Start()
    {
        UnitMovementComplete = false;
        unitCurrentMoveDistance = UnitMaxMoveDistance;
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
        Debug.Log(unitCurrentMoveDistance);

        unitCurrentMoveDistance = CalculateMoveDistance();
        unitIsMoving = false;
        Debug.Log(unitCurrentMoveDistance);
        Debug.Log(distanceBetweenNodes);

        if (unitCurrentMoveDistance < distanceBetweenNodes)
        {
            UnitMovementComplete = true;
            unitHasMovesLeft = false;
        }
        
    }

    public void ResetUnitMovement()
    {
        unitCurrentMoveDistance = UnitMaxMoveDistance;
        UnitMovementComplete = false;
        unitHasMovesLeft = true;
    }

    public void SetUnitPath(List<Vector3> newPath)
    {
        SetTurnStartPos();
        path = newPath;
        unitIsMoving = true;
        UnitMovementComplete = false;
        unitMovement.MoveUnit(path);
    }

    public void UnitIsMoving()
    {
        unitMovement.MoveUnit(path);
    }

    public void AttackEnemy(GameObject enemy)
    {
        Health enemyHealth = enemy.GetComponent<Health>();
        
        enemyHealth.TakeDamage(unitDamagePower);
    }

    private float CalculateMoveDistance()
    {
        float distanceMovedThisTurn = 0;

        distanceMovedThisTurn = Vector3.Distance(unitStartPos, transform.position);

        return distanceMovedThisTurn;
    }

    private void SetTurnStartPos()
    {
        unitStartPos = transform.position;
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
