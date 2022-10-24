using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private int width;
    private int height;
    private Cell[,] grid;
    
    // Start is called before the first frame update
    void __init__(int init_width, int init_height)
    {
        width = init_width;
        height = init_height;  
        grid = new Cell[width,height];
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
