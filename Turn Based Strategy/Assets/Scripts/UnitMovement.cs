using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 currentMousePos;
    private Pathfinding pathfinding;
    private List<Vector3> path;
    int index;
    private bool agentMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pathfinding = GetComponent<Pathfinding>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.isPressed && !agentMoving)
        {
            path = pathfinding.SetPath();
            agentMoving = true;
            index = 0;
        }
        
        if (!agent.hasPath && path != null)
        {
            if (index != path.Count)
            {
                agent.SetDestination(path[index]);
                index++;
            }
            else
            {
                index = 0;
                path.Clear();
                agentMoving = false;
                pathfinding.SetNewStartNode();
            }
        }
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
