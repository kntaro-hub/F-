using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Hunger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hungerText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hungerText.text = SequenceMGR.instance.Player.Param.hunger.ToString();
    }
}
