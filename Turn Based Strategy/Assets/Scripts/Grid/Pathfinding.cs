using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pathfinding : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 1;
    private const int MOVE_DIAGONAL_COST = 2;

    private Unit unit;
    private Grid grid;
    [SerializeField] private DrawUnitPath drawUnitPath;

    private bool pathHasBeenSet = false;
    
    private Vector3 mousePos;
    private Vector3 unitPos;

    private Node startNode = null;
    private Node endNode = null;
    
    private bool objectMoving = false;
    private float maxMoveCost = 0;
    public float pathCost = 0;
    
    private List<Node> openList;
    private List<Node> closedList;
    private List<Node> path;
    private List<Vector3> nodePositionsPath = new List<Vector3>();
    
    private void OnEnable()
    {
        grid = FindObjectOfType<Grid>();
    }

    public void SetActiveUnit(Unit newUnit)
    {
        pathHasBeenSet = false;
        drawUnitPath.DisablePathLine();
        if (startNode != null)
        {
            startNode.unitIsOnNode = true;
        }
        unit = newUnit;
        maxMoveCost = unit.unitLeftoverMoveDistance;
        
        GetClosestNodeToUnit();
        objectMoving = false;
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
            CheckMouseDistanceFromUnit();
            GetClosestNodeToMouse();
            
            if (endNode != null && startNode != null)
            {
                path = FindPath();

                if (!pathHasBeenSet)
                {
                    UpdateListOfNodePositions();
                    drawUnitPath.DrawPotentialPath(nodePositionsPath);

                }

            }
        }
    }

    private void CheckMouseDistanceFromUnit()
    {
        float mouseDistanceFromPlayer = Vector3.Distance(startNode.nodePosition, mousePos);


        Vector3 dir = (mousePos - startNode.nodePosition).normalized;
        mousePos = startNode.nodePosition + dir * mouseDistanceFromPlayer;
        
    }

    private void GetClosestNodeToUnit()
    {
        unitPos = unit.transform.position;
        startNode = grid.nodeArray[0,0];

        float distanceToStartNode = Vector3.Distance(unitPos, startNode.nodePosition);

        for (int width = 0; width < grid.nodeArray.GetLength(0); width++)
        {
            for (int height = 0; height < grid.nodeArray.GetLength(1); height++)
            {
                Node currentNode = grid.nodeArray[width, height];
                float distanceToCurrentNode = Vector3.Distance(unitPos, currentNode.nodePosition);
                
                if (distanceToStartNode > distanceToCurrentNode && currentNode.isNodeWalkable)
                {
                    distanceToStartNode = distanceToCurrentNode;
                    startNode = currentNode;
                }
            }
        }

        startNode.unitIsOnNode = false;

    }

    public List<Vector3> SetPath()
    {
        objectMoving = true;
        pathCost = path[0].hCost;
        pathHasBeenSet = true;
        drawUnitPath.SetPathLine(nodePositionsPath);
        
        startNode = path[path.Count - 1];
        return nodePositionsPath;
    }

    private void UpdateListOfNodePositions()
    {
        if (nodePositionsPath != null)
            nodePositionsPath.Clear();


        if (path != null)
        {
            foreach (var node in path)
            {
                nodePositionsPath.Add(node.nodePosition);
            }
        }

        
    }
    void GetClosestNodeToMouse()
    {
        if (endNode == null)
            endNode = grid.nodeArray[0, 0];
        
        float distToEndNode = Vector3.Distance(mousePos, endNode.nodePosition);

        for (int width = 0; width < grid.nodeArray.GetLength(0); width++)
        {
            for (int height = 0; height < grid.nodeArray.GetLength(1); height++)
            {
                Node currentNode = grid.nodeArray[width, height];
                float distanceToCurrentNode = Vector3.Distance(mousePos, currentNode.nodePosition);
                
                if (distToEndNode > distanceToCurrentNode && currentNode.isNodeWalkable && !currentNode.unitIsOnNode)
                {
                    distToEndNode = distanceToCurrentNode;
                    endNode = currentNode;
                }
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
            
            //End node has been found, path complete
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

                if (!neighbourNode.isNodeWalkable || currentNode.unitIsOnNode)
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

        //If the path cost exceeds the max move cost of the unit return a reduced path
        if (path[0].hCost >= maxMoveCost)
            path = ReducePathToMaxCost(path);
        

        return path;
    }

    private List<Node> ReducePathToMaxCost(List<Node> path)
    {
        List<Node> reducedPath = new List<Node>();
        
        foreach (var node in path)
        {
            if (node.gCost <= maxMoveCost)
            {
                reducedPath.Add(node);
            }
            else
                break;
        }
        return reducedPath;
    }

    private float CalculateDistanceCost(Node a, Node b)
    {
        float xDistance = Mathf.Abs(a.nodePosition.x - b.nodePosition.x);
        float yDistance = Mathf.Abs(a.nodePosition.z - b.nodePosition.z);
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
