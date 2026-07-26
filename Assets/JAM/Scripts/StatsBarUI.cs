using UnityEngine;

// Goes on the StatsBar GameObject. Holds the four bars in stat1..stat4 order.
public class StatsBarUI : MonoBehaviour
{
    [SerializeField] private StatBar[] bars = new StatBar[GameManagerJAM.StatCount];

    public void SetStat(int index, int value, bool animated = true)
    {
        if (index < 0 || index >= bars.Length) return;
        if (bars[index] == null) return;

        bars[index].SetValue(value, animated);
    }

    public void SetAll(int[] values, bool animated = true)
    {
        if (values == null) return;

        for (int i = 0; i < bars.Length && i < values.Length; i++)
        {
            SetStat(i, values[i], animated);
        }
    }
}
