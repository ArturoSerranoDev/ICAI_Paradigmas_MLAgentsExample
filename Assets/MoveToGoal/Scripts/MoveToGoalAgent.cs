using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform agentTransform;
    
    [SerializeField] private float moveSpeed = 2f;


    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        
        agentTransform.localPosition = new Vector3(0, 1, 0);
        targetTransform.localPosition = new Vector3(UnityEngine.Random.Range(-4f, 4f), 1, UnityEngine.Random.Range(-4f, 4f));
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(agentTransform.localPosition);
        
        SetReward(0.001f);
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        
        var action = actions.DiscreteActions[0];
        Vector3 dirToGo = Vector3.zero;

        switch (action)
        {
            case 1:
                dirToGo = agentTransform.forward * 1f;
                break;
            case 2:
                dirToGo = agentTransform.forward * -1f;
                break;
            case 3:
                dirToGo = agentTransform.right * -1f;
                break;
            case 4:
                dirToGo = agentTransform.right * 1f;
                break;
        }
        
        agentTransform.localPosition += dirToGo * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(in actionsOut);
        
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        agentTransform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(1);
            EndEpisode();
        }
        else if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-0.1f);
            EndEpisode();
        }
    }
  
}
