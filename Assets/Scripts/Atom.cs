using System;
using TMPro;
using UnityEngine;

public class Atom : MonoBehaviour
{
    public AtomData atomData;

    [SerializeField] AtomManager atomManager;
    [SerializeField] GameObject atomPrefab;
    [SerializeField] TextMeshPro atomSymbol;
    [SerializeField] TextMeshPro atomID;
    [SerializeField] Transform atomSize;
    [SerializeField] SpriteRenderer atomColor;

    float bottomOfAtom;

    private Transform redLine;
    private bool isGameOverTriggered = false;
    private float timeAboveLine = 0f;
    bool inClaw = true;
    Rigidbody2D rb;
    void Awake()
    {
        redLine = GameObject.Find("RedLine").GetComponent<Transform>();
        atomManager = GameObject.Find("AtomManager").GetComponent<AtomManager>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {

        Debug.Log("Atom has been created: " + atomData.atomSymbol + " GameObject name: " + gameObject.name);

        atomSize = GetComponent<Transform>();
        atomSize.localScale = atomData.atomSize * Vector3.one;
        atomSymbol.text = atomData.atomSymbol;
        atomID.text = atomData.id.ToString();
        atomColor.color = atomData.atomColor;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Atom otherAtom = collision.gameObject.GetComponent<Atom>();
        if (otherAtom != null && atomData.id == otherAtom.atomData.id)
        {
            // Sadece bir atom merge işlemini yapabilsin (örneğin InstanceID'ye göre)
            if (GetInstanceID() < otherAtom.GetInstanceID())
            {
                Vector3 getMiddlePoint = (transform.position + collision.transform.position) / 2;

                GameObject mergedAtom = Instantiate(atomPrefab, getMiddlePoint, Quaternion.identity);

                mergedAtom.GetComponent<Atom>().atomData = atomManager.atomDataArray[atomData.id];
                mergedAtom.GetComponent<Atom>().atomSize = mergedAtom.GetComponent<Transform>();
                mergedAtom.transform.SetParent(atomManager.contentBox);
                mergedAtom.name = "Element_" + mergedAtom.GetComponent<Atom>().atomData.id;
                UIManager.Instance.UpdateScore((int)MathF.Pow(2, atomData.id));

                Destroy(otherAtom.gameObject);
                Destroy(gameObject);
            }
        }
    }
    void Update()
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            inClaw = true;
        }
        else
        {
            inClaw = false;
        }
        // Check if the atom is above the red line

        bottomOfAtom = transform.position.y - (atomSize.localScale.y / 2); // 0 50 - 0 25
        if (bottomOfAtom > redLine.position.y)
        {
            timeAboveLine += Time.deltaTime;

            // If it stayed above for 5 seconds straight → game over
            if (!inClaw && !isGameOverTriggered && timeAboveLine >= 5f)
            {
                isGameOverTriggered = true;
                GameOver();
            }
        }
        else
        {
            // Reset timer if it goes back below the line
            timeAboveLine = 0f;
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! Atom stayed above the red line for 5 seconds. ID: " + atomData.id);

        // atomManager.enabled = false;
        // Time.timeScale = 0f;

        // UIManager.Instance.GameOver(); // uncomment when ready
    }
}
