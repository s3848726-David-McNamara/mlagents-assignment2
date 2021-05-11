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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingLocalPosition = transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        //if (transform.localPosition.y < -10 || transform.localPosition.y > 10 ||
        //    transform.localPosition.x < -10 || transform.localPosition.x > 10)
        //{
        //    rb.velocity = Vector2.zero;
        //    transform.localPosition = Vector2.zero;
        //}

        ResetTargetPosition();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //// Actions, size = 2
        //Vector2 controlSignal = Vector2.zero;
        //controlSignal.x = actionBuffers.ContinuousActions[0];
        //controlSignal.y = actionBuffers.ContinuousActions[1];
        //rb.velocity = controlSignal * velocityMultiplier * Time.fixedDeltaTime;

        ////// Rewards
        ////float distanceToTarget = Vector3.Distance(transform.localPosition, targetTransform.localPosition);

        ////// Reached target
        ////if (distanceToTarget < 1.42f)
        ////{
        ////    SetReward(1.0f);
        ////    EndEpisode();
        ////}

        //// Fell off platform
        //if (transform.localPosition.y < -10 || transform.localPosition.y > 10 ||
        //    transform.localPosition.x < -10 || transform.localPosition.x > 10)
        //{
        //    EndEpisode();
        //}
        Vector2 movementDirection = Vector2.zero;

        int movementX = actionBuffers.DiscreteActions[0];
        int movementY = actionBuffers.DiscreteActions[1];

        if (movementX == 1)
        {
            movementDirection.x = 1;
        }
        if (movementX == 2)
        {
            movementDirection.x = -1;
        }
        if (movementY == 1)
        {
            movementDirection.y = 1;
        }
        if (movementY == 2)
        {
            movementDirection.y = -1; 
        }

        rb.velocity = movementDirection.normalized * velocityMultiplier * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            AddReward(1.0f);
            
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("BackgroundWall"))
        {
            rb.velocity = Vector2.zero;
            transform.localPosition = Vector2.zero;
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector2.zero;
            transform.localPosition = Vector2.zero;
            EndEpisode();
        }
    }

    public void ResetTargetPosition()
    {
        targetTransform.localPosition = Vector2.zero;
        for (int i = 0; i < numChecks; i++)
        {
            Vector3 possiblePosition = new Vector3(targetTransform.localPosition.x + Random.Range(-10, 10), targetTransform.localPosition.y + Random.Range(-10, 10), targetTransform.localPosition.z);

            if (!Physics2D.OverlapBox(possiblePosition, targetTransform.localScale, 0f))
            {
                targetTransform.localPosition = possiblePosition;
                break;
            }
        }
    }
}
