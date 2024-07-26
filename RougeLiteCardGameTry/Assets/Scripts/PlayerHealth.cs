using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour
{
    private int currentHealth;
    [SerializeField]private SpriteRenderer playerIcon;
    [SerializeField]private TextMeshProUGUI playerHealthText;
    [SerializeField]private TextMeshPro playerHealthText_;
    [SerializeField]
    private Material whiteMat;
    private Material defaultMat;

    [SerializeField]
    private Transform portraitIcon;

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
        currentHealth -= damageAmount;
      
        playerHealthText.text = currentHealth.ToString();
        playerHealthText_.text = currentHealth.ToString();

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            return;
        }
        playerIcon.material = whiteMat;
        Invoke("ResetMat", 0.35f);
        StartPlayerHealth.playerHealth = currentHealth;
    }

    void ResetMat()
    {
        playerIcon.material = defaultMat;
    }

}
