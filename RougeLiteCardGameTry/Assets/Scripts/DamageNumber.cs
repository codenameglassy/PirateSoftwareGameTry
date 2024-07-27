using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class DamageNumber : MonoBehaviour
{
    public TextMeshPro damageNumberText;
    public RectTransform rectTransform;
    public float finalYPos;
   

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            ShowDamageNumber(20);
        }
    }*/

    public void ShowDamageNumber(int damageNumber)
    {
        damageNumberText.text = damageNumber.ToString();

        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + finalYPos, .8f).OnComplete(() => Destroy(gameObject));
    }
}
