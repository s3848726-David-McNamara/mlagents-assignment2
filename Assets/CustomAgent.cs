using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CustomAgent : Agent
{
    private Rigidbody2D rb;
    private Vector3 startingLocalPosition;
    public Transform targetTransform;
    public int numChecks;
    public float velocityMultiplier = 10;

    /*
     * Starting range is normally set to 3, so a max spawn area of +3 x and -3 x, and +3 y and -3 y.
     * Refer to Tristan if that doesnt make sense. 
     */
    public float startingRange;

    /*
     * Range increment is like 0.05, it just increases the range when a particular number of episodes has elapsed.
     */
    public float rangeIncrement;

    /*
     * This is a really important attribute, since there the only way for an episode to reset is for the agent to get to 
     * the goal it is important to tinker with this attribute, I forgot what I set it to but I think it was like maybe 50 or even 100. IDK, soz 
     */
    public int episodesTillRangeIncrement;

    /*
     * This is set to 10, unless you change the size of the play area, dont change this value.
     */
    public float maxRange;

    private float range;
    private int episodeCount;

    private void Start()
    {
        range = startingRange;
        episodeCount = 0;
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = 0f;
        startingLocalPosition = transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        /*
         * So essentially, each time the agent reaches a goal this will increase the episode count,
         * if then agent reaches the goal enough, the goal will be able to spawn into a larger radius around the map,
         * this basically tricks the agent at the start into collecting / realising that going to the goal is good and thus
         * by the time the goal is starts to spawn far away the agent realises that it needs to go to the goal.
         */
        episodeCount += 1;
        if (episodeCount >= episodesTillRangeIncrement)
        {
            if (range < maxRange)
            {
                range += rangeIncrement;
            }
        }

        /*
         * It would be cool for later experiments to remove this and just have the agent freely move the enviroment.
         */
        transform.localPosition = Vector2.zero;

        ResetTargetPosition();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        //sensor.AddObservation(targetTransform.localPosition);

        //Target offset from Agent
        Vector3 relativeTargetPosition = targetTransform.localPosition - transform.localPosition;
        //var d = Mathf.Sqrt(relativeTargetPosition.x * relativeTargetPosition.x + relativeTargetPosition.y * relativeTargetPosition.y);
        //var xy = 1 / d * relativeTargetPosition;
        //var positionRep = e 
        
        sensor.AddObservation(relativeTargetPosition);

        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(rb.velocity);

        


        /*
         * Possible observations to add: 
         * 1. distance from the agent to the target
         * 2. A raycast or boolean value that is true if there are no walls in the way of the agent and false if there is.
         */
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        /*
         * it would be cool to experiment with continous actions.
         */


        Vector2 movementDirection = Vector2.zero;

        int movement = actionBuffers.DiscreteActions[0];

        if (movement == 0)
        {
            movementDirection.x = 1;
        }
        if (movement == 1)
        {
            movementDirection.x = -1;
        }
        if (movement == 2)
        {
            movementDirection.y = 1;
        }
        if (movement == 3)
        {
            movementDirection.y = -1;
        }

        rb.velocity = movementDirection.normalized * velocityMultiplier * Time.fixedDeltaTime;

        if (movementDirection != Vector2.zero)
        {
            transform.up = rb.velocity;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            AddReward(1.0f);
            EndEpisode();
        }
    }

    public void ResetTargetPosition()
    {
        targetTransform.localPosition = Vector2.zero;
        for (int i = 0; i < numChecks; i++)
        {
            Vector3 possiblePosition = new Vector3(targetTransform.localPosition.x + Random.Range(-range, range), targetTransform.localPosition.y + Random.Range(-range, range), targetTransform.localPosition.z);

            if (!Physics2D.OverlapBox(possiblePosition, targetTransform.localScale, 0f))
            {
                targetTransform.localPosition = possiblePosition;
                break;
            }
        }
    }
}
