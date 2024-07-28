using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameControl.instance.DiscardCards();
    }
}
