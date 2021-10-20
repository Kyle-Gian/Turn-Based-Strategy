using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Grid : MonoBehaviour
{
    private int width;
    private int height;
    [SerializeField] private float obstructionRadiusCheck = .5f;
    [SerializeField] private int nodesPerZone = 2;
    
    public Node[,] nodeArray;

    private Mesh objectMesh;

    private void Awake()
    {
        objectMesh = GetComponent<MeshCollider>().sharedMesh;
        height = (int)objectMesh.bounds.size.x * nodesPerZone;
        width = (int)objectMesh.bounds.size.z * nodesPerZone;
        nodeArray = new Node[width + 1, height + 1];

        CreateGrid();
    }

    public void CreateGrid()
    {
        float distanceToNextSphere = objectMesh.bounds.size.x / width;
        float nextSphereDistanceZ = 0;
        float nextSphereDistanceX = distanceToNextSphere;

        for (int x = 0; x != nodeArray.GetLength(0); x++)
        {
            nextSphereDistanceX = 0;
            for (int y = 0; y != nodeArray.GetLength(1); y++)
            {
                Vector3 newNodePos = transform.TransformPoint(new Vector3(objectMesh.bounds.min.x + nextSphereDistanceX,
                    0, objectMesh.bounds.min.z + nextSphereDistanceZ));
                
                nodeArray[x, y] = new Node(newNodePos, CheckForObstruction(newNodePos),x, y);
                
                nextSphereDistanceX += distanceToNextSphere;
            }
            nextSphereDistanceZ += distanceToNextSphere;
        }
    }

    private bool CheckForObstruction(Vector3 position)
    {
       Collider[] obstructingColliders = Physics.OverlapSphere(position,obstructionRadiusCheck);
       
        if (obstructingColliders.Length != 0)
        {
            Collider thisCollider = gameObject.GetComponent<Collider>();
            
            foreach (var collider in obstructingColliders)
            {
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
                    if (nodeArray[x,y].isNodeWalkable)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(nodeArray[x,y].nodePosition, .1f);

                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(nodeArray[x,y].nodePosition, .1f);

                    }

                }
            }
        }
    }
}
