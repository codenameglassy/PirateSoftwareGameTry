using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class NoEnergy : MonoBehaviour
{
    private void OnEnable()
    {
        transform.DOScale(new Vector2(1.1f, 1.1f), .4f).SetEase(Ease.OutBack).OnComplete(() => transform.DOScale(new Vector2(1, 1), .2f));
    }


}
