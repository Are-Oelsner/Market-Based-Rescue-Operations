using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 position;
    public Vector3 parent_pos;
    public float cost;
    public float h_cost;

    public Node(Vector3 pos, Vector3 par_pos, float c, float h)
    {
        position = pos;
        parent_pos = par_pos;
        cost = c; 
        h_cost = h;
    }
}

public class Agent : MonoBehaviour
{
    public float position_x;
    public float position_y;
    public int num_gas_masks;
    public string goal = "Goal 1";
    public const float STEP_COST = 1.0f;

    // Storing helper variables so we don't need to reallocate space for them everytime Path_Nav is called
    private float[] dists;
    private Vector3 goal_pos;
    private Game game;

    void Start()
    {
        game = GameObject.Find("Game").GetComponent(typeof(Game)) as Game;
        InvokeRepeating("Path_Nav", 1.0f, 1.0f);
        Vector3 surv1_pos = GameObject.Find("Circle").transform.position; 
        Vector3 surv2_pos = GameObject.Find("Circle (1)").transform.position;

        if(Vector3.Distance(surv1_pos, transform.position) > Vector3.Distance(surv2_pos, transform.position))
        {
            goal_pos = surv2_pos;
        }
        else
        {
            goal_pos = surv1_pos;
        }
    }

    // Update is called once per frame
    void Path_Nav()
    {

        //Debug.Log(A_star(transform.position, goal_pos));

        if(Vector3.Distance(goal_pos, transform.position) < 0.5f)
        {
            return;
        }

        Vector3[] transforms = {new Vector3(STEP_COST,0,0) + transform.position, new Vector3(-STEP_COST,0,0) + transform.position, new Vector3(0,STEP_COST,0) + transform.position, new Vector3(0,-STEP_COST,0) + transform.position};

        dists = new float[4];

        for (int i = 0; i < 4; i++)
        {
            if (game.InObstacle(transforms[i]))
            {
                Debug.Log("" + i + ": In Obstacle: " + transforms[i]);
                dists[i] = 9999f;
                //dists[i] = Vector3.Distance(transforms[i], goal_pos);
            }
            else
            {
                dists[i] = Vector3.Distance(transforms[i], goal_pos);
            }
        }

        int min_dist = get_min_index(dists);
        transform.position = transforms[min_dist];

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

    //A star search
    private float A_star(Vector3 start, Vector3 goal)
    {
        //List<Vector3> path = new List<Vector3>();

        // frontier is a sorted list with best candidate to expand first
        List<Node> frontier = new List<Node>();
        List<Node> explored = new List<Node>();
        Vector3[] transforms = {new Vector3(1,0,0), new Vector3(-1,0,0), new Vector3(0,1,0), new Vector3(0,-1,0)};

        // first item is its own parent, to indicate end of list
        Node first = new Node(start, start, 0f, Vector3.Distance(start,goal));

        frontier.Add(first);

        while(true)
        {
            // pop the node with the lowest cost
            Node node = frontier[0];
            frontier.RemoveAt(0);

            if(Vector3.Distance(node.position, goal) < 0.5f)
            {
                // if we have found the goal, return the path to it by iterating over the parents, for now just null
                return node.cost;
            }

            foreach(Vector3 t in transforms)
            {
                Vector3 new_pos = node.position + t;
                if(!game.InObstacle(new_pos))
                {
                    //Create child node with distance equal to parent_cost + cost to move from parent to child + heuristic (straight line dist)
                    float act_cost = node.cost + STEP_COST;
                    Node child = new Node(new_pos, node.position, act_cost, act_cost + Vector3.Distance(new_pos, goal));

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

}
