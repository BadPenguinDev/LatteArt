using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using Unity.Mathematics;
using UnityEditor;
#endif

namespace MadPenguin.LatteArt
{
    [CreateAssetMenu(menuName = "MadPenguin/Latte Art Data")]
    [System.Serializable]
    public class LatteArtData : ScriptableObject
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

        [SerializeField] private Sprite _spriteCoffee;
        [SerializeField] private Sprite _maskScore;
        [SerializeField] private Column[] _scoreArray;

        public Sprite spriteCoffee
        {
            get { return _spriteCoffee; }
        }

        public Column[] scoreArray
        {
            get { return _scoreArray; }
        }

        public bool CheckScoring(int x, int y)
        {
            return _scoreArray[x].row[y];
        }


        #if UNITY_EDITOR
        public void UpdateScoreArray()
        {
            if (_maskScore == null)
            {
                Debug.Log("Score Mask is invalid.");
                return;
            }

            var texture2D = _maskScore.texture;
            var columnCount = _maskScore.texture.width;
            var rowCount = _maskScore.texture.height;

            _scoreArray = new Column[columnCount];
            for (var i = 0; i < columnCount; i++)
            {
                _scoreArray[i] = new Column(rowCount);
                for (var j = 0; j < rowCount; j++)
                {
                    if (texture2D.GetPixel(i, j) == Color.black)
                        _scoreArray[i].row[j] = false;
                    else
                        _scoreArray[i].row[j] = true;
                }
            }
        }
        #endif
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(LatteArtData))]
    public class LatteArtDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var data = (LatteArtData)target;
            if (GUILayout.Button("Update Score Array"))
            {
                data.UpdateScoreArray();
                EditorUtility.SetDirty(target);
            }
        }
    }
    #endif

}