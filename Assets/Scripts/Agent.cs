using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float position_x;
    public float position_y;
    public int num_gas_masks;

    void Start()
    {
        InvokeRepeating("Path_Nav", 2.0f, 1.0f);
    }

    // Update is called once per frame
    void Path_Nav()
    {
        Vector3 goal_pos = GameObject.Find("Agent (1)").transform.position; 

        if(Vector3.Distance(goal_pos, transform.position) < 0.5f)
        {
            return;
        }

        Vector3[] transforms = {new Vector3(1,0,0) + transform.position, new Vector3(-1,0,0) + transform.position, new Vector3(0,1,0) + transform.position, new Vector3(0,-1,0) + transform.position};
        float[] dists = new float[4];

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
