using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float UnitMaxMoveDistance;
    public int unitDamagePower;
    public int attackRange;
    private UnitMovement unitMovement;
    private List<Vector3> path;
    public bool unitIsMoving = false;
    public bool UnitMovementComplete;

    private void Start()
    {
        attackRange = attackRange * FindObjectOfType<Grid>().nodesPerZone;
        UnitMovementComplete = false;
        unitMovement = GetComponent<UnitMovement>();
        unitMovement.OnPathComplete += UnitHasFinishedMovement;
    }

    private void OnDestroy()
    {
        unitMovement.OnPathComplete -= UnitHasFinishedMovement;
    }

    private void UnitHasFinishedMovement()
    {
        unitIsMoving = false;
        UnitMovementComplete = true;
    }

    public void ResetUnitMovement()
    {
        UnitMovementComplete = false;
    }

    public void SetUnitPath(List<Vector3> newPath)
    {
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
