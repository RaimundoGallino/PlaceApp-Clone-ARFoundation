//
//Filename: maxCamera.cs
//
// original: http://www.unifycommunity.com/wiki/index.php?title=MouseOrbitZoom
//
// --01-18-2010 - create temporary target, if none supplied at start
 
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace DimBoxes
{
    [AddComponentMenu("Camera-Control/3dsMax Camera Modified")]
    public class maxCamera : MonoBehaviour
    {
        public Transform target;
        public GameObject terrainMesh;
        public Vector3 targetOffset;
        public float distance = 5.0f;
        public float maxDistance = 20;
        public float minDistance = .6f;
        public float xSpeed = 200.0f;
        public float ySpeed = 200.0f;
        public float aboveYmin = 0.8f;
        public float yMaxLimit = 80f;
        public float zoomRate = 40;
        public float panSpeed = 0.3f;
        public float zoomDampening = 5.0f;

        private float xDeg = 0.0f;
        private float yDeg = 0.0f;
        private float currentDistance;
        private float desiredDistance;
        private Quaternion currentRotation;
        private Quaternion desiredRotation;
        private Quaternion rotation;
        private Vector3 position;
        private float yMinLimit;
        private float viewerYmin;
        private Vector2 desiredInputPosition;
        private Vector2 currentInputPosition;
        private Vector3 currentTargetPosition;
        private Vector3 hitPoint = Vector3.zero;
        private bool dragging = false;

        void Start() { Init(); }
        void OnEnable() { Init(); }

        public void Init()
        {
            //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
            viewerYmin = aboveYmin;
            if (!target)
            {
                GameObject go = new GameObject("Cam Target");
                go.transform.position = Vector3.zero;//transform.position + (transform.forward * distance);
                target = go.transform;
            }

            distance = Vector3.Distance(transform.position, target.position);
            currentDistance = distance;
            desiredDistance = distance;

            //be sure to grab the current rotations as starting points.
            transform.LookAt(target);

            position = transform.position;
            rotation = transform.rotation;
            currentRotation = transform.rotation;
            desiredRotation = transform.rotation;

            xDeg = (rotation.eulerAngles.y + 180) % 360 - 180; // Vector3.Angle(Vector3.right, transform.right);
            yDeg = (rotation.eulerAngles.x + 180) % 360 - 180; // Vector3.Angle(Vector3.up, transform.up);
        }

        /*
         * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
         */
        void LateUpdate()
        {
            // If right mouse? ORBIT
            if (Input.GetMouseButton(1) && EventSystem.current.currentSelectedGameObject == null && !EventSystem.current.IsPointerOverGameObject())
            {
                xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                ////////OrbitAngle
            }
            // otherwise if left mouse we pan by way of moving the target over the terrain
            else if (Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == null && !EventSystem.current.IsPointerOverGameObject())
            {
                desiredInputPosition = Input.mousePosition;

                if (Input.GetMouseButtonDown(0)) currentInputPosition = desiredInputPosition;

                RaycastHit[] hits;
                //Vector3 hitPoint = Vector3.zero;
                hits = Physics.RaycastAll(GetComponentInChildren<Camera>().ScreenPointToRay(currentInputPosition), 100);
                var hl = hits.Length;
                //if(hl==0) return;

                bool prevDrag = dragging;

                dragging = false;

                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.gameObject == terrainMesh)
                    {
                        hitPoint = hit.point;
                        dragging = true;
                        break;
                    }
                }
                //if(flag) return;
                //StartCoroutine ("DragObject", hitPoint);
                if (!prevDrag && dragging)
                {
                    currentInputPosition = desiredInputPosition;
                    StartCoroutine("DragObject", hitPoint);

                }
            }
            //Clamp the vertical axis for the orbit
            yMinLimit = -Mathf.Rad2Deg * Mathf.Asin((target.position.y - viewerYmin) / currentDistance);
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;

            ////////Orbit Position

            // affect the desired Zoom distance if we roll the scrollwheel
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
            //clamp the zoom min/max
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            // For smoothing of the zoom, lerp distance
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

            // calculate position based on the new currentDistance 

            currentTargetPosition = target.position;////
            currentInputPosition = Vector2.Lerp(currentInputPosition, desiredInputPosition, Time.deltaTime * 5f);///


            position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
            transform.position = position;


        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        void Update()
        {

        }


        IEnumerator DragObject(Vector3 startingHit)
        {
            while (Input.GetMouseButton(0) && dragging)
            {
                var translation = startingHit - hitPoint;
                translation.x = Mathf.Clamp(translation.x, -0.1f, 0.1f);
                translation.z = Mathf.Clamp(translation.z, -0.1f, 0.1f);
                transform.position = transform.position + translation;
                target.position = target.position + translation;//
                yield return null;
            }
            dragging = false;
        }

        public void SetRadius(float val)
        {
            desiredDistance = val;
        }
    }
}