using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Platform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        // Debug.Log("called super");
        if (transform.position.y < -10)
        {
            Destroy(this.gameObject);
        }
    }
}
