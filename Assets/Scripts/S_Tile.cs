using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_Tile : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _valueText;

    [Header("References")]
    [SerializeField] private S_TileAnimator _animator;

    private SO_TileData _tileData;
    private int _dataIndex;
    

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
    
    public int GetIDataIndex()
    {
        return _dataIndex;
    }

    public void AnimateMerge()
    {
        _animator.AnimateMerge();
    }
    public void AnimateToPosition(Vector2 targetPosition)
    {
        _animator.AnimateToPosition(targetPosition);
    }
    
    public void AnimateSpawn()
    {
        _animator.AnimateSpawn();
    }

}