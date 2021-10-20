using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Pathfinding : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private float maxMoveDistance;
    

    [SerializeField] private GameObject unit;
    private Grid grid;
    private Vector3 mousePos;
    private Node startNode = null;
    private Node newStartNode = null;

    private Node endNode = null;
    private Vector3 unitPos;
    private bool objectMoving = false;

    private List<Node> openList;
    private List<Node> closedList;
    private List<Node> path;
    private void Start()
    {
        grid = FindObjectOfType<Grid>();
        GetClosestNodeToUnit();
    }

    private void Update()
    {
        if (unit != null)
        {
            endNode = null;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, -1))
                return;

            unitPos = unit.transform.position;

            mousePos = hit.point;
            CheckMouseDistanceFromPlayer();
            GetClosestNodeToMouse();
            
            if (endNode != null && startNode != null)
            {
                path = FindPath();
            }
        }
    }

    private void CheckMouseDistanceFromPlayer()
    {

        float mouseDistanceFromPlayer = Vector3.Distance(startNode.nodePosition, mousePos);

        if (mouseDistanceFromPlayer > maxMoveDistance)
        {
            Vector3 dir = (mousePos - startNode.nodePosition).normalized;
            mousePos = startNode.nodePosition + (maxMoveDistance * dir);
        }

    }

    private void GetClosestNodeToUnit()
    {
        unitPos = unit.transform.position;
        float distanceToNode = 0;
        Node currentNode;
        bool startNodeFound = false;
        
        for (int width = 0; width < grid.nodeArray.GetLength(0); width++)
        {
            for (int height = 0; height < grid.nodeArray.GetLength(1); height++)
            {
                currentNode = grid.nodeArray[width, height];
                distanceToNode = Vector3.Distance(unitPos, grid.nodeArray[width, height].nodePosition);
                
                if (distanceToNode < 2f && currentNode.isNodeWalkable)
                {
                    startNode = currentNode;
                    startNodeFound = true;
                    break;
                }
            }

            if (startNodeFound)
            {
                break;
            }
        }

        if (startNode != null)
        {
            startNode = CheckNeighboursDist(startNode, distanceToNode);
        }
       
    }

    private Node CheckNeighboursDist(Node currentNode, float distance)
    {
        for (int x = currentNode.WidthPos - 1; x <= currentNode.WidthPos + 1; x++) 
        {
            //if out of bounds then skip
            if (x > grid.nodeArray.GetLength(0) - 1|| x < 0)
                continue;
            
            for (int y = currentNode.HeightPos - 1; y <= currentNode.HeightPos + 1; y++) 
            {
                //if out of bounds then skip
                if (y > grid.nodeArray.GetLength(1) - 1 || y < 0)
                    continue;
                
                //Skip if the node to check is the start node
                if (x == currentNode.WidthPos && y == currentNode.HeightPos)
                    continue;

                //skip the Node if it is not walkable
                if (!grid.nodeArray[x, y].isNodeWalkable)
                    continue;

                float distanceTocheck = Vector3.Distance(unitPos, grid.nodeArray[currentNode.WidthPos, currentNode.HeightPos].nodePosition);
                
                if (distanceTocheck < distance)
                {
                    currentNode = grid.nodeArray[x, y];
                }
            }
        }

        return currentNode;
    }

    public List<Vector3> SetPath()
    {
        objectMoving = true;
        List<Vector3> newAgentPath = new List<Vector3>();

        foreach (var node in path)
        {
            newAgentPath.Add(node.nodePosition);
        }

        newStartNode = path[path.Count - 1];
        return newAgentPath;
    }

    public void SetNewStartNode()
    {
        startNode = newStartNode;
        objectMoving = false;

    }

    void GetClosestNodeToMouse()
    {
        Node currentNode;
        float distanceToNode = 0;
        bool endNodeFound = false;

        for (int width = 0; width < grid.nodeArray.GetLength(0); width++)
        {
            for (int height = 0; height < grid.nodeArray.GetLength(1); height++)
            {
                currentNode = grid.nodeArray[width, height];
                distanceToNode = Vector3.Distance(mousePos, currentNode.nodePosition);
                
                if (distanceToNode < 3f && currentNode.isNodeWalkable)
                {
                    endNode = currentNode;
                    endNodeFound = true;
                    break;
                }
            }

            if (endNodeFound)
            {
                break;
            }
        }

        if (endNode != null)
        {
            endNode = CheckNeighboursDist(endNode, distanceToNode);
            if (!endNode.isNodeWalkable)
            {
                endNode = null;
            }
        }
    }

    private List<Node> FindPath()
    {
        openList = new List<Node>() {startNode};
        closedList = new List<Node>();
        
        for (int width = 0; width < grid.nodeArray.GetLength(0); width++)
        {
            for (int height = 0; height < grid.nodeArray.GetLength(1); height++)
            {
                Node pathNode = grid.nodeArray[width, height];
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.prevNode = null;

            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }

                if (!neighbourNode.isNodeWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                float tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.prevNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                    
                }

            }

        }

        return null;
    }

    private List<Node> CalculatePath(Node endNode)
    {
        List<Node> path = new List<Node>();
        path.Add(endNode);
        Node currentNode = endNode;

        while (currentNode.prevNode != null)
        {
            path.Add(currentNode.prevNode);
            currentNode = currentNode.prevNode;
        }
        path.Reverse();

        return path;
    }

    private float CalculateDistanceCost(Node a, Node b)
    {
        float xDistance = Mathf.Abs(a.nodePosition.x - b.nodePosition.x);
        float yDistance = Mathf.Abs(a.nodePosition.y - b.nodePosition.y);
        float remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;

    }

    private List<Node> GetNeighbourList(Node currentNode)
    {
        
        List<Node> neighbourList = new List<Node>();
        
        for (int x = currentNode.WidthPos - 1; x <= currentNode.WidthPos + 1; x++) 
        {
            //if out of bounds then skip
            if (x > grid.nodeArray.GetLength(0) - 1 || x < 0)
                continue;
            
            for (int y = currentNode.HeightPos - 1; y <= currentNode.HeightPos + 1; y++) 
            {
                //if out of bounds then skip
                if (y > grid.nodeArray.GetLength(1) - 1 || y < 0)
                    continue;
                
                //Skip if the node to check is the start node
                if (x == currentNode.WidthPos && y == currentNode.HeightPos)
                    continue;
                neighbourList.Add(grid.nodeArray[x,y]);
            }
        }

        return neighbourList;
    }

    private Node GetLowestFCostNode(List<Node> pathNodeList)
    {
        Node LowestFCostNode = pathNodeList[0];

        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < LowestFCostNode.fCost)
            {
                LowestFCostNode = pathNodeList[i];
            }
        }

        return LowestFCostNode;
    }
    private void OnDrawGizmos()
    {

        if (path != null && !objectMoving)
        {
            Node otherNode = path[0];
            
            foreach (var node in path)
            {
                Gizmos.DrawLine(otherNode.nodePosition, node.nodePosition);
                otherNode = node;
            }
        }
    }
}
