using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MessageWindow : MonoBehaviour
{
    // テキストの基
    [SerializeField] TextMeshProUGUI TextMeshPrefab;

    // 表示しているテキスト
    List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();

    // 表示する予定のテキスト
    List<TextMeshProUGUI> reserves = new List<TextMeshProUGUI>();

    // テキストが流れている途中か

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            this.AddMessage("tarako", Color.red);
        }
    }


    public void AddMessage(string text, Color color)
    {
        TextMeshProUGUI textMesh = Instantiate(TextMeshPrefab);
        Vector3 InitPos = textMesh.transform.localPosition;

        textMesh.transform.SetParent(this.transform);

        textMesh.color = color;

        textMesh.text = text;

        foreach(var itr in texts)
        {
            itr.transform.DOLocalMoveY(itr.transform.localPosition.y + 60.0f, 0.5f);
        }

        if (texts.Count > 0)
        {
            textMesh.transform.localPosition = new Vector2(InitPos.x, texts[texts.Count - 1].transform.localPosition.y - 60.0f);
        }
        else
        {
            textMesh.transform.localPosition = new Vector2(InitPos.x, InitPos.y - 60.0f);
        }

        textMesh.transform.DOLocalMoveY(InitPos.y, 0.5f);

        texts.Add(textMesh);
    }

    private IEnumerator TextFadeTimer()
    {
        yield return new WaitForSeconds(0.5f);


    }

    #region singleton

    static MessageWindow _instance;

    public static MessageWindow instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(MessageWindow));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use MidiBridge in the scene hierarchy.");
                    _instance = (MessageWindow)previous;
                }
                else
                {
                    var go = new GameObject("MessageWindow");
                    _instance = go.AddComponent<MessageWindow>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
