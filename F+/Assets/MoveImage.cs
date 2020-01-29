using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MoveImage : MonoBehaviour
{
    [SerializeField] private Vector3 MoveValue;
    private Vector3 MovedPoint;
    [SerializeField] private float MoveTime = 1.0f;
    
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponent<Image>();
        MovedPoint = this.transform.position;
        this.transform.position += MoveValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveStart()
    {
        this.image.rectTransform.DOMove(MovedPoint, MoveTime);
    }
}
