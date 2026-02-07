using UnityEngine;
using UnityEngine.Splines;

//This script is used to control the map view character token's movement.
public class TokenController : MonoBehaviour
{
    [SerializeField] private Sprite characterSprite;
    private CharacterData characterData;
    public int characterIndex;
    [SerializeField] private float moveSpeed;

    private InputController IC;
    private SplineContainer currentPath;
    private SplineAnimate splineAnimateScript;
    private SpriteRenderer spriteRenderer;
    public bool _is_travelling_along_path;
    private bool _cursor_in_trigger;
    private bool _is_clicked_on;

    public CharacterData GetCharacterData() => characterData;

    void Awake()
    {
        if (characterData == null)
        {
            // Example: assign first party member
            characterData = GameState.Instance.Data.partyMembers[characterIndex];

            // Or find by name if needed
            // characterData = GameState.Instance.Data.partyMembers
            //                     .First(c => c.characterName == "Alice");
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        splineAnimateScript = GetComponent<SplineAnimate>();

        Debug.Assert(
            spriteRenderer != null, 
            "SpriteRenderer missing on Token"
        );
        Debug.Assert(
            splineAnimateScript != null,
            "SplineAnimate missing on Token"
        );
    }

    void Start()
    {
        IC = InputController.instance;

        if (characterSprite != null)
        {
            spriteRenderer.sprite = characterSprite;
        }

        if (characterData == null)
            Debug.LogWarning(
                $"Token {gameObject.name} has no CharacterData assigned!"
            );
    }

    void Update()
    {
        if(IC.GetClickState() > 0 && _cursor_in_trigger)
        {
            _is_clicked_on = true;
        }
        else if(IC.GetClickState() == 0)
        {
            ResetClicked();
        }
    }

    // --- Public accessors ---
    public bool GetIsTravellingAlongPath() => _is_travelling_along_path;
    public void SetIsTravellingAlongPath(bool state) => _is_travelling_along_path = state;

    public bool GetIsClicked() => _is_clicked_on;
    public void ResetClicked() => _is_clicked_on = false;

    //Attach's this gameObject to the splinePathToFollow
    public void AttachToPath(SplineContainer path)
    {
        currentPath = path;

        splineAnimateScript.Container = path;
        splineAnimateScript.Loop = SplineAnimate.LoopMode.Once;
        splineAnimateScript.AnimationMethod = SplineAnimate.Method.Speed;
        splineAnimateScript.MaxSpeed = moveSpeed;

        splineAnimateScript.Restart(true);
        splineAnimateScript.Play();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Cursor")_cursor_in_trigger = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Cursor")_cursor_in_trigger = false;
    }
}
