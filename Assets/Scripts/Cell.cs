using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
    public SpriteRenderer sprite_renderer;
    public Sprite cell_sprite;
    public Bounds leftBound;
    public Bounds topBound;
    public Bounds rightBound;
    public Bounds bottomBound;

    


    public Cell()
    {

    }

    public void init_cell(CellStatus init_status, int start_x, int start_y, Sprite init_cell_sprite, float cellSize=1)
    {
        status = init_status;
        x = start_x;
        y = start_y;
        position_x = start_x * cellSize;
        position_y = start_y * cellSize;
        self = new GameObject("cell[" + x + "," + y + "]");

        // Bounding boxes for collision checking, detects obstacles in neighboring Cells
        leftBound = new Bounds(new Vector3(x-cellSize, y, 0), new Vector3(cellSize, cellSize, 1));
        topBound = new Bounds(new Vector3(x, y+cellSize, 0), new Vector3(cellSize, cellSize, 1));
        rightBound = new Bounds(new Vector3(x+cellSize, y, 0), new Vector3(cellSize, cellSize, 1));
        bottomBound = new Bounds(new Vector3(x, y-cellSize, 0), new Vector3(cellSize, cellSize, 1));

        cell_sprite = init_cell_sprite;
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
        //print(self.transform.position);
        sprite_renderer = gameObject.GetComponent<SpriteRenderer>();
        sprite_renderer.sprite = cell_sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
