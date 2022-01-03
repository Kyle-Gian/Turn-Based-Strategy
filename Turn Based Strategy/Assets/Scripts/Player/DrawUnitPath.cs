using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawUnitPath : MonoBehaviour
{
    [SerializeField] private LineRenderer pathRenderer;
    [SerializeField] private Color32 setPathColor;
    [SerializeField] private Color32 findPathColor;

    private bool ColorHasBeenSetToDrawPotentialPath = false;

    public void SetPathLine(List<Vector3> path)
    {
        ColorHasBeenSetToDrawPotentialPath = false;
        pathRenderer.material.color = setPathColor;
        pathRenderer.positionCount = path.Count;
        pathRenderer.SetPositions(path.ToArray());
    }

    public void DrawPotentialPath(List<Vector3> path)
    {
        if (!ColorHasBeenSetToDrawPotentialPath)
        {
            pathRenderer.enabled = true;
            ColorHasBeenSetToDrawPotentialPath = true;
            pathRenderer.material.color = findPathColor;
        }
        
        pathRenderer.positionCount = path.Count;
        pathRenderer.SetPositions(path.ToArray());
    }
    
    public void DisablePathLine()
    {
        ColorHasBeenSetToDrawPotentialPath = false;
        pathRenderer.enabled = false;
    }
}
