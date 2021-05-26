using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class CustomAgent : Agent
{
    private Rigidbody2D rb;
    private Transform childSpriteTransform;
    private Vector3 origin;
    private float healthBonus = 1.0f;

    public Transform targetTransform;
    public int numChecks;
    public float velocityMultiplier = 10;
    public float distanceFadeMultiplier = 0.1f;
    

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
        childSpriteTransform = transform.GetChild(0);
        origin = childSpriteTransform.position;
        episodeCount = 0;
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = 0f;
        
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

        ResetTargetPosition();
        healthBonus = 1.0f;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //calculating the distance representation to Target
        //formula from Week 11 lecture notes
        Vector2 offset = targetTransform.localPosition - transform.localPosition;
        var distance = Mathf.Sqrt(offset.x * offset.x + offset.y * offset.y);
        Vector2 direction = (1 / distance) * offset;
        var positionRep = Mathf.Exp(-1 * distanceFadeMultiplier * distance) * direction;

        // Target and Agent positions
        sensor.AddObservation(positionRep);
        sensor.AddObservation(targetTransform); //TODO put offset
        sensor.AddObservation(transform.localPosition); //TODO rid
        sensor.AddObservation(StepCount / MaxStep);
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
            childSpriteTransform.up = rb.velocity;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //reaches the Target
        if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log($"y,{healthBonus},{StepCount}");

            var file = new
            StreamWriter("E:/Program Files/Unity/Hub/Editor/Projects/mlagents-assignment2/mlagents-assignment2/Observations/obs.csv", append: true);
            file.WriteLine($"y,{healthBonus},{StepCount}");
            file.Close();

            AddReward(healthBonus);
            EndEpisode();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //reaches the Target
        if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log($"y,{healthBonus},{StepCount}");

            var file = new
            StreamWriter("E:/Program Files/Unity/Hub/Editor/Projects/mlagents-assignment2/mlagents-assignment2/Observations/obs.csv", append: true);
            file.WriteLine($"y,{healthBonus},{StepCount}");
            file.Close();

            AddReward(healthBonus);
            EndEpisode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //gets hit by Torpedo
        if (collision.gameObject.CompareTag("Torpedo"))
        {
            if(healthBonus > 0)
            {
                healthBonus -= 0.1f;
            }
            else
            {
                //if health is gone, reset the episode
                Debug.Log($"n,0,{StepCount}");

                var file = new
            StreamWriter("E:/Program Files/Unity/Hub/Editor/Projects/mlagents-assignment2/mlagents-assignment2/Observations/obs.csv", append: true);
                file.WriteLine($"n,0,{StepCount}");
                file.Close();

                this.gameObject.transform.position = origin;
                EndEpisode();
            }


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
