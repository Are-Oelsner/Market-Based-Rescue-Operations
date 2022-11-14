using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BidList
{
    public bool bid_received;
    public List<float> bid_list;

    public BidList()
    {
        //always empty when initialized
        bid_list = new List<float>();
        bid_received = false;
    }

}
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
    //private BoxCollider2D collision_checker_collider;
    // private var obstacle_collider;
    private bool initialize_time;

    private List<BidList> bids = new List<BidList>(); // rows are agents, columns are survivors
    private bool bids_received = false;



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
            bids.Add(new BidList());
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

        //Run assign paths function
        InvokeRepeating("AssignPaths", 0.0f, 0.5f);
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
        bids[agent_ind].bid_list = agent_bids;
        bids[agent_ind].bid_received = true;
        Debug.Log("Got path length " + agent_bids[0]);

        string s = "\n";

        foreach(BidList bid in bids)
        {
            foreach(float val in bid.bid_list)
            {
                s += val + " ";
            }
            s += "\n";
        }

        // check if all bids have now been added
        bids_received = true;
        foreach(BidList bid in bids)
        {
            if(!bid.bid_received)
            {
                bids_received = false;
            }
        }

        Debug.Log(s);
    }

    public void AssignPaths()
    {
        int num_assigned = 0;
        int num_agents = agent_objects.Length;
        float best_bid;
        int best_agent;
        int best_survivor;
        List<int> claimed_survivors = new List<int>();

        if(bids_received)
        {
            // iterate through bid list, assigning the best bids
            while(num_assigned != num_agents)
            {
                best_bid = -1f;
                best_agent = -1;
                best_survivor = -1;
                for(int i = 0; i < bids.Count; i++)
                {
                    for(int j = 0; j < bids[i].bid_list.Count; j++)
                    {
                        if((best_agent == -1 || bids[i].bid_list[j] < best_bid) && !claimed_survivors.Contains(j))
                        {
                            best_agent = i;
                            best_survivor = j;
                            best_bid = bids[i].bid_list[j];
                        }
                    }
                }

                num_assigned++;
                if(best_agent != -1)
                {
                    // assign the best agent to that path
                    agent_objects[best_agent].AssignPath(best_survivor);
                    claimed_survivors.Add(best_survivor);

                    // remove that list, this agent is done bidding
                    bids[best_agent].bid_list = new List<float>();
                }
            }

            // this method should only be run once
            bids_received = false;
        }
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
        Collider2D obstacle_collider;
        obstacle_collider = obstacle.GetComponent<BoxCollider2D>();
        if (obstacle_collider == null)
        {
            print("circle collider");
            obstacle_collider = obstacle.GetComponent<CircleCollider2D>();
            print("after circle collider"+ obstacle_collider);
        }
        // try
        // {
        //     obstacle_collider = obstacle.GetComponent<BoxCollider2D>();
        // } catch (MissingComponentException e)
        // {
        //     obstacle_collider = obstacle.GetComponent<CircleCollider2D>();
        // }
        float offset = .4f;
        if (obstacle_collider.bounds.Contains(position)) // Check center point
        {
            return true;
        } // else check four corners offset from center point
        //else if(obstacle_collider.bounds.Contains(position + new Vector3(offset, offset, 0)) || obstacle_collider.bounds.Contains(position+ new Vector3(offset, -offset, 0)) || obstacle_collider.bounds.Contains(position+ new Vector3(-offset, -offset, 0)) || obstacle_collider.bounds.Contains(position+new Vector3(-offset, offset, 0)))
        //{
        //    return true;
        //}
        return false;
    }

}
