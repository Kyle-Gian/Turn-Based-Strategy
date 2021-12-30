using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
    private int width;
    private int height;
    [SerializeField] private float obstructionRadiusCheck = .5f;
    public int nodesPerZone = 2;
    [SerializeField] private GameObject nodePosSphere;
    private LayerMask nonWalkableLayerMask;
    private LayerMask unitLayerMask;
    public float distanceToNextSphere;

    public Node[,] nodeArray;

    private Mesh objectMesh;

    private void Awake()
    {

        objectMesh = GetComponent<MeshCollider>().sharedMesh;
        height = (int)objectMesh.bounds.size.x * nodesPerZone;
        width = (int)objectMesh.bounds.size.z * nodesPerZone;
        nodeArray = new Node[width + 1, height + 1];
        distanceToNextSphere = objectMesh.bounds.size.x / width;

        nonWalkableLayerMask = LayerMask.GetMask("NonWalkableObject");
        unitLayerMask = LayerMask.GetMask("Unit");

        CreateGrid();
    }

    private void CreateGrid()
    {
        float nextSphereDistanceZ = 0;
        float nextSphereDistanceX = distanceToNextSphere;

        for (int x = 0; x != nodeArray.GetLength(0); x++)
        {
            nextSphereDistanceX = 0;
            for (int y = 0; y != nodeArray.GetLength(1); y++)
            {
                Vector3 newNodePos = transform.TransformPoint(new Vector3(objectMesh.bounds.min.x + nextSphereDistanceX,
                    0, objectMesh.bounds.min.z + nextSphereDistanceZ));
                
                nodeArray[x, y] = new Node(newNodePos, CheckForObstruction(newNodePos), CheckForPlayerObstruction(newNodePos),x, y);

                //if (nodeArray[x,y].isNodeWalkable)
                //{
                    //Instantiate(nodePosSphere, newNodePos, Quaternion.identity);
                //}
                
                nextSphereDistanceX += distanceToNextSphere;
            }
            nextSphereDistanceZ += distanceToNextSphere;
        }
    }

    public bool RedrawGrid()
    {
        nodeArray = null;
        CreateGrid();

        return true;
    }

    private bool CheckForPlayerObstruction(Vector3 position)
    {
        Collider[] obstructingColliders = Physics.OverlapSphere(position,obstructionRadiusCheck, unitLayerMask);
       
        if (obstructingColliders.Length != 0)
        {
            Collider thisCollider = gameObject.GetComponent<Collider>();
            
            foreach (var collider in obstructingColliders)
            {
                //Check to make sure collider is not our own
                if (collider != thisCollider)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    private bool CheckForObstruction(Vector3 position)
    {
       Collider[] obstructingColliders = Physics.OverlapSphere(position,obstructionRadiusCheck, nonWalkableLayerMask);
       
        if (obstructingColliders.Length != 0)
        {
            Collider thisCollider = gameObject.GetComponent<Collider>();
            
            foreach (var collider in obstructingColliders)
            {
                //Check to make sure collider is not our own
                if (collider != thisCollider)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        if (nodeArray != null)
        {
            for (int x = 0; x != nodeArray.GetLength(0); x++)
            {
                for (int y = 0; y != nodeArray.GetLength(1); y++)
                {
                    Node nodeToDraw = nodeArray[x, y];
                    if (nodeToDraw.isNodeWalkable && !nodeToDraw.unitIsOnNode)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(nodeToDraw.nodePosition, .1f);

                    }
                    else if(!nodeToDraw.isNodeWalkable && !nodeToDraw.unitIsOnNode)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(nodeToDraw.nodePosition, .1f);

                    }
                    else
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(nodeToDraw.nodePosition, .1f);
                    }

                }
            }
        }
    }
}
