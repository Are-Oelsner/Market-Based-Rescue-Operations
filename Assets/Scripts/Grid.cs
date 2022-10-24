using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour
{
    private int width;
    private int height;
    private float tileSize;
    private Cell[,] grid;
    
    public Grid()
    {

    }
    // Start is called before the first frame update
    public Grid(int init_width=20, int init_height=10, float init_tilesize=1)
    {
        width = init_width;
        height = init_height;  
        grid = new Cell[width,height];
        tileSize = init_tilesize;
    }

    public void init_grid(int init_width, int init_height, float init_tilesize = 1)
    {
        width = init_width;
        height = init_height;  
        grid = new Cell[width,height];
        tileSize = init_tilesize;

    }

    void Start()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                //grid[x, y] = new Cell(Cell.CellStatus.Free, x, y, tileSize);
                grid[x, y] = gameObject.AddComponent(typeof(Cell)) as Cell;
                grid[x, y].init_cell(Cell.CellStatus.Free, x, y, tileSize);
            }
        }

    }

    void drawGrid()
    {
        for(var x = 0; x < width; x++)
        {
            for(var y = 0; y < height; y++)
            {
                grid[x, y].drawCell();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
