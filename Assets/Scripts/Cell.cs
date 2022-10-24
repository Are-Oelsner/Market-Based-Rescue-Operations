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
    public int x;
    public int y;
    public GameObject self;

    public Cell()
    {

    }
    public Cell(CellStatus init_status, int start_x, int start_y, float cellSize=1)
    {
        status = init_status;
        x = start_x;
        y = start_y;
        position_x = start_x * cellSize;
        position_y = start_y * cellSize;
        self = new GameObject("cell[" + x + "," + y + "]");
    }

    public void init_cell(CellStatus init_status, int start_x, int start_y, float cellSize=1)
    {
        status = init_status;
        x = start_x;
        y = start_y;
        position_x = start_x * cellSize;
        position_y = start_y * cellSize;
        self = new GameObject("cell[" + x + "," + y + "]");
    }

    CellStatus getStatus()
    {
        return status;
    }

    void setStatus(CellStatus new_status)
    {
        status = new_status;
    }

    public void drawCell()
    {

    }
    
    // TODO draw the cell?
    void Start()
    {
        self.transform.position = new Vector3(position_x, position_y, 0);
        print(self.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
