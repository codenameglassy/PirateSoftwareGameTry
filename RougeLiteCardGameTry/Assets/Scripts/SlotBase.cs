using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBase : MonoBehaviour
{
    public enum SlotType
    {
        CardSlot,
        HandSlot
    }
    public SlotType slotType;

    [Header("Components")]
    private SpriteRenderer sr;

    [Header("Selection")]
    public bool isOccupied = false;
  

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetOccupied(bool bool_)
    {
        if (bool_)
        {
            isOccupied = true;
        }
        else
        {
            isOccupied = false;
        }
    }
    private void Start()
    {
        SetAlpha(GameControl.instance.defaultAlpha);
    }

    // Function to set the alpha value
    public void SetAlpha(float alpha)
    {
        Debug.Log("Setting alpha" + alpha);
        if (sr != null)
        {
            // Get the current color
            Color color = sr.color;
            // Set the alpha value
            color.a = alpha;
            // Assign the new color back to the SpriteRenderer
            sr.color = color;
        }
    }

   /* public void AdjustEnergy(bool isAdding)
    {
        if (slotType == SlotType.CardSlot && !isAdding)
        {
            GameControl.instance.TakeEnergy();
        }
        else if (slotType == SlotType.HandSlot && isAdding)
        {
            GameControl.instance.AddEnergy();
        }
    }*/
}
