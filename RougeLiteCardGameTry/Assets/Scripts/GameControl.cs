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
    public List<Transform> playerHandSnapPoints = new List<Transform>();
    public List<Transform> currentThreeSnapPoints = new List<Transform>();
    public List<Transform> currentThreeSnapPointsCpu = new List<Transform>();
    public Transform closestSnapPoint_ = null;

    [Header("Card Alpha")]
    public float selectedAlpha;
    public float defaultAlpha;

    [Header("Cost")]
    public int maxEnergy;
    public TextMeshProUGUI energyAmountText;
    public TextMeshPro energyAmountText_;
    private int currentEnergy;

    [Header("Canvas")]
    public CanvasGroup fadeCanvas;
    public GameObject gameover;
    public GameObject gamewin;

    [Header("Gameplay - General")]
    public List<CardBase> currentCardsInScene = new List<CardBase>();
    public enum GameState
    {
        DrawingCard,
        PlayerTurn,
        EndingTurn,
        GameEnd
    }
    public GameState currentGameState;

    [Header("Gameplay - Player")]
    public TextMeshPro playerAttackText;
    public TextMeshPro playerDefenseText;
    private int currentPlayerAttack, currentPlayerDefense;

    [Header("Gameplay - Enemy")]
    public TextMeshPro cpuAttackText;
    public TextMeshPro cpuDefenseText;
    private int currentCpuAttack, currentCpuDefense;

   

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentEnergy = maxEnergy;
        energyAmountText.text = currentEnergy.ToString();
        energyAmountText_.text = currentEnergy.ToString();

        fadeCanvas.alpha = 1.0f;
        fadeCanvas.DOFade(0, 1f);

        StartCoroutine(Enum_StartGame());
    }


    #region Gameplay
    
    IEnumerator Enum_StartGame()
    {
        yield return new WaitForSeconds(2f);
        SetupCard();
    }

    public void SetupCard()
    {
        StartCoroutine(Enum_SetupCard());
    }

    IEnumerator Enum_SetupCard()
    {
        currentGameState = GameState.DrawingCard;
        yield return Enum_ResetTurn(); //Reset Turn
        yield return CardDrawManager.instance.Enum_CpuDrawCard(); //Draw Cpu Card
        yield return CardDrawManager.instance.Enum_PlayerDrawCard(); //Draw Player Card
        yield return Enum_TweenButtons(); // Show Buttons
        currentGameState = GameState.PlayerTurn;
    }

    IEnumerator Enum_ResetTurn()
    {
        yield return new WaitForEndOfFrame();
        ResetEnergy();
        ResetPlayerStats();
        ResetCpuStat();
    }

    IEnumerator Enum_TweenButtons()
    {
        yield return new WaitForEndOfFrame();
    }

    public void EndTurnButton()
    {
        StartCoroutine(Enum_EndTurn());
    }

    IEnumerator Enum_EndTurn()
    {
        currentGameState = GameState.EndingTurn;
        Debug.Log("Ending Turn");

        //attack player
        yield return Enum_PlayerAttack();
        //Cpu Attacks
        yield return Enum_CpuAttack();
        //reset stats
        CameraShake.instance.ShakeCamera();
        yield return Enum_ResetTurn();
        //desapwn Cards
        yield return Enum_DespawnCards();

        currentCardsInScene.Clear();
        yield return new WaitForSeconds(.5f);

        //setup cards
        SetupCard();
    }

    IEnumerator Enum_PlayerAttack()
    {
        yield return null;

        if (PlayerHealth.instance.GetCurrentHealth() <= 0)
        {
            yield break;
        }

        //calculate damage
        int currentPlayerDamage = currentPlayerAttack - currentCpuDefense;
        if(currentPlayerDamage <= 0)
        {
            currentPlayerDamage = 0;
        }

        Debug.Log("Player doing damage : " + currentPlayerDamage);
        //do damage to enemy
        FindFirstObjectByType<EnemyHealth>().TakeDamage(currentPlayerDamage);
    }

    IEnumerator Enum_CpuAttack()
    {
        yield return null;

        if(EnemyHealth.instance.GetCurrentHealth() <= 0)
        {
           yield break;
        }

        //calculate damage
        int currentCpuDamage = currentCpuAttack - currentPlayerDefense;
        if (currentCpuDamage <= 0)
        {
            currentCpuDamage = 0;
        }
        Debug.Log("Cpu doing damage : " + currentCpuDamage);
        //do damage to player
        FindFirstObjectByType<PlayerHealth>().TakeDamage(currentCpuDamage);
    }

    IEnumerator Enum_DespawnCards()
    {
        int currentCardsInSceneCount = currentCardsInScene.Count;
        Debug.Log("Despawning Cards");
        for (int i = 0; i < currentCardsInSceneCount; i++)
        {
            currentCardsInScene[i].DespawnCard();
            yield return new WaitForSeconds(0.1f);
        }

    }

    #endregion

    #region Energy
    public int GetCurrentEnergy()
    {
        return currentEnergy;
    }
    public void ResetEnergy()
    {
        currentEnergy = maxEnergy;
        energyAmountText.text = currentEnergy.ToString();
        energyAmountText_.text = currentEnergy.ToString();
    }
    public void AddEnergy(int amountToAdd)
    {
        Debug.Log("Adding Energy");
        currentEnergy += amountToAdd;
        energyAmountText.text = currentEnergy.ToString();
        energyAmountText_.text = currentEnergy.ToString();
    }
    public void TakeEnergy(int amountToTake)
    {
        Debug.Log("Taking Energy");
        currentEnergy -= amountToTake;
        energyAmountText.text = currentEnergy.ToString();
        energyAmountText_.text = currentEnergy.ToString();
    }
    #endregion

    #region Snap Points





    #endregion

    #region Player Stats

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

    public void ResetPlayerStats() //Reset Player Stat
    {
        currentPlayerAttack = 0;
        currentPlayerDefense = 0;
        playerAttackText.text = currentPlayerAttack.ToString();
        playerDefenseText.text = currentPlayerDefense.ToString();
    }

    #endregion

    #region Enemy Stats
    public void IncreaseCpuAttack(int amountToIncrease)
    {
        currentCpuAttack += amountToIncrease;
        cpuAttackText.text = currentCpuAttack.ToString();
    }

    public void IncreaseCpuDefense(int amountToIncrease)
    {
        currentCpuDefense += amountToIncrease;
        cpuDefenseText.text = currentCpuDefense.ToString();
    }

    public void ResetCpuStat()
    {
        currentCpuAttack = 0;
        currentCpuDefense = 0;
        cpuAttackText.text = currentCpuAttack.ToString();
        cpuDefenseText.text = currentCpuDefense.ToString();
    }

    #endregion
}
