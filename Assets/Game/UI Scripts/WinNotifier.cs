using System.Collections;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;

public class WinNotifier : MonoBehaviour
{
    [SerializeField] private GameEvent winEvent, hideEvent; //temp secondary event for resetting the win screen
    [SerializeField] private UIDocument uidoc;
    private VisualElement root, winScreen;
    private Label missionLabel, completeLabel, rewardLabel;
    
    [SerializeField] private float opacityTimer, textTimer;
    [SerializeField] private AnimationCurve opacityCurve, posCurve, scaleCurve;

    [SerializeField] private Vector2 missionLabelStartOffset, missionLabelEndOffset,
        completeLabelStartOffset, completeLabelEndOffset;
    [SerializeField] private Vector2 missionLabelStartSize, missionLabelEndSize,
        completeLabelStartSize, completeLabelEndSize;
    [SerializeField] private Vector2 missionLabelStartGlowPos, missionLabelEndGlowPos,
        completeLabelStartGlowPos, completeLabelEndGlowPos;

    [SerializeField] private MissionManager missionManager;

    private void Start()
    {
        var winListener = gameObject.AddComponent<GameEventListener>();
        winListener.Events.Add(winEvent);
        winListener.Response = new();
        winListener.Response.AddListener(() => StartCoroutine(ShowWin()));
        winEvent.RegisterListener(winListener);

        var hideListener = gameObject.AddComponent<GameEventListener>();
        hideListener.Events.Add(hideEvent);
        hideListener.Response = new();
        hideListener.Response.AddListener(() => StartCoroutine(HideWin()));
        hideEvent.RegisterListener(hideListener);

        root = uidoc.rootVisualElement;
        winScreen = root.Q<VisualElement>("WinScreen");
        missionLabel = winScreen.Q<Label>("MissionLabel");
        completeLabel = winScreen.Q<Label>("CompleteLabel");
        rewardLabel = winScreen.Q<Label>("RewardLabel");

        StartCoroutine(HideWin());
    }

    [ContextMenu("Win Demonstration")]
    public void ShowWinArbitrarily()
    {
        StartCoroutine(ShowWin());
    }

    private IEnumerator HideWin()
    {
        winScreen.style.display = DisplayStyle.None;
        yield return null;
    }

    private IEnumerator Fade(float start, float end)
    {
        float journey = 0f;

        while (journey <= opacityTimer)
        {
            var opaCurvedPercent = opacityCurve.Evaluate(journey / opacityTimer);
            var opacity = Mathf.Lerp(start, end, opaCurvedPercent);
            winScreen.style.opacity = opacity;
            journey += Time.deltaTime;
            yield return null;
        }
    }

    private void HandleLabelTranslateOffset(Label label, Vector2 startOffset, Vector2 endOffset, float curvedPercent)
    {
        var offset = Vector2.Lerp(startOffset, endOffset, curvedPercent);
        label.style.translate = new Translate(new Length(offset.x, LengthUnit.Pixel), new Length(offset.y, LengthUnit.Pixel));
    }

    private void HandleLabelSize(Label label, Vector2 startSize, Vector2 endSize, float curvedPercent)
    {
        var size = Vector2.Lerp(startSize, endSize, curvedPercent);
        label.style.scale = new Scale(size);
    }

    private void HandleLabelGlowOffset(Label label, Vector2 startOffset, Vector2 endOffset, float curvedPercent)
    {
        var offset = Vector2.Lerp(startOffset, endOffset, curvedPercent);
        var current = label.style.textShadow.value;
        //print($"Current color: {current.color}");
        current.offset = offset;
        current.color = new(1,1,1,0.5f);
        current.blurRadius = 2f;
        label.style.textShadow = current;
    }

    private IEnumerator StartTextTransition()
    {
        float journey = 0f;
        float? intendedReward = missionManager?.ActiveMission?.CashReward();
        if (intendedReward == null) intendedReward = 20f;

        while (journey <= textTimer / 2f)
        {
            var posCurvedPercent = posCurve.Evaluate(journey / textTimer);
            var scaleCurvedPercent = scaleCurve.Evaluate(journey / textTimer);
            var opacityCurvedPercent = opacityCurve.Evaluate(journey / textTimer);

            HandleLabelTranslateOffset(missionLabel, missionLabelStartOffset, missionLabelEndOffset, posCurvedPercent);
            HandleLabelTranslateOffset(completeLabel, completeLabelStartOffset, completeLabelEndOffset, posCurvedPercent);

            HandleLabelSize(missionLabel, missionLabelStartSize, Vector2.one, scaleCurvedPercent);
            HandleLabelSize(completeLabel, completeLabelStartSize, Vector2.one, scaleCurvedPercent);

            HandleLabelGlowOffset(missionLabel, missionLabelStartGlowPos, missionLabelEndGlowPos, opacityCurvedPercent);
            HandleLabelGlowOffset(completeLabel, completeLabelStartGlowPos, completeLabelEndGlowPos, opacityCurvedPercent);

            var reward = Mathf.CeilToInt(Mathf.Lerp(0, (float)intendedReward, journey / (textTimer / 4f)));
            rewardLabel.text = $"REWARD: ${reward:000}";

            journey += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator EndTextTransition()
    {
        float journey = textTimer / 2f;
        while (journey >= textTimer / 2f && journey <= textTimer)
        {
            var posCurvedPercent = posCurve.Evaluate(journey / textTimer);
            var scaleCurvedPercent = scaleCurve.Evaluate(journey / textTimer);
            var opacityCurvedPercent = opacityCurve.Evaluate(journey / textTimer);

            HandleLabelTranslateOffset(missionLabel, missionLabelStartOffset, missionLabelEndOffset, posCurvedPercent);
            HandleLabelTranslateOffset(completeLabel, completeLabelStartOffset, completeLabelEndOffset, posCurvedPercent);

            HandleLabelSize(missionLabel, Vector2.one, missionLabelEndSize, scaleCurvedPercent);
            HandleLabelSize(completeLabel, Vector2.one, completeLabelEndSize, scaleCurvedPercent);

            HandleLabelGlowOffset(missionLabel, missionLabelStartGlowPos, missionLabelEndGlowPos, opacityCurvedPercent);
            HandleLabelGlowOffset(completeLabel, completeLabelStartGlowPos, completeLabelEndGlowPos, opacityCurvedPercent);

            journey += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ShowWin()
    {
        winScreen.style.display = DisplayStyle.Flex;

        StartCoroutine(Fade(0f, 1f));

        StartCoroutine(StartTextTransition());
        yield return new WaitForSeconds(textTimer / 2f);

        StartCoroutine(EndTextTransition());
        yield return new WaitForSeconds((textTimer / 2f) - opacityTimer);

        StartCoroutine(Fade(1f, 0f));
        yield return new WaitForSeconds(opacityTimer);
        StartCoroutine(HideWin());
    }
}
