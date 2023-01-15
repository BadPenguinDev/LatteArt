using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace MadPenguin
{
    public class Commons : MonoBehaviour
    {
        public static List<Vector2Int> GetPointsInLine(Vector2Int start, Vector2Int end)
        {
            var points = new List<Vector2Int> { start };
            if (start == end) 
                return points;
            
            // 처음 찍을 점은 시작점으로 한다.
            var curPoint = start;
            var lineSize = new Vector2Int(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));
                
            var increaseX = (end.x > start.x) ? 1 : -1;
            var increaseY = (end.y > start.y) ? 1 : -1;
            
            // 기울기 <= 1
            if (lineSize.y <= lineSize.x)
            {
                var point = 2 * (lineSize.y - lineSize.x);
                var y = start.y;
            
                for (var x = start.x; 
                    (start.x <= end.x ? x <= end.x : x >= end.x); 
                     x += increaseX)
                {
                    if (0 >= point)
                    {
                        point += 2 * lineSize.y;
                    }
                    else
                    {
                        point += 2 * (lineSize.y - lineSize.x);
                        y += increaseY;
                    }
                    points.Add(new Vector2Int(x, y));
                }
            }
            // 기울기 > 1 
            else 
            { 
                var point = 2 * (lineSize.x - lineSize.y);
                var x = start.x; 
                
                for (var y = start.y; 
                    (start.y <= end.y ? y <= end.y : y >= end.y); 
                     y += increaseY)
                {
                    if (0 >= point)
                    { 
                        point += 2 * lineSize.x; 
                    } 
                    else 
                    { 
                        point += 2 * (lineSize.x - lineSize.y); 
                        x += increaseX; 
                    } 
                    points.Add(new Vector2Int(x, y)); 
                } 
            } 
            return points; 
        }
    }
}