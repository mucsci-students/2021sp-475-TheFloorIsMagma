using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Light light;
    private bool activated = false;
    private float colorTimer = -1;

    void Start()
    {
        light = GetComponent<Light>();
    }

    void Update()
    {
        if (colorTimer >= 0)
        {
            colorTimer += Time.deltaTime * 2;
            light.color = (Color.red * (1 - colorTimer)) + (Color.green * colorTimer);
            if (colorTimer > 1)
            {
                colorTimer = -1;
                tag = "Untagged";
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !activated)
        {
            activated = true;
            colorTimer = 0;
        }
    }
}
