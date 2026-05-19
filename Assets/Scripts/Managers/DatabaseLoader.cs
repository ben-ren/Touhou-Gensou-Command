using UnityEngine;

public class DatabaseLoader : MonoBehaviour
{
    public Sprite[] characterSprites;
    
    void Awake()
    {
        CharacterDatabase.SetIcons(characterSprites);
    }
}
