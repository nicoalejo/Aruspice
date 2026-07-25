using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]
public class SOCards : ScriptableObject
{
    public Sprite image;
    [TextArea] public string flavorText;

    public CardChoiceStats accepted;
    public CardChoiceStats rejected;
}

[System.Serializable]
public class CardChoiceStats
{
    public int stat1;
    public int stat2;
    public int stat3;
    public int stat4;

    public int[] ToArray()
    {
        return new[] { stat1, stat2, stat3, stat4 };
    }
}