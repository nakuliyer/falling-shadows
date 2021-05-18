using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMain : MonoBehaviour
{
    /* Objects */
    public GameObject player;

    /* Animation Time */
    [Serializable]
    public enum AnimationMode
    {
        kZooming,
        kTracking
    }
    public float zoomAnimationTime = 1.5f;
    public float trackAnimationTime = 3.0f;
    public AnimationMode initialAnimationMode = AnimationMode.kTracking;
    public AnimationMode defaultAnimationMode = AnimationMode.kTracking;

    /* Movement */
    public Vector3 optimalDistanceToPlayer = new Vector3(0, 4.0f, -20.0f);
    public Vector3 zoomDistance = new Vector3(0, 2.0f, -5.0f);

    [Tooltip("This is the sensitivity of the camera to moving")]
    public float minAnimationDistance = 0.5f;

    /* Rotation */
    public float rotationSpeed = 0.2f;
    public float rotationDampening = 0.4f;

    /* Privates */
    private bool isMoving = false;
    private AnimationMode animationMode;
    private (Vector3, Vector3) animationEndpoints;
    private uint currentFrame;
    private uint totalFrames;

    // Start is called before the first frame update
    void Start()
    {
        animationEndpoints = (Vector3.zero, transform.position);
        animationMode = initialAnimationMode;
        GetComponentInParent<Animator>().SetTrigger("StartInitialZoom");
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.GetComponent<PlayerMain>().hitBottom || GetComponentInParent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            return;
        }

        float playerY = player.transform.position.y;
        Vector3 optimalPosition;
        if (player.GetComponent<PlayerMain>().squashed)
        {
            optimalPosition = new Vector3(player.transform.position.x, playerY, 0) + zoomDistance;
            animationMode = AnimationMode.kZooming;
        } else
        {
            optimalPosition = new Vector3(0, playerY, 0) + optimalDistanceToPlayer;
            animationMode = AnimationMode.kTracking;
            rotationDampening = 0;
            rotationSpeed = 0.1f;
        }

        if (Vector3.Distance(animationEndpoints.Item2, optimalPosition) > minAnimationDistance) {
            isMoving = true;
            animationEndpoints = (transform.position, optimalPosition);
            currentFrame = 0;

            if (animationMode == AnimationMode.kZooming)
            {
                totalFrames = (uint)(zoomAnimationTime / Time.deltaTime);
            }
            else if (animationMode == AnimationMode.kTracking)
            {
                totalFrames = (uint)(trackAnimationTime / Time.deltaTime);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            AnimatePosition();
        }
        AnimateRotation();
    }

    private void AnimatePosition()
    {
        // Debug.Log("animating from " + animationEndpoints.Item1 + " to " + animationEndpoints.Item2 + " at frame " + currentFrame + " of " + totalFrames);
        if (currentFrame < totalFrames)
        {
            float frameCoverage = Mathf.Sin(Mathf.PI / 2 * currentFrame / totalFrames);
            transform.position = animationEndpoints.Item1 + frameCoverage * (animationEndpoints.Item2 - animationEndpoints.Item1);
            ++currentFrame;
        }
        else
        {
            isMoving = false;
            animationMode = defaultAnimationMode;
        }
    }

    private void AnimateRotation()
    {
        Vector3 rotationDirection = player.transform.position - transform.position;
        rotationDirection.y = 0; // keep strictly horizontal
        Quaternion q = Quaternion.LookRotation(rotationDirection * rotationDampening);
        float rotationStep = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, q, rotationStep);
    }
}
