using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using ToonBoom.TBGRenderer;

[
    RequireComponent(typeof(Animator)),
    RequireComponent(typeof(AudioSource)),
    RequireComponent(typeof(TBGRenderer))
]
public class UIGame : MonoBehaviour
{
    public GameSettings GameSettings;

    [System.Serializable]
    public class InGameBeakPhys
    {
        public Transform BeakHolderAnchor;
        public FollowTransform2D BlanketTop;
        public FollowTransform2D BlanketBottom;
        public FollowTransform2D PhysicsTop;
        public Transform PhysicsBottom;
    }
    public InGameBeakPhys InGameBeakPhysRef;

    public Transform BeakHolderAnchor;
    public FollowTransform2D BlanketTop;
    public FollowTransform2D BlanketBottom;
    public FollowTransform2D PhysicsTop;
    public Transform PhysicsBottom;

    public Camera Camera3D;
    public GameObject BirdPrefab;
    public GameObject BassinetGoal;

    public GameObject PartyHat;
    public GameObject BackButtonRef;
    public GameObject[] ButtonsRef;
    public GameObject[] ChecksRef;
    public GameObject[] LocksRef;

    public GameObject FxPrefab;

    private Animator Animator;
    private AudioSource AudioSource;
    private TBGRenderer TBGRenderer;

    public void Start()
    {
        // Get animator and audio source
        Animator = GetComponent<Animator>();
        AudioSource = GetComponent<AudioSource>();
        TBGRenderer = GetComponent<TBGRenderer>();

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
        BackButtonRef.AddComponent<BoxCollider2D>();
        foreach (GameObject buttonRef in ButtonsRef)
        {
            buttonRef.AddComponent<BoxCollider2D>();
        }
        // Ensure buttons have a oppotunity to appear before calculating bounds
        {
            Animator.Update(0f);
            TBGRenderer.UpdateRenderer();
        }
        UpdateButtonBounds();

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

    [System.Serializable]
    public class GameState
    {
        public GameObject Instance;
        public string CurrentScene;
        public Transform[] Obstacles;
        public float TimeRemaining = 30;
        public bool BabyDelivered = false;
    }

    public int CurrentLevel = 0;

    public GameState DefaultGameState = new GameState();

    public GameState TheGameState = new GameState();


    // Update is called once per frame
    void Update()
    {
        ModelToView();
        ViewToModel();

        // Game Play!
        if (UIState == Animator.StringToHash(GameSettings.GameState))
        {
            // Update the game state
            TheGameState.TimeRemaining -= Time.deltaTime;

            // If the game is over, go to the game over screen
            if (TheGameState.TimeRemaining <= 0)
            {
                Animator.SetTrigger(GameSettings.Trigger.GameFail);
            }
        }
    }

    public void ViewToModel()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Get the collider at the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var collided = Physics2D.GetRayIntersection(ray, Mathf.Infinity).collider?.gameObject;

        // Debug.Log("Clicked " + collided.name);

        // Back button (all the time!)
        if (collided == BackButtonRef)
        {
            Debug.Log("Clicked back");
            Animator.SetTrigger(GameSettings.Trigger.ClickBack);
        }

        // Main Menu
        if (UIState == Animator.StringToHash(GameSettings.MenuState))
        {
            Debug.Log("Clicked main menu");
            // First button goes to level select
            if (collided == ButtonsRef[0])
            {
                Debug.Log("Clicked level select");
                Animator.SetTrigger(GameSettings.Trigger.ClickNewGame);
            }
            // Second button goes to settings
            else if (collided == ButtonsRef[1])
            {
                Debug.Log("Clicked settings");
                Animator.SetTrigger(GameSettings.Trigger.ClickSettings);
            }
        }

        // Settings
        if (UIState == Animator.StringToHash(GameSettings.SettingsState))
        {
            Debug.Log("Clicked settings");
            // First button toggles music!
            if (collided == ButtonsRef[0])
            {
                PlayMusic = !PlayMusic;
                ChecksRef[0].SetActive(PlayMusic);
                AudioSource.enabled = PlayMusic;
            }
            // Second button plays FX
            else if (collided == ButtonsRef[1])
            {
                var fx = Instantiate(FxPrefab, Vector3.zero, Quaternion.identity);
                // Pick random fx trigger from settings
                var fxTrigger = GameSettings.FXTrigger.GetType().GetFields()
                    .Select(f => f.GetValue(GameSettings.FXTrigger))
                    .OrderBy(x => Random.value).First();
                fx.GetComponent<Animator>().SetTrigger(fxTrigger.ToString());
                // Despawn after 1 second
                Destroy(fx, 1f);
            }
        }

        // Level Select
        if (UIState == Animator.StringToHash(GameSettings.LevelSelectState))
        {
            // Check if the collider is one of the level buttons
            for (int i = 0; i < LevelCount; i++)
            {
                if (collided == ButtonsRef[i])
                {
                    // If the level is locked, play the frustration animation
                    if (IsLocked(i))
                    {
                        Animator.SetTrigger(GameSettings.FXTrigger.FrustrationVessle);
                    }
                    else
                    {
                        // Otherwise, load the level!
                        Animator.SetTrigger(GameSettings.Trigger.ClickLevel);
                        // Animator.SetInteger("Level", i);
                        CurrentLevel = i;
                    }
                }
            }
        }
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

            UpdateButtonBounds();

            if (UIState == Animator.StringToHash(GameSettings.LevelSelectState))
            {
                LevelHydrate();
            }

            // If we are switching to GameState, create the level instance
            if (UIState == Animator.StringToHash(GameSettings.GameState))
            {
                TheGameState = DefaultGameState;
                var levelSettings = GameSettings.Levels[CurrentLevel];
                TheGameState.Instance = Instantiate(levelSettings.Prefab);
                TheGameState.Obstacles = GameObject
                    .FindGameObjectsWithTag(GameSettings.Tweaks.ObstacleTag)
                    .Select(x => x.transform)
                    .ToArray();
                TheGameState.TimeRemaining = levelSettings.TimeLimit;
            }

            // If we are switching away from GameState, destroy the level instance
            if (LastUIState == Animator.StringToHash(GameSettings.GameState))
            {
                Destroy(TheGameState.Instance);
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

    void UpdateButtonBounds()
    {
        // Resize collider bounds on buttons.
        UpdateButtonBounds(BackButtonRef);
        foreach (GameObject buttonRef in ButtonsRef)
        {
            UpdateButtonBounds(buttonRef);
        }
    }
    void UpdateButtonBounds(GameObject buttonRef)
    {
        var collider = buttonRef.GetComponent<BoxCollider2D>();
        var sprite = buttonRef.GetComponent<SpriteRenderer>().sprite;
        if (sprite == null)
            return;
        var bounds = sprite.bounds;
        collider.size = bounds.size;
        collider.offset = bounds.center;
    }
}
