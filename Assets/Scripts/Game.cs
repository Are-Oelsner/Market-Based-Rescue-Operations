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

    Scoreboard scoreboard;

    public GameObject collision_checker;
    //private BoxCollider2D collision_checker_collider;
    // private var obstacle_collider;
    private bool initialize_time;

    private List<BidList> bids = new List<BidList>(); // rows are agents, columns are survivors
    private bool bids_received = false;

    public float soft_limit;
    public float chance_per_step;
    public float hard_limit;

    private bool timer_started = false;
    private int time_count = 0;

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
            agent.SetGame(this, survivor_objects);
            bids.Add(new BidList());
            counter++;
        }

        // initialize scoreboard
        scoreboard = GameObject.Find("Survivor Stats").GetComponent(typeof(Scoreboard)) as Scoreboard;
        for(int i = 0; i < survivor_objects.Length; i++)
        {
            scoreboard.ShowSurvivorStats(i, survivor_objects[i].num_survivors + " survivors");
        }

        // initialize obstacles
        GameObject obstacles_parent = GameObject.Find("Obstacles");
        int num_obstacles = obstacles_parent.transform.childCount;
        obstacles = new GameObject[num_obstacles];
        for(int i = 0; i < num_obstacles; i++)
        {
            obstacles[i] = obstacles_parent.transform.GetChild(i).gameObject;
        }

        collision_checker = GameObject.Find("CollisionChecker");

        // set up periodic functions
        InvokeRepeating("AssignPaths", 0.0f, 1.0f);
        InvokeRepeating("Timer", 0.0f, 1.0f);

    }

    public void Timer()
    {
        if(timer_started)
        {
            if(time_count == hard_limit * .25f)
            {
                for(int i = 0; i < survivor_objects.Length; i++)
                {
                    if(survivor_objects[i].GetNumSaved() == 0)
                    {
                        scoreboard.ShowSurvivorStats(i, "0 saved,\n" + survivor_objects[i].GetOrigSurvivors() + " deceased");
                    }
                }
            }
            time_count++;
        }
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

        //Debug.Log(s);
    }

    private float WeightedSurvival(float num_survivors, float path_distance)
    {
        // chance of death per survivor increases by 5% each step past the soft limit
        float num_overridden;
        
        if(path_distance <= soft_limit)
        {
            return (float) num_survivors;
        }
        else if(path_distance >= hard_limit)
        {
            return 0;
        }
        else
        {
            num_overridden = path_distance - soft_limit;
            return (1.0f - chance_per_step * num_overridden) * (float) num_survivors;
        }

    }

    public int GetNumSaved(int survivor_ind, int path_length, int num_gas_masks)
    {
        // chance of death per survivor increases by 5% each step past the soft limit
        float num_overridden = path_length - soft_limit;
        int num_alive = 0;
        int num_already_saved = survivor_objects[survivor_ind].GetNumSaved();
        int num_saved = 0;
        int total_num_saved;
        int num_to_save = (int)survivor_objects[survivor_ind].GetOrigSurvivors() - num_already_saved;

        if(path_length <= soft_limit)
        {
            num_alive = num_to_save;
            num_saved = Math.Min(num_alive, num_gas_masks);
        }
        else
        {
            float percent_chance_of_survival = (1 - chance_per_step * num_overridden) * 100.0f;
            int prob;
            System.Random rand = new System.Random();

            for(int i = 0; i < num_to_save; i++)
            {
                prob = rand.Next(100);
                if(prob < percent_chance_of_survival)
                {
                    num_alive += 1;
                }
            }

            num_saved = Math.Min(num_alive, num_gas_masks);
        }
        total_num_saved = num_saved + num_already_saved;
        survivor_objects[survivor_ind].SaveNum(num_saved);
        scoreboard.ShowSurvivorStats(survivor_ind, total_num_saved + " saved,\n" + ((int)survivor_objects[survivor_ind].GetOrigSurvivors() - total_num_saved) + " deceased");

        return num_saved;

    }


    public void AssignPaths()
    {
        int num_assigned = 0;
        int num_agents = agent_objects.Length;
        float best_num_saved;
        float best_bid_path;
        int best_agent;
        int best_survivor;
        float num_saved;
        float num_survivors;

        if(bids_received)
        {
            // iterate through bid list, assigning the best bids
            while(num_assigned != num_agents)
            {
                best_num_saved = -1;
                best_bid_path = -1f;
                best_agent = -1;
                best_survivor = -1;
                num_saved = 0;
                for(int i = 0; i < bids.Count; i++)
                {
                    for(int j = 0; j < bids[i].bid_list.Count; j++)
                    {
                        // number of people that can be saved is minimum of gas masks and survivors in cluster
                        num_survivors = WeightedSurvival(survivor_objects[j].num_survivors, bids[i].bid_list[j].path_distance);
                        num_saved = Math.Min((float)bids[i].bid_list[j].gas_num, (float)num_survivors);

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
                if(best_agent != -1 && best_num_saved > 0)
                {
                    Debug.Log("Agent " + best_agent + " has " + agent_objects[best_agent].num_gas_masks + " masks and path length " + best_bid_path + " and can probably save " + best_num_saved + " people");


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
            timer_started = true;
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
