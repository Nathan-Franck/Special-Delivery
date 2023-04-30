using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    public GameObject[] ButtonsRef;
    public GameObject[] ChecksRef;
    public GameObject[] LocksRef;

    public void Start()
    {
        //Hide locks and checks
        foreach (GameObject lockRef in LocksRef)
        {
            lockRef.SetActive(false);
        }
        foreach (GameObject checkRef in ChecksRef)
        {
            checkRef.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Hydrate();
    }

    void Hydrate()
    {

    }
}
