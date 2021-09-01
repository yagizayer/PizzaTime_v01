using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PositionCell : MonoBehaviour
{

    public List<PositionCell> Neighbors = new List<PositionCell>();
    public List<AcceptedEntities> PlaceFor = new List<AcceptedEntities>(){
        AcceptedEntities.Player
    };

    [HideInInspector]
    public int Row = -1;
    [HideInInspector]
    public int Column = -1;
    [HideInInspector]
    public Image MyImage ;
    private void Start() {
        MyImage = GetComponent<Image>();
    }
}
