using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 nodePosition;
    public bool isNodeWalkable;
    public bool unitIsOnNode;
    public Node prevNode;

    public int WidthPos;
    public int HeightPos;
    
    public float fCost;
    public float hCost;
    public float gCost;

    public Node(Vector3 newPosition, bool isWalkable, bool isUnitOnNode, int width, int height)
    {
        nodePosition = newPosition;
        isNodeWalkable = isWalkable;
        unitIsOnNode = isUnitOnNode;
        WidthPos = width;
        HeightPos = height;

    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}
