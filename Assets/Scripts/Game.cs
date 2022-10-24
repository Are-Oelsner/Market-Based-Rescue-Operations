using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Grid grid;


    // Start is called before the first frame update
    void Start()
    {
        //grid = new Grid(20, 10);
        grid = gameObject.AddComponent<Grid>() as Grid;
        grid.init_grid(20, 10, 1);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
