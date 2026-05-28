using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_Tile : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _valueText;

    [Header("Animation Settings")]
    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private float moveDuration = 0.1f;
    [SerializeField] private AnimationCurve spawnCurve;
    [SerializeField] private float spawnDuration = 0.15f;
    [SerializeField] private AnimationCurve mergeCurve;
    [SerializeField] private float mergeDuration = 0.15f;

    
    

    private SO_TileData _tileData;
    private int _dataIndex;

    private static readonly AnimationCurve LinearCurve = AnimationCurve.Linear(0, 0, 1, 1);
    

    public void Init(SO_TileData data, int dataIndex)
    {
        _tileData = data;
        _dataIndex = dataIndex;
        UpdateColor();
        UpdateText();
    }

    public void UpdateColor() 
    {
        Color color = _tileData.color;
        _background.color = color;   
    }
    
    private void UpdateText()
    {
        if (_valueText != null)
            _valueText.text = _tileData.value.ToString();
    }
    
    public int GetValue()
    {
        return _tileData.value;
    }
    
    public void UpgradeData(SO_TileData newData)
    {
        _tileData = newData;
        _dataIndex++;
        UpdateColor();
        UpdateText();
        AnimateMerge();
    }

    public void AnimateMerge()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(MergeCoroutine(mergeDuration));
        else
            transform.localScale = Vector3.one;
    }

    private IEnumerator MergeCoroutine(float duration)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 originalScale = Vector3.one;
        Vector3 expandedScale = Vector3.one * 1.2f;

        AnimationCurve curve = mergeCurve != null ? mergeCurve : LinearCurve;

        float halfDuration = duration / 2f;
        float elapsedTime = 0f;

        // Expandir
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            float curveValue = curve.Evaluate(t);
            rectTransform.localScale = Vector3.Lerp(originalScale, expandedScale, curveValue);
            yield return null;
        }

        // Contraer
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            float curveValue = curve.Evaluate(t);
            rectTransform.localScale = Vector3.Lerp(expandedScale, originalScale, curveValue);
            yield return null;
        }

        rectTransform.localScale = originalScale;
    }
    public int GetIDataIndex()
    {
        return _dataIndex;
    }

    public void AnimateToPosition(Vector2 targetPosition)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(MoveToPositionCoroutine(targetPosition, moveDuration));
    }

    public void AnimateSpawn()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(SpawnCoroutine(spawnDuration));
        else
            transform.localScale = Vector3.one;
    }

    private IEnumerator SpawnCoroutine(float duration)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.zero;
        float elapsedTime = 0f;

        AnimationCurve curve = spawnCurve != null ? spawnCurve : LinearCurve;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float curveValue = curve.Evaluate(t);
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, curveValue);
            yield return null;
        }

        rectTransform.localScale = Vector3.one;
    }

    private IEnumerator MoveToPositionCoroutine(Vector2 targetPosition, float duration)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsedTime = 0f;

        AnimationCurve curve = moveCurve != null ? moveCurve : LinearCurve;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float curveValue = curve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, curveValue);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }

}