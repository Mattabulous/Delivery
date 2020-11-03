using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnManager : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Transform[] spawnPoints;
    public List<bool> willSpawn;
    public List<bool> willExtend;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            willSpawn.Add(Random.Range(0, 2) == 1);
            willExtend.Add(Random.Range(0, 2) == 1);
        }

        for(int y = 0; y < willSpawn.Count; y++)
        {
            if(willSpawn[y])
            {
                GameObject go = Instantiate(objectToSpawn, spawnPoints[y]);
                if(!willExtend[y])
                {
                    go.GetComponent<RandomSpawnManager>().enabled = false;
                }
            }    
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
