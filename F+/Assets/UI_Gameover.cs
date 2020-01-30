using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Gameover : MonoBehaviour
{
    [SerializeField] private GameObject gameover;
    [SerializeField] private Fade fade;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fade.IsTranslucent && !gameover.activeSelf)
        {
            gameover.SetActive(true);
        }
    }
}
