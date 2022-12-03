using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BidList
{
    public bool bid_received;
    public List<Bid> bid_list;

    public BidList()
    {
        //always empty when initialized
        bid_list = new List<Bid>();
        bid_received = false;
    }

}
public class Game : MonoBehaviour
{
    public GameObject[] agents;
    Agent[] agent_objects;
    Survivors[] survivor_objects;
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
        survivor_objects = GameObject.Find("Survivors").GetComponentsInChildren<Survivors>();
        int counter = 0;
        foreach(Agent agent in agent_objects)
        {
            Debug.Log("Agent " + counter + " has sib index " + agent.GetInd());
            agent.SetGame(this, survivor_objects);
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
    public void AddBids(int agent_ind, List<Bid> agent_bids)
    {
        bids[agent_ind].bid_list = agent_bids;
        bids[agent_ind].bid_received = true;
        Debug.Log("Got path length " + agent_bids[0]);

        string s = "\n";

        foreach(BidList bidlist in bids)
        {
            foreach(Bid bid in bidlist.bid_list)
            {
                s += bid.gas_num + " ";
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
        int best_num_saved;
        float best_bid_path;
        int best_agent;
        int best_survivor;
        int num_saved;

        if(bids_received)
        {
            // iterate through bid list, assigning the best bids
            while(num_assigned != num_agents)
            {
                best_num_saved = -1;
                best_bid_path = -1f;
                best_agent = -1;
                best_survivor = -1;
                for(int i = 0; i < bids.Count; i++)
                {
                    for(int j = 0; j < bids[i].bid_list.Count; j++)
                    {
                        // number of people that can be saved is minimum of gas masks and survivors in cluster
                        num_saved = Math.Min(bids[i].bid_list[j].gas_num, survivor_objects[j].num_survivors);

                        if(best_agent == -1 
                            || num_saved > best_num_saved
                            || (num_saved == best_num_saved && bids[i].bid_list[j].path_distance < best_bid_path)) //tiebreaker
                        {
                            best_agent = i;
                            best_survivor = j;
                            best_num_saved = num_saved;
                            best_bid_path = bids[i].bid_list[j].path_distance;
                        }
                    }
                }

                num_assigned++;
                if(best_agent != -1)
                {
                    // assign the best agent to that path
                    agent_objects[best_agent].AssignPath(best_survivor);

                    // adjust the number of people that can be saved, since some are already saved by the gas masks
                    survivor_objects[best_survivor].num_survivors -= agent_objects[best_agent].num_gas_masks;
                    

                    // remove that list, this agent is done bidding
                    bids[best_agent].bid_list = new List<Bid>();
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
        BoxCollider2D box_collider = obstacle.GetComponent<BoxCollider2D>();
        float offset = .35f;
        if (box_collider != null)
        {
            if (box_collider.bounds.Contains(position)) // Check center point
            {
                return true;
            } // else check four corners offset from center point
            //else if(obstacle_collider.bounds.Contains(position + new Vector3(offset, offset, 0)) || obstacle_collider.bounds.Contains(position+ new Vector3(offset, -offset, 0)) || obstacle_collider.bounds.Contains(position+ new Vector3(-offset, -offset, 0)) || obstacle_collider.bounds.Contains(position+new Vector3(-offset, offset, 0)))
            //{
            //    return true;
            //}
        }
        else
        {
            CircleCollider2D circle_collider = obstacle.GetComponent<CircleCollider2D>();
            if (circle_collider.bounds.Contains(position)) // Check center point
            {
                return true;
            } // else check four corners offset from center point
              //else if(obstacle_collider.bounds.Contains(position + new Vector3(offset, offset, 0)) || obstacle_collider.bounds.Contains(position+ new Vector3(offset, -offset, 0)) || obstacle_collider.bounds.Contains(position+ new Vector3(-offset, -offset, 0)) || obstacle_collider.bounds.Contains(position+new Vector3(-offset, offset, 0)))
              //{
              //    return true;
              //}
        }
        return false;
    }

}
