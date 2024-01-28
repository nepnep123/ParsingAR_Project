/* 모델링 터치, 회전, 크기조절, 이동 스크립트*/
/* 각각 1터치(짧게), 1터치(드레그), 2터치, 3터치 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ModelingTouchModule : MonoBehaviour
{
    [Header(" [ 할당 필요 ] ")]
    [SerializeField] public Camera modelCamera;
    //[SerializeField] public GameObject rotateObject;

    [SerializeField] bool rotateChangeState = true, scaleChangeState = true, positionChangeState = true;

    //[Range(0f, 5.0f)]
    [SerializeField] float minScale, maxScale;
    //[SerializeField] FPS debug_text;

    float touchpreDis, touchnowDis, startT, endT;
    Vector3 scaleDef, prevPoint, scrSpace, offset;

    [SerializeField] Vector3 default_pos, default_scale;
    [SerializeField] Quaternion default_rot;
    Transform getTrans;

    [SerializeField] bool disableTrigger = false;

    //[SerializeField] public bool isARTargetting = false; //AR 타겟 객체는 최초로 터치했을때 제어가 필요하다 
    //[SerializeField] public TargetModelingSetting targetModelSetting_cs; 

    private void Awake()
    {
        default_pos = gameObject.transform.localPosition;
        default_scale = gameObject.transform.localScale;
        default_rot = gameObject.transform.localRotation;

        disableTrigger = true;
    }


    private void OnEnable()
    {
        if (disableTrigger)
        {
            //Debug.Log("INIT MODELING");
            ResetTransform();
        }

        StartCoroutine(ModelingTouchCount());
    }

    //private void OnDisable()
    //{
    //    if (disableTrigger)
    //    {
    //        //Debug.Log("INIT MODELING");
    //        ResetTransform();
    //    }
    //}

    //객체 위치값 초기화 
    public void ResetTransform()
    {
        gameObject.transform.localPosition = default_pos;
        gameObject.transform.localRotation = default_rot;
        gameObject.transform.localScale = default_scale;
    }

    // UI 터치 상태에 따른 bool값 저장
    public bool IsPointerOverUIObject(Vector2 touchPos) 
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = touchPos;
        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    // 터치 카운트
    public IEnumerator ModelingTouchCount()
    {
        while (true)
        {
            if (Input.touchCount == 1 && !IsPointerOverUIObject(Input.mousePosition))
            {
                //if (isARTargetting)
                //    TouchCheck();

                //if(debug_text != null) debug_text.other_text.text = "TOUCH COUNT 1 : " + Input.mousePosition.x.ToString();
                StopCoroutine(TouchCountOne());
                StartCoroutine(TouchCountOne());
            }
            else if (Input.touchCount == 2 && scaleChangeState && !IsPointerOverUIObject(Input.mousePosition))
            {
                //if (isARTargetting)
                //    TouchCheck();

                //if (debug_text != null) debug_text.other_text.text = "TOUCH COUNT 2 : " + Input.mousePosition.x.ToString();
                TouchCountTwo();
            }
            else if (Input.touchCount == 3 && positionChangeState && !IsPointerOverUIObject(Input.mousePosition))
            {
                //if (isARTargetting)
                //    TouchCheck();

                //if (debug_text != null) debug_text.other_text.text = "TOUCH COUNT 3 : " + Input.mousePosition.x.ToString();
                TouchCountThree();
            }
            else
            {
                //if (debug_text != null) debug_text.other_text.text = "TOUCH COUNT X ";
            }
            yield return null;
        }
    }

    //AR 타겟 인식때 체크해야됨
    //void TouchCheck()
    //{
    //    Debug.Log("돌아감");
    //    if (targetModelSetting_cs)
    //    {
    //        if (targetModelSetting_cs.equipInfoImages.activeInHierarchy)
    //        {
    //            targetModelSetting_cs.equipInfoImages.SetActive(false);
    //        }
    //    }
    //}

    IEnumerator TouchCountOne()
    {
        while (true)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                prevPoint = Input.GetTouch(0).position;
                startT = Time.time;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Stationary) // 긴 터치(Don't Move) 이벤트
            {
                transform.Rotate(new Vector3((Input.GetTouch(0).position.y - prevPoint.y) * 0.06f, -(Input.GetTouch(0).position.x - prevPoint.x) * 0.06f, 0), Space.World);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && rotateChangeState) // 긴 터치(Move) 이벤트
            {
                // 현 오브젝트 회전
                transform.Rotate(new Vector3((Input.GetTouch(0).position.y - prevPoint.y) * 0.06f, -(Input.GetTouch(0).position.x - prevPoint.x) * 0.06f, 0), Space.World);

                prevPoint = Input.GetTouch(0).position;
                if (Input.touchCount >= 2)
                {
                    break;
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                endT = Time.time;
                if (endT - startT < 0.15f)
                {
                    break;
                }
                else
                {
                    break;
                }
            }
            yield return null;
        }
    }

    void TouchCountTwo()
    {
        // 현 오브젝트 크기 조절
        if (Input.GetTouch(1).phase == TouchPhase.Began)
        {
            touchpreDis = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            scaleDef = transform.localScale;
        }
        if (Input.GetTouch(1).phase == TouchPhase.Moved && touchpreDis > 0) // 크기 변환 min, max크기 조정
        {
            touchnowDis = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            float scaleFactor = touchnowDis / touchpreDis;

            getTrans = gameObject.transform;

            if (getTrans.localScale.x < minScale)
                getTrans.localScale = Vector3.one * minScale;
            else if (getTrans.localScale.x == minScale && scaleFactor > 1)
                getTrans.localScale = scaleDef * scaleFactor;
            else if (getTrans.localScale.x > minScale && getTrans.localScale.x < maxScale)
                getTrans.localScale = scaleDef * scaleFactor;
            else if (getTrans.localScale.x == maxScale && scaleFactor < 1)
                getTrans.localScale = scaleDef * scaleFactor;
            else if (getTrans.localScale.x > maxScale)
                getTrans.localScale = Vector3.one * maxScale;
        }
    }

    void TouchCountThree()
    {
        getTrans = gameObject.transform;

        // 현 오브젝트 위치 조절
        if (Input.GetTouch(2).phase == TouchPhase.Began)
        {
            scrSpace = modelCamera.WorldToScreenPoint(getTrans.position);
            offset = getTrans.position - modelCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, scrSpace.z));
        }
        if (Input.GetTouch(2).phase == TouchPhase.Moved && scrSpace.z != 0)
        {
            Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, scrSpace.z);
            Vector3 curPosition = modelCamera.ScreenToWorldPoint(curScreenSpace) + offset;
            getTrans.position = curPosition;
        }
    }



    //void CheckRayCastHit()
    //{
    //    ray = modelCamera.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        objTrigger = hit.transform.gameObject;
    //        //debug_text.other_text.text = "HIT : " + hit.transform.gameObject.name;
    //    }
    //    //else
    //    //{
    //    //    if(GameObject.Find("EmptyObject") != null)
    //    //        objTrigger = GameObject.Find("EmptyObject");
    //    //} 
    //}

}