using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class Node
{
    public Vector3 position;
    public Node parent;
    public float cost;
    public float h_cost;

    public Node(Vector3 pos, Node parent_node, float c, float h)
    {
        position = pos;
        parent = parent_node;
        cost = c; 
        h_cost = h;
    }

}

public class Bid
{
    public float path_distance;
    public int gas_num;

    public Bid(float p, int n)
    {
        path_distance = p;
        gas_num = n;
    }
}

public class Agent : MonoBehaviour
{
    public int num_gas_masks;
    public const float STEP_COST = 1.0f;// Distance traveled in a single step of movement, also amount of time between calls of Path_Nav()

    private Game game;                  // Reference to Game script for using InObstacle
    private float[] dists;              // variable used in early path_nav, can be deleted when A* is implemented TODO
    public Vector3 goal_pos;           // variable used for storing goal position
    public GameObject goal_object;      // public field for setting goal manually through unity
    public GameObject LineRenderer;

    LineRenderer line_renderer;         // Component for drawing lines 
    private int position_number = 0;    // number of positions the agent has been to, used for drawing position history lines
    private Vector3[] position_history = new Vector3[10000]; // Array of positions the agent has visited
    private List<Vector3> a_star_path;
    private int current_timestep = 0;
    private bool path_assigned = false;
    private bool bids_submitted = false;
    private int survivor_assigned_to;

    private Survivors[] survivor_objects;

    Vector3[] a_star_path_array;

    void Start()
    {
        //game = GameObject.Find("Game").GetComponent(typeof(Game)) as Game;

        // Set up line renderer for drawing paths
        // line_renderer = GameObject.Find("Line").GetComponent(typeof(LineRenderer)) as LineRenderer;  
        //line_renderer = gameObject.GetComponent(typeof(LineRenderer)) as LineRenderer;  
        line_renderer = LineRenderer.GetComponent(typeof(LineRenderer)) as LineRenderer;  
        position_history[position_number++] = gameObject.transform.position; // Set initial position to agent's current position


        InvokeRepeating("Path_Nav", 1.0f, .5f*STEP_COST);
    }

    public int GetInd()
    {
        return transform.GetSiblingIndex();
    }

    public void SetGame(Game game_script, Survivors[] s)
    {
        game = game_script;
        survivor_objects = s;
    }

    public void AssignPath(int survivor_num)
    {
        // recompute the path to that group of survivors, instead of storing every path
        goal_pos = GameObject.Find("Survivors").transform.GetChild(survivor_num).gameObject.transform.position;
        survivor_assigned_to = survivor_num;
        Debug.Log("Agent " + GetInd() + " assigned to Survivor " + survivor_num);
        a_star_path = A_star(transform.position, goal_pos);
        a_star_path_array = a_star_path.ToArray();
        DrawPath(a_star_path_array);
        path_assigned = true;
    }

    // Update is called once per frame
    public void Path_Nav()
    {       
        // This essentially replaces init function, ensures everything necessary has been initialized
        if(!bids_submitted)
        {
            // get a list of lengths to the goal
            List<float> path_lens = new List<float>();

            GameObject survivors_parent = GameObject.Find("Survivors");
            int num_survivor_groups = survivors_parent.transform.childCount;
            GameObject[] survivor_groups = new GameObject[num_survivor_groups];
            float path_len;

            List<Bid> survivor_bids = new List<Bid>();
            for (int i = 0; i < num_survivor_groups; i++)
            {
                survivor_groups[i] = survivors_parent.transform.GetChild(i).gameObject;
                List<Vector3> a_s = A_star(transform.position, survivor_groups[i].transform.position);

                if(a_s == null)
                {
                    path_len = game.hard_limit;
                }
                else
                {
                    path_len = a_s.Count;
                }

                survivor_bids.Add(new Bid(path_len, num_gas_masks));
                //Debug.Log(gameObject.name + ": " + survivor_distances[i]);
            }
            // add your bids to the game
            game.AddBids(transform.GetSiblingIndex(), survivor_bids);
            bids_submitted = true;
        }

        // Go to the next position!
        if(path_assigned && current_timestep != a_star_path.Count)
        {
            transform.position = a_star_path[a_star_path.Count - current_timestep - 1];
            current_timestep++;

            if(current_timestep == 1)
            {
                Debug.Log("Agent " + GetInd() + " saved " + game.GetNumSaved(survivor_assigned_to, a_star_path.Count, num_gas_masks) + " people");
            }
        }

        // Draw path of movement history, can use this to draw A* paths
        //position_history[position_number++] = transform.position;
        //DrawPath(position_history);

    }

    int get_min_index(float[] arr)
    {
        int min_ind = 0;
        for(int i = 0; i<arr.Length; i++)
        {
            if(arr[i] < arr[min_ind])
            {
                min_ind = i;
            }

        }
        return min_ind;
    }

    // Add the node according to its estimated cost
    private void insert_to_frontier(List<Node> list, Node to_add)
    {
        int i;
        for(i = 0; i < list.Count; i++)
        {
            if(to_add.h_cost < list[i].h_cost)
            {
                break;
            }
        }

        if(i == list.Count)
        {
            list.Add(to_add);
        }
        else
        {
            list.Insert(i, to_add);
        }
    }

    private (bool, int) is_in_list(List<Node> list, Vector3 pos)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if(list[i].position == pos)
            {
                return (true, i);
            }
        }
        return (false, -1);
    }

    private List<Vector3> Solution(Node node)
    {
        List<Vector3> ret = new List<Vector3>();

        while(node.parent != null)
        {
            ret.Add(node.position);
            node = node.parent;
        }
        return ret;
    }

    //A star search
    private List<Vector3> A_star(Vector3 start, Vector3 goal)
    {
        //List<Vector3> path = new List<Vector3>();

        // frontier is a sorted list with best candidate to expand first
        List<Node> frontier = new List<Node>();
        List<Node> explored = new List<Node>();
        Vector3[] transforms = {new Vector3(STEP_COST,0,0), new Vector3(-STEP_COST,0,0), new Vector3(0,STEP_COST,0), new Vector3(0,-STEP_COST,0)};

        // first item is its own parent, to indicate end of list
        Node first = new Node(start, null, 0f, Vector3.Distance(start,goal));

        frontier.Add(first);

        while(true)
        {
            // pop the node with the lowest cost
            Node node = frontier[0];

            if(node.cost >= game.hard_limit)
            {
                return null;
            }

            frontier.RemoveAt(0);

            if(Vector3.Distance(node.position, goal) < STEP_COST)
            {
                // if we have found the goal, return the path to it by iterating over the parents
                //return node.cost;
                return Solution(node);
            }

            foreach(Vector3 t in transforms)
            {
                Vector3 new_pos = node.position + t;
                if(!game.InObstacle(new_pos)) // TODO uncomment
                {
                    //Create child node with distance equal to parent_cost + cost to move from parent to child + heuristic (straight line dist)
                    float act_cost = node.cost + STEP_COST;
                    Node child = new Node(new_pos, node, act_cost, act_cost + Vector3.Distance(new_pos, goal));

                    // if the child is in explored, skip it
                    if(!is_in_list(explored, new_pos).Item1)
                    {
                        // if child is not in explored and is in frontier with higher cost, replace it
                        (bool, int) in_frontier = is_in_list(frontier, new_pos);
                        if(in_frontier.Item1 && frontier[in_frontier.Item2].cost > child.cost)
                        {
                            frontier.RemoveAt(in_frontier.Item2);
                            insert_to_frontier(frontier, child);
                        }
                        // if not in frontier, add to frontier at correct location
                        else if(!in_frontier.Item1)
                        {
                            insert_to_frontier(frontier, child);
                        }
                    }
                }
                // If in obstacle, skip this child
            }
            // add the current expanded node to the explored list
            explored.Add(node);
        }

        //add four nearest to the frontier, if they are not an obstacle

    }

    // Given an array of Vector3 positions, draw a line through the given points
    public void DrawPath(Vector3[] positions)
    {
        line_renderer.positionCount = positions.Length;
        line_renderer.SetPositions(positions);
    }

}
