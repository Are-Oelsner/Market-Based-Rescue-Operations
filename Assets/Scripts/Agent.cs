using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float position_x;
    public float position_y;
    public int num_gas_masks;
    public string goal = "Goal 1";

    // Storing helper variables so we don't need to reallocate space for them everytime Path_Nav is called
    private float[] dists;
    private Vector3 goal_pos;

    void Start()
    {
        InvokeRepeating("Path_Nav", 1.0f, 1.0f);
        Vector3 surv1_pos = GameObject.Find("Circle").transform.position; 
        Vector3 surv2_pos = GameObject.Find("Circle (1)").transform.position;

        if(Vector3.Distance(surv1_pos, transform.position) > Vector3.Distance(surv2_pos, transform.position))
        {
            goal_pos = surv1_pos;
        }
        else
        {
            goal_pos = surv2_pos;
        }
    }

    // Update is called once per frame
    void Path_Nav()
    {
        bool x = Game.InObstacle(transform.position);
        Debug.Log("Is " + (x ? "" : "not ") + "in obstacle");

        if(Vector3.Distance(goal_pos, transform.position) < 0.5f)
        {
            return;
        }

        Vector3[] transforms = {new Vector3(1,0,0) + transform.position, new Vector3(-1,0,0) + transform.position, new Vector3(0,1,0) + transform.position, new Vector3(0,-1,0) + transform.position};
        // TODO collision checking
        for(int i = 0; i < transforms.Length; i++)
        {
            // If transforms[i]'s node is occupied
            // Remove it from consideration
            // Need to find an efficient way to check for obstacles in an area repeatedly. 
                // Can have all obstacles set up with colliders and then have four square bounding boxes, one on each side of each agent, and then check for collisions.
                // Can potentially keep the same four bounding boxes and move them as the agent moves
        }
        dists = new float[4];

        for(int i = 0; i < 4; i++)
        {
            dists[i] = Vector3.Distance(transforms[i], goal_pos);
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

}
