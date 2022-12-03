using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survivors : MonoBehaviour
{
    public float num_survivors;
    private float orig_num;
    // Start is called before the first frame update
    void Start()
    {
        orig_num = num_survivors;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetOrigSurvivors()
    {
        return orig_num;
    }
}
