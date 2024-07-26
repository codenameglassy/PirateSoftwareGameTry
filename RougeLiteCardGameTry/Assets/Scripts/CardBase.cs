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
    Sequence mySequence;

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
    public enum SlotType
    {
        Player,
        Cpu
    }
    public SlotType mySlotType;

    

    private void Start()
    {
        SpawnCard();

        mainCamera = Camera.main;
        mySequence = DOTween.Sequence();
        //DropCard();
     
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

    private void OnMouseOver()
    {
        if (mySlotType == SlotType.Cpu)
        {
            Debug.LogWarning("Cpu card, cannot interact");
            return;
        }

        if(mySequence != null)
        {
            mySequence.Kill();
        }
        
        if (cardEnergy > GameControl.instance.GetCurrentEnergy() && currentSlotBase.slotType == SlotBase.SlotType.HandSlot && GameControl.instance.currentGameState == GameControl.GameState.PlayerTurn)
        {
            Debug.Log("Not enough energy" + cardEnergy + "/" + GameControl.instance.GetCurrentEnergy());
            return;
        }
        mySequence.Append(transform.DOScale(new Vector2(1.1f, 1.1f), 0.3f));
    }

    private void OnMouseExit()
    {
        if (mySlotType == SlotType.Cpu)
        {
            Debug.LogWarning("Cpu card, cannot interact");
            return;
        }

        mySequence.Append(transform.DOScale(new Vector2(1f, 1f), 0.3f));
    }

    private void OnMouseDown()
    {
        if(mySlotType == SlotType.Cpu)
        {
            Debug.LogWarning("Cpu card, cannot interact");
            return;
        }
        if (cardEnergy > GameControl.instance.GetCurrentEnergy() && currentSlotBase.slotType == SlotBase.SlotType.HandSlot && GameControl.instance.currentGameState == GameControl.GameState.PlayerTurn)
        {
            Debug.Log("Not enough energy" + cardEnergy + "/" + GameControl.instance.GetCurrentEnergy());
            return;
        }
        PickCard();
       
    }

    private void OnMouseDrag()
    {
        if (mySlotType == SlotType.Cpu)
        {
            Debug.LogWarning("Cpu card, cannot interact");
            return;
        }
        if (cardEnergy > GameControl.instance.GetCurrentEnergy() && currentSlotBase.slotType == SlotBase.SlotType.HandSlot && GameControl.instance.currentGameState == GameControl.GameState.PlayerTurn)
        {
            Debug.Log("Not enough energy" + cardEnergy + "/" + GameControl.instance.GetCurrentEnergy());
            return;
        }
        DragCard();
    }

    private void OnMouseUp()
    {
        if (mySlotType == SlotType.Cpu)
        {
            Debug.LogWarning("Cpu card, cannot interact");
            return;
        }
        if (cardEnergy > GameControl.instance.GetCurrentEnergy() && currentSlotBase.slotType == SlotBase.SlotType.HandSlot && GameControl.instance.currentGameState == GameControl.GameState.PlayerTurn)
        {
            Debug.Log("Not enough energy" + cardEnergy + "/" + GameControl.instance.GetCurrentEnergy());
            return;
        }
        ResetAlpha();
        DropCard();
        
    }

    private void SpawnCard()
    {
        GameControl.instance.currentCardsInScene.Add(this);
        SetSortingLayer(picked);
        isDragging = false;
        StartCoroutine(SmoothSpawnToClosestPoint(.15f, mySlotType));
        transform.DOScale(new Vector2(1f, 1f), 0.3f).SetEase(Ease.OutBounce);
        return;
    }

    public void DespawnCard()
    {
        //GameControl.instance.currentCardsInScene.Remove(this);

        if (currentSlotBase != null)
        {
            currentSlotBase.SetOccupied(false);
        }
       
        Destroy(gameObject);
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
        SetSortingLayer(picked);
    }

    void DropCard()
    {
        isDragging = false;
        StartCoroutine(SmoothSnapToClosestPoint(snapSmoothTime));
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

    private IEnumerator SmoothSnapToClosestPoint(float snapSmoothTime_)
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

            while (elapsedTime < snapSmoothTime_)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / snapSmoothTime_);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            SetSortingLayer(placed);
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

    private IEnumerator SmoothSpawnToClosestPoint(float snapSmoothTime_, SlotType mySlotType_)
    {
        float minDistance = Mathf.Infinity;
        Transform closestPoint = null;
        List<Transform> mySnapPoints = new List<Transform>();

        switch (mySlotType_)
        {
            case SlotType.Player:
                mySnapPoints = GameControl.instance.playerHandSnapPoints;
                break;

            case SlotType.Cpu:
                mySnapPoints = GameControl.instance.currentThreeSnapPointsCpu;
                break;
        }

        //yield return new WaitForSeconds(0.1f);

        foreach (Transform snapPoint in mySnapPoints)
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

            while (elapsedTime < snapSmoothTime_)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / snapSmoothTime_);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            SetSortingLayer(placed);
            //GameControl.instance.IncrementOfSnapPointIndex(); //increase currentSnapIndex and set current three snap points...
            GameControl.instance.closestSnapPoint_ = null;

            if (currentSlotBase != null)
            {
                previousSlotBase = currentSlotBase;
            }

            currentSlotBase = closestPoint.GetComponent<SlotBase>();
            currentSlotBase.SetOccupied(true);

            AdjustEnergyOnSlotChange();

        }
    }

   /* private void SetCurrentSlot(SlotBase newSlotBase)
    {
        previousSlotBase = currentSlotBase;
        currentSlotBase = newSlotBase;

        AdjustEnergyOnSlotChange();
    }*/

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

    #endregion

    #region Card Energy

    private void AdjustEnergyOnSlotChange()
    {
       
        switch (mySlotType)
        {
            case SlotType.Player:

                if (previousSlotBase == null || currentSlotBase == null)
                    return;

                if (previousSlotBase.slotType != currentSlotBase.slotType)
                {
                    CheckCardType();
                }
                break;

            case SlotType.Cpu:
                if (currentSlotBase == null)
                    return;

                CheckCardType();
                break;
        }

    }

    private void CheckCardType()
    {
        Debug.Log("Adjusting Slot");

        switch (mySlotType)
        {
            case SlotType.Player:

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

                break;

            case SlotType.Cpu:
                IncreaseCardPowerToStat();

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

    #region Card Power
    public void IncreaseCardPowerToStat()
    {
        switch (mySlotType)
        {
            case SlotType.Player:
                switch (myCardType)
                {
                    case CardTypes.Attack:
                        GameControl.instance.IncreasePlayerAttack(cardPower);
                        break;

                    case CardTypes.Defense:
                        GameControl.instance.IncreasePlayerDefense(cardPower);
                        break;
                }
                break;

            case SlotType.Cpu:
                switch (myCardType)
                {
                    case CardTypes.Attack:
                        GameControl.instance.IncreaseCpuAttack(cardPower);
                        break;

                    case CardTypes.Defense:
                        GameControl.instance.IncreaseCpuDefense(cardPower);
                        break;
                }
                break;
        }
        
    }

    public void DecreaseCardPowerToStat()
    {
        switch (mySlotType)
        {
            case SlotType.Player:
                switch (myCardType)
                {
                    case CardTypes.Attack:
                        GameControl.instance.DecreasePlayerAttack(cardPower);
                        break;

                    case CardTypes.Defense:
                        GameControl.instance.DecreasePlayerDefense(cardPower);
                        break;
                }
                break;

            case SlotType.Cpu:

                break;
        }
     
    }
    #endregion

}
