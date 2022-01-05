using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;

public class SeekerAgentOld : Agent
{
    private Rigidbody m_AgentRb;
    private SettingsHideAndSeek m_Settings;
    private ControllerHideAndSeekOld m_GameController;
    public GameObject Controller;
    private HiderAgentOld hiderAgent;
    public GameObject HiderAgent;

    private Animator anim;

    
    public override void Initialize()
    {
        anim = GetComponent<Animator>();
        m_GameController = Controller.GetComponent<ControllerHideAndSeekOld>();
        hiderAgent = HiderAgent.GetComponent<HiderAgentOld>();
        m_AgentRb = GetComponent<Rigidbody>();
        m_Settings = FindObjectOfType<SettingsHideAndSeek>();
    }
    
    
    public override void OnEpisodeBegin()
    {
        //m_AgentRb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ |
        // RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

        //m_AgentRb.constraints = ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ); 
        
        //m_AgentRb.constraints = ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ); 
        //transform.localPosition = new Vector3 (Random.Range(-4f,+1f),0,Random.Range(-2f,+2f));
        //targetTransform.localPosition = new Vector3 (Random.Range(+2f,+5f),0,Random.Range(-2.5f,+5));
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(transform.localRotation);

        //sensor.AddObservation(HiderAgent.transform.localPosition);
        //sensor.AddObservation(HiderAgent.transform.localRotation);
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
                anim.SetFloat("X_speed",1f);
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                anim.SetFloat("X_speed",-1f);
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                anim.SetFloat("Y_speed",-1f);
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                anim.SetFloat("Y_speed",1f);
                break;
        }
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 100f);
        m_AgentRb.AddForce(dirToGo * m_Settings.agentRunSpeed,
            ForceMode.VelocityChange);
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Move the agent using the action.
        MoveAgent(actionBuffers.DiscreteActions);
        AddReward(-1f/MaxStep);
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 5;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 6;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[0] = 4;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("hider"))
        {
            AddReward(10);
            m_GameController.Catched();
            EndEpisode();
            //m_GameController.TouchedHazard(this);
        }
    }
}
