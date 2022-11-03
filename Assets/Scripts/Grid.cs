using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour
{
    private int width;
    private int height;
    private float tileSize;
    public Cell[,] grid;
    public GameObject self;
    public Sprite cell_sprite;
    
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

    public void init_grid(int init_width, int init_height, Sprite init_cell_sprite, float init_tilesize = 1)
    {
        width = init_width;
        height = init_height;  
        grid = new Cell[width,height];
        tileSize = init_tilesize;
        cell_sprite = init_cell_sprite;

    }

    void Start()
    {
        self = GameObject.Find("Grid");
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                //grid[x, y] = new Cell(Cell.CellStatus.Free, x, y, tileSize);
                grid[x, y] = self.AddComponent(typeof(Cell)) as Cell;
                grid[x, y].init_cell(Cell.CellStatus.Free, x, y, cell_sprite, tileSize);
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

    Vector2 Pos_To_Coords(float xpos, float ypos)
    {
        return new Vector2((int)xpos / tileSize, (int)ypos / tileSize);
    }
    bool Is_Occupied(float xpos, float ypos)
    {
        Vector2 cell = Pos_To_Coords(xpos, ypos);
        return grid[(int)cell.x, (int)cell.y].status != Cell.CellStatus.Obstacle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
