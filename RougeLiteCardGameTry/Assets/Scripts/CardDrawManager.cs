using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrawManager : MonoBehaviour
{
    public static CardDrawManager instance;

    public List<CardBase> playerAvailableCards = new List<CardBase>();
    public List<CardBase> cpuAvailableCards = new List<CardBase>();
    public Transform cardSpawnPos, cardSpawnPosCpu;
    
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
       
    }

   

    public IEnumerator Enum_CpuDrawCard()
    {
        CpuSpawnCard();
        yield return new WaitForSeconds(0.5f);
        CpuSpawnCard();
        yield return new WaitForSeconds(0.5f);
        CpuSpawnCard();
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator Enum_PlayerDrawCard()
    {
        PlayerSpawnCard();
        yield return new WaitForSeconds(0.5f);
        PlayerSpawnCard();
        yield return new WaitForSeconds(0.5f);
        PlayerSpawnCard();
        yield return new WaitForSeconds(0.5f);
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
        int random = Random.Range(0, cpuAvailableCards.Count);
        Instantiate(cpuAvailableCards[random], cardSpawnPosCpu.position, Quaternion.identity);
    }


}
