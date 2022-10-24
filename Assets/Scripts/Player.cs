using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float position_x;
    public float position_y;
    public int num_gas_masks;

    // Start is called before the first frame update
    void Start(float start_position_x, float start_position_y, int gas_masks)
    {
        position_x = start_position_x;
        position_y = start_position_y;
        num_gas_masks = gas_masks;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void move_to(float goal_x, float goal_y)
    {

    }
}
