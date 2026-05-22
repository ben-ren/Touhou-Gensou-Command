using UnityEngine;

public class DatabaseLoader : MonoBehaviour
{
    public Sprite[] characterSprites;
    public Sprite[] levelSprites;
    
    void Awake()
    {
        CharacterDatabase.SetIcons(characterSprites);
        LevelDatabase.SetIcons(levelSprites);
    }
}
