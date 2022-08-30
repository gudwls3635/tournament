using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Set : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution(1080, 1920, false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
