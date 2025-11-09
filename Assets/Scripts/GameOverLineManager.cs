using System.Collections.Generic;
using UnityEngine;

public class GameOverLineManager : MonoBehaviour
{
    // References
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private GameObject warningIcon;

    [Header("Timers")]
    [SerializeField] private float timeToWarning = 2f;
    [SerializeField] private float timeToGameOver = 5f;

    // Store individual timers for each atom in the danger zone
    // (Atom's collider is the key and timer is the value)
    [SerializeField] private Dictionary<Collider2D, float> atomTimers = new Dictionary<Collider2D, float>();

    private float dangerLineY;

    void Start()
    {
        dangerLineY = boxCollider2D.bounds.min.y;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Atom") && !atomTimers.ContainsKey(other))
        {
            atomTimers.Add(other, 0f);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.tag.Equals("Atom"))
            return;

        float atomBottomY = other.bounds.min.y;

        if (atomBottomY > dangerLineY)
        {
            atomTimers[other] += Time.deltaTime;
        }
        else
        {
            atomTimers[other] = 0f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Atom") && atomTimers.ContainsKey(other))
        {
            atomTimers.Remove(other);
        }
    }

    void LateUpdate()
    {
        bool showWarning = false;
        float maxTimer = 0f;

        // Check all atom timers to update maxTimer
        foreach (float timer in atomTimers.Values)
        {
            if (timer > maxTimer)
            {
                maxTimer = timer;
            }
        }

        if (maxTimer >= timeToGameOver)
        {
            UIManager.Instance.GameOver();
            this.enabled = false; // Oyunu bitir
            return;
        }
        
        if (maxTimer >= timeToWarning)
        {
            showWarning = true;
        }

        // Uyarı ikonunu sadece tek bir yerden, net bir kararla aç/kapat
        warningIcon.SetActive(showWarning);
    }
}