using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System.Linq;

public class ControllerHideAndSeekOld : MonoBehaviour
{
    [System.Serializable]
    public class SeekerInfo
    {
        public SeekerAgentOld Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
        [HideInInspector]
        public Collider Col;
    }

    [System.Serializable]
    public class HiderInfo
    {
        public HiderAgentOld Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
        [HideInInspector]
        public Collider Col;
    }

    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    /// <returns></returns>
    [Header("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;
    private int m_ResetTimer;

    /// <summary>
    /// The area bounds.
    /// </summary>
    [HideInInspector]
    public Bounds areaBounds;
    /// <summary>
    /// The ground. The bounds are used to spawn the elements.
    /// </summary>
    public GameObject ground;

    Material m_GroundMaterial; //cached on Awake()

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>
    Renderer m_GroundRenderer;

    public List<SeekerInfo> SeekersList = new List<SeekerInfo>();
    public List<HiderInfo> HidersList = new List<HiderInfo>();
    private Dictionary<PushAgentEscape, SeekerInfo> m_PlayerDict = new Dictionary<PushAgentEscape, SeekerInfo>();
    public bool UseRandomAgentRotation = true;
    public bool UseRandomAgentPosition = true;
    SettingsHideAndSeek HideAndSeekSettings;

    private int m_NumberOfRemainingPlayers;
    private SimpleMultiAgentGroup m_AgentGroup;

    private TotalAccuratedScaledTimer totalAccuratedScaledTimer;

    public GameObject[] MapArray;
    
    void Start()
    {

        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;
        // Get the ground renderer so we can change the material when a goal is scored
        m_GroundRenderer = ground.GetComponent<Renderer>();
        // Starting material
        m_GroundMaterial = m_GroundRenderer.material;
        HideAndSeekSettings = FindObjectOfType<SettingsHideAndSeek>();

        //Reset Players Remaining
        m_NumberOfRemainingPlayers = SeekersList.Count;

        // Initialize TeamManager
        /*
        m_AgentGroup = new SimpleMultiAgentGroup();
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            item.Col = item.Agent.GetComponent<Collider>();
            // Add to team manager
            m_AgentGroup.RegisterAgent(item.Agent);
        }
        foreach (var item in DragonsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.T = item.Agent.transform;
            item.Col = item.Agent.GetComponent<Collider>();
        }
        */
        ResetScene();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            //m_AgentGroup.GroupEpisodeInterrupted();
            
            ResetScene();
        }
        //totalAccuratedScaledTimer.Step();
        //print("timer"+totalAccuratedScaledTimer.timer);
    }

    public void TouchedHazard(SeekerAgent agent)
    {
        m_NumberOfRemainingPlayers--;
        if (m_NumberOfRemainingPlayers == 0)
        {
            m_AgentGroup.EndGroupEpisode();
            ResetScene();
        }
        else
        {
            agent.gameObject.SetActive(false);
        }
    }

    public void UnlockDoor()
    {
        m_AgentGroup.AddGroupReward(1f);
        StartCoroutine(GoalScoredSwapGroundMaterial(HideAndSeekSettings.goalScoredMaterial, 0.5f));

        print("Unlocked Door");
        m_AgentGroup.EndGroupEpisode();

        ResetScene();
    }

    public void Catched()
    {
        print("Catched()");
        //m_AgentGroup.AddGroupReward(1f);
        StartCoroutine(GoalScoredSwapGroundMaterial(HideAndSeekSettings.goalScoredMaterial, 0.5f));
        ResetScene();
    }

    /// <summary>
    /// Use the ground's bounds to pick a random spawn position.
    /// </summary>
    public Vector3 GetRandomSpawnPos()
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false)
        {
            var randomPosX = Random.Range(-areaBounds.extents.x * HideAndSeekSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.x * HideAndSeekSettings.spawnAreaMarginMultiplier);

            var randomPosZ = Random.Range(-areaBounds.extents.z * HideAndSeekSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.z * HideAndSeekSettings.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 0.79f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f)) == false)
            {
                foundNewSpawnLocation = true;
            }
        }
        return randomSpawnPos;
    }

    /// <summary>
    /// Swap ground material, wait time seconds, then swap back to the regular material.
    /// </summary>
    IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time); // Wait for 2 sec
        m_GroundRenderer.material = m_GroundMaterial;
    }

    public void BaddieTouchedBlock()
    {
        m_AgentGroup.EndGroupEpisode();

        // Swap ground material for a bit to indicate we scored.
        StartCoroutine(GoalScoredSwapGroundMaterial(HideAndSeekSettings.failMaterial, 0.5f));
        ResetScene();
    }

    Quaternion GetRandomRot()
    {
        return Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
    }

    // Randomly activates an inactive game object    
    
    public void ActivateRandomMap()
    {
        for(int i = 0; i < MapArray.Length; i++)
         {
             MapArray[i].SetActive(false);
         }
     GameObject selection = MapArray.Where(i=>!i.activeSelf).OrderBy(n=>Random.value).FirstOrDefault();
 
     // selection will be null if all game objects are already active
     if (selection != null) selection.SetActive(true);
    }

    void ResetScene()
    {
        ActivateRandomMap();  
        //print("Reset Scene()");

        //Reset counter
        m_ResetTimer = 0;

        //Reset Players Remaining
        m_NumberOfRemainingPlayers = SeekersList.Count;

        //Random platform rot
        var rotation = Random.Range(0, 4);
        var rotationAngle = rotation * 90f;
        transform.Rotate(new Vector3(0f, rotationAngle, 0f));

        var pos = UseRandomAgentPosition ? GetRandomSpawnPos() : SeekersList[0].StartingPos;
        var rot = UseRandomAgentRotation ? GetRandomRot() : SeekersList[0].StartingRot;

        SeekersList[0].Agent.transform.SetPositionAndRotation(pos, rot);
        //AgentsList[0].Rb.velocity = Vector3.zero;
        //AgentsList[0].Rb.angularVelocity = Vector3.zero;
        
        //AgentsList[0].Agent.SetRandomWalkSpeed();
            //m_AgentGroup.RegisterAgent(item.Agent);
            
        pos = UseRandomAgentPosition ? GetRandomSpawnPos() : HidersList[0].StartingPos;
        rot = UseRandomAgentRotation ? GetRandomRot() : HidersList[0].StartingRot;
        HidersList[0].Agent.transform.SetPositionAndRotation(pos, rot);
/*
        //Reset Agents
        
        foreach (var item in AgentsList)
        {
            var pos = UseRandomAgentPosition ? GetRandomSpawnPos() : item.StartingPos;
            var rot = UseRandomAgentRotation ? GetRandomRot() : item.StartingRot;

            item.Agent.transform.SetPositionAndRotation(pos, rot);
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
            item.Agent.gameObject.SetActive(true);
            //m_AgentGroup.RegisterAgent(item.Agent);
        }
        
        
        //End Episode
        foreach (var item in DragonsList)
        {
            if (!item.Agent)
            {
                return;
            }
            item.Agent.transform.SetPositionAndRotation(item.StartingPos, item.StartingRot);
            item.Agent.SetRandomWalkSpeed();
            item.Agent.gameObject.SetActive(true);
        }
        */
    }
}
