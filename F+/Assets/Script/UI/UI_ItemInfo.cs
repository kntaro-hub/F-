using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_ItemInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textPrefab;

    private Color PanelInitColor = new Color();
    private Color TextInitColor = new Color();
    private TextMeshProUGUI text;
    private Image panel = null;
    // Start is called before the first frame update
    void Start()
    {
        panel = this.GetComponent<Image>();
        text = Instantiate(textPrefab, this.transform);

        TextInitColor = text.color;
        PanelInitColor = panel.color;

        text.rectTransform.sizeDelta = new Vector2(panel.rectTransform.sizeDelta.x - 10.0f, panel.rectTransform.sizeDelta.y + 4.0f);

        text.rectTransform.position = panel.rectTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        panel.color = PanelInitColor;
        text.color = TextInitColor;
    }

    public void Hide()
    {
        panel.color = Color.clear;
        text.color = Color.clear;
    }

    public void SetText(string String)
    {
        // テキストの内容を更新
        text.text = String;
    }
}
