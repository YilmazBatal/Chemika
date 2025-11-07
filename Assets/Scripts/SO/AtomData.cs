using UnityEngine;

[CreateAssetMenu(fileName = "AtomData00", menuName = "Scriptable Objects/Create New Atom")]
public class AtomData : ScriptableObject
{
    public int id;
    public float atomSize;
    public string atomName;
    public string atomSymbol;
    public Color atomColor;
}
