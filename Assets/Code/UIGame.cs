using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(AudioSource))]
public class UIGame : MonoBehaviour
{

    public Transform BeakHolderAnchor;
    public FollowTransform2D BlanketTop;
    public FollowTransform2D BlanketBottom;
    public FollowTransform2D PhysicsTop;
    public Transform PhysicsBottom;
    
    public GameObject PartyHat;
    public GameObject[] ButtonsRef;
    public GameObject[] ChecksRef;
    public GameObject[] LocksRef;

    private Animator Animator;
    private AudioSource AudioSource;

    private int TestEnable = 0;

    public void Start()
    {
        // Get animator and audio source
        Animator = GetComponent<Animator>();
        AudioSource = GetComponent<AudioSource>();

        //Hide locks and checks and hat
        PartyHat.SetActive(false);
        foreach (GameObject lockRef in LocksRef)
        {
            lockRef.SetActive(false);
        }
        foreach (GameObject checkRef in ChecksRef)
        {
            checkRef.SetActive(false);
        }

        // Setup follow chain
        PhysicsTop.target = BeakHolderAnchor;
        BlanketTop.target = PhysicsTop.transform;
        BlanketBottom.target = PhysicsBottom;
    }

    // Update is called once per frame
    void Update()
    {
        Hydrate();
    }

    void Hydrate()
    {
        // Every 10 frames, disable previous check or lock, increment the test, enable new check or lock
        var thingsToEnable = LocksRef.Concat(ChecksRef).ToArray();
        if (Time.frameCount % 10 == 0)
        {
            thingsToEnable[TestEnable].SetActive(false);
            TestEnable = (TestEnable + 1) % thingsToEnable.Length;
            thingsToEnable[TestEnable].SetActive(true);
        }

        //ENable hat every 5 seconds, then disable for 5 seconds
        if (Time.frameCount % 300 == 0)
        {
            PartyHat.SetActive(true);
        }
        if (Time.frameCount % 600 == 0)
        {
            PartyHat.SetActive(false);
        }

    }
}
