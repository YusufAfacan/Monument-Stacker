using DG.Tweening;
using UnityEngine;

public class BrickToBeActivated : MonoBehaviour
{
    void Start()
    {
        transform.DOShakePosition(0.5f, 0.5f);
        transform.DOShakeRotation(0.5f, 0.5f);
        transform.DOShakeScale(0.5f, 0.5f);
    }
}
