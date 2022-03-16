using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARInteractionManager : MonoBehaviour
{

    [SerializeField] private Camera arCamera;

    private ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private GameObject arPointer;
    private GameObject item3DModel;
    private GameObject itemSelected;
    private bool isInitialPosition;
    private bool isOverUI;
    private bool isOver3DModel;

    private Vector2 initialTouchPos;

    public GameObject Item3DModel
    {
        set
        {
            item3DModel = value;
            item3DModel.transform.position = arPointer.transform.position;
            item3DModel.transform.parent = arPointer.transform;
            isInitialPosition = true;
        }
    }

    void Start()
    {
        arPointer = transform.GetChild(0).gameObject;
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        GameManager.instance.OnMainMenu += SetItemPosition;
    }

    void Update()
    {
        if (isInitialPosition)
        {
            Vector2 middlePointScreen = new Vector2(Screen.width / 2, Screen.height / 2);
            arRaycastManager.Raycast(middlePointScreen , hits, TrackableType.Planes);
            if (hits.Count > 0)
            {
                transform.position = hits[0].pose.position;
                transform.rotation = hits[0].pose.rotation;
                
                arPointer.SetActive(true);
                isInitialPosition = false;
            }
        }

        if (Input.touchCount > 0)
        {
            Touch touchOne = Input.GetTouch(0);
            
            if (touchOne.phase == TouchPhase.Began)
            {
                var touchPosition = touchOne.position;
                isOverUI = isTouchOverUI(touchPosition);
                isOver3DModel = isTouchOver3DModel(touchPosition);
            }
            // Moving the object
            if (touchOne.phase == TouchPhase.Moved)
            {
                if (arRaycastManager.Raycast(touchOne.position, hits, TrackableType.Planes))
                {
                    Pose hitPose = hits[0].pose;
                    if (!isOverUI && isOver3DModel)
                    {
                        transform.position = hitPose.position;
                    }
                }
            }
            // Rotating the object

            if (Input.touchCount == 2)
            {
                Touch touchTwo = Input.GetTouch(1);
                if (touchOne.phase == TouchPhase.Began || touchTwo.phase == TouchPhase.Began)
                {
                    initialTouchPos = touchTwo.position - touchOne.position;
                }

                if (touchOne.phase == TouchPhase.Moved || touchTwo.phase == TouchPhase.Moved)
                {
                    Vector2 currentTouchPos = touchTwo.position - touchOne.position;
                    float angle = Vector2.SignedAngle(initialTouchPos, currentTouchPos);
                    item3DModel.transform.rotation = Quaternion.Euler(0, item3DModel.transform.eulerAngles.y - angle, 0 );
                    initialTouchPos = currentTouchPos;
                }
            }

            if (isOver3DModel && item3DModel == null && !isOverUI)
            {
                GameManager.instance.ARPosition();
                item3DModel = itemSelected;
                itemSelected = null;
                arPointer.SetActive(true);
                transform.position = item3DModel.transform.position;
                item3DModel.transform.parent = arPointer.transform;

            }
            
        }
    }

    private bool isTouchOver3DModel(Vector2 touchPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit3DModel))
        {
            if (hit3DModel.collider.CompareTag("Item"))
            {
                itemSelected = hit3DModel.transform.gameObject;
                return true;
            }
        }

        return false;
    }

    private bool isTouchOverUI(Vector2 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touchPosition.x, touchPosition.y);

        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, result);

        return result.Count > 0;
    }

    private void SetItemPosition()
    {
        if (item3DModel)
        {
            item3DModel.transform.parent = null;
            arPointer.SetActive(false);
            item3DModel = null;
        }
    }

    public void DeleteItem()
    {
        Destroy(item3DModel);
        arPointer.SetActive(false);
        GameManager.instance.MainMenu();
    }

    public void ToggleMeasurementTool()
    {
        var measurementTool = item3DModel.transform.GetChild(0).gameObject;
        measurementTool.SetActive(!measurementTool.activeSelf);
    }
}
