using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]
public class SOCards : ScriptableObject
{
    // Character cards are people you can date. Turn this off for event cards,
    // which still change the stats but never count towards the romance win.
    public bool isCharacter = true;
    // Name of the character. Used to count dates and to name the winner.
    public string characterName;
    public Sprite image;
    [TextArea] public string flavorText;

    public CardChoiceStats accepted;
    public CardChoiceStats rejected;

    // Characters fall back to the asset name so they are never nameless on screen.
    // A nameless event just shows nothing instead of leaking the asset name.
    public string DisplayName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(characterName)) return characterName;
            return isCharacter ? name : string.Empty;
        }
    }
}

[System.Serializable]
public class CardChoiceStats
{
    [Tooltip("Créditos")] public int stat1;
    [Tooltip("Tiempo")] public int stat2;
    [Tooltip("Puntaje Social")] public int stat3;
    [Tooltip("Estabilidad")] public int stat4;

    public int[] ToArray()
    {
        return new[] { stat1, stat2, stat3, stat4 };
    }
}