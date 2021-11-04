using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer FloorMeshrender;
    
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3 (Random.Range(-4f,+1f),0,Random.Range(-2f,+2f));
        targetTransform.localPosition = new Vector3 (Random.Range(+2f,+5f),0,Random.Range(-2.5f,+5));
    }

    
    public override void CollectObservations(VectorSensor sensor)
    {
        // Data AI needs to solve the problem
        // Moving the player (player position, and target position)
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 5f;
        transform.localPosition += new Vector3(moveX,0,moveZ)* Time.deltaTime * moveSpeed;
        AddReward(-1f/MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.name == "PlatformBorder"){
            FloorMeshrender.material = loseMaterial;
            SetReward(-10f);
            EndEpisode();
        }
        if (other.name =="LargeGoal"){
            FloorMeshrender.material = winMaterial;
            SetReward(100f);
            EndEpisode();
        }
        
    }
}
