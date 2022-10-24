using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum CellStatus
    {
        Free, 
        Agent,
        Obstacle,
        Goal
    }
    public CellStatus status;
    // Start is called before the first frame update
    void __init__(CellStatus init_status)
    {
        status = init_status;
    }

    CellStatus getStatus()
    {
        return status;
    }

    void setStatus(CellStatus new_status)
    {
        status = new_status;
    }
    
    // TODO draw the cell?
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
