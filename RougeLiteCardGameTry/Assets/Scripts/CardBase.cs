using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CardBase : MonoBehaviour
{
    [Header("Components")]
    private Camera mainCamera;
    [SerializeField] private SpriteRenderer[] cardParts;
    [SerializeField] private TextMeshPro[] textParts;

    [Header("Drag and Drop")]
    public float dragSmoothTime = 0.01f;
    public float snapSmoothTime = 0.01f;
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3 velocity = Vector3.zero;
    private SlotBase currentSlotBase;
    private SlotBase previousSlotBase;

    [Header("Card")]
    public int cardEnergy;
    public int cardPower;
    public string picked;
    public string placed;
    public CardTypes myCardType;
    public enum CardTypes
    {
        Attack,
        Defense
    }

    private void Start()
    {
        mainCamera = Camera.main;
        DropCard();
    }

    // Update is called once per frame
    private float waitingTimer;
    void FixedUpdate()
    {
        waitingTimer -= Time.deltaTime;
        if (waitingTimer <= 0)
        {
            float waitingTimerMax = .07f; // 70ms
            waitingTimer = waitingTimerMax;

            if (!isDragging)
            {
                return;
            }
            SetClosestSnapPoint(); // mainly for card alpha
        }
    }

    private void OnMouseDown()
    {
        PickCard();
        SetSortingLayer(picked);
    }

    private void OnMouseDrag()
    {
        DragCard();
    }

    private void OnMouseUp()
    {
        ResetAlpha();
        DropCard();
        SetSortingLayer(placed);
    }


    #region Drag and Drop Card

    void PickCard()
    {
        if(GameControl.instance.currentThreeSnapPoints.Count <= 0)
        {
            Debug.LogWarning("No snap points available");
            return;
        }

        if(currentSlotBase != null)
        {
            currentSlotBase.SetOccupied(false);
        }

        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;
        transform.DOScale(new Vector2(1.2f, 1.2f), 0.3f).SetEase(Ease.OutBounce);
    }

    void DropCard()
    {
        isDragging = false;
        StartCoroutine(SmoothSnapToClosestPoint());
        transform.DOScale(new Vector2(1f, 1f), 0.3f).SetEase(Ease.OutBounce);
    }

    void DragCard()
    {
        if (isDragging)
        {
            Vector3 targetPosition = GetMouseWorldPosition() + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, dragSmoothTime);
            
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 10.0f; // Distance from the camera to the object
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    
   

    private IEnumerator SmoothSnapToClosestPoint()
    {
        float minDistance = Mathf.Infinity;
        Transform closestPoint = null;

        foreach (Transform snapPoint in GameControl.instance.currentThreeSnapPoints)
        {
            // Check if the snap point is not occupied
            if (!snapPoint.GetComponent<SlotBase>().isOccupied)
            {
                float distance = Vector3.Distance(transform.position, snapPoint.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = snapPoint;
                    GameControl.instance.closestSnapPoint_ = snapPoint;
                }
            }
        }

        if (closestPoint != null)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = closestPoint.position;
            float elapsedTime = 0f;

            while (elapsedTime < snapSmoothTime)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / snapSmoothTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            //GameControl.instance.IncrementOfSnapPointIndex(); //increase currentSnapIndex and set current three snap points...
            GameControl.instance.closestSnapPoint_ = null;

            if(currentSlotBase != null)
            {
                previousSlotBase = currentSlotBase;
            }
          
            currentSlotBase = closestPoint.GetComponent<SlotBase>();
            currentSlotBase.SetOccupied(true);

            AdjustEnergyOnSlotChange();

        }
    }

    private void SetCurrentSlot(SlotBase newSlotBase)
    {
        previousSlotBase = currentSlotBase;
        currentSlotBase = newSlotBase;

        AdjustEnergyOnSlotChange();
    }

    private void AdjustEnergyOnSlotChange()
    {
        if (previousSlotBase == null || currentSlotBase == null)
            return;

        if (previousSlotBase.slotType != currentSlotBase.slotType)
        {
            CheckCardType();
        }
    }

    private void CheckCardType()
    {
        Debug.Log("Adjusting Slot");
        switch (currentSlotBase.slotType)
        {
            case SlotBase.SlotType.CardSlot:
                GameControl.instance.TakeEnergy(cardEnergy);
                IncreaseCardPowerToStat();
                break;

            case SlotBase.SlotType.HandSlot:
                GameControl.instance.AddEnergy(cardEnergy);
                DecreaseCardPowerToStat();
                break;

        }
    }

    #endregion

    #region Slot Alpha

    private void SetClosestSnapPoint()
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform target in GameControl.instance.currentThreeSnapPoints)
        {
            // Check if the snap point is not occupied
            if (!target.GetComponent<SlotBase>().isOccupied)
            {

                float distance = Vector3.Distance(currentPosition, target.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }
        }

        foreach (Transform target in GameControl.instance.currentThreeSnapPoints)
        {
            SlotBase slotBase = target.GetComponent<SlotBase>();

            if (target == closestTarget && !slotBase.isOccupied)
            {
                slotBase.SetAlpha(GameControl.instance.selectedAlpha);
            }
            else
            {
                slotBase.SetAlpha(GameControl.instance.defaultAlpha);
            }
        }

    }

    public void ResetAlpha()
    {
        foreach (Transform target in GameControl.instance.currentThreeSnapPoints)
        {
            target.GetComponent<SlotBase>().SetAlpha(GameControl.instance.defaultAlpha);
        }
    }
    #endregion

    public void SetSortingLayer(string layerName)
    {
        for (int i = 0; i < cardParts.Length; i++)
        {
            // Change the sorting layer
            cardParts[i].sortingLayerName = layerName;
        }
        for (int i = 0; i < textParts.Length; i++)
        {
            textParts[i].sortingLayerID = SortingLayer.NameToID(layerName);
        }
    }

    public void IncreaseCardPowerToStat()
    {
        switch (myCardType)
        {
            case CardTypes.Attack:
                GameControl.instance.IncreasePlayerAttack(cardPower);
                break;

            case CardTypes.Defense:
                GameControl.instance.IncreasePlayerDefense(cardPower);
                break;
        }
    }

    public void DecreaseCardPowerToStat()
    {
        switch (myCardType)
        {
            case CardTypes.Attack:
                GameControl.instance.DecreasePlayerAttack(cardPower);
                break;

            case CardTypes.Defense:
                GameControl.instance.DecreasePlayerDefense(cardPower);
                break;
        }
    }

}
