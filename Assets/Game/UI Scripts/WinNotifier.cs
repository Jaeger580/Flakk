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

    private IEnumerator StartTextTransition()
    {
        float journey = 0f;
        float? intendedReward = missionManager?.ActiveMission?.CashReward();
        if (intendedReward == null) intendedReward = 20f;

        while (journey <= textTimer / 2f)
        {
            var posCurvedPercent = posCurve.Evaluate(journey / textTimer);
            var scaleCurvedPercent = scaleCurve.Evaluate(journey / textTimer);

            var missionLabelOffset = Vector2.Lerp(missionLabelStartOffset, missionLabelEndOffset, posCurvedPercent);
            missionLabel.style.translate = new Translate(new Length(missionLabelOffset.x, LengthUnit.Pixel), new Length(missionLabelOffset.y, LengthUnit.Pixel));

            var completeLabelOffset = Vector2.Lerp(completeLabelStartOffset, completeLabelEndOffset, posCurvedPercent);
            completeLabel.style.translate = new Translate(new Length(completeLabelOffset.x, LengthUnit.Pixel), new Length(completeLabelOffset.y, LengthUnit.Pixel));

            var missionLabelSize = Vector2.Lerp(missionLabelStartSize, Vector2.one, scaleCurvedPercent);
            missionLabel.style.scale = new Scale(missionLabelSize);

            var completeLabelSize = Vector2.Lerp(completeLabelStartSize, Vector2.one, scaleCurvedPercent);
            completeLabel.style.scale = new Scale(completeLabelSize);

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

            var missionLabelOffset = Vector2.Lerp(missionLabelStartOffset, missionLabelEndOffset, posCurvedPercent);
            missionLabel.style.translate = new Translate(new Length(missionLabelOffset.x, LengthUnit.Pixel), new Length(missionLabelOffset.y, LengthUnit.Pixel));

            var completeLabelOffset = Vector2.Lerp(completeLabelStartOffset, completeLabelEndOffset, posCurvedPercent);
            completeLabel.style.translate = new Translate(new Length(completeLabelOffset.x, LengthUnit.Pixel), new Length(completeLabelOffset.y, LengthUnit.Pixel));

            var missionLabelSize = Vector2.Lerp(Vector2.one, missionLabelEndSize, scaleCurvedPercent);
            missionLabel.style.scale = new Scale(missionLabelSize);

            var completeLabelSize = Vector2.Lerp(Vector2.one, completeLabelEndSize, scaleCurvedPercent);
            completeLabel.style.scale = new Scale(completeLabelSize);

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
