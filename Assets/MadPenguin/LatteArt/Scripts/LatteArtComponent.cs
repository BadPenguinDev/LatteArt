using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MadPenguin.LatteArt
{
    public class LatteArtComponent : MonoBehaviour
    {
        [System.Serializable]
        public class Column
        {
            public bool[] row;

            public Column(int height)
            {
                row = new bool[height];
            }
            
            public int GetRowCount()
            {
                return row.Length;
            }
        }
    
        [SerializeField] private Image _imageOverlay;
        [SerializeField] private RawImage _imageLatte;

        private LatteArtData _targetData;
        private Column[] _scoreArray;
        private Texture2D _latteTexture;

        private RectTransform _rectTransform;
        private Canvas _rootCanvas;

        [HideInInspector] public UnityEvent<int> scoreUpdated;

        public void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rootCanvas = GetComponent<Canvas>().rootCanvas;
        }


        public void SetLatteArtData(LatteArtData latteArtData)
        {
            _targetData = latteArtData;
            
            var columnCount = latteArtData.scoreArray.Length;
            var rowCount = latteArtData.scoreArray[0].GetRowCount();
            
            // Initialize Score Array
            _scoreArray = new Column[columnCount];
            for (var i = 0; i < columnCount; i++)
            {
                _scoreArray[i] = new Column(rowCount);
                for (var j = 0; j < rowCount; j++)
                {
                    _scoreArray[i].row[j] = false;
                }
            }
            
            // Set Coffee Texture
            _imageOverlay.sprite = latteArtData.spriteCoffee;
            
            // Clean Latte Texture
            var transparent = new Color(1f, 1f, 1f, 0f);
            _latteTexture = new Texture2D(columnCount, rowCount);
            for (var i = 0; i < columnCount; i++)
            {
                for (var j = 0; j < rowCount; j++)
                {
                    _latteTexture.SetPixel(i, j, transparent);
                }
            }
            _latteTexture.Apply();
            _imageLatte.texture = _latteTexture;
        }

        public void PaintLatte(Vector2 mousePosition, int size)
        {
            // Get Local Point
            Vector2 localPoint;
            if (_rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay && 
                _rootCanvas.worldCamera != null)
            {
                //Canvas is in Camera mode
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, mousePosition, _rootCanvas.worldCamera, out localPoint);
            }
            else
            {
                //Canvas is in Overlay mode
                var position = _rectTransform.position;
                localPoint = mousePosition - new Vector2(position.x, position.y);
                
                var lossyScale = _rectTransform.lossyScale;
                localPoint = new Vector2(localPoint.x / lossyScale.x, localPoint.y / lossyScale.y);
            }
            
            // Calculate Local Point by Pivot and Anchor
            localPoint += _rectTransform.pivot * _rectTransform.sizeDelta;
            
            // Collect Updated Point
            var updatedPoints = new List<Vector2Int>();
            var radius = 2 + size * 3;
            
            var localPointInt = Vector2Int.RoundToInt(localPoint);
            for (var i = localPointInt.x - radius; i <= localPointInt.x + radius; i++)
            {
                // Check Point X out of Texture
                if (i < 0 || 
                    i >= _latteTexture.width)
                    continue;
                
                for (var j = localPointInt.y - radius; j <= localPointInt.y + radius; j++)
                {
                    // Check Point Y out of Texture
                    if (j < 0 || 
                        j >= _latteTexture.height)
                        continue;
                    
                    // Check Point in radius
                    if (Vector2Int.Distance(new Vector2Int(i, j), localPointInt) > radius)
                        continue;
                    
                    // Check Already Painted
                    if (_scoreArray[i].row[j])
                        continue;
                    
                    updatedPoints.Add(new Vector2Int(i, j));
                }
            }
            
            // Paint Collected Point
            int score = 0;
            foreach (var point in updatedPoints)
            {
                if (_targetData.CheckScoring(point.x, point.y))
                {
                    _latteTexture.SetPixel(point.x, point.y, Color.white);
                    score += 1;
                }
                else
                {
                    _latteTexture.SetPixel(point.x, point.y, Color.red);
                    score -= 2;
                }

                _scoreArray[point.x].row[point.y] = true;
            }
            _latteTexture.Apply();
            _imageLatte.texture = _latteTexture;
            
            // Broadcast Score
            if (score != 0)
                scoreUpdated.Invoke(score);
        }
    }
}