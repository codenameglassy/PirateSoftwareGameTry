using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class EnemyHealth : MonoBehaviour
{
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
        currentHealth -= damageAmount;
     
        enemyHealthText.text = currentHealth.ToString();
        enemyHealthText_.text = currentHealth.ToString();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            return;
        }
        
        enemyIcon.material = whiteMat;
        Invoke("ResetMat", 0.35f);

    }

    void ResetMat()
    {
        enemyIcon.material = defaultMat;
    }


}
