using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;

public class SeekerAgent : Agent
{
    private Rigidbody m_AgentRb;
    private SettingsHideAndSeek m_Settings;
    private ControllerHideAndSeek m_GameController;
    public GameObject Controller;
    public override void Initialize()
    {
        m_GameController = Controller.GetComponent<ControllerHideAndSeek>();
        m_AgentRb = GetComponent<Rigidbody>();
        m_Settings = FindObjectOfType<SettingsHideAndSeek>();
    }
    
    public override void OnEpisodeBegin()
    {
        //transform.localPosition = new Vector3 (Random.Range(-4f,+1f),0,Random.Range(-2f,+2f));
        //targetTransform.localPosition = new Vector3 (Random.Range(+2f,+5f),0,Random.Range(-2.5f,+5));
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];

        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                break;
        }
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * m_Settings.agentRunSpeed,
            ForceMode.VelocityChange);
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Move the agent using the action.
        MoveAgent(actionBuffers.DiscreteActions);

        var RaycastSensor = this.gameObject.transform.GetChild(0);        
        var Output = RayPerceptionSensor.Perceive(RaycastSensor.GetComponent<RayPerceptionSensorComponent3D>().GetRayPerceptionInput());
        var foundSeeker = false;
        for (int i = 0; i<15; i++){
            //print(Output.RayOutputs[i].HitGameObject.name);
            switch (Output.RayOutputs[i].HitTagIndex)
            {
                case 2:
                    foundSeeker=true;
                    print($"The tag {Output.RayOutputs[i].HitGameObject.name} was found!");
                    break;                
                default:
                    print("Looking for tag");
                    break;
            }
        }
        if (foundSeeker){
                AddReward(0.1f);
                foundSeeker=false;
            }
        
        //AddReward(-1f/MaxStep);

        //https://forum.unity.com/threads/how-to-get-rayperceptionsensor-values.1010440/

        // https://docs.unity3d.com/Packages/com.unity.ml-agents@1.0/api/Unity.MLAgents.Sensors.RayPerceptionOutput.RayOutput.html
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("dragon"))
        {
            AddReward(1);
            m_GameController.Catched();
            //m_GameController.TouchedHazard(this);
        }
        
    }
    
}
