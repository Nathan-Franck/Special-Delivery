using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(AudioSource))]
public class UIGame : MonoBehaviour
{
    public GameSettings GameSettings;

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

        // Assign 2D box colliders to buttons
        foreach (GameObject buttonRef in ButtonsRef)
        {
            buttonRef.AddComponent<BoxCollider2D>();
        }

        // Setup follow chain
        PhysicsTop.target = BeakHolderAnchor;
        BlanketTop.target = PhysicsTop.transform;
        BlanketBottom.target = PhysicsBottom;
    }

    public int UIState = 0;
    public int LastUIState = 0;
    public static int LevelCount = 5;
    public bool[] LevelComplete = new bool[LevelCount];
    public bool PlayMusic = true;


    // Update is called once per frame
    void Update()
    {
        ModelToView();
    }

    public void ModelToView()
    {
        // Get current state from main layer of Animator as a string
        UIState = Animator
            .GetCurrentAnimatorStateInfo(0)
            .IsName("Entry")
                ? 0
                : Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

        if (UIState != LastUIState)
        {
            // State changed!
            Debug.Log("State changed to " + UIState);

            if (UIState == Animator.StringToHash(GameSettings.LevelSelectState))
            {
                LevelHydrate();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Escape key pressed, go back to level select
            Animator.SetTrigger(GameSettings.Trigger.ClickBack);
        }


        if (UIState == Animator.StringToHash(GameSettings.SettingsState))
        {
            SettingsHydrate();
        }

        LastUIState = UIState;
    }

    bool IsLocked(int level)
    {
        // If the previous levels is complete, then this level is UNLOCKED! Otherwise locked.
        return level > 0 && !LevelComplete[level - 1];
    }

    void LevelHydrate()
    {
        // Update locks using IsLocked and enable/disable visuals.
        for (int i = 0; i < LevelCount; i++)
        {
            LocksRef[i].SetActive(IsLocked(i));
        }

        // Update checks using LevelComplete and enable/disable visuals.
        for (int i = 0; i < LevelCount; i++)
        {
            ChecksRef[i].SetActive(LevelComplete[i]);
        }
    }

    void SettingsHydrate()
    {
        // Disable locks.
        for (int i = 0; i < LevelCount; i++)
        {
            LocksRef[i].SetActive(false);
        }

        // Disable checks.
        for (int i = 0; i < LevelCount; i++)
        {
            ChecksRef[i].SetActive(false);
        }

        // First check enabled if Music is enabled
        ChecksRef[0].SetActive(PlayMusic);
    }
}
