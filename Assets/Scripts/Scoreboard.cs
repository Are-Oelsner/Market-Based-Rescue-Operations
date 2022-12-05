using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Class that receives information from the Game script about the number of survivors 
public class Scoreboard : MonoBehaviour
{
    // Number of Survivor Groups
    // Array of Survivor Group Stat blocks
    // Start is called before the first frame update
    private TMPro.TextMeshProUGUI[] textFields;

    void Start()
    {
        int numChildren = gameObject.transform.childCount;
        textFields = new TMPro.TextMeshProUGUI[numChildren];
        for (int i = 0; i < numChildren; i++)
        {
            Debug.Log("In Child " + i);
            var child = gameObject.transform.GetChild(i).GetChild(1);
            textFields[i] = child.transform.GetComponent<TMPro.TextMeshProUGUI>();
            textFields[i].text = "Group " + i + "\n?/? saved";
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSurvivorStats(int survivorNum, string survivorData)
    {
        Debug.Log("Survivor Stats for " + survivorNum);
        textFields[survivorNum].text = survivorData;
    }
}
