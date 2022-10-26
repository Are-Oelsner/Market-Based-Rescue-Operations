using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Grid grid;
    public GameObject self;
    public Agent[] agents;
    public int num_agents = 2;
    public Sprite cell_sprite;


    // Start is called before the first frame update
    void Start()
    {
        self = GameObject.Find("Game");

        // Initialize Grid
        grid = self.AddComponent<Grid>() as Grid;
        grid.init_grid(20, 10, cell_sprite, 1);

        // Initialize Agents
        agents = new Agent[num_agents];
        for (int i = 0; i < num_agents; i++)
        {
            agents[i] = self.AddComponent<Agent>() as Agent;
            // TODO initialize agents and make them game objects
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
