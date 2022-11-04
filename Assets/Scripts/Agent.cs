using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class Agent : MonoBehaviour
{
    public int num_gas_masks;
    public const float STEP_COST = 0.1f;// Distance traveled in a single step of movement, also amount of time between calls of Path_Nav()

    private Game game;                  // Reference to Game script for using InObstacle
    private float[] dists;              // variable used in early path_nav, can be deleted when A* is implemented TODO
    private Vector3 goal_pos;           // variable used for storing goal position
    public GameObject goal_object;      // public field for setting goal manually through unity
    public GameObject LineRenderer;

    LineRenderer line_renderer;         // Component for drawing lines 
    private int position_number = 0;    // number of positions the agent has been to, used for drawing position history lines
    private Vector3[] position_history = new Vector3[10000]; // Array of positions the agent has visited
    private List<Vector3> a_star_path;
    private int current_timestep = 0;

    void Start()
    {
        game = GameObject.Find("Game").GetComponent(typeof(Game)) as Game;

        // Set up line renderer for drawing paths
        // line_renderer = GameObject.Find("Line").GetComponent(typeof(LineRenderer)) as LineRenderer;  
        //line_renderer = gameObject.GetComponent(typeof(LineRenderer)) as LineRenderer;  
        line_renderer = LineRenderer.GetComponent(typeof(LineRenderer)) as LineRenderer;  
        position_history[position_number++] = gameObject.transform.position; // Set initial position to agent's current position
        Vector3 surv1_pos = GameObject.Find("Group 1").transform.position; 
        Vector3 surv2_pos = GameObject.Find("Group 2").transform.position;
        if(Vector3.Distance(surv1_pos, transform.position) > Vector3.Distance(surv2_pos, transform.position))
        {
            goal_pos = surv2_pos;
        }
        else
        {
            goal_pos = surv1_pos;
        }
        if (goal_object != null)
        {
            goal_pos = goal_object.transform.position;
        }
        InvokeRepeating("Path_Nav", 1.0f, STEP_COST);

        a_star_path = A_star(transform.position, goal_pos);
        Debug.Log(a_star_path[0]);
    }

    // Update is called once per frame
    void Path_Nav()
    {
        // Go to the next position!
        if(current_timestep != a_star_path.Count)
        {
            transform.position = a_star_path[a_star_path.Count - current_timestep - 1];
            current_timestep++;
        }

        // Draw path of movement history, can use this to draw A* paths
        position_history[position_number++] = transform.position;
        DrawPath(position_history);

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
            frontier.RemoveAt(0);

            if(Vector3.Distance(node.position, goal) < STEP_COST/2f)
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
        line_renderer.positionCount = position_number;
        line_renderer.SetPositions(positions);
    }

}
