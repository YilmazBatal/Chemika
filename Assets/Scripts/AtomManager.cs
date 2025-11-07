using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AtomManager : MonoBehaviour
{
    [SerializeField] Transform atomHolder;
    [SerializeField] public Transform contentBox;
    [SerializeField] GameObject atomPrefab;
    [SerializeField] int maxPossibleId = 4;
    GameObject currentAtom;
    bool isHoldingAtom = false;
    Vector3 newPos;
    bool isSpawning = false;

    [SerializeField] public AtomData[] atomDataArray;

    public AtomData currentAtomData;
    public AtomData upcomingAtomData;

    void Start()
    {
        GenerateUpcomingData();
        ProduceAtom();
        ClawManagement(false);
    }

    void Update()
    {
        ManageTapAction();
    }

    /// <summary>
    /// in the start current atom data will be null so generated upcoming data
    /// will be moved to current data. and new upcoming data will be generated.
    /// once current atom data is used up, this function will be called again.
    /// so current atom data is always filled. and upcoming data is always ready.
    /// </summary>
    void ProduceAtom()
    {
        if (currentAtomData == null)
        {
            currentAtomData = upcomingAtomData;
            upcomingAtomData = null;
            GenerateUpcomingData(); // Generate new upcoming data
        }
    }

    /// <summary>
    /// Generate upcoming atom data. This function is called upon request of the ProduceAtom function.
    /// </summary>
    void GenerateUpcomingData()
    {
        upcomingAtomData = atomDataArray[ChooseAtom()];
    }

    /// <summary>
    /// This function will generate the atom according to current atom data.
    /// And upon dropping the current atom, it will generate new atom after the cooldown.
    /// 
    /// </summary>
    void ClawManagement(bool isMerged)
    {
        Debug.Log("Trying to generate atom. isHoldingAtom: " + isHoldingAtom);
        if (!isHoldingAtom)
        {
            currentAtom = Instantiate(atomPrefab, atomHolder.position, Quaternion.identity);
            Debug.Log("Generating atom with ID: " + currentAtomData.name);
            currentAtom.GetComponent<Atom>().atomData = currentAtomData;
            currentAtom.transform.SetParent(atomHolder);
            currentAtom.transform.localPosition = new Vector3(0, -1, 0);
            currentAtom.name = "Element_" + currentAtom.GetComponent<Atom>().atomData.id;

            // Call back the produce atom function to refill current atom data
            UIManager.Instance.UpdateUpcomingAtom();
            currentAtomData = null;
            ProduceAtom();

            ManageBodyType(isMerged);

            isHoldingAtom = true;
        }
    }

    private void ManageBodyType(bool isMerged)
    {
        Rigidbody2D rb = currentAtom.GetComponent<Rigidbody2D>();
        // if the atom was created through merging, make it dynamic
        if (isMerged)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            currentAtom.GetComponent<CircleCollider2D>().enabled = true;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            currentAtom.GetComponent<CircleCollider2D>().enabled = false;
        }

        rb.WakeUp();
    }

    /// <summary>
    /// Choose an atom with the random id from 0 to maxPossibleId
    /// </summary>
    int ChooseAtom()
    {
        int getRandomId = Random.Range(0, maxPossibleId); //1-4
        return getRandomId;
    }

    private void ManageTapAction()
    {
        if (!IsPointerOverUIObject())
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // Eğer dokunulan yer bir UI öğesi üzerindeyse, geri dön
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                if (touch.phase == TouchPhase.Began)
                {
                    float z = Camera.main.WorldToScreenPoint(atomHolder.position).z;
                    newPos = new Vector3(touch.position.x, Camera.main.WorldToScreenPoint(atomHolder.position).y, z);
                    atomHolder.position = Camera.main.ScreenToWorldPoint(newPos);
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    float z = Camera.main.WorldToScreenPoint(atomHolder.position).z;
                    newPos = new Vector3(touch.position.x, Camera.main.WorldToScreenPoint(atomHolder.position).y, z);
                    atomHolder.position = Camera.main.ScreenToWorldPoint(newPos);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    currentAtom.transform.SetParent(contentBox);

                    Rigidbody2D rb = currentAtom.GetComponent<Rigidbody2D>();
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    currentAtom.GetComponent<CircleCollider2D>().enabled = true;
                    rb.WakeUp();

                    // Spawn'ı bisr frame geciktir
                    isHoldingAtom = false;

                    if (!isSpawning)
                    {
                        StartCoroutine(SpawnNextAtom());
                    }
                }
            }
        }
    }

    IEnumerator SpawnNextAtom()
    {
        isSpawning = true;
        yield return new WaitForEndOfFrame(); // fizik sistemi güncellensin
        yield return new WaitForSeconds(1f);
        // how to not make it call again this Numerator when its waiting for 1 seconds
        ClawManagement(false);
        isSpawning = false;
    }

    private bool IsPointerOverUIObject()
    {
        if (Input.touchCount == 0)
            return false; // no touches, nothing to check
        
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.GetTouch(0).position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
