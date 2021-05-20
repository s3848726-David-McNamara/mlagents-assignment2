using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoCannonFSM : MonoBehaviour
{
    public float shootDistance;
    public float shootTime;
    public GameObject projectile;
    public GameObject[] projectileList;
    public int poolCount;
    public VisionCone vc;
    private float timer;
    private GameObject entity;

    private void Start()
    {
        projectileList = new GameObject[poolCount];
        for (int i = 0; i < poolCount; i++)
        {
            projectileList[i] = Instantiate(projectile, transform.localPosition, transform.localRotation, transform.parent);
            projectileList[i].SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        entity = vc.GetObjectInVisionCone(0f);

        timer -= Time.deltaTime;

        if (entity != null)
        {
            transform.up = (entity.transform.localPosition - transform.localPosition).normalized;

            if (timer < 0)
            {
                timer = shootTime;
                
                foreach (GameObject proj in projectileList)
                {
                    if (!proj.activeSelf)
                    {
                        proj.transform.localPosition = transform.localPosition;
                        proj.transform.localRotation = transform.localRotation;
                        proj.SetActive(true);
                        break;
                    }
                }
            }
            
        }
    }
    

}
