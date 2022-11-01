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

    public GameObject[] obstacles;
    public int num_obstacles = 2;

    public GameObject collision_checker;
    private BoxCollider2D collision_checker_collider;
    private BoxCollider2D obstacle_collider;



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
            agent.layer = 2;//LayerMask.NameToLayer("Foreground");
            // Add sprites, set goals, set starting positions
        }

        obstacles = new GameObject[num_obstacles];
        obstacles[0] = GameObject.Find("Obstacle 1");
        obstacles[1] = GameObject.Find("Obstacle 1");


        collision_checker = GameObject.Find("CollisionChecker");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool InObstacle(Vector3 loc)
    {
        collision_checker.transform.position = loc;
        foreach(GameObject obstacle in obstacles)
        {
            if(CheckCollision(obstacle))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckCollision(GameObject obstacle)
    {
        // TODO check for collision between collision_checker and obstacle 2D colliders
        collision_checker_collider = collision_checker.GetComponent<BoxCollider2D>();
        obstacle_collider = obstacle.GetComponent<BoxCollider2D>();
        if (collision_checker_collider.bounds.Intersects(obstacle_collider.bounds))
        {
            return true;
        }
        return false;
    }

}
