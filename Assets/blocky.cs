using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blocky : MonoBehaviour
{
    [HideInInspector]
    public HiderAgent agent;  //

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spotted(){
        print("Spotted!");
        
    }

    void OnCollisionEnter(Collision col)
    {
        // Touched goal.
        if (col.gameObject.name =="Wall 1")
        {
            //agent.ScoredAGoal();
        }
    }
}
