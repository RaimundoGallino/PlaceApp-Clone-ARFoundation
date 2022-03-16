using UnityEngine;
using System.Collections.Generic;
using System;

//using Vector3 = UnityEngine.Vector3;

using MathGeoLib;

#if UNITY_EDITOR
using Threading;
using UnityEditor;
#endif
#region TextMeshPro
/*using UnityEngine.UI;
using TMPro;*/
#endregion

namespace DimBoxes
{
    [System.Serializable]
    public class bool_xyz
    {
        public bool width = false;
        public bool depth = false; public bool height = false;
        public bool Width    
        {
            set => width = value;
        }
        public bool Depth
        {
            set => depth = value;
        }
        public bool Height
        {
            set => height = value;
        }
    }

   

    [ExecuteInEditMode]

    public class DimBox : MonoBehaviour
    {
        public Camera mainCamera;

        bool colliderBased = false;
        //[Tooltip("permanent or On/Off onMouseDown")]
        //public bool interactive = true; //onMouseDown

        #region floating placement vars
        [HideInInspector]
        [SerializeField]
        private Transform[] PossiblePlacementsHeight;
        [HideInInspector]
        [SerializeField]
        private Transform[] PossiblePlacementsWidth;
        [HideInInspector]
        [SerializeField]
        private Transform[] PossiblePlacementsDepth;

        private Vector3[] EdgesHeight;
        private Vector3[] EdgesWidth;
        private Vector3[] EdgesDepth;

        private Vector3[,] PossibleHeightExtLines;
        private Vector3[,] PossibleWidthExtLines;
        private Vector3[,] PossibleDepthExtLines;
        private Vector3[][][] PossibleHeightArrows;
        private Vector3[][][] PossibleDepthArrows;
        private Vector3[][][] PossibleWidthArrows;
        [HideInInspector]
        public GameObject template;
        #endregion
        [System.Serializable]
        public class PlacementOptions
        {
            public bool floating = false;
            public bool Floating    // the Name property
            {
                set => floating = value;
            }
            //public w_placement widthPlacement;
            [Range(0, 7)]
            public int _widthPlacement = 0;
            public bool w_flip = false;
            //public d_placement depthPlacement;
            [Range(0, 7)]
            public int _depthPlacement = 0;
            public bool d_flip = false;
            //public h_placement heightPlacement;
            [Range(0, 7)]
            public int _heightPlacement = 0;
            public bool h_flip = false;
        }

        public PlacementOptions placementOptions = new PlacementOptions();
       
        public bool_xyz extensions = new bool_xyz();
        public bool_xyz faceCamera = new bool_xyz();

        public Material lineMaterial;
        public Color wireColor = new Color(0f, 1f, 0.4f, 0.74f);
        public float extentionDist = 0.1f;
        public bool drawArrows = true;
        public float arrowSize = 0.12f;
        

        [System.Serializable]
        public class Formatter
        {
            [Tooltip ("The default Unity units are meters. Keep 100 for cm or put 1 for m; 1000 for mm; 39.37 for inch; 3.281 for foot or 1.094 for yard etc.")]
            public float factor = 100.0f;//The default Unity units are meters. Keep 100 for cm or put 1 for m; 1000 for mm; 39.37 for inch; 3.281 for foot or 1.094 for yard etc.
            public string caption = "dimension: ";
            public string format = "0.00";
            public string unit = "cm";
            public bool captionOnly = false;

            public string Format(float f)    
            {
                return caption + (captionOnly?"": (factor * f).ToString(format) + " " + unit);
            }

        }
        public Formatter hformatter = new Formatter();
        public Formatter wformatter = new Formatter();
        public Formatter dformatter = new Formatter();

        [HideInInspector]
        public bool Z_up_orientation = false;

        private Vector3 topFrontLeft;
        private Vector3 topFrontRight;
        private Vector3 topBackLeft;
        private Vector3 topBackRight;
        private Vector3 bottomFrontLeft;
        private Vector3 bottomFrontRight;
        private Vector3 bottomBackLeft;
        private Vector3 bottomBackRight;

        private Vector3[] widthExt;

        private Vector3[] depthExt;
        private Vector3 depthLookAt;
        private Vector3[] heightExt;

        [HideInInspector]
        [SerializeField]
        protected GameObject hDimensionMesh;
        [HideInInspector]
        [SerializeField]
        protected GameObject dDimensionMesh;
        [HideInInspector]
        [SerializeField]
        protected GameObject wDimensionMesh;

        [HideInInspector]
        public bool useTextMeshPro = false;

        #region TextMesh
        public enum dimension_anchor { UpperCenter = TextAnchor.UpperCenter, LowerCenter = TextAnchor.LowerCenter };
        [System.Serializable]
        public class AnchorOptions
        {
            public dimension_anchor width = dimension_anchor.UpperCenter;
            public dimension_anchor depth = dimension_anchor.UpperCenter;
            public dimension_anchor height = dimension_anchor.UpperCenter;
        }
        [Header("TextMesh Settings")]
        public AnchorOptions anchorOptions = new AnchorOptions();
        public Material letterMaterial;
        public Font font;
        public float charSize = 1.0f;
        [HideInInspector]
        [SerializeField]
        protected TextMesh htm;
        [HideInInspector]
        [SerializeField]
        protected TextMesh dtm;
        [HideInInspector]
        [SerializeField]
        protected TextMesh wtm;
        #endregion

        #region TextMeshPro
        /*public enum dimension_alignment { UpperCenter = TextAlignmentOptions.Top, Baseline = TextAlignmentOptions.Baseline, LowerCenter = TextAlignmentOptions.Bottom };
        [System.Serializable]
        public class AlignmentOptions
        {
            public dimension_alignment width = dimension_alignment.UpperCenter;
            public dimension_alignment depth = dimension_alignment.UpperCenter;
            public dimension_alignment height = dimension_alignment.UpperCenter;
        }
        [Header("TextMeshPro Settings")]
        public AlignmentOptions alignmentOptions = new AlignmentOptions();
        public Material letterProMaterial;
        public TMP_FontAsset fontPro;
        [HideInInspector]
        [SerializeField]
        protected TextMeshPro htmp;
        [HideInInspector]
        [SerializeField]
        protected TextMeshPro dtmp;
        [HideInInspector]
        [SerializeField]
        protected TextMeshPro wtmp;
        public float tmpCharSize = 1.0f;*/
        #endregion

        protected Bounds bound;
        protected Vector3 boundOffset = Vector3.zero;
        [HideInInspector]//
        public Bounds meshBound;
        [HideInInspector]//
        public Vector3 meshBoundOffset = Vector3.zero;

        protected Vector3[] corners = new Vector3[0];
        private Vector3[][] arrows;

        protected Vector3[,] lines = new Vector3[0,0];

        protected Vector3[][] triangles = new Vector3[0][];

        [HideInInspector]
        private Vector3 previousScale;
        private Vector3 previousPosition;
        private Quaternion previousRotation;
        private bool mainThreadUpdated = true;

        private Bounds bounds;
        [SerializeField]
        [HideInInspector]
        public bool mApplied = false;

        //Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time. This function is only called in editor mode.
        void Reset()
        {
            Debug.Log("reset");
            TextMesh[] tms = GetComponentsInChildren<TextMesh>();
            foreach(TextMesh tm in tms)
            {
                if (tm.name == "hDimensionMesh") { htm = tm; hDimensionMesh = tm.gameObject; }
                if (tm.name == "wDimensionMesh") { wtm = tm; wDimensionMesh = tm.gameObject; }
                if (tm.name == "dDimensionMesh") { dtm = tm; dDimensionMesh = tm.gameObject; }
            }

            AccurateBounds();
            Start();
        }

        //Reset is called before OnEnable
        void Awake()
        {
            if (!mApplied)
            {
//Can not use dll's attached to MathGeoLib in WebGL
#if UNITY_WEBGL
                CalculateBounds(); //very, very slow
#else
                AccurateBounds(); //based on MathGeoLib
#endif
                Init();
            }
        }

        void Start()
        {
            previousPosition = transform.position;
            previousRotation = transform.rotation;
            previousScale = transform.localScale;

            if (mainCamera == null) mainCamera = Camera.main;
    }

#if UNITY_EDITOR
    public void OnValidate()
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
            {
                PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                Debug.LogWarning("The DimBox class does not support Prefabs, the prefab gets unpacked");
                return;

            }
            Init();
        }
#endif

        void OnDestroy()
        {
            for (int i = 0; i < 8; i++)
            {
                DestroyImmediate(PossiblePlacementsWidth[i].gameObject);
                DestroyImmediate(PossiblePlacementsDepth[i].gameObject);
                DestroyImmediate(PossiblePlacementsHeight[i].gameObject);
            }
        }

        protected virtual void Init()
        {
            if (!mainThreadUpdated) return;
            SetPoints();
            SetLines();
            if (placementOptions.floating) SortDimPositions();     
        }

        void CalculateBounds()
        {
            Quaternion quat = transform.rotation;//object axis AABB
            Vector3 sc = transform.localScale; 

            meshBound = new Bounds();
            MeshFilter[] meshes = GetComponentsInChildren<MeshFilter>();
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            for (int i = 0; i < meshes.Length; i++)
            {
                Mesh ms = meshes[i].sharedMesh;

                int vc = ms.vertexCount;
                for (int j = 0; j < vc; j++)
                {
                    Vector3 localPoint;
                    if (meshes[i].transform != transform)
                    {
                        Vector3 worldPoint = meshes[i].transform.TransformPoint(ms.vertices[j]);
                        localPoint = transform.InverseTransformPoint(worldPoint);
                    }
                    else
                    {
                        //localPoint = 
                    }
                    if (i == 0 && j == 0)
                    {
                        meshBound = new Bounds(meshes[i].transform.TransformPoint(ms.vertices[j]), Vector3.zero);
                    }
                    else
                    {
                        meshBound.Encapsulate(meshes[i].transform.TransformPoint(ms.vertices[j]));
                    }
                }
            }
            transform.rotation = quat;
            transform.localScale = sc;
            meshBoundOffset = meshBound.center - transform.position;
            mApplied = true;
        }


        void SetPoints()
        {
            if (colliderBased)
            {
                BoxCollider bc = GetComponent<BoxCollider>();
                if (!bc)
                {
                    Debug.LogError("no BoxCollider - add BoxCollider to " + gameObject.name + " gameObject");
                    return;

                }
                bound = new Bounds(bc.center, bc.size);
                boundOffset = bc.center;
            }

            else
            {
                bound = meshBound;
                boundOffset = transform.position + transform.rotation*meshBoundOffset;
                boundOffset = meshBoundOffset;
            }
            //startingBoundCenterLocal = transform.InverseTransformPoint(bound.center);
            //bound.size = new Vector3(bound.size.x * transform.localScale.x / startingScale.x, bound.size.y * transform.localScale.y / startingScale.y, bound.size.z * transform.localScale.z / startingScale.z);
            //boundOffset = new Vector3(boundOffset.x * transform.localScale.x / startingScale.x, boundOffset.y * transform.localScale.y / startingScale.y, boundOffset.z * transform.localScale.z / startingScale.z);

            float orientationAngle = Z_up_orientation ? 90 : 0;

            topFrontRight = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(orientationAngle, Vector3.right) * new Vector3(1, 1, 1));
            topFrontLeft = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(orientationAngle, Vector3.right) * new Vector3(-1, 1, 1));
            topBackLeft = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(orientationAngle, Vector3.right) * new Vector3(-1, 1, -1));
            topBackRight = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(orientationAngle, Vector3.right) * new Vector3(1, 1, -1));
            bottomFrontRight = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(orientationAngle, Vector3.right) * new Vector3(1, -1, 1));
            bottomFrontLeft = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(orientationAngle, Vector3.right) * new Vector3(-1, -1, 1));
            bottomBackLeft = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(orientationAngle, Vector3.right) * new Vector3(-1, -1, -1));
            bottomBackRight = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(orientationAngle, Vector3.right) * new Vector3(1, -1, -1));

            corners = new Vector3[] { topFrontRight, topFrontLeft, topBackLeft, topBackRight, bottomFrontRight, bottomFrontLeft, bottomBackLeft, bottomBackRight };
            arrows = new Vector3[6][];
            heightExt = new Vector3[4];
            depthExt = new Vector3[4];
            widthExt = new Vector3[4];

            //float textHeight = GetTextHeight();

            PossiblePlacementsHeight = new Transform[8];
            PossiblePlacementsWidth = new Transform[8];
            PossiblePlacementsDepth = new Transform[8];
            PossibleHeightExtLines = new Vector3[8, 4];
            PossibleWidthExtLines = new Vector3[8, 4];
            PossibleDepthExtLines = new Vector3[8, 4];
            PossibleHeightArrows = new Vector3[8][][];
            PossibleDepthArrows = new Vector3[8][][];
            PossibleWidthArrows = new Vector3[8][][];
            EdgesHeight = new Vector3[4];
            EdgesDepth = new Vector3[4];
            EdgesWidth = new Vector3[4];
            //Vector3 scale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y, 1f / transform.localScale.z);
            for (int i = 0; i < 4; i++)
            {
                InitPlacementTransform(2 * i, PossiblePlacementsHeight, "H");
                InitPlacementTransform(2 * i, PossiblePlacementsWidth, "W");
                InitPlacementTransform(2 * i, PossiblePlacementsDepth, "D");

                //Vector3 hExtDirection = Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(0, 0, -1);//1, 0, 0 / 0, 0, -1
                //Vector3 hTextForward = Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(-1, 0, 0);//0, 0, -1 /-1, 0, 0
                Vector3 hExtDirection = Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, 0, 0);
                Vector3 hTextForward = Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(0, 0, -1);
                Vector3 dExtDirection = Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(0, 1, 0);
                Vector3 dTextForward = Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(-1, 0, 0);
                Vector3 wExtDirection = Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(0, 1, 0);
                Vector3 wTextForward = Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(0, 0, 1);

                Vector3 hExtVector = extensions.height ? extentionDist * hExtDirection : Vector3.zero;
                Vector3 dExtVector = extensions.depth ? extentionDist * dExtDirection : Vector3.zero;
                Vector3 wExtVector = extensions.width ? extentionDist * wExtDirection : Vector3.zero;
                //Vector3 hOffsetVector = faceCamera.height ? Vector3.zero : hExtDirection;// * textHeight;
                //Vector3 dOffsetVector = faceCamera.depth ? Vector3.zero : dExtDirection;// * textHeight;
                //Vector3 wOffsetVector = faceCamera.width ? Vector3.zero : wExtDirection;// * textHeight;

                PossiblePlacementsHeight[2 * i].localPosition = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, 0, -1)) + hExtVector;// + hOffsetVector;
                EdgesHeight[i] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, 0, -1));
                PossiblePlacementsHeight[2 * i].localRotation = Quaternion.LookRotation(hTextForward, hExtDirection);
                PossiblePlacementsWidth[2 * i].localPosition = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(0, 1, -1)) + wExtVector;// + wOffsetVector;
                EdgesWidth[i] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(0, 1, -1));
                PossiblePlacementsWidth[2 * i].localRotation = Quaternion.LookRotation(wTextForward, wExtDirection);
                PossiblePlacementsDepth[2 * i].localPosition = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 1, 0)) + dExtVector;// + dOffsetVector;
                EdgesDepth[i] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 1, 0));
                PossiblePlacementsDepth[2 * i].localRotation = Quaternion.LookRotation(dTextForward, dExtDirection);

                PossibleHeightExtLines[2 * i, 0] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, 1, -1));
                PossibleHeightExtLines[2 * i, 1] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, 1, -1)) + hExtVector;
                PossibleHeightExtLines[2 * i, 2] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, -1, -1)) + hExtVector;
                PossibleHeightExtLines[2 * i, 3] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, -1, -1));

                PossibleWidthExtLines[2 * i, 0] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(1, 1, -1));
                PossibleWidthExtLines[2 * i, 1] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(1, 1, -1)) + wExtVector;
                PossibleWidthExtLines[2 * i, 2] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(-1, 1, -1)) + wExtVector;//(-90 * i, Vector3.forward) * new Vector3(1, 1, -1)) + wExtVector;
                PossibleWidthExtLines[2 * i, 3] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(-1, 1, -1));

                PossibleDepthExtLines[2 * i, 0] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 1, 1));
                PossibleDepthExtLines[2 * i, 1] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 1, 1)) + dExtVector;
                PossibleDepthExtLines[2 * i, 2] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 1, -1)) + dExtVector;//(-90 * i, Vector3.right) * new Vector3(-1, 1, 1)) + dExtVector;
                PossibleDepthExtLines[2 * i, 3] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 1, -1));

                Vector3 _lineDirection = -Vector3.up;
                PossibleHeightArrows[2 * i] = new Vector3[2][];
                PossibleHeightArrows[2 * i][0] = new Vector3[3] { PossibleHeightExtLines[2 * i, 1], PossibleHeightExtLines[2 * i, 1] + (_lineDirection + hExtDirection * 0.3f) * arrowSize, PossibleHeightExtLines[2 * i, 1] + (_lineDirection - hExtDirection * 0.3f) * arrowSize };
                PossibleHeightArrows[2 * i][1] = new Vector3[3] { PossibleHeightExtLines[2 * i, 2], PossibleHeightExtLines[2 * i, 2] + (-_lineDirection + hExtDirection * 0.3f) * arrowSize, PossibleHeightExtLines[2 * i, 2] + (-_lineDirection - hExtDirection * 0.3f) * arrowSize };

                _lineDirection = -Vector3.right;
                PossibleWidthArrows[2 * i] = new Vector3[2][];
                PossibleWidthArrows[2 * i][0] = new Vector3[3] { PossibleWidthExtLines[2 * i, 1], PossibleWidthExtLines[2 * i, 1] + (_lineDirection + wExtDirection * 0.3f) * arrowSize, PossibleWidthExtLines[2 * i, 1] + (_lineDirection - wExtDirection * 0.3f) * arrowSize };
                PossibleWidthArrows[2 * i][1] = new Vector3[3] { PossibleWidthExtLines[2 * i, 2], PossibleWidthExtLines[2 * i, 2] + (-_lineDirection + wExtDirection * 0.3f) * arrowSize, PossibleWidthExtLines[2 * i, 2] + (-_lineDirection - wExtDirection * 0.3f) * arrowSize };

                _lineDirection = -Vector3.forward;
                PossibleDepthArrows[2 * i] = new Vector3[2][];
                PossibleDepthArrows[2 * i][0] = new Vector3[3] { PossibleDepthExtLines[2 * i, 1], PossibleDepthExtLines[2 * i, 1] + (_lineDirection + dExtDirection * 0.3f) * arrowSize, PossibleDepthExtLines[2 * i, 1] + (_lineDirection - dExtDirection * 0.3f) * arrowSize };
                PossibleDepthArrows[2 * i][1] = new Vector3[3] { PossibleDepthExtLines[2 * i, 2], PossibleDepthExtLines[2 * i, 2] + (-_lineDirection + dExtDirection * 0.3f) * arrowSize, PossibleDepthExtLines[2 * i, 2] + (-_lineDirection - dExtDirection * 0.3f) * arrowSize };

                InitPlacementTransform(2 * i + 1, PossiblePlacementsHeight, "H");
                InitPlacementTransform(2 * i + 1, PossiblePlacementsWidth, "W");
                InitPlacementTransform(2 * i + 1, PossiblePlacementsDepth, "D");

                //hExtDirection = Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, 0, 0);
                //hTextForward = Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(0, 0, -1);
                hExtDirection = Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(0, 0, -1);//1, 0, 0 / 0, 0, -1
                hTextForward = Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(-1, 0, 0);//0, 0, -1 /-1, 0, 0
                dExtDirection = Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 0, 0);
                dTextForward = Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(0, 1, 0);
                wExtDirection = Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(0, 0, -1);
                wTextForward = Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(0, 1, 0);

                hExtVector = extensions.height ? extentionDist * hExtDirection : Vector3.zero;
                dExtVector = extensions.depth ? extentionDist * dExtDirection : Vector3.zero;
                wExtVector = extensions.width ? extentionDist * wExtDirection : Vector3.zero;
                //hOffsetVector = faceCamera.height ? Vector3.zero : hExtDirection;// * textHeight;
                //dOffsetVector = faceCamera.depth ? Vector3.zero : dExtDirection;// * textHeight;
                //wOffsetVector = faceCamera.width ? Vector3.zero : wExtDirection;// * textHeight;

                PossiblePlacementsHeight[2 * i + 1].localPosition = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, 0, -1)) + hExtVector;// + hOffsetVector;
                PossiblePlacementsHeight[2 * i + 1].localRotation = Quaternion.LookRotation(hTextForward, hExtDirection);
                PossiblePlacementsWidth[2 * i + 1].localPosition = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(0, 1, -1)) + wExtVector;// + wOffsetVector;//-90 * i, Vector3.forward) * new Vector3(1, 1, 0)
                PossiblePlacementsWidth[2 * i + 1].localRotation = Quaternion.LookRotation(wTextForward, wExtDirection);
                PossiblePlacementsDepth[2 * i + 1].localPosition = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 1, 0)) + dExtVector;// + dOffsetVector;//-90 * i, Vector3.right) * new Vector3(0, 1, 1)
                PossiblePlacementsDepth[2 * i + 1].localRotation = Quaternion.LookRotation(dTextForward, dExtDirection);

                PossibleHeightExtLines[2 * i + 1, 0] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, 1, -1));
                PossibleHeightExtLines[2 * i + 1, 1] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, 1, -1)) + hExtVector;
                PossibleHeightExtLines[2 * i + 1, 2] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, -1, -1)) + hExtVector;
                PossibleHeightExtLines[2 * i + 1, 3] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(90 * i, Vector3.up) * new Vector3(1, -1, -1));

                PossibleWidthExtLines[2 * i + 1, 0] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(1, 1, -1));
                PossibleWidthExtLines[2 * i + 1, 1] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(1, 1, -1)) + wExtVector;
                PossibleWidthExtLines[2 * i + 1, 2] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(-1, 1, -1)) + wExtVector;//(-90 * i, Vector3.forward) * new Vector3(1, 1, -1)) + wExtVector;
                PossibleWidthExtLines[2 * i + 1, 3] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.right) * new Vector3(-1, 1, -1));

                PossibleDepthExtLines[2 * i + 1, 0] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 1, 1));
                PossibleDepthExtLines[2 * i + 1, 1] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 1, 1)) + dExtVector;
                PossibleDepthExtLines[2 * i + 1, 2] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 1, -1)) + dExtVector;//(-90 * i, Vector3.right) * new Vector3(-1, 1, 1)) + dExtVector;
                PossibleDepthExtLines[2 * i + 1, 3] = boundOffset + Vector3.Scale(bound.extents, Quaternion.AngleAxis(-90 * i, Vector3.forward) * new Vector3(1, 1, -1));

                _lineDirection = -Vector3.up;
                PossibleHeightArrows[2 * i + 1] = new Vector3[2][];
                PossibleHeightArrows[2 * i + 1][0] = new Vector3[3] { PossibleHeightExtLines[2 * i + 1, 1], PossibleHeightExtLines[2 * i + 1, 1] + (_lineDirection + hExtDirection * 0.3f) * arrowSize, PossibleHeightExtLines[2 * i + 1, 1] + (_lineDirection - hExtDirection * 0.3f) * arrowSize };
                PossibleHeightArrows[2 * i + 1][1] = new Vector3[3] { PossibleHeightExtLines[2 * i + 1, 2], PossibleHeightExtLines[2 * i + 1, 2] + (-_lineDirection + hExtDirection * 0.3f) * arrowSize, PossibleHeightExtLines[2 * i + 1, 2] + (-_lineDirection - hExtDirection * 0.3f) * arrowSize };

                _lineDirection = -Vector3.right;
                PossibleWidthArrows[2 * i + 1] = new Vector3[2][];
                PossibleWidthArrows[2 * i + 1][0] = new Vector3[3] { PossibleWidthExtLines[2 * i + 1, 1], PossibleWidthExtLines[2 * i + 1, 1] + (_lineDirection + wExtDirection * 0.3f) * arrowSize, PossibleWidthExtLines[2 * i + 1, 1] + (_lineDirection - wExtDirection * 0.3f) * arrowSize };
                PossibleWidthArrows[2 * i + 1][1] = new Vector3[3] { PossibleWidthExtLines[2 * i + 1, 2], PossibleWidthExtLines[2 * i + 1, 2] + (-_lineDirection + wExtDirection * 0.3f) * arrowSize, PossibleWidthExtLines[2 * i + 1, 2] + (-_lineDirection - wExtDirection * 0.3f) * arrowSize };

                _lineDirection = -Vector3.forward;
                PossibleDepthArrows[2 * i + 1] = new Vector3[2][];
                PossibleDepthArrows[2 * i + 1][0] = new Vector3[3] { PossibleDepthExtLines[2 * i + 1, 1], PossibleDepthExtLines[2 * i + 1, 1] + (_lineDirection + dExtDirection * 0.3f) * arrowSize, PossibleDepthExtLines[2 * i + 1, 1] + (_lineDirection - dExtDirection * 0.3f) * arrowSize };
                PossibleDepthArrows[2 * i + 1][1] = new Vector3[3] { PossibleDepthExtLines[2 * i + 1, 2], PossibleDepthExtLines[2 * i + 1, 2] + (-_lineDirection + dExtDirection * 0.3f) * arrowSize, PossibleDepthExtLines[2 * i + 1, 2] + (-_lineDirection - dExtDirection * 0.3f) * arrowSize };
            }

            AddText();
        }

        private void InitPlacementTransform(int i, Transform[] _array, string s)
        {
            string name = s + i.ToString();
            Transform g = transform.Find(name);
            if (!g)
            {
                if (false)
                {
                    _array[i] = UnityEngine.Object.Instantiate(template).transform;
                }
                else
                {
                    _array[i] = new GameObject(name).transform;
                }
                _array[i].name = name;
                _array[i].SetParent(transform);
            }
            else
            {
                _array[i] = g.transform;
            }
        }

        protected virtual void SetLines()
        {

            List<Vector3[]> _lines = new List<Vector3[]>();

            //box lines
            Vector3[] _line;
            if (corners.Length == 0) return;
            for (int i = 0; i < 4; i++)
            {
                //bottom rect
                _line = new Vector3[] { corners[4 + i], corners[4 + (i + 1) % 4] };
                _lines.Add(_line);
                //height
                _line = new Vector3[] { i % 2 * corners[i + 4] + (i + 1) % 2 * corners[i], i % 2 * corners[i] + (i + 1) % 2 * corners[i + 4] };
                _lines.Add(_line);
                //top rect
                _line = new Vector3[] { corners[i], corners[(i + 1) % 4] };
                _lines.Add(_line);
            }
            //*/
            triangles = new Vector3[0][];
            DoExtensionsAndTriangles(_lines);

            lines = new Vector3[_lines.Count, 2];
            for (int j = 0; j < _lines.Count; j++)
            {
                lines[j, 0] = _lines[j][0];
                lines[j, 1] = _lines[j][1];
            }

        }

        protected void DoExtensionsAndTriangles(List<Vector3[]> _lines)
        {
            if (extensions.width)
            {
                _lines.Add(new Vector3[] { widthExt[0], widthExt[1] });
                _lines.Add(new Vector3[] { widthExt[1], widthExt[2] });
                _lines.Add(new Vector3[] { widthExt[2], widthExt[3] });
            }
            if (extensions.depth)
            {
                _lines.Add(new Vector3[] { depthExt[0], depthExt[1] });
                _lines.Add(new Vector3[] { depthExt[1], depthExt[2] });
                _lines.Add(new Vector3[] { depthExt[2], depthExt[3] });
            }
            if (extensions.height)
            {
                _lines.Add(new Vector3[] { heightExt[0], heightExt[1] });
                _lines.Add(new Vector3[] { heightExt[1], heightExt[2] });
                _lines.Add(new Vector3[] { heightExt[2], heightExt[3] });
            }


            if (drawArrows)
            {
                triangles = new Vector3[arrows.Length][];
                for (int i = 0; i < arrows.Length; i++)
                {
                    triangles[i] = new Vector3[arrows[i].Length];
                    for (int j = 0; j < arrows[i].Length; j++)
                    {
                        triangles[i][j] = arrows[i][j];
                    }
                }
            }
        }
        public Vector3 GetSize()
        {
            return Vector3.Scale(bound.size, transform.localScale);
        }
        void AddText()
        {
            //Debug.Log("addText");
            Quaternion flipped = Quaternion.Euler(0,180f,0);
            Vector3 scale = transform.localScale;
            if (!hDimensionMesh)
            {
                hDimensionMesh = new GameObject("hDimensionMesh");
            }
            int i = placementOptions._heightPlacement;//(int)placementOptions.heightPlacement;
#if UNITY_EDITOR
            if (!PrefabUtility.IsPartOfAnyPrefab(hDimensionMesh))
#endif
            hDimensionMesh.transform.SetParent(PossiblePlacementsHeight[i]);// hDimension.transform;
            hDimensionMesh.transform.localPosition = Vector3.zero;
            hDimensionMesh.transform.localRotation = placementOptions.h_flip ? flipped : Quaternion.identity;
            if (useTextMeshPro)
            {
                #region TextMeshPro
                /*if (!htmp)
                {
                    RemoveAllComponents(hDimensionMesh);
                    htmp = hDimensionMesh.AddComponent<TextMeshPro>();
                    ContentSizeFitter csf = hDimensionMesh.AddComponent<ContentSizeFitter>();
                    csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    csf.verticalFit = ContentSizeFitter.FitMode.MinSize;
                }
                if (fontPro) htmp.font = fontPro;
                htmp.fontMaterial = fontPro.material;
                htmp.alignment = TextAlignmentOptions.Center;
                htmp.alignment = (TextAlignmentOptions)alignmentOptions.height;
                if (letterProMaterial) htmp.fontMaterial = letterProMaterial;
                htmp.fontSize = tmpCharSize;
                htmp.text = hformatter.Format(bound.size.y * scale.y);
                hDimensionMesh.transform.localScale = Vector3.one;*/
                #endregion
            }
            else
            {
                #region TextMesh
                if (!htm)
                {
                    RemoveAllComponents(hDimensionMesh);
                    htm = hDimensionMesh.AddComponent<TextMesh>();
                }
                htm.anchor = (TextAnchor)anchorOptions.height;
                htm.font = font;
                htm.characterSize = charSize;
                htm.GetComponent<Renderer>().material = letterMaterial;
                htm.GetComponent<Renderer>().enabled = true;
                //htm.text = formatter.captionMode ? formatter.captionSettings.heightCaption : formatter.Format(bound.size.y * scale.y);// *remove
                htm.text = hformatter.Format(bound.size.y * scale.y);// *uncomment separate setting for each dimension
                hDimensionMesh.transform.localScale = 0.02f * Vector3.one;
                #endregion
            }
            if (faceCamera.height)
            {
                hDimensionMesh.transform.localRotation = Quaternion.Euler(new Vector3(180f,  0, -90f));
            }
            arrows[4] = PossibleHeightArrows[i][0];
            arrows[5] = PossibleHeightArrows[i][1];
            heightExt[0] = PossibleHeightExtLines[i, 0];
            heightExt[1] = PossibleHeightExtLines[i, 1];
            heightExt[2] = PossibleHeightExtLines[i, 2];
            heightExt[3] = PossibleHeightExtLines[i, 3];

            if (!wDimensionMesh)
            {
                wDimensionMesh = new GameObject("wDimensionMesh");
            }
            i = (int)placementOptions._widthPlacement;//.widthPlacement;
#if UNITY_EDITOR
            if (!PrefabUtility.IsPartOfAnyPrefab(wDimensionMesh))
#endif
            wDimensionMesh.transform.SetParent(PossiblePlacementsWidth[i]); //wDimension.transform;
            wDimensionMesh.transform.localPosition = Vector3.zero;
            wDimensionMesh.transform.localRotation = placementOptions.w_flip ? flipped : Quaternion.identity;
            if (useTextMeshPro)
            {
                #region TextMeshPro
                /*if (!wtmp)
                {
                    RemoveAllComponents(wDimensionMesh);
                    wtmp = wDimensionMesh.AddComponent<TextMeshPro>();
                    ContentSizeFitter csf = wDimensionMesh.AddComponent<ContentSizeFitter>();
                    csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    csf.verticalFit = ContentSizeFitter.FitMode.MinSize;
                }
                if (fontPro) wtmp.font = fontPro;
                wtmp.alignment = TextAlignmentOptions.Center;
                wtmp.alignment = (TextAlignmentOptions)alignmentOptions.width;
                if (letterProMaterial) wtmp.fontMaterial = letterProMaterial;
                wtmp.fontSize = tmpCharSize;
                wtmp.text = wformatter.Format(bound.size.x * scale.x);
                wDimensionMesh.transform.localScale = Vector3.one;*/
                #endregion
            }
            else
            {
                #region TextMesh
                if (!wtm)
                {
                    RemoveAllComponents(wDimensionMesh);
                    wtm = wDimensionMesh.AddComponent<TextMesh>();
                }
                wtm.anchor = (TextAnchor)anchorOptions.width;
                wtm.font = font;
                wtm.characterSize = charSize;
                wtm.GetComponent<Renderer>().material = letterMaterial;
                wtm.GetComponent<Renderer>().enabled = true;
                wtm.text = wformatter.Format(bound.size.x * scale.x);
                hDimensionMesh.transform.localScale = 0.02f * Vector3.one;
                wDimensionMesh.transform.localScale = 0.02f * Vector3.one;
                #endregion
            }
            if (faceCamera.width)
            {
               wDimensionMesh.transform.localRotation = Quaternion.Euler(new Vector3(180f, 0f, -90f));
            }
            arrows[0] = PossibleWidthArrows[i][0];
            arrows[1] = PossibleWidthArrows[i][1];
            widthExt[0] = PossibleWidthExtLines[i, 0];
            widthExt[1] = PossibleWidthExtLines[i, 1];
            widthExt[2] = PossibleWidthExtLines[i, 2];
            widthExt[3] = PossibleWidthExtLines[i, 3];

            if (!dDimensionMesh)
            {
                dDimensionMesh = new GameObject("dDimensionMesh");
            }
            i = (int)placementOptions._depthPlacement;//depthPlacement;
                                         
#if UNITY_EDITOR
            if (!PrefabUtility.IsPartOfAnyPrefab(dDimensionMesh))
#endif
            dDimensionMesh.transform.SetParent(PossiblePlacementsDepth[i]); //dDimension.transform;
            dDimensionMesh.transform.localPosition = Vector3.zero;
            dDimensionMesh.transform.localRotation = placementOptions.d_flip ? flipped : Quaternion.identity;
            if (useTextMeshPro)
            {
                #region TextMeshPro
                /*if (!dtmp)
                {
                    RemoveAllComponents(dDimensionMesh);
                    MeshFilter mf = dDimensionMesh.GetComponent<MeshFilter>();
                    if (mf) DestroyImmediate(mf);
                    dtmp = dDimensionMesh.AddComponent<TextMeshPro>();
                    ContentSizeFitter csf = dDimensionMesh.AddComponent<ContentSizeFitter>();
                    csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    csf.verticalFit = ContentSizeFitter.FitMode.MinSize;
                }
                if (fontPro) dtmp.font = fontPro;
                dtmp.alignment = TextAlignmentOptions.Center;
                dtmp.alignment = (TextAlignmentOptions)alignmentOptions.depth;
                if (letterProMaterial) dtmp.fontMaterial = letterProMaterial;
                dtmp.fontSize = tmpCharSize;
                dtmp.text = dformatter.Format(bound.size.z * scale.z);
                dDimensionMesh.transform.localScale = Vector3.one;*/
                #endregion
            }
            else
            {
                #region TextMesh
                if (!dtm)
                {
                    RemoveAllComponents(dDimensionMesh);
                    MeshFilter mf = dDimensionMesh.GetComponent<MeshFilter>();
                    if (mf) DestroyImmediate(mf);
                    dtm = dDimensionMesh.AddComponent<TextMesh>();
                }
                dtm.anchor = (TextAnchor)anchorOptions.depth;
                dtm.font = font;
                dtm.characterSize = charSize;
                dtm.GetComponent<Renderer>().material = letterMaterial;
                dtm.GetComponent<Renderer>().enabled = true;
                dtm.text = dformatter.Format(bound.size.z * scale.z);// *remove
                //dtm.text = formatter.captionMode ? formatter.captionSettings.depthCaption : dformatter.Format(bound.size.z * scale.z);// *uncomment separate setting for each dimension
                dDimensionMesh.transform.localScale = 0.02f * Vector3.one;
                #endregion
            }
            if (faceCamera.depth)
            {
                dDimensionMesh.transform.localRotation = Quaternion.Euler(180f, 0f, -90f);
            }
            arrows[2] = PossibleDepthArrows[i][0];
            arrows[3] = PossibleDepthArrows[i][1];
            depthExt[0] = PossibleDepthExtLines[i, 0];
            depthExt[1] = PossibleDepthExtLines[i, 1];
            depthExt[2] = PossibleDepthExtLines[i, 2];
            depthExt[3] = PossibleDepthExtLines[i, 3];
        }


        void SortDimPositions()
        {
            if (!mainCamera) return;
            Quaternion camRot = mainCamera.transform.rotation;
            Vector3 angles = camRot.eulerAngles;
            angles.z = 0;
            mainCamera.transform.rotation = Quaternion.Euler(angles);
            Vector3 CPoint = mainCamera.WorldToScreenPoint(transform.TransformPoint(boundOffset));
            float hDist = 0, dDist = 0, wDist = 0;
            int newHpos = 0, newDpos = 0, newWpos = 0;
            for (int i = 0; i < 4; i++)
            {
                Vector3 hPoint = mainCamera.WorldToScreenPoint(transform.TransformPoint(EdgesHeight[i]));
                Vector3 wPoint = mainCamera.WorldToScreenPoint(transform.TransformPoint(EdgesWidth[i]));
                Vector3 dPoint = mainCamera.WorldToScreenPoint(transform.TransformPoint(EdgesDepth[i]));
                if (i == 0)
                {
                    hDist = hPoint.x;
                    wDist = wPoint.y;
                    dDist = dPoint.y;
                }

                if (hPoint.x < hDist)
                {
                    newHpos = 2 * i;
                    hDist = hPoint.x;
                }

                if (wPoint.y > wDist)
                {
                    newWpos = 2 * i;
                    wDist = wPoint.y;
                }

                if (dPoint.y > dDist)
                {
                    newDpos = 2 * i;
                    dDist = dPoint.y;
                }
            }
            Vector3 campos = mainCamera.transform.position;
            float dot0, dot1, adot0, adot1;
            Quaternion hLocRot = Quaternion.identity;
            if (faceCamera.height)
            {
                hLocRot = Quaternion.Euler(new Vector3(180f, 0, -90f));
                if (extensions.height)
                {
                    Vector3 h0Point = mainCamera.WorldToScreenPoint(PossiblePlacementsHeight[newHpos].transform.position);
                    //Debug.DrawRay(campos, PossiblePlacementsHeight[newHpos].transform.position - campos, Color.red);
                    Vector3 h1Point = mainCamera.WorldToScreenPoint(PossiblePlacementsHeight[(newHpos + 1) % 8].transform.position);
                    //Debug.DrawRay(campos, PossiblePlacementsHeight[(newHpos + 4) % 8].transform.position - campos, Color.red);
                    if (h1Point.x < h0Point.x) newHpos = (newHpos + 1) % 8;
                }
            }
            else
            {
                dot0 = Vector3.Dot(PossiblePlacementsHeight[newHpos].transform.forward, campos - PossiblePlacementsHeight[newHpos].transform.position);
                dot1 = Vector3.Dot(PossiblePlacementsHeight[(newHpos + 1) % 8].transform.forward, campos - PossiblePlacementsHeight[(newHpos + 1) % 8].transform.position);
                adot0 = Mathf.Abs(dot0);
                adot1 = Mathf.Abs(dot1);
                if (adot1 > adot0)
                {
                    newHpos = (newHpos + 1) % 8;
                    if (dot1 > 0) hLocRot = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    if (dot0 > 0) hLocRot = Quaternion.Euler(0, 180, 0);
                }
            }
            hDimensionMesh.transform.SetParent(PossiblePlacementsHeight[newHpos]);
            hDimensionMesh.transform.localPosition = Vector3.zero;
            hDimensionMesh.transform.localRotation = hLocRot;

            //

            Quaternion wLocRot = Quaternion.identity;
            if (faceCamera.width)
            {
                wLocRot = Quaternion.Euler(new Vector3(180f, 0, -90f));
                if (extensions.width)
                {
                    Vector3 w0Point = mainCamera.WorldToScreenPoint(PossiblePlacementsWidth[newWpos].transform.position);
                    Vector3 w1Point = mainCamera.WorldToScreenPoint(PossiblePlacementsWidth[(newWpos + 1) % 8].transform.position);
                    if (w1Point.y > w0Point.y) newWpos = (newWpos + 1) % 8;
                }
            }
            else
            {
                dot0 = Vector3.Dot(PossiblePlacementsWidth[newWpos].transform.forward, campos - PossiblePlacementsWidth[newWpos].transform.position);
                dot1 = Vector3.Dot(PossiblePlacementsWidth[(newWpos + 1) % 8].transform.forward, campos - PossiblePlacementsWidth[(newWpos + 1) % 8].transform.position);
                adot0 = Mathf.Abs(dot0);
                adot1 = Mathf.Abs(dot1);
                if (adot1 > adot0)
                {
                    newWpos = (newWpos + 1) % 8;
                    if (dot1 > 0) wLocRot = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    if (dot0 > 0) wLocRot = Quaternion.Euler(0, 180, 0);
                }
            }
            wDimensionMesh.transform.SetParent(PossiblePlacementsWidth[newWpos]);
            wDimensionMesh.transform.localPosition = Vector3.zero;        
            wDimensionMesh.transform.localRotation = wLocRot;

            //

            Quaternion dLocRot = Quaternion.identity;
            if (faceCamera.depth)
            {
                dLocRot = Quaternion.Euler(new Vector3(180f, 0, -90f));
                if (extensions.depth)
                {
                    Vector3 d0Point = mainCamera.WorldToScreenPoint(PossiblePlacementsDepth[newDpos].transform.position);
                    Vector3 d1Point = mainCamera.WorldToScreenPoint(PossiblePlacementsDepth[(newDpos + 1) % 8].transform.position);
                    if (d1Point.y > d0Point.y) newDpos = (newDpos + 1) % 8;
                }
            }
            else
            {
                dot0 = Vector3.Dot(PossiblePlacementsDepth[newDpos].transform.forward, campos - PossiblePlacementsDepth[newDpos].transform.position);
                dot1 = Vector3.Dot(PossiblePlacementsDepth[(newDpos + 1) % 8].transform.forward, campos - PossiblePlacementsDepth[(newDpos + 1) % 8].transform.position);
                adot0 = Mathf.Abs(dot0);
                adot1 = Mathf.Abs(dot1);
                if (adot1 > adot0)
                {
                    newDpos = (newDpos + 1) % 8;
                    if (dot1 > 0) dLocRot = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    if (dot0 > 0) dLocRot = Quaternion.Euler(0, 180, 0);
                }
            }

            mainCamera.transform.rotation = camRot;

            dDimensionMesh.transform.SetParent(PossiblePlacementsDepth[newDpos]);
            dDimensionMesh.transform.localPosition = Vector3.zero;
            dDimensionMesh.transform.localRotation = dLocRot;

            arrows[4] = PossibleHeightArrows[newHpos][0];
            arrows[5] = PossibleHeightArrows[newHpos][1];

            arrows[0] = PossibleWidthArrows[newWpos][0];
            arrows[1] = PossibleWidthArrows[newWpos][1];

            arrows[2] = PossibleDepthArrows[newDpos][0];
            arrows[3] = PossibleDepthArrows[newDpos][1];

            if (drawArrows && placementOptions.floating)
            {
                triangles = new Vector3[arrows.Length][];
                for (int i = 0; i < arrows.Length; i++)
                {
                    triangles[i] = new Vector3[arrows[i].Length];
                    for (int j = 0; j < arrows[i].Length; j++)
                    {
                        triangles[i][j] = transform.TransformPoint(arrows[i][j]);
                    }
                }
            }
            else
            {
                triangles = new Vector3[0][];
            }
            heightExt[0] = PossibleHeightExtLines[newHpos, 0];
            heightExt[1] = PossibleHeightExtLines[newHpos, 1];
            heightExt[2] = PossibleHeightExtLines[newHpos, 2];
            heightExt[3] = PossibleHeightExtLines[newHpos, 3];

            widthExt[0] = PossibleWidthExtLines[newWpos, 0];
            widthExt[1] = PossibleWidthExtLines[newWpos, 1];
            widthExt[2] = PossibleWidthExtLines[newWpos, 2];
            widthExt[3] = PossibleWidthExtLines[newWpos, 3];

            depthExt[0] = PossibleDepthExtLines[newDpos, 0];
            depthExt[1] = PossibleDepthExtLines[newDpos, 1];
            depthExt[2] = PossibleDepthExtLines[newDpos, 2];
            depthExt[3] = PossibleDepthExtLines[newDpos, 3];
        }

        /*private void Update()
        {
            if (!Application.isPlaying)
            {
                //Debug.Log("update");
            }
        }*/

        private void LateUpdate()
        {
            if (transform.localScale != previousScale)
            {
                SetPoints();
                AddText();
            }
            if (transform.position != previousPosition || transform.rotation != previousRotation || transform.localScale != previousScale || placementOptions.floating)
            { 
                previousRotation = transform.rotation;
                previousPosition = transform.position;
                previousScale = transform.localScale;
            }
            ///

            ///
            Vector3 campos = mainCamera.transform.position;
            //cameralines.setOutlines(lines, lineColor, triangles);

            if (placementOptions.floating) SortDimPositions();
            SetLines();
            //cameralines.setOutlines(lines, lineColor, triangles);

            
            if (faceCamera.height)
            {
                Transform hDim = hDimensionMesh.transform.parent;
                hDim.LookAt(campos, Z_up_orientation? transform.forward : transform.up);
                //prevent text upside down
                if(mainCamera.transform.InverseTransformDirection(hDim.up).x<0) hDim.Rotate(0,0,180f);
            }

            if (faceCamera.width)
            {
                Transform wDim = wDimensionMesh.transform.parent;
                wDim.LookAt(campos, Z_up_orientation ? transform.forward : transform.right);
                if (mainCamera.transform.InverseTransformDirection(wDim.up).x < 0) wDim.transform.Rotate(0, 0, 180f);
            }

            if (faceCamera.depth)
            {
                Transform dDim = dDimensionMesh.transform.parent;
                dDim.LookAt(campos, transform.forward);
                if (mainCamera.transform.InverseTransformDirection(dDim.up).x < 0) dDim.transform.Rotate(0, 0, 180f);
            }
        }



        void OnDrawGizmos()
        {
            Gizmos.color = wireColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            for (int i = 0; i < lines.GetLength(0); i++)
            {
                Gizmos.DrawLine(lines[i, 0], lines[i, 1]);
            }
            Gizmos.color = Color.white;
            // Update the main thread after the bound box calculations
#if UNITY_EDITOR
            // Ensure continuous Update calls.
            if (!Application.isPlaying && !mainThreadUpdated)
            {
                EditorApplication.QueuePlayerLoopUpdate();
                SceneView.RepaintAll();
            }
#endif
        }

        void OnRenderObject()
        {
            if (lines == null || !lineMaterial) return;
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.LINES);

            GL.Color(wireColor);
            for (int i = 0; i < lines.GetLength(0); i++)
            {
                GL.Vertex(lines[i, 0]);
                GL.Vertex(lines[i, 1]);
            }

            GL.End();

            GL.Begin(GL.TRIANGLES);
            //Debug.Log(triangles.Count.ToString());

            GL.Color(wireColor);
            for (int i = 0; i < triangles.GetLength(0); i++)
            {
                //Debug.Log(j.ToString()+ " | " + i.ToString());
                GL.Vertex(triangles[i][0]);
                GL.Vertex(triangles[i][1]);
                GL.Vertex(triangles[i][2]);
            }

            GL.End();

            GL.PopMatrix();
        }

        void OnEnable()
        {
            if (hDimensionMesh) hDimensionMesh.SetActive(true);
            if (dDimensionMesh) dDimensionMesh.SetActive(true);
            if (wDimensionMesh) wDimensionMesh.SetActive(true);
            Init();
        }

        void OnDisable()
        {
            if (hDimensionMesh) hDimensionMesh.SetActive(false);
            if (dDimensionMesh) dDimensionMesh.SetActive(false);
            if (wDimensionMesh) wDimensionMesh.SetActive(false);
        }

        float GetTextHeight()
        {
            return 0.1f*charSize;
        }

        public bool FaceCameraWidth
        {
            set
            {
                faceCamera.width = value;
                SetPoints();
                AddText();
            }
        }

        public bool FaceCameraDepth
        {
            set
            {
                faceCamera.depth = value;
                SetPoints();
                AddText();
            }
        }

        public bool FaceCameraHeight
        {
            set
            {
                faceCamera.height = value;
                SetPoints();
                AddText();
            }
        }
        public bool AutoPlacement
        {
            set
            {
                placementOptions.floating = value;
                AddText();
            }
        }

        public struct VertexData
        {
            public Vector3[] vertices;
            public Matrix4x4 matrix;
            public VertexData(Vector3[] vert, Matrix4x4 m)
            {
                vertices = vert;
                matrix = m;
            }
        }
#if UNITY_EDITOR
        private readonly Queue<Action> _actionQueue = new Queue<Action>();
        public Queue<Action> ActionQueue
        {
            get
            {
                lock (Async.GetLock("ActionQueue"))
                {
                    return _actionQueue;
                }
            }
        }
#endif
        public static Bounds OBB(VertexData[] vdata, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            DateTime dt = DateTime.Now;
            OrientedBoundingBox obb = new OrientedBoundingBox();
            Debug.Log("meshesCount " + vdata.Length.ToString());
            for (int i = 0; i < vdata.Length; i++)
            {
                VertexData ms = vdata[i];
                int vc = vdata[i].vertices.Length;
                Debug.Log("vertices " + vc.ToString());
                for (int j = 0; j < vc; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        obb = new OrientedBoundingBox(vdata[i].matrix.MultiplyPoint3x4(vdata[i].vertices[j]), Vector3.zero, v1, v2, v3);
                    }
                    else
                    {
                        obb.Enclose(vdata[i].matrix.MultiplyPoint3x4(vdata[i].vertices[j]));
                    }
                }
            }

            TimeSpan ts = DateTime.Now - dt;
            //Debug.Log(ts.ToString());
            Bounds _bounds = new Bounds(obb.Center, 2.0f * obb.Extent);
            return _bounds;
        }

        public void AccurateBounds()
        {
            if (!mainThreadUpdated)
            {
                Debug.Log("!mainThreadUpdated");
                return;
            }
            MeshFilter[] meshes = GetComponentsInChildren<MeshFilter>();
#if UNITY_EDITOR
            if (meshes.Length == 0)
            {
                EditorUtility.DisplayDialog("Dimbox message", "The object contains no meshes!\n- please reassign", "Continue");
            }
#endif
            VertexData[] vertexData = new VertexData[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                Mesh ms = meshes[i].sharedMesh;
                vertexData[i] = new VertexData(ms.vertices, meshes[i].transform.localToWorldMatrix);
            }
            Vector3 v1 = transform.right;
            Vector3 v2 = transform.up;
            Vector3 v3 = transform.forward;
#if UNITY_EDITOR //use threading only in editor
            if (!Application.isPlaying)
            {
                Async.Run(() =>
                {
                    mainThreadUpdated = false;
                    Debug.Log("thread start");
                    meshBound = OBB(vertexData, v1, v2, v3);
                }).ContinueInMainThread(() =>
                {

                    Debug.Log("back to main thread");
                    meshBoundOffset = transform.InverseTransformPoint(meshBound.center);
                    mainThreadUpdated = true;
                    mApplied = true;

                });
            }
            else
            {
#endif
                meshBound = OBB(vertexData, v1, v2, v3);
                meshBoundOffset = transform.InverseTransformPoint(meshBound.center);
                mApplied = true;
                Debug.Log("mApplied = true");
#if UNITY_EDITOR
            }
#endif

        }

        public void RecalculateBounds()
        {
            CalculateBounds();
            Init();
        }

        void RemoveAllComponents(GameObject go, bool immediate)
        {
            foreach (var comp in go.GetComponents<Component>())
            {
                if (!(comp is Transform))
                {
                    if (immediate)
                    {
                        DestroyImmediate(comp);
                    }
                    else
                    {
                        Destroy(comp);
                    }
                }
            }
        }

        void RemoveAllComponents(GameObject go)
        {
            RemoveAllComponents(go, false);
        }
#region TextMeshPro
        /*public void SwitchToTMPro()
        {
            DestroyImmediate(htm);
            RemoveAllComponents(hDimensionMesh, true);
            DestroyImmediate(wtm);
            RemoveAllComponents(wDimensionMesh, true);
            DestroyImmediate(dtm);
            RemoveAllComponents(dDimensionMesh, true);
            useTextMeshPro = true;
            AddText();
        }

        public void SwitchToTextMesh()
        {
            DestroyImmediate(htmp);
            RemoveAllComponents(hDimensionMesh, true);
            DestroyImmediate(wtmp);
            RemoveAllComponents(wDimensionMesh, true);
            DestroyImmediate(dtmp);
            RemoveAllComponents(dDimensionMesh, true);
            useTextMeshPro = false;
            AddText();
        }*/
#endregion
    }
}