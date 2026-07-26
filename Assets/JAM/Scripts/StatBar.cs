using System.Collections;
using UnityEngine;

// Goes on each of the four bar GameObjects inside StatsBar.
// The background stays still and the "Over" rect changes its width to show the value.
public class StatBar : MonoBehaviour
{
    // How many frames to wait for the canvas layout before giving up.
    private const int LayoutRetryFrames = 10;

    [Header("Bar Parts")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform over;

    [Header("Fill")]
    // Width of the bar at 100. Leave at 0 to take it from the background.
    [SerializeField] private float fullWidth = 0f;
    // Seconds the bar takes to travel to the new value. 0 makes it instant.
    [SerializeField] private float animationDuration = 0.25f;

    private float cachedFullWidth = -1f;
    private float currentValue = -1f;
    private Coroutine fillRoutine;
    private Coroutine layoutRoutine;
    private float pendingValue;

    public void SetValue(int value, bool animated = true)
    {
        if (over == null) return;

        float target = Mathf.Clamp(value, GameManagerJAM.MinStat, GameManagerJAM.MaxStat);

        StopFill();

        // The first value of the game snaps into place, there is nothing to travel from.
        if (!animated || animationDuration <= 0f || currentValue < 0f || !gameObject.activeInHierarchy)
        {
            currentValue = target;
            ApplyWidth(target);
            return;
        }

        fillRoutine = StartCoroutine(FillTo(target));
    }

    private void StopFill()
    {
        if (fillRoutine == null) return;
        StopCoroutine(fillRoutine);
        fillRoutine = null;
    }

    private IEnumerator FillTo(float target)
    {
        float start = currentValue;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            ApplyWidth(Mathf.Lerp(start, target, t));
            yield return null;
        }

        currentValue = target;
        ApplyWidth(target);
        fillRoutine = null;
    }

    private void ApplyWidth(float value)
    {
        float full = GetFullWidth();

        // Called before the canvas built its layout (the stats start at 50 during
        // Awake), so the width is not known yet. Retry once the rects are real.
        if (full <= 0f)
        {
            pendingValue = value;
            if (layoutRoutine == null && gameObject.activeInHierarchy)
            {
                layoutRoutine = StartCoroutine(ApplyWhenLayoutReady());
            }
            return;
        }

        over.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, full * (value / GameManagerJAM.MaxStat));
    }

    private IEnumerator ApplyWhenLayoutReady()
    {
        for (int frame = 0; frame < LayoutRetryFrames; frame++)
        {
            yield return new WaitForEndOfFrame();

            if (GetFullWidth() > 0f)
            {
                layoutRoutine = null;
                ApplyWidth(pendingValue);
                yield break;
            }
        }

        layoutRoutine = null;
        Debug.LogWarning($"[StatBar] '{name}' has no usable width. Assign the Background, or set Full Width by hand.", this);
    }

    // Read lazily: with a layout group above, the rect is not final during Awake.
    private float GetFullWidth()
    {
        if (cachedFullWidth > 0f) return cachedFullWidth;

        if (fullWidth > 0f) cachedFullWidth = fullWidth;
        else if (background != null && background.rect.width > 0f) cachedFullWidth = background.rect.width;
        else if (over.rect.width > 0f) cachedFullWidth = over.rect.width;

        return cachedFullWidth > 0f ? cachedFullWidth : 0f;
    }
}