using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntervalMGR : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        SoundMGR.StopBgm();
        Fade.instance.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
