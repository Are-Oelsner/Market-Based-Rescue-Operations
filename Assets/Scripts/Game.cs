using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // public Grid grid;
    public GameObject self;
    public GameObject[] agents;
    public int num_agents = 2;
    public Sprite cell_sprite;


    // Start is called before the first frame update
    void Start()
    {
        self = GameObject.Find("Game");

        // Initialize Grid
        // grid = self.AddComponent<Grid>() as Grid;
        // grid.init_grid(20, 10, cell_sprite, 1);

        // Initialize Agents
        agents = new GameObject[num_agents];
        GameObject agent;
        for (int i = 0; i < num_agents; i++)
        {

            agents[i] = new GameObject("Agent" + i);
            agent = agents[i];
            agent.AddComponent<Agent>();// as Agent;
            agent.AddComponent<SpriteRenderer>();
            SpriteRenderer spriteRenderer = agent.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            spriteRenderer.sprite = cell_sprite;
            agent.transform.position = new Vector3(1, 1, 0);
            agent.transform.parent = self.gameObject.transform;
            // Add sprites, set goals, set starting positions
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool InObstacle(Vector3 loc)
    {
        return false;
    }

}
