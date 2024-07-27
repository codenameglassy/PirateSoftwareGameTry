using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class EnemyHealth : MonoBehaviour
{
    public static EnemyHealth instance;
    public int maxHealth;
    private int currentHealth;
    [SerializeField] private SpriteRenderer enemyIcon;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshPro enemyHealthText_;
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
        portraitIcon.position = new Vector2(20, portraitIcon.position.y);
        portraitIcon.DOMove(finalPos, 1.5f);

        currentHealth = maxHealth;
        enemyHealthText.text = currentHealth.ToString();
        enemyHealthText_.text = currentHealth.ToString();

        defaultMat = enemyIcon.material;
    }


    public void TakeDamage(int damageAmount)
    {
        if (damageAmount <= 0)
        {
            Debug.LogWarning("zero damage");
            //portraitIcon.DOShakePosition(.25f, new Vector3(.5f, 0f, 0), 10, 90, false, true);
            portraitIcon.DOScale(new Vector2(.42f, .42f), .2f).SetEase(Ease.InOutBounce).OnComplete(() => portraitIcon.DOScale(new Vector2(.4f, .4f), .1f));
            return;
        }
        currentHealth -= damageAmount;
        DamageNumber myDamageNumber = Instantiate(damageNumber, damageNumberSpawnPos.position, Quaternion.identity).GetComponent<DamageNumber>();
        myDamageNumber.ShowDamageNumber(damageAmount);

        enemyHealthText.text = currentHealth.ToString();
        enemyHealthText_.text = currentHealth.ToString();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            enemyHealthText.text = currentHealth.ToString();
            enemyHealthText_.text = currentHealth.ToString();

            GameWin();
            return;
        }
        
        enemyIcon.material = whiteMat;
        portraitIcon.DOScale(new Vector2(.45f, .45f), .2f).SetEase(Ease.InOutBounce).OnComplete(() => portraitIcon.DOScale(new Vector2(.4f, .4f), .1f));
        Invoke("ResetMat", 0.35f);

    }
    public void GameWin()
    {
        StartCoroutine(Enum_GameWin());
    }

    IEnumerator Enum_GameWin()
    {
        Debug.Log("Player won - Win");
        GameControl.instance.currentGameState = GameControl.GameState.GameEnd;
        yield return new WaitForSeconds(1f);
        GameControl.instance.gamewin.SetActive(true);
    }
    void ResetMat()
    {
        enemyIcon.material = defaultMat;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
