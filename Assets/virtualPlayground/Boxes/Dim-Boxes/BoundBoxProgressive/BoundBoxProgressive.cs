using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace DimBoxes
{
    public interface BoxProgressive
    {
        bool IsEnabled();
        void SetProgress(float val);
        void IsEnabled(bool val);

    }

    [ExecuteInEditMode]
    public class BoundBoxProgressive : BoundBox, BoxProgressive
    {
        public Camera mainCamera;
        [HideInInspector]

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
                //if (value == 2 |


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
                if ((int)animation_mode == 1)
                {
                    boxProgress = value;
                    if (Application.isPlaying) SetLines();
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

        private Vector3[] animationEndPoints = new Vector3[12];

        private void Start()
        {
            if (mainCamera == null) mainCamera = Camera.main;
        }
        public override void Init()
        {
            base.Init();
            anim_mode = (int)animation_mode;
            Progress = progress;
        }

        protected override void SetLines()
        {
            if (wire_renderer)
            {
                List<Vector3[]> _lines = new List<Vector3[]>();

                if (boxProgress < 1)
                {
                    DoEndPoints();
                }

                Vector3[] _line;
                Vector3 endpoint;
                for (int i = 0; i < 4; i++)
                {
                    //bottom rect
                    endpoint = (boxProgress < 1) ? animationEndPoints[i] : corners[4 + (i + 1) % 4];
                    _line = new Vector3[] { corners[4 + i], endpoint };
                    _lines.Add(_line);
                    //height
                    endpoint = (boxProgress < 1) ? animationEndPoints[i + 4] : i % 2 * corners[i] + (i + 1) % 2 * corners[i + 4];
                    _line = new Vector3[] { i % 2 * corners[i + 4] + (i + 1) % 2 * corners[i], endpoint };
                    _lines.Add(_line);
                    //top rect
                    endpoint = (boxProgress < 1) ? animationEndPoints[i + 8] : corners[(i + 1) % 4];
                    _line = new Vector3[] { corners[i], endpoint };
                    _lines.Add(_line);

                }

                lines = new Vector3[_lines.Count, 2];
                for (int j = 0; j < _lines.Count; j++)
                {
                    lines[j, 0] = _lines[j][0];
                    lines[j, 1] = _lines[j][1];
                }
            }
            //
            if (line_renderer)
            {
                lineList = GetComponentsInChildren<LineRenderer>();
                if (lineList.Length == 0)
                {
                    lineList = new LineRenderer[12];
                    for (int i = 0; i < 12; i++)
                    {
#if UNITY_EDITOR
                        GameObject go = PrefabUtility.InstantiatePrefab(linePrefab) as GameObject;
                        go.transform.SetParent(transform);
                        go.transform.position = Vector3.zero;
                        go.transform.rotation = Quaternion.identity;
#else
                        GameObject go = (GameObject)Instantiate(linePrefab, transform, false);
#endif
                        lineList[i] = go.GetComponent<LineRenderer>();
                     }
                }
                else
                {
                    for (int i = 0; i < lineList.Length; i++)
                    {

#if UNITY_EDITOR
                        if (!Application.isPlaying)
                        {
                            if (PrefabUtility.GetCorrespondingObjectFromSource(lineList[i].gameObject) == linePrefab)
                            {
                                lineList[i].enabled = true;
                            }
                            else
                            {
                                lineList[i].gameObject.SetActive(false);
                                GameObject go = PrefabUtility.InstantiatePrefab(linePrefab) as GameObject;
                                go.transform.SetParent(transform);
                                go.transform.position = Vector3.zero;
                                go.transform.rotation = Quaternion.identity;
                                lineList[i] = go.GetComponent<LineRenderer>();
                            }
                        }
#endif
                        lineList[i].enabled = (boxProgress > 0);
                    }
                }
                Vector3 endpoint;
                for (int i = 0; i < 4; i++)
                {
                    //bottom rect

                    endpoint = (boxProgress < 1) ? animationEndPoints[i] : corners[4 + (i + 1) % 4];
                    lineList[i].SetPositions(new Vector3[] { transform.TransformPoint(corners[4 + i]), transform.TransformPoint(endpoint) });
                    //height
                    endpoint = (boxProgress < 1) ? animationEndPoints[i + 4] : i % 2 * corners[i] + (i + 1) % 2 * corners[i + 4];
                    lineList[i + 4].SetPositions(new Vector3[] { transform.TransformPoint(i % 2 * corners[i + 4] + (i + 1) % 2 * corners[i]), transform.TransformPoint(endpoint) });
                    //top rect
                    endpoint = (boxProgress < 1) ? animationEndPoints[i + 8] : corners[(i + 1) % 4];
                    lineList[i + 8].SetPositions(new Vector3[] { transform.TransformPoint(corners[i]), transform.TransformPoint(endpoint) });
                }
            }
        }

        public override void SetLineRenderers()
        {
            Gradient colorGradient = new Gradient();
            colorGradient.SetKeys(new GradientColorKey[] { new GradientColorKey(lineColor, 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(lineColor.a, 0.0f) });
            foreach (LineRenderer lr in GetComponentsInChildren<LineRenderer>(true))
            {
                lr.startWidth = lineWidth;
                lr.enabled = line_renderer && (boxProgress > 0);
                lr.numCapVertices = numCapVertices;
                lr.colorGradient = colorGradient;
                lr.material = lineMaterial;
            }
        }

        void DoEndPoints()
        {
            for (int i = 0; i < 4; i++)
            {
                float _scale = i % 2 * bound.size.x + (i + 1) % 2 * bound.size.z;
                //bottom rect
                animationEndPoints[i] = corners[i + 4] - Quaternion.AngleAxis(-90 * i, Vector3.up) * Vector3.forward * _scale * boxProgress;
                //height
                animationEndPoints[i + 4] = i % 2 * corners[i + 4] + (i+1) % 2 * corners[i] - Vector3.up * bound.size.y * (1 - 2*(i%2)) * boxProgress;
                //top rect
                animationEndPoints[i + 8] = corners[i] - Quaternion.AngleAxis(-90 * i, Vector3.up) * Vector3.forward * _scale * boxProgress;
            }
        }


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
            Quaternion camRot = mainCamera.transform.rotation;
            Vector3 angles = camRot.eulerAngles;
            angles.z = 0;
            mainCamera.transform.rotation = Quaternion.Euler(angles);

            List<PointData> PointsList = new List<PointData>();

            for (int i = 0; i < corners.Length; i++)
            {
                PointsList.Add(new PointData(i, mainCamera.WorldToScreenPoint(transform.TransformPoint(corners[i]))));
            }

            var result = PointsList.OrderBy(pd => pd.vector.y).ToList();
            int selectedIndex = result[0].index;
            int selectedIndex2 = result[1].index;
            int corner = selectedIndex;

            if (result[1].vector.x < result[0].vector.x)
            {
                corner = selectedIndex2;
            }
            mainCamera.transform.rotation = camRot;
            return corner;
        }

        protected override void OnMouseDown()
        {

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