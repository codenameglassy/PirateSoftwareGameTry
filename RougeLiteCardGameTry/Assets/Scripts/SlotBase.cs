using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBase : MonoBehaviour
{
    [Header("SlotType")]
    public SlotType_ mySlotType_;
    public enum SlotType_
    {
        Player,
        Cpu
    }
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

    [Header("Check Card - Bug Fix")]
    private Transform checkCardPos;
    [SerializeField] private LayerMask whatIsCard;
    [SerializeField] private float checkCardRadius;
    private float waitingTimer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        SetAlpha(GameControl.instance.defaultAlpha);
    }
    private void FixedUpdate()
    {
        if (mySlotType_ == SlotType_.Cpu)
        {
            // return if enemy slot
            return;
        }


        waitingTimer -= Time.deltaTime;
        if (waitingTimer <= 0)
        {
            float waitingTimerMax = .2f; // 70ms
            waitingTimer = waitingTimerMax;
          
            if (isOccupied)
            {
                bool isCardStillThere = Physics2D.OverlapCircle(transform.position, checkCardRadius, whatIsCard);

                if (isCardStillThere)
                {

                }
                else
                {
                    isOccupied = false;
                }
            }

        }

    
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkCardRadius);
    }
}
