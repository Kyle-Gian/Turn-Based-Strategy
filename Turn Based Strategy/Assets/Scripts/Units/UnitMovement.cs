using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    int index;
    public event Action OnPathComplete;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    public void MoveUnit(List<Vector3> path)
    {
        if (agent != null)
        {
            agent.SetDestination(path[index]);
        }
        if (index != path.Count - 1)
        {
            if (agent.remainingDistance <= 0.02f)
            {
                index++;
                agent.SetDestination(path[index]);
            }
        }
        else
        {
            index = 0;
            path.Clear();
            OnPathComplete?.Invoke();
        }
    }
}
