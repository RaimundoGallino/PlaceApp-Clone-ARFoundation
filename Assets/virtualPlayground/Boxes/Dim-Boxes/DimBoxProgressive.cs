using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace DimBoxes
{
    [System.Serializable]
    public enum Animation_mode { none, stroke, centre_out, centre_in, centre_diag_out, centre_diag_in, corner_diag };
    [ExecuteInEditMode]
    public class DimBoxProgressive : DimBox, BoxProgressive
    {
        [SerializeField]
        private Animation_mode animation_mode = Animation_mode.stroke;

        public int anim_mode
        {
            get
            {
                return (int)animation_mode;
            }
            set
            {
                animation_mode = (Animation_mode)value;
                //reset shaders
                lineMaterial.SetFloat("_Animated", 0);
                lineMaterial.DisableKeyword("_ANIMATED_CENTRAL");
                lineMaterial.DisableKeyword("_ANIMATED_DIAGONAL_CORNER");
                lineMaterial.DisableKeyword("_ANIMATED_DIAGONAL_CENTRE");
                //

                if (value == 2 || value == 3 || value == 4 || value == 5)
                {
                    lineMaterial.SetVector("_Centre", transform.TransformPoint(boundOffset));
                }

                if (value == 2 || value == 3)
                {
                    lineMaterial.SetVector("_BoxDirZ", transform.forward);
                    lineMaterial.SetVector("_BoxDirY", transform.up);
                    lineMaterial.SetVector("_BoxDirX", transform.right);
                }

                if (value == 3 || value == 5)
                {
                    lineMaterial.SetFloat("_inverse", 1);
                }
                else
                {
                    lineMaterial.SetFloat("_inverse", 0);
                }
                lineMaterial.SetVector("_BoxExtent", bound.extents);
                int k = 0;

                if (value == 4 || value == 5 || value == 6)
                {
                    k = SortCornerForDiagonalAnimation();
                    int l = (k + 2) % 4;
                    //Debug.Log(k.ToString() + " | " + l.ToString());
                    Vector3 diagPlane = transform.TransformPoint(corners[(k + 2) % 4]) - transform.TransformPoint(corners[k]);
                    lineMaterial.SetVector("_DiagPlane", diagPlane.normalized);
                }

                if (value == 6)
                {
                    lineMaterial.SetVector("_Centre", transform.TransformPoint(corners[k]));
                }
                if (value == 2 || value == 3)
                {
                    lineMaterial.SetFloat("_Animated", 1);
                    lineMaterial.EnableKeyword("_ANIMATED_CENTRAL");
                }

                if (value == 6)
                {
                    lineMaterial.SetFloat("_Animated", 2);
                    lineMaterial.EnableKeyword("_ANIMATED_DIAGONAL_CORNER");
                }

                if (value == 4 || value == 5)
                {
                    lineMaterial.SetFloat("_Animated", 3);
                    lineMaterial.EnableKeyword("_ANIMATED_DIAGONAL_CENTRE");
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        private float progress = 0.5f;

        private float boxProgress;

        public float Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                if((int)animation_mode == 1)
                {
                    boxProgress = value;
                    if(Application.isPlaying) SetLines();
                }
                else
                {
                    boxProgress = 1;
                    if ((int)animation_mode == 3 || (int)animation_mode == 5)
                    {
                        lineMaterial.SetFloat("_offset", 1 - value);
                    }
                    else
                    {
                        lineMaterial.SetFloat("_offset", value);
                    }
                }
            }
        }

        protected override void Init()
        {
            base.Init();
            anim_mode = (int)animation_mode;
            Progress = progress;
            //string[] enum_names = enum.GetNames(Animation_mode);
        }

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
            int m = (int)animation_mode;
            if (m>0)
            {
                if (hDimensionMesh) hDimensionMesh.GetComponent<Renderer>().enabled = progress > 0.5f;
                if (dDimensionMesh) dDimensionMesh.GetComponent<Renderer>().enabled = progress > 0.5f;
                if (wDimensionMesh) wDimensionMesh.GetComponent<Renderer>().enabled = progress > 0.5f;
            }

            if (m == 1)
            {
                if (hDimensionMesh) hDimensionMesh.GetComponent<Renderer>().enabled = (progress >= 1.0f);
                if (dDimensionMesh) dDimensionMesh.GetComponent<Renderer>().enabled = (progress >= 1.0f);
                if (wDimensionMesh) wDimensionMesh.GetComponent<Renderer>().enabled = (progress >= 1.0f);
            }

        }

        /* void OnMouseDown()
         {
             if (!interactive) return;
             enabled = !enabled;
             hDimensionMesh.SetActive(enabled);
             dDimensionMesh.SetActive(enabled);
             wDimensionMesh.SetActive(enabled);
         }*/

        struct PointData
        {
            public int index;
            public Vector3 vector;
            public PointData(int _index, Vector3 _vector)
            {
                index = _index;
                vector = _vector;
            }
        }

        int SortCornerForDiagonalAnimation()
        {
            Quaternion camRot = Camera.main.transform.rotation;
            Vector3 angles = camRot.eulerAngles;
            angles.z = 0;
            Camera.main.transform.rotation = Quaternion.Euler(angles);

            List<PointData> PointsList = new List<PointData>();

            for (int i = 0; i < corners.Length; i++)
            {
                PointsList.Add(new PointData(i, Camera.main.WorldToScreenPoint(transform.TransformPoint(corners[i]))));
            }

            var result = PointsList.OrderBy(pd => pd.vector.y).ToList();
            int selectedIndex = result[0].index;
            int selectedIndex2 = result[1].index;
            int corner = selectedIndex;

            if (result[1].vector.x < result[0].vector.x)
            {
                corner = selectedIndex2;
            }
            Camera.main.transform.rotation = camRot;
            return corner;
        }

        float GetTextHeight()
        {
            return 0.1f*charSize;
        }

        public void SetProgress(float val)
        {
            Progress = val;
        }
        public bool IsEnabled()
        {
            return enabled;
        }
        public void IsEnabled(bool val)
        {
            enabled = val;
        }
    }
}
