using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public static float fadeTime = 1.0f;
    private Image fadeImage = null;
    GuassianBlurEffect guass = null;
    private bool isTranslucent = false;
    public bool IsTranslucent
    {
        get { return isTranslucent; }
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeImage = this.GetComponent<Image>();
        guass = Camera.main.transform.GetComponent<GuassianBlurEffect>();
            
        this.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown( KeyCode.H))
        {
            this.FadeOut_Reset("Interval");
        }
    }

    public void Translucent()
    {
        fadeImage.color = Color.clear;
        fadeImage.DOColor(new Color(0.0f, 0.0f, 0.0f, 0.5f), fadeTime);
        guass.FadeInBlur();

        StartCoroutine(this.TranslucentTimer());
    }

    private IEnumerator TranslucentTimer()
    {
        yield return new WaitForSeconds(fadeTime);

        isTranslucent = true;
    }

    public void FadeIn()
    {// 明るくなるほう
        fadeImage.color = Color.black;
        fadeImage.DOColor(Color.clear, fadeTime);
        guass.FadeInBlur();
    }

    public void FadeOut_Reset(string sceneName)
    {// 暗くなるほう
        fadeImage.color = Color.clear;
        fadeImage.DOColor(Color.black, fadeTime);
        guass.FadeOutBlur();
        StartCoroutine(FadeInCoroutine(sceneName));
    }

    public void FadeOut_Reset()
    {// 暗くなるほう
        fadeImage.color = Color.clear;
        fadeImage.DOColor(Color.black, fadeTime);
        guass.FadeOutBlur();
        StartCoroutine(FadeInCoroutine());
    }

    public void FadeOut(string sceneName)
    {// 暗くなるほう
        fadeImage.DOColor(Color.black, fadeTime);
        guass.FadeOutBlur();
        StartCoroutine(FadeInCoroutine(sceneName));
    }

    public void FadeOut()
    {// 暗くなるほう
        fadeImage.DOColor(Color.black, fadeTime);
        guass.FadeOutBlur();
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(fadeTime);

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeInCoroutine()
    {
        yield return new WaitForSeconds(fadeTime);

        FloorMGR.instance.NextFloor();
    }

    #region singleton

    static Fade _instance;

    public static Fade instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(Fade));
                if (previous)
                {
                    _instance = (Fade)previous;
                }
                else
                {
                    var go = new GameObject("Fade");
                    _instance = go.AddComponent<Fade>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
