using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaScript : MonoBehaviour
{
    public float xSpeed = .1f;
    public float zSpeed = .1f;
    public ParticleSystem particles;
    public AudioClip freezeClip;

    private float realXSpeed;
    private float realZSpeed;
    private float colorTimer = -1;
    private bool isLit = true;

    Renderer render;
    BoxCollider collider;
    AudioSource audio;
    AudioClip originalClip;
    float xOffset;
    float zOffset;

    void Start()
    {
        render = GetComponent<Renderer>();
        collider = GetComponent<BoxCollider>();
        audio = GetComponent<AudioSource>();
        originalClip = audio.clip;
        realXSpeed = xSpeed;
        realZSpeed = zSpeed;
    }

    // Scroll the lava texture to give the appearance of movement
    void Update()
    {
        xOffset += Time.deltaTime * realXSpeed;
        zOffset += Time.deltaTime * realZSpeed;
        if(xOffset > 1)
            xOffset -= 2;
        if(zOffset > 1)
            zOffset -= 2;
        render.material.mainTextureOffset = new Vector2(xOffset,zOffset);

        if (colorTimer >= 0)
        {
            colorTimer += Time.deltaTime * 2;
            render.material.color = isLit ? Color.white * colorTimer : Color.white * (1 - colorTimer);
            if (colorTimer > 1)
            {
                colorTimer = -1;
                if (isLit)
                {
                    SetSafe(false);
                }
            }
        }
    }

    public void SetSafe(bool isSafe)
    {
        realXSpeed = isSafe ? 0 : xSpeed;
        realZSpeed = isSafe ? 0 : zSpeed;
        tag = isSafe ? "Platform" : "Hazard";
        collider.isTrigger = !isSafe;
        particles.enableEmission = !isSafe;
        if(isSafe) {
            audio.clip = freezeClip;
            audio.loop = false;
            audio.Play();
        }
        else {
            audio.clip = originalClip;
            audio.loop = true;
            audio.Play();
        }
    }

    public void Freeze()
    {
        SetSafe(true);
        isLit = false;
        colorTimer = 0;
    }

    public void Melt()
    {
        isLit = true;
        colorTimer = 0;
    }
}
