using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    private UIController UIC;
    [SerializeField] private UIDocument _document;

    //--------------Menu Input Controls----------------
    
    

    //--------------Pause Controls---------------------
    private bool isPaused;
    private bool pauseConsumed;

    private void Start()
    {
        UIC = UIController.instance;
        //-----Menu Controls------
        

        //-----pause control------
        _document.enabled = false;
        isPaused = false;
        pauseConsumed = false;
    }

    void Update()
    {
        PauseGame();
    }

    void PauseGame()
    {
        if (UIC.GetPause() > 0)
        {
            if(!pauseConsumed){
                isPaused = !isPaused;
                Time.timeScale = isPaused ? 0f : 1f;
                _document.enabled = isPaused;
                pauseConsumed = true;
            }
        }
        else
        {
            pauseConsumed = false;
        }
    }
    
}
