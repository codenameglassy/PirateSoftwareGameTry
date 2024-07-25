using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;

    [Header("Snap Points")]
    public List<Transform> availableSnapPoints = new List<Transform>();
    private int currentSnapIndex = 0;
    public List<Transform> currentThreeSnapPoints = new List<Transform>();


    //public Transform previousClosestSnapPoint = null;
    public Transform closestSnapPoint_ = null;

    [Header("Card Alpha")]
    public float selectedAlpha;
    public float defaultAlpha;

    [Header("Cost")]
    public int maxEnergy;
    public TextMeshProUGUI energyAmountText;
    private int currentEnergy;

    [Header("Canvas")]
    public CanvasGroup fadeCanvas;

    [Header("Gameplay")]
    public TextMeshPro playerAttackText;
    public TextMeshPro playerDefenseText;
    private int currentPlayerAttack, currentPlayerDefense;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        

        currentEnergy = maxEnergy;
        energyAmountText.text = currentEnergy.ToString();

        fadeCanvas.alpha = 1.0f;
        fadeCanvas.DOFade(0, 1f);
    }

    private void Update()
    {
        
    }

    #region Energy
    public void ResetEnergy()
    {
        currentEnergy = maxEnergy;
        energyAmountText.text = currentEnergy.ToString();
    }
    public void AddEnergy(int amountToAdd)
    {
        Debug.Log("Adding Energy");
        currentEnergy += amountToAdd;
        energyAmountText.text = currentEnergy.ToString();
    }
    public void TakeEnergy(int amountToTake)
    {
        Debug.Log("Taking Energy");
        currentEnergy -= amountToTake;
        energyAmountText.text = currentEnergy.ToString();
    }
    #endregion

    #region Snap Points
    public void IncrementOfSnapPointIndex()
    {
        currentSnapIndex++;
        SetCurrentSnapIndex(currentSnapIndex);
    }

    // Method to update currentThreeSnapPoints based on currentSnapIndex
    public void UpdateCurrentThreeSnapPoints()
    {
        currentThreeSnapPoints.Clear();
        int startIndex = currentSnapIndex * 3;
        for (int i = startIndex; i < startIndex + 3 && i < availableSnapPoints.Count; i++)
        {
            currentThreeSnapPoints.Add(availableSnapPoints[i]);
        }
    }

    // Example method to change the snap index and update points
    public void SetCurrentSnapIndex(int index)
    {
        currentSnapIndex = index;
        UpdateCurrentThreeSnapPoints();
    }

    #endregion

    public void IncreasePlayerAttack(int amountToIncrease)
    {
        currentPlayerAttack += amountToIncrease;
        playerAttackText.text = currentPlayerAttack.ToString();
    }

    public void DecreasePlayerAttack(int amountToDecrease)
    {
        currentPlayerAttack -= amountToDecrease;
        playerAttackText.text = currentPlayerAttack.ToString();
    }

    public void IncreasePlayerDefense(int amountToIncrease)
    {
        currentPlayerDefense += amountToIncrease;
        playerDefenseText.text = currentPlayerDefense.ToString();

    }

    public void DecreasePlayerDefense(int amountToDecrease)
    {
        currentPlayerDefense -= amountToDecrease;
        playerDefenseText.text = currentPlayerDefense.ToString();
    }

    public void ResetPlayerAttackAndDefense()
    {
        currentPlayerAttack = 0;
        currentPlayerDefense = 0;
    }
}
