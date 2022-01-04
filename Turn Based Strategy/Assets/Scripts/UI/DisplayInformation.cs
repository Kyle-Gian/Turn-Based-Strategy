using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayInformation : MonoBehaviour
{
    [SerializeField] private TMP_Text currentPlayerNameText;
    [SerializeField] private TMP_Text currentUnitNameText;
    [SerializeField] private TMP_Text moveCostText;

    public void SetActivePlayerText(string playerName)
    {
        currentPlayerNameText.text = playerName;
    }
    
    public void SetMoveCostText(float cost)
    {
        moveCostText.text = cost.ToString("n2");
    }
    
    public void SetCurrentActiveUnitText(string unitName)
    {
        currentUnitNameText.text = unitName;
    }
    
}
