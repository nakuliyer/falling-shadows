using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    /* Truly public */
    public bool hitBottom = false;
    public bool squashed = false;

    /* Movement */
    public float jumpWaitTime = 0.5f;
    public float upwardsThrust = 6.0f;
    public float upwardsThrustDegradation = 1.0f;
    public float directionalThrust = 3.0f;
    public float directionalThrustDegradation = 0.3f;
    public uint maxJumpCount = 8;

    /* Component */
    public GameObject player;
    private Rigidbody rb;

    /* Player variables */
    private bool isOnGround = false;
    private float lastJumpTime = 0f;
    private uint jumpCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<BoxCollider>().size = player.transform.localScale;

        if (squashed)
        {
            return;
        }

        if (Time.time - lastJumpTime > jumpWaitTime)
        {
            UseKeyboard();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (squashed)
        {
            return;
        }

        float colliderY = collision.gameObject.transform.position.y;
        float squashDistance = transform.localScale.y / 2;

        bool somethingAbove = colliderY - transform.position.y >= squashDistance;
        bool somethingBelow = transform.position.y - colliderY >= squashDistance;

        if (somethingAbove && isOnGround)
        {
            Debug.Log("detected");
            OnSquashed();
        } else if (somethingBelow && collision.gameObject.tag == "Ground")
        {
            OnHitGround();
        }
    }

    /** Uses the Keyboard as a placeholder for mobile input */
    private void UseKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump(Vector3.zero);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump(Vector3.forward);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Jump(Vector3.back);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Jump(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Jump(Vector3.right);
        }
    }

    private void Jump(Vector3 horizontalDirection)
    {
        if (jumpCount < maxJumpCount)
        {
            GetComponentInChildren<Animator>().SetTrigger("About To Jump");
            float actualUpwardsThrust = upwardsThrust - jumpCount * upwardsThrustDegradation;
            float actualDirectionalThrust = directionalThrust - jumpCount * directionalThrustDegradation;
            rb.velocity = Vector3.up * actualUpwardsThrust + horizontalDirection * actualDirectionalThrust;
            lastJumpTime = Time.time;
            ++jumpCount;
            isOnGround = false;
        }
    }

    private void OnSquashed()
    {
        squashed = true;
        GetComponentInChildren<Animator>().SetTrigger("Death");
        rb.velocity = Vector3.zero;
    }

    private void OnHitGround()
    {
        jumpCount = 0;
        isOnGround = true;
        hitBottom = true;
        if (GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            GetComponentInChildren<Animator>().SetTrigger("Player Hit Bottom");
        }
        // Debug.Log("bot");
    }
}
