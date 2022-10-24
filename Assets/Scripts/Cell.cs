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
    public float position_x;
    public float position_y;
    // Start is called before the first frame update
    void __init__(CellStatus init_status, float start_x, float start_y)
    {
        status = init_status;
        position_x = start_x;
        position_y = start_y;  
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
