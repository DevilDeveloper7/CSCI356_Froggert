using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firefly : MonoBehaviour
{
    private Animator firefly;
    public Material glow;
    public GameObject pointlight;
    public GameObject halolight;
    public GameObject MainCamera;

    void Start()
    {
        firefly = GetComponent<Animator>();
    }
    void Update()
    {
        if (firefly.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            firefly.SetBool("takeoff", false);
            firefly.SetBool("landing", false);
            firefly.SetBool("fly", false);
            firefly.SetBool("hit", false);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            firefly.SetBool("walk", true);
            firefly.SetBool("idle", false);
        }
        if ((Input.GetKeyUp(KeyCode.W))||(Input.GetKeyUp(KeyCode.A))||(Input.GetKeyUp(KeyCode.D)))
        {
            firefly.SetBool("idle", true);
            firefly.SetBool("walk", false);
            firefly.SetBool("turnleft", false);
            firefly.SetBool("turnright", false);
            firefly.SetBool("fly", true);
            firefly.SetBool("flyleft", false);
            firefly.SetBool("flyright", false);
        }
        if (Input.GetKey("left"))
        {
            glow.EnableKeyword("_EMISSION");
            pointlight.GetComponent<Light>().enabled = true;
            halolight.GetComponent<Light>().enabled = true;
        }
        if (Input.GetKey("right"))
        {
            glow.DisableKeyword("_EMISSION");
            pointlight.GetComponent<Light>().enabled = false;
            halolight.GetComponent<Light>().enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            firefly.SetBool("takeoff", true);
            firefly.SetBool("idle", false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            firefly.SetBool("landing", true);
            firefly.SetBool("fly", false);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            firefly.SetBool("idle", false);
            firefly.SetBool("turnleft", true);
            firefly.SetBool("flyleft", true);
            firefly.SetBool("fly", false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            firefly.SetBool("idle", false);
            firefly.SetBool("turnright", true);
            firefly.SetBool("flyright", true);
            firefly.SetBool("fly", false);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            firefly.SetBool("hit", true);
            firefly.SetBool("idle", false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            firefly.SetBool("die", true);
            firefly.SetBool("idle", false);
        }
        if (Input.GetKeyDown("up"))
        {
            MainCamera.GetComponent<CameraFollow>().enabled = false;
        }
        if (Input.GetKeyUp("up"))
        {
            MainCamera.GetComponent<CameraFollow>().enabled = true;
        }
    }
}
