using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UI_PlayerImage : MonoBehaviour
{
    public enum Face
    {
        normal = 0,
        laugh,
        damage,
        max
    }

    [SerializeField, Tooltip("表情画像")]
    private Sprite[] ExpressionImages = new Sprite[(int)Face.max];

    // 表示画像
    private Image expression = null;

    private Vector2 initPos;

    // Start is called before the first frame update
    void Start()
    {
        this.expression = this.GetComponent<Image>();
        this.initPos = this.expression.rectTransform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeExpression(Face face, bool isShake, bool isKeep)
    {
        this.expression.sprite = this.ExpressionImages[(int)face];

        if(isShake)
        {
            this.expression.rectTransform.DOShakePosition(0.3f, 10.0f, 25).SetEase(Ease.InBounce);
            StartCoroutine(this.FixPos());
        }

        if(!isKeep)
        {
            StartCoroutine(this.ReturnExpression());
        }
    }
    public void ChangeExpression(Face face, bool isShake)
    {
        this.ChangeExpression(face, isShake, false);
    }
    public void ChangeExpression(Face face)
    {
        this.ChangeExpression(face, false, false);
    }

    private IEnumerator FixPos()
    {
        yield return new WaitForSeconds(0.3f);

        this.expression.rectTransform.transform.position = this.initPos;
    }

    private IEnumerator ReturnExpression()
    {
        yield return new WaitForSeconds(0.6f);

        this.expression.sprite = this.ExpressionImages[(int)Face.normal];
    }
}
