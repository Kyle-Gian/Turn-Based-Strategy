using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    public float unitSpeed = 2f;
    int index;
    public event Action OnPathComplete;
    private Vector3 posToMoveTowards;
    public void MoveUnit(List<Vector3> path)
    {
        float step = unitSpeed * Time.deltaTime;
        
        //Used for first iteration if the position has not been set
        if (posToMoveTowards == Vector3.zero)
            posToMoveTowards = new Vector3(path[index].x, path[index].y + .5f, path[index].z);
        
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, posToMoveTowards, step);
        
        //When unit reaches destination increase index/move to next position
        if (Vector3.Distance(transform.position, posToMoveTowards) <= 0.001f)
            index++;
        //Reached the end of the path, reset for next move
        if(index == path.Count)
        {
            index = 0;
            posToMoveTowards = Vector3.zero;
            path.Clear();
            OnPathComplete?.Invoke();
        }
        else
            posToMoveTowards = new Vector3(path[index].x, path[index].y + 1f, path[index].z);
    }
}
