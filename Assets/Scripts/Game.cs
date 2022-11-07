using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject[] agents;
    Agent[] agent_objects;
    public int num_agents = 2;
    public Sprite cell_sprite;
    public bool create_agents = false;

    public GameObject[] obstacles;
    public int num_obstacles = 2;

    public GameObject collision_checker;
    private BoxCollider2D collision_checker_collider;
    private BoxCollider2D obstacle_collider;
    private bool initialize_time;

    private List<List<float>> bids = new List<List<float>>(); // rows are agents, columns are survivors



    // Start is called before the first frame update
    void Start()
    {
        // Initialize Agents
        InitAgents(create_agents);

        // initialize lists for each agent except the last one (LineRenderers)
        agent_objects = GameObject.Find("Agents").GetComponentsInChildren<Agent>();

        int counter = 0;
        foreach(Agent agent in agent_objects)
        {
            Debug.Log("Agent " + counter + " has sib index " + agent.GetInd());
            agent.SetGame(this);
            bids.Add(new List<float>());
            counter++;
        }

        GameObject obstacles_parent = GameObject.Find("Obstacles");
        int num_obstacles = obstacles_parent.transform.childCount;
        obstacles = new GameObject[num_obstacles];
        for(int i = 0; i < num_obstacles; i++)
        {
            obstacles[i] = obstacles_parent.transform.GetChild(i).gameObject;
        }

        //obstacles[0] = GameObject.Find("Obstacle 1");

        collision_checker = GameObject.Find("CollisionChecker");
    }

    private void InitAgents(bool _create_agents)
    {
        if (_create_agents)
        {
            Vector3[] agent_start_positions = new Vector3[num_agents];
            agent_start_positions[0] = new Vector3(7.5f, 1.5f, 0);
            agent_start_positions[1] = new Vector3(-7.5f, -2.5f, 0);

            // Initialize Grid
            // grid = gameObject.AddComponent<Grid>() as Grid;
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
                spriteRenderer.sortingLayerName = "Foreground";
                agent.transform.position = agent_start_positions[i];
                agent.transform.parent = gameObject.transform;
                agent.layer = 2;//LayerMask.NameToLayer("Foreground");
                                // Add sprites, set goals, set starting positions
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // allows the agent to add its bids to this game
    public void AddBids(int agent_ind, List<float> agent_bids)
    {
        bids[agent_ind] = agent_bids;
        Debug.Log("Got path length " + agent_bids[0]);

        string s = "\n";

        foreach(List<float> bid in bids)
        {
            foreach(float val in bid)
            {
                s += val + " ";
            }
            s += "\n";
        }

        Debug.Log(s);
    }


    public bool InObstacle(Vector3 position)
    {
        //collision_checker.transform.position = loc;// transform.TransformPoint(loc);
        foreach(GameObject obstacle in obstacles)
        {
            if(CheckCollision(obstacle, position))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckCollision(GameObject obstacle, Vector3 position)
    {
        // TODO check for collision between collision_checker and obstacle 2D colliders
        //collision_checker_collider = collision_checker.GetComponent<BoxCollider2D>();
        obstacle_collider = obstacle.GetComponent<BoxCollider2D>();
        //if (collision_checker_collider.bounds.Intersects(obstacle_collider.bounds))
        //{
        //    return true;
        //} // TODO check four points from step_size offset
        if (obstacle_collider.bounds.Contains(position))
        {
            return true;
        }
        return false;
    }

}
