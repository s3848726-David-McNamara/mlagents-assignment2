using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public LayerMask visionLayerMask;
    public float locateRadius;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetObjectInVisionCone(float forwardDistance)
    {
        GameObject foundObject = null;

        Collider2D possibleObject = Physics2D.OverlapCircle(transform.localPosition, locateRadius, visionLayerMask);

        //if (possibleObject != null)
        //{
        //    foundObject = possibleObject.gameObject;
        //}

        if (possibleObject != null)
        {
            foundObject = player;
        }

        //foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, locateRadius, visionLayerMask))
        //{
        //    bool hitWall = false;
        //    if (collider.gameObject.CompareTag("Player"))
        //    {



        //        //RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, collider.transform.position - transform.position, VectorUtility.distance(transform.position, collider.transform.position));

        //        //foreach (RaycastHit2D hit in hits)
        //        //{
        //        //    if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("BackGroundWall"))
        //        //    {
        //        //        hitWall = true;
        //        //    }
        //        //}

        //        //if (!hitWall)
        //        //{
        //        //    foundObject = collider.gameObject;
        //        //}
        //    }
        //}
        return foundObject;
    }

    

}
