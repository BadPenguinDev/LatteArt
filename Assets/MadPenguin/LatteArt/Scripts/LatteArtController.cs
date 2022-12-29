using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MadPenguin.LatteArt
{
    public class LatteArtController : MonoBehaviour
    {
        [Header("Latte Art")]
        [SerializeField] private List<LatteArtData> _latteArtDatas;
        [SerializeField] private LatteArtComponent _latteArtComponent;
        [SerializeField] private float _timeLimit;
        
        [Header("GUI")]
        [SerializeField] private Text _textScore;
        [SerializeField] private Scrollbar _scrollbarTimeLimit;
        
        [Space(8f)]
        [SerializeField] private Image _imageCursorBrush;
        [SerializeField] private List<Sprite> _spriteCursorBrushes;

        private IEnumerator _startTimeLimitCoroutine;
        private RectTransform _cursorRectTransform;
        private int _cursorSize = 2;
        private int _score;
        
        void Start()
        {
            _cursorRectTransform = _imageCursorBrush.GetComponent<RectTransform>();
            
            _latteArtComponent.scoreUpdated.AddListener(OnScoreUpdated);
            
            ChangeLatteArtData();
        }

        private void Update()
        {
            
            var cursorPosition = Vector2Int.RoundToInt(Input.mousePosition * 240f / Screen.width);
            _cursorRectTransform.anchoredPosition = cursorPosition;
            
            if (Input.GetMouseButton(0))
            {
                _latteArtComponent.PaintLatte(Input.mousePosition, _cursorSize);
            }
        }

        void ChangeLatteArtData()
        {
            // Set Random Latte Art Data
            var index = Random.Range(0, _latteArtDatas.Count - 1);
            _latteArtComponent.SetLatteArtData(_latteArtDatas[index]);
            
            // Clear Score
            _score = 0;
            _textScore.text = _score.ToString("n0");
            
            // Start Time Limit Coroutine
            if (_startTimeLimitCoroutine != null)
                StopCoroutine(_startTimeLimitCoroutine);

            _startTimeLimitCoroutine = StartTimeLimitCoroutine();
            StartCoroutine(_startTimeLimitCoroutine);
        }
        IEnumerator StartTimeLimitCoroutine()
        {
            for (var t = 1.0f; t > 0; t -= Time.deltaTime / _timeLimit)
            {
                _scrollbarTimeLimit.size = t;
                yield return null;
            }
            ChangeLatteArtData();
        }

        // Component Delegate
        public void OnScoreUpdated(int score)
        {
            _score += score;
            _textScore.text = _score.ToString("n0");
        }

        // Button Delegate
        public void OnToggleBrushChanged(int size)
        {
            _cursorSize = size;
            _imageCursorBrush.sprite = _spriteCursorBrushes[_cursorSize];
        }
        public void OnButtonCompletePressed()
        {
            ChangeLatteArtData();
        }
    }
}
