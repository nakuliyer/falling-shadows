using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManagerMain : MonoBehaviour
{
    public GameObject player;

    /** this represents a platform type */
    [System.Serializable]
    public class PlatformType
    {
        public GameObject prefab;
        public float maxCountInRow;
        public float timeBetweenPlatformDrops;
        public float fallWeight;
    }
    /** these are all the platforms available */
    public PlatformType[] platformTypes;

    public uint platformQueueSize = 5;

    /** this is for the cue of platforms */
    private class PlatformToDrop
    {
        public PlatformType platformType;
        public float countRemaining;

        public PlatformToDrop(PlatformType platformType, float countRemaining)
        {
            this.platformType = platformType;
            this.countRemaining = countRemaining;
        }
    }
    private Queue<PlatformToDrop> platformQueue;
    private PlatformToDrop currentPlatformToDrop;

    private float lastFallTime = 0.0f;
    private float fallWeightsSums = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        for (uint i = 0; i < platformTypes.Length; ++i)
        {
            fallWeightsSums += platformTypes[i].fallWeight;
        }

        platformQueue = new Queue<PlatformToDrop>();
        for (uint i = 0; i < platformQueueSize; ++i)
        {
            AddNextToQueue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.GetComponent<PlayerMain>().hitBottom)
        {
            return;
        }

        if (currentPlatformToDrop == null)
        {
            currentPlatformToDrop = platformQueue.Dequeue();
            AddNextToQueue();
        }

        if (Time.time - lastFallTime > currentPlatformToDrop.platformType.timeBetweenPlatformDrops)
        {
            GameObject platform = Instantiate(currentPlatformToDrop.platformType.prefab, player.transform.position + new Vector3(0, 10, 0), Quaternion.identity);

            --currentPlatformToDrop.countRemaining;
            // Debug.Log("dropping - count remaining: " + currentPlatformToDrop.countRemaining);
            if (currentPlatformToDrop.countRemaining == 0)
            {
                currentPlatformToDrop = null;
            }
            lastFallTime = Time.time;
        }
    }

    void AddNextToQueue()
    {
        float randomWeight = Random.Range(0.001f, fallWeightsSums);
        float currentWeightSum = 0.0f;
        uint i = 0;
        while (currentWeightSum <= randomWeight && i < platformTypes.Length)
        {
            currentWeightSum += platformTypes[i].fallWeight;
            ++i;
        }
        // Debug.Log("Random Number: " + randomWeight + " - platformType: " + (i - 1));
        PlatformType chosenPlatformType = platformTypes[i - 1];
        PlatformToDrop nextPlatform = new PlatformToDrop(chosenPlatformType, chosenPlatformType.maxCountInRow);
        platformQueue.Enqueue(nextPlatform);
    }
}
