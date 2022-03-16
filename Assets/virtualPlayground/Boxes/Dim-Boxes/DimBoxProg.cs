using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace DimBoxes
{
    [ExecuteInEditMode]

    public class DimBoxProg : DimBox
    {
        private bool coroutineRunning = false;
        [Range(0, 1)]
        public float boxProgress = 0.5f;

        protected override void SetLines()
        {
            List<Vector3[]> _lines = new List<Vector3[]>();

            //box lines
            Vector3[] _line;
            Vector3 endpoint;
            if (corners.Length == 0) return;
            for (int i = 0; i < 4; i++)
            {
                float _scale = i % 2 * bound.size.x + (i + 1) % 2 * bound.size.z;
                //bottom rect
                if (boxProgress < 1)
                {
                    endpoint = corners[i + 4] - Quaternion.AngleAxis(-90 * i, Vector3.up) * Vector3.forward * _scale * boxProgress;
                }
                else
                {
                    endpoint = corners[4 + (i + 1) % 4];
                }
                _line = new Vector3[] { corners[4 + i], endpoint };
                _lines.Add(_line);
                //height
                if (boxProgress < 1)
                {
                    endpoint = i % 2 * corners[i + 4] + (i + 1) % 2 * corners[i] - Vector3.up * bound.size.y * (1 - 2 * (i % 2)) * boxProgress;
                }
                else
                {
                    endpoint = i % 2 * corners[i] + (i + 1) % 2 * corners[i + 4];
                }
                _line = new Vector3[] { i % 2 * corners[i + 4] + (i + 1) % 2 * corners[i], endpoint };
                _lines.Add(_line);
                //top rect
                if (boxProgress < 1)
                {
                    endpoint = corners[i] - Quaternion.AngleAxis(-90 * i, Vector3.up) * Vector3.forward * _scale * boxProgress;
                }
                else
                {
                    endpoint = corners[(i + 1) % 4];
                }
                _line = new Vector3[] { corners[i], endpoint };
                _lines.Add(_line);
            }

            triangles = new Vector3[0][];

            if (boxProgress >= 1) DoExtensionsAndTriangles(_lines);

            lines = new Vector3[_lines.Count, 2];
            for (int j = 0; j < _lines.Count; j++)
            {
                lines[j, 0] = _lines[j][0];
                lines[j, 1] = _lines[j][1];
            }
        }
    }
}
