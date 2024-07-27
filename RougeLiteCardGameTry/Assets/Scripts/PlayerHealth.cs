using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    private int currentHealth;
    [SerializeField]private SpriteRenderer playerIcon;
    [SerializeField]private TextMeshProUGUI playerHealthText;
    [SerializeField]private TextMeshPro playerHealthText_;
    [SerializeField]
    private Material whiteMat;
    private Material defaultMat;

    [SerializeField]
    private Transform portraitIcon;

    [SerializeField] private GameObject damageNumber;
    [SerializeField] private Transform damageNumberSpawnPos;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Vector2 finalPos = portraitIcon.position;
        portraitIcon.position = new Vector2(-20, portraitIcon.position.y);
        portraitIcon.DOMove(finalPos, 1.5f);

        currentHealth = StartPlayerHealth.playerHealth;
        playerHealthText.text = currentHealth.ToString();
        playerHealthText_.text = currentHealth.ToString();

        defaultMat = playerIcon.material;
    }

    public void TakeDamage(int damageAmount)
    {
        if(damageAmount <= 0)
        {
            Debug.LogWarning("zero damage");
            //portraitIcon.DOShakePosition(.25f, new Vector3(.5f, 0, 0), 4, 90, false, true);
            portraitIcon.DOScale(new Vector2(.42f, .42f), .2f).SetEase(Ease.InOutBounce).OnComplete(() => portraitIcon.DOScale(new Vector2(.4f, .4f), .1f));
            return;
        }
        currentHealth -= damageAmount;
        DamageNumber myDamageNumber = Instantiate(damageNumber, damageNumberSpawnPos.position, Quaternion.identity).GetComponent<DamageNumber>();
        myDamageNumber.ShowDamageNumber(damageAmount);

        playerHealthText.text = currentHealth.ToString();
        playerHealthText_.text = currentHealth.ToString();

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            playerHealthText.text = currentHealth.ToString();
            playerHealthText_.text = currentHealth.ToString();

            Gameover();
            return;
        }
        playerIcon.material = whiteMat;
        portraitIcon.DOScale(new Vector2(.45f, .45f), .2f).SetEase(Ease.InOutBounce).OnComplete(() => portraitIcon.DOScale(new Vector2(.4f, .4f), .1f));
        Invoke("ResetMat", 0.35f);
        StartPlayerHealth.playerHealth = currentHealth;
    }

    public void Gameover()
    {
        StartCoroutine(Enum_Gameover());
    }

    IEnumerator Enum_Gameover()
    {
        Debug.Log("Enemy won - Gameover");
        GameControl.instance.currentGameState = GameControl.GameState.GameEnd;
        yield return new WaitForSeconds(1f);
        GameControl.instance.gameover.SetActive(true);
    }
    void ResetMat()
    {
        playerIcon.material = defaultMat;
    }
    public int GetCurrentHealth()
    {
        return currentHealth;
    }


}
