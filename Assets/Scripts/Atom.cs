using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Merges atoms of the same type upon collision to create a higher-level atom.
/// </summary>
public class Atom : MonoBehaviour
{
    public AtomData atomData;

    [SerializeField] AtomManager atomManager;
    [SerializeField] GameObject atomPrefab;
    [SerializeField] TextMeshPro atomSymbol;
    [SerializeField] TextMeshPro atomID;
    [SerializeField] Transform atomSize;
    [SerializeField] SpriteRenderer atomColor;

    void Awake()
    {
        atomManager = GameObject.Find("AtomManager").GetComponent<AtomManager>();
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
}
