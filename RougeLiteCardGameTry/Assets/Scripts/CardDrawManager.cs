using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrawManager : MonoBehaviour
{
    public static CardDrawManager instance;

    [Header("Card Withdraw - General")]
    public List<CardBase> playerAvailableCards = new List<CardBase>();
    public List<CardBase> cpuAvailableCards = new List<CardBase>();
    public Transform cardSpawnPos, cardSpawnPosCpu;

    [Header("Enemy")]
    [Range(0,1)]
    public float lightAttackProbability = 0.6f;
    [Range(0, 1)]
    public float heavyAttackProbability = 0.2f;
    [Range(0, 1)]
    public float defendProbability = 0.2f;
    public enum EnemyAttackState 
    {
        Attack,
        Defend,
        HeavyAttack
    }
    public EnemyAttackState currentEnemyAttackState;
    public CardBase enemyDefendCard;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
       
    }

    public void GetRandomAttack()
    {
        float total = lightAttackProbability + heavyAttackProbability + defendProbability;
        float randomPoint = Random.value * total;

        if (randomPoint < lightAttackProbability)
        {
            // "Light Attack";
            currentEnemyAttackState = EnemyAttackState.Attack;
        }
        else if (randomPoint < lightAttackProbability + heavyAttackProbability)
        {
            // "Heavy Attack";
            currentEnemyAttackState = EnemyAttackState.HeavyAttack;
        }
        else
        {
            // "defend Attack";
            currentEnemyAttackState = EnemyAttackState.Defend;
        }
    }


    public IEnumerator Enum_CpuDrawCard()
    {
        //int randomAttack = Random.Range(1, 3);
        GetRandomAttack();

        switch (currentEnemyAttackState)
        {

            case EnemyAttackState.Attack:
                CpuSpawnCard();
                yield return new WaitForSeconds(0.5f);
                break;

            case EnemyAttackState.HeavyAttack:
                CpuSpawnCard();
                yield return new WaitForSeconds(0.5f);
                CpuSpawnCard();
                yield return new WaitForSeconds(0.5f);
                break;

            case EnemyAttackState.Defend:
                CpuSpawnCard();
                break;

        }
      
        /* CpuSpawnCard();
        yield return new WaitForSeconds(0.5f);*/
    }

    public IEnumerator Enum_PlayerDrawCard()
    {
        PlayerSpawnCard();
        yield return new WaitForSeconds(0.6f);
        PlayerSpawnCard();
        yield return new WaitForSeconds(0.6f);
        PlayerSpawnCard();
        yield return new WaitForSeconds(0.6f);
        PlayerSpawnCard();
    }
    public void PlayerDrawCard()
    {
        StartCoroutine(Enum_PlayerDrawCard());
    }

    public void CpuDrawCard()
    {
        StartCoroutine(Enum_CpuDrawCard());
    }

    void PlayerSpawnCard()
    {
        int random = Random.Range(0, playerAvailableCards.Count);
        Instantiate(playerAvailableCards[random], cardSpawnPos.position, Quaternion.identity);
    }

    void CpuSpawnCard()
    {
        if(currentEnemyAttackState == EnemyAttackState.Defend)
        {
            Instantiate(enemyDefendCard, cardSpawnPosCpu.position, Quaternion.identity);
            return;
        }
        int random = Random.Range(0, cpuAvailableCards.Count);
        Instantiate(cpuAvailableCards[random], cardSpawnPosCpu.position, Quaternion.identity);
    }


}


