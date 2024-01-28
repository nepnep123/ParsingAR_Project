using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//도해구성품 리소스 
[System.Serializable]
public struct EquipUnderResource
{
    public string objectName;

    //한페이지에 여러 도해 파일이 있는 경우가 있다. 
    public ObjectInfoList[] object_pre; //동적 생성 
    public RectTransform obj_tr;

    [SerializeField] public ObjectInfoList[] object_active; //MODEL 할당
}

//정비 절차 리소스 
[System.Serializable]
public struct OperationStepPrefab
{
    public string title;
    public string subTitle;

    public GameObject step_pre;
    public Transform pre_tr;

    [SerializeField] public GameObject step_obj;
    [SerializeField] public Animator stepAnim; //클릭할시 할당되는 객체 

    public int animOrder; //같은 애니메이터를 공유하는경우 장비 번호 
    [HideInInspector] public ModelingTouchModule modelModule; //제어를 위해 

    public int stepAnim_clipCount; //자동재생을 위해 

    public OperObjectUseTool useTool_img; //사용 공구 
}

public class ObjectListSetting : MonoBehaviour
{
    [SerializeField] public EquipUnderResource[] subComponent_data;

    [SerializeField] public OperationStepPrefab[] operationStep_data;

    [SerializeField] public FigureItem[] figureitems;
    [HideInInspector] public FigureItem currentItem;
 
    [SerializeField] public RectTransform figureitems_rectr;


    [Header(" [ CONTROL BUTTON ] ")]
    [SerializeField] public ToggleButtonController setAuto_TB; //정비 절차에서만 사용 한다. 
    [SerializeField] public ToggleButtonController setFullMode_TB;
    [SerializeField] RectTransform listBox_rectr;

    [Header(" [ SCRIPTS RESOURCE ] ")]
    [SerializeField] public DetailSettingCanvas detailSettingCanvas_sc;
    [SerializeField] public UIManager UM;

    private void Start()
    {
        setAuto_TB.gameObject.SetActive(false);
        setFullMode_TB.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        setAuto_TB.gameObject.SetActive(false);
        setFullMode_TB.gameObject.SetActive(false);

        //InitObject();
        //InitObject_Operation();
        //InitFigure();

        ListBoxInit(); //위치 수정 
    }

    #region INIT
    public void InitObject() //도해 구성품 
    {
        if (subComponent_data.Length == 0) return;

        for (int i = 0; i < subComponent_data.Length; i++)
        {
            if (subComponent_data[i].object_active != null)
            {
                for (int j = 0; j < subComponent_data[i].object_active.Length; j++)
                {
                    Destroy(subComponent_data[i].object_active[j].gameObject);
                }
                //메모리 해제 
                subComponent_data[i].object_active = null;
            }

        }
    }

    public void InitObject_Operation() //정비절차 
    {
        StopAllCoroutines();

        setAuto_TB.SetDefault();
        setAuto_TB.gameObject.SetActive(false);

        for (int i = 0; i < operationStep_data.Length; i++)
        {
            if (operationStep_data[i].useTool_img != null)
                operationStep_data[i].useTool_img.InitUseToolImage();

            if (operationStep_data[i].step_obj != null)
            {
                Destroy(operationStep_data[i].step_obj.gameObject);

                operationStep_data[i].step_obj = null;
                operationStep_data[i].stepAnim = null;
                operationStep_data[i].modelModule = null;
            }
        }
    }

    //Figure(이미지, 모델링) 리소스 초기화 
    public void InitFigure()
    {
        toggleTrigger = false;

        ResetFigureData();

        if (currentItem != null)
        {
            if (currentItem.GetComponent<DiagramController>())
                currentItem.GetComponent<DiagramController>().EndSetting();

            if (currentItem.GetComponent<HeaderController>())
                currentItem.GetComponent<HeaderController>().header_obj.SetActive(false);
        }
    }

    //컨텐츠 생성 전 초기화 
    public void ResetFigureData()
    {
        for (int i = 0; i < figureitems.Length; i++)
        {
            if (figureitems[i].itemType == ITEMTYPE.IMAGE)
            {
                figureitems[i].gameObject.SetActive(false);

                if (figureitems[i].listbox != null)
                {
                    ListBoxInit();
                    figureitems[i].listbox.gameObject.SetActive(false);
                }
            }
        }

        if (currentItem != null)
        {
            Destroy(currentItem.gameObject);
            currentItem = null;
        }
    }

    #endregion



    #region 도해 구성품 
    //메인 메뉴 버튼 클릭 (카테고리 테이블에 필요한 모델링을 링크해준다)
    public void ObjectSetting(string subSystemCode, string subsubSystemCode, string assyCode, CatalogInfo catalog)
    {
        //Debug.Log("subSystemCode : " + subSystemCode + " subsubSystemCode : " + subsubSystemCode + " assyCode : " + assyCode);
        if (subSystemCode == "") return;

        setFullMode_TB.gameObject.SetActive(true);

        //InitObject();
        InitFigure();
        //if (UM.isHW)
        //{
        //    if(subSystemCode == "2")
        //    {
        //        if (assyCode == "0000")
        //        {
        //            subComponent_data[0].object_active = new ObjectInfoList[subComponent_data[0].object_pre.Length];

        //            //한페이지에 여러 도해 파일이 있는 경우가 있다. 
        //            for (int i = 0; i < subComponent_data[0].object_active.Length; i++)
        //            {
        //                subComponent_data[0].object_active[i] = Instantiate(subComponent_data[0].object_pre[i], subComponent_data[0].obj_tr);
        //                subComponent_data[0].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
        //            }

        //            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
        //            //catalog.link_pre = subComponent_data[0].object_active[0].gameObject;
        //        }
        //        else
        //        {
        //            //
        //        }
        //    }
        //    else
        //    {
        //        //
        //    }
        //}
        //else
        //{
            switch (subSystemCode)
            {
                //부대정비 
                case "0":
                    if (subsubSystemCode == "0")
                    {
                        //부대정비 송수신처리장치 랙조립체  
                        if (assyCode == "W631")
                        {
                            subComponent_data[0].object_active = new ObjectInfoList[subComponent_data[0].object_pre.Length];

                            //한페이지에 여러 도해 파일이 있는 경우가 있다. 
                            for (int i = 0; i < subComponent_data[0].object_active.Length; i++)
                            {
                                subComponent_data[0].object_active[i] = Instantiate(subComponent_data[0].object_pre[i], subComponent_data[0].obj_tr);
                                subComponent_data[0].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            //catalog.link_pre = subComponent_data[0].object_active[0].gameObject;
                        }
                        //부대정비 전원공급장치 랙조립체  
                        else if (assyCode == "W651")
                        {
                            subComponent_data[1].object_active = new ObjectInfoList[subComponent_data[0].object_pre.Length];

                            //한페이지에 여러 도해 파일이 있는 경우가 있다. 
                            for (int i = 0; i < subComponent_data[1].object_active.Length; i++)
                            {
                                subComponent_data[1].object_active[i] = Instantiate(subComponent_data[1].object_pre[i], subComponent_data[1].obj_tr);
                                subComponent_data[1].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            //catalog.link_pre = subComponent_data[1].object_active[0].gameObject;
                        }
                    }
                    break;
                //야전정비
                //안테나 장치 
                case "2":
                    if (assyCode == "0000")
                    {
                        subComponent_data[0].object_active = new ObjectInfoList[subComponent_data[0].object_pre.Length];

                        //한페이지에 여러 도해 파일이 있는 경우가 있다. 
                        for (int i = 0; i < subComponent_data[0].object_active.Length; i++)
                        {
                            subComponent_data[0].object_active[i] = Instantiate(subComponent_data[0].object_pre[i], subComponent_data[0].obj_tr);
                            subComponent_data[0].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                        }

                        //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                        //catalog.link_pre = subComponent_data[0].object_active[0].gameObject;
                    }
                    else if (assyCode == "0100")
                    {
                        subComponent_data[1].object_active = new ObjectInfoList[subComponent_data[1].object_pre.Length];

                        for (int i = 0; i < subComponent_data[1].object_active.Length; i++)
                        {
                            subComponent_data[1].object_active[i] = Instantiate(subComponent_data[1].object_pre[i], subComponent_data[1].obj_tr);
                            subComponent_data[1].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                        }

                        //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                        //catalog.link_pre = subComponent_data[1].object_active[0].gameObject;
                    }
                    break;
                //송수신 처리 장치 
                case "4":
                    if (subsubSystemCode == "0")
                    {
                        if (assyCode == "0000")
                        {
                            subComponent_data[2].object_active = new ObjectInfoList[subComponent_data[2].object_pre.Length];

                            for (int i = 0; i < subComponent_data[2].object_active.Length; i++)
                            {
                                subComponent_data[2].object_active[i] = Instantiate(subComponent_data[2].object_pre[i], subComponent_data[2].obj_tr);
                                subComponent_data[2].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[2].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[2].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[2].object_active[i].gameObject;
                                }
                                else subComponent_data[2].object_active[i].gameObject.SetActive(false);
                            }
                        }
                        else if (assyCode == "0100")
                        {
                            subComponent_data[3].object_active = new ObjectInfoList[subComponent_data[3].object_pre.Length];

                            for (int i = 0; i < subComponent_data[3].object_active.Length; i++)
                            {
                                subComponent_data[3].object_active[i] = Instantiate(subComponent_data[3].object_pre[i], subComponent_data[3].obj_tr);
                                subComponent_data[3].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[3].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[3].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[3].object_active[i].gameObject;
                                }
                                else subComponent_data[3].object_active[i].gameObject.SetActive(false);
                            }
                        }
                        else if (assyCode == "0200")
                        {
                            subComponent_data[4].object_active = new ObjectInfoList[subComponent_data[4].object_pre.Length];

                            for (int i = 0; i < subComponent_data[4].object_active.Length; i++)
                            {
                                subComponent_data[4].object_active[i] = Instantiate(subComponent_data[4].object_pre[i], subComponent_data[4].obj_tr);
                                subComponent_data[4].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[4].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[4].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[4].object_active[i].gameObject;
                                }
                                else subComponent_data[4].object_active[i].gameObject.SetActive(false);
                            }
                        }
                        else if (assyCode == "0300")
                        {
                            subComponent_data[5].object_active = new ObjectInfoList[subComponent_data[5].object_pre.Length];

                            for (int i = 0; i < subComponent_data[5].object_active.Length; i++)
                            {
                                subComponent_data[5].object_active[i] = Instantiate(subComponent_data[5].object_pre[i], subComponent_data[5].obj_tr);
                                subComponent_data[5].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[5].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[5].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[5].object_active[i].gameObject;
                                }
                                else subComponent_data[5].object_active[i].gameObject.SetActive(false);
                            }
                        }
                        else if (assyCode == "1900")
                        {
                            subComponent_data[6].object_active = new ObjectInfoList[subComponent_data[6].object_pre.Length];

                            for (int i = 0; i < subComponent_data[6].object_active.Length; i++)
                            {
                                subComponent_data[6].object_active[i] = Instantiate(subComponent_data[6].object_pre[i], subComponent_data[6].obj_tr);
                                subComponent_data[6].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[6].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[6].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[6].object_active[i].gameObject;
                                }
                                else subComponent_data[6].object_active[i].gameObject.SetActive(false);
                            }
                        }
                    }
                    else if (subsubSystemCode == "1")
                    {
                        //if (subComponent_data[7].object_active.Length == 0)
                        //{
                            subComponent_data[7].object_active = new ObjectInfoList[subComponent_data[7].object_pre.Length];

                            for (int i = 0; i < subComponent_data[7].object_active.Length; i++)
                            {
                                subComponent_data[7].object_active[i] = Instantiate(subComponent_data[7].object_pre[i], subComponent_data[7].obj_tr);
                                subComponent_data[7].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[7].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[7].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[7].object_active[i].gameObject;
                                }
                                else subComponent_data[7].object_active[i].gameObject.SetActive(false);
                            }
                        //}
                    }

                    break;

                //전원공급장치
                case "6":
                    if (subsubSystemCode == "0")
                    {
                        //전원공급장치
                        if (assyCode == "0000")
                        {
                            subComponent_data[8].object_active = new ObjectInfoList[subComponent_data[8].object_pre.Length];

                            for (int i = 0; i < subComponent_data[8].object_active.Length; i++)
                            {
                                subComponent_data[8].object_active[i] = Instantiate(subComponent_data[8].object_pre[i], subComponent_data[8].obj_tr);
                                subComponent_data[8].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[8].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[8].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[8].object_active[i].gameObject;
                                }
                                else subComponent_data[8].object_active[i].gameObject.SetActive(false);
                            }
                        }
                        //전면판조립체
                        else if (assyCode == "0100")
                        {
                            subComponent_data[9].object_active = new ObjectInfoList[subComponent_data[9].object_pre.Length];

                            for (int i = 0; i < subComponent_data[9].object_active.Length; i++)
                            {
                                subComponent_data[9].object_active[i] = Instantiate(subComponent_data[9].object_pre[i], subComponent_data[9].obj_tr);
                                subComponent_data[9].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[9].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[9].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[9].object_active[i].gameObject;
                                }
                                else subComponent_data[9].object_active[i].gameObject.SetActive(false);
                            }
                        }
                        //직류출력조립체
                        else if (assyCode == "0200")
                        {
                            subComponent_data[10].object_active = new ObjectInfoList[subComponent_data[10].object_pre.Length];

                            for (int i = 0; i < subComponent_data[10].object_active.Length; i++)
                            {
                                subComponent_data[10].object_active[i] = Instantiate(subComponent_data[10].object_pre[i], subComponent_data[10].obj_tr);
                                subComponent_data[10].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[10].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[10].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[10].object_active[i].gameObject;
                                }
                                else subComponent_data[10].object_active[i].gameObject.SetActive(false);
                            }
                        }
                        //입력보호조립체
                        else if (assyCode == "0300")
                        {
                            subComponent_data[11].object_active = new ObjectInfoList[subComponent_data[11].object_pre.Length];

                            for (int i = 0; i < subComponent_data[11].object_active.Length; i++)
                            {
                                subComponent_data[11].object_active[i] = Instantiate(subComponent_data[11].object_pre[i], subComponent_data[11].obj_tr);
                                subComponent_data[11].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[11].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[11].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[11].object_active[i].gameObject;
                                }
                                else subComponent_data[11].object_active[i].gameObject.SetActive(false);
                            }
                        }
                        //전원정류조립체
                        else if (assyCode == "0400")
                        {
                            subComponent_data[12].object_active = new ObjectInfoList[subComponent_data[12].object_pre.Length];

                            for (int i = 0; i < subComponent_data[12].object_active.Length; i++)
                            {
                                subComponent_data[12].object_active[i] = Instantiate(subComponent_data[12].object_pre[i], subComponent_data[12].obj_tr);
                                subComponent_data[12].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[12].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[12].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[12].object_active[i].gameObject;
                                }
                                else subComponent_data[12].object_active[i].gameObject.SetActive(false);
                            }
                        }
                    }
                    else if (subsubSystemCode == "1")
                    {
                        //랙조립체
                        if (assyCode == "0000")
                        {
                            subComponent_data[13].object_active = new ObjectInfoList[subComponent_data[13].object_pre.Length];

                            for (int i = 0; i < subComponent_data[13].object_active.Length; i++)
                            {
                                subComponent_data[13].object_active[i] = Instantiate(subComponent_data[13].object_pre[i], subComponent_data[13].obj_tr);
                                subComponent_data[13].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //첫데이터 삽입 이후에 카탈로그데이터에 삽입 
                            for (int i = 0; i < subComponent_data[13].object_active.Length; i++)
                            {
                                if (i == 0)
                                {
                                    subComponent_data[13].object_active[i].gameObject.SetActive(true);
                                    //catalog.link_pre = subComponent_data[13].object_active[i].gameObject;
                                }
                                else subComponent_data[13].object_active[i].gameObject.SetActive(false);
                            }
                        }
                    }

                    break;
                default:
                    InitObject();
                    Debug.Log("SUBSYSTEMCODE : " + subSystemCode + " 존재하는 모델링 리소스가 없습니다 ! ");
                    break;
            }
        //}
    }

    //오브젝트의 키값을 받아 해당 이벤트를 발생 시킨다. 
    public void ObjectController(string subSystemCode, string subsubSystemCode, string figureNumber, string assyCode, string id)
    {
        //Debug.Log("SUBSYSTEMCODE : " + subSystemCode + " FIGURENUMBER : " + figureNumber + " ID : " + id);

        //if (UM.isHW)
        //{
        //    if (subSystemCode == "2")
        //    {
        //        if (assyCode == "0000")
        //        {
        //            if (figureNumber == "01")
        //            {
        //                if (subComponent_data[0].object_active.Length != 0)
        //                {
        //                    for (int i = 0; i < subComponent_data[0].object_active.Length; i++)
        //                    {
        //                        if (i == 0) continue;

        //                        subComponent_data[0].object_active[i].gameObject.SetActive(false);
        //                    }
        //                    subComponent_data[0].object_active[0].gameObject.SetActive(true);

        //                    //yield return new WaitForSeconds(1.0f);


        //                    subComponent_data[0].object_active[0].ActionEffect(id);
        //                    subComponent_data[0].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //
        //        }
        //    }
        //    else
        //    {
        //        //
        //    }
        //}
        //else
        //{
            //모델링이 있다는 조건 
            switch (subSystemCode)
            {
                //부대정비
                case "0":
                    if (subsubSystemCode == "0")
                    {
                        //부대정비 송수신처리장치 랙조립체  
                        if (assyCode == "W631")
                        {
                            if (figureNumber == "01")
                            {
                                if (subComponent_data[0].object_active.Length != 0)
                                {
                                    for (int i = 0; i < subComponent_data[0].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[0].object_active[i].gameObject.SetActive(false);
                                    }
                                    subComponent_data[0].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);

                                    subComponent_data[0].object_active[0].ActionEffect(id);
                                    subComponent_data[0].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                        else //부대정비 전원공급장치 랙조립체  
                        if (assyCode == "W651")
                        {
                            if (figureNumber == "01")
                            {
                                if (subComponent_data[1].object_active.Length != 0)
                                {
                                    for (int i = 0; i < subComponent_data[1].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[1].object_active[i].gameObject.SetActive(false);
                                    }
                                    subComponent_data[1].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);

                                    subComponent_data[1].object_active[0].ActionEffect(id);
                                    subComponent_data[1].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                    }
                    break;


                //야전정비
                //안테나장치
                case "2":
                    if (assyCode == "0000")
                    {
                        if (figureNumber == "01")
                        {
                            if (subComponent_data[0].object_active.Length != 0)
                            {
                                for (int i = 0; i < subComponent_data[0].object_active.Length; i++)
                                {
                                    if (i == 0) continue;

                                    subComponent_data[0].object_active[i].gameObject.SetActive(false);
                                }
                                subComponent_data[0].object_active[0].gameObject.SetActive(true);

                                //yield return new WaitForSeconds(1.0f);


                                subComponent_data[0].object_active[0].ActionEffect(id);
                                subComponent_data[0].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                            }
                        }
                    }
                    else if (assyCode == "0100")
                    {
                        if (figureNumber == "01")
                        {
                            if (subComponent_data[1].object_active.Length != 0)
                            {
                                for (int i = 0; i < subComponent_data[1].object_active.Length; i++)
                                {
                                    if (i == 0) continue;

                                    subComponent_data[1].object_active[i].gameObject.SetActive(false);
                                }
                                subComponent_data[1].object_active[0].gameObject.SetActive(true);

                                //yield return new WaitForSeconds(1.0f);


                                subComponent_data[1].object_active[0].ActionEffect(id);
                                subComponent_data[1].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                            }
                        }
                    }
                    break;

                //송수신 처리 장치
                case "4":
                    if (subsubSystemCode == "0")
                    {
                        if (assyCode == "0000")
                        {
                            if (subComponent_data[2].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[2].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[2].object_active[i].gameObject.SetActive(false);
                                    }

                                    subComponent_data[2].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(0.01f);

                                    subComponent_data[2].object_active[0].ActionEffect(id);
                                    subComponent_data[2].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();

                                }
                                else if (figureNumber == "02")
                                {
                                    for (int i = 0; i < subComponent_data[2].object_active.Length; i++)
                                    {
                                        if (i == 1) continue;

                                        subComponent_data[2].object_active[i].gameObject.SetActive(false);
                                    }

                                    subComponent_data[2].object_active[1].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[2].object_active[1].ActionEffect(id);
                                    subComponent_data[2].object_active[1].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                                else if (figureNumber == "03")
                                {
                                    for (int i = 0; i < subComponent_data[2].object_active.Length; i++)
                                    {
                                        if (i == 2) continue;

                                        subComponent_data[2].object_active[i].gameObject.SetActive(false);
                                    }

                                    subComponent_data[2].object_active[2].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[2].object_active[2].ActionEffect(id);
                                    subComponent_data[2].object_active[2].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                        else if (assyCode == "0100")
                        {
                            if (subComponent_data[3].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[3].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[3].object_active[i].gameObject.SetActive(false);
                                    }

                                    subComponent_data[3].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);
                                }
                                subComponent_data[3].object_active[0].ActionEffect(id);
                                subComponent_data[3].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                            }
                        }
                        else if (assyCode == "0200")
                        {
                            if (subComponent_data[4].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[4].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[4].object_active[i].gameObject.SetActive(false);
                                    }

                                    subComponent_data[4].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[4].object_active[0].ActionEffect(id);
                                    subComponent_data[4].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                        else if (assyCode == "0300")
                        {
                            if (subComponent_data[5].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[5].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[5].object_active[i].gameObject.SetActive(false);
                                    }
                                    subComponent_data[5].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[5].object_active[0].ActionEffect(id);
                                    subComponent_data[5].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                        else if (assyCode == "1900")
                        {
                            if (subComponent_data[6].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[6].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[6].object_active[i].gameObject.SetActive(false);
                                    }
                                    subComponent_data[6].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[6].object_active[0].ActionEffect(id);
                                    subComponent_data[6].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }

                        }
                    }
                    else if (subsubSystemCode == "1")
                    {
                        if (assyCode == "0000")
                        {
                            if (subComponent_data[7].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[7].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[7].object_active[i].gameObject.SetActive(false);
                                    }
                                    subComponent_data[7].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[7].object_active[0].ActionEffect(id);
                                    subComponent_data[7].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                    }
                    break;

                //송수신 처리 장치
                case "6":
                    if (subsubSystemCode == "0")
                    {
                        if (assyCode == "0000")
                        {
                            if (subComponent_data[8].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[8].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[8].object_active[i].gameObject.SetActive(false);
                                    }
                                    subComponent_data[8].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[8].object_active[0].ActionEffect(id);
                                    subComponent_data[8].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                        else if (assyCode == "0100")
                        {
                            if (subComponent_data[9].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[9].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[9].object_active[i].gameObject.SetActive(false);
                                    }
                                    subComponent_data[9].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[9].object_active[0].ActionEffect(id);
                                    subComponent_data[9].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                        else if (assyCode == "0200")
                        {
                            if (subComponent_data[10].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[10].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[10].object_active[i].gameObject.SetActive(false);
                                    }
                                    subComponent_data[10].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[10].object_active[0].ActionEffect(id);
                                    subComponent_data[10].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                        else if (assyCode == "0300")
                        {
                            if (subComponent_data[11].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[11].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[11].object_active[i].gameObject.SetActive(false);
                                    }
                                    subComponent_data[11].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[11].object_active[0].ActionEffect(id);
                                    subComponent_data[11].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                        else if (assyCode == "0400")
                        {
                            if (subComponent_data[12].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[12].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[12].object_active[i].gameObject.SetActive(false);
                                    }
                                    subComponent_data[12].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[12].object_active[0].ActionEffect(id);
                                    subComponent_data[12].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                    }
                    else if (subsubSystemCode == "1")
                    {
                        if (assyCode == "0000")
                        {
                            if (subComponent_data[13].object_active.Length != 0)
                            {
                                if (figureNumber == "01")
                                {
                                    for (int i = 0; i < subComponent_data[13].object_active.Length; i++)
                                    {
                                        if (i == 0) continue;

                                        subComponent_data[13].object_active[i].gameObject.SetActive(false);
                                    }
                                    subComponent_data[13].object_active[0].gameObject.SetActive(true);

                                    //yield return new WaitForSeconds(1.0f);


                                    subComponent_data[13].object_active[0].ActionEffect(id);
                                    subComponent_data[13].object_active[0].GetComponent<ModelingTouchModule>().ResetTransform();
                                }
                            }
                        }
                    }
                    break;

                default:
                    InitObject();
                    Debug.Log("SUBSYSTEMCODE : " + subSystemCode + " FIGURENUMBER : " + figureNumber + " 존재하는 모델링 리소스가 없습니다 ! ");
                    break;
            }
        //}
    }
    #endregion

    //공통으로 쓰는 객체
    //GameObject tranceive_CardOff, tranceive_UnderOff, powerFeeder_FieldOper, CableTableOff;

    #region 정비 절차 
    //처음 기본 상태 셋팅 처음 (셋팅됬을때만 생성된다.)
    public void OperationReady(string title, string subtitle, OperationSetting opersetting)
    {
        InitFigure();
        //Debug.Log("Title : " + title + " / subTitle : " + subtitle);

        //추후에 SubTitle 사라질 예정 
        if (UM.operation == UIManager.Operation.Field)
        {
            for (int i = 0; i < operationStep_data.Length; i++)
            {
                if (operationStep_data[i].title == title)
                {
                    if (operationStep_data[i].subTitle == subtitle)
                    {
                        InitObject_Operation();

                        setAuto_TB.gameObject.SetActive(true);

                        if (operationStep_data[i].step_pre == null)
                            Debug.Log("NO RESOURCE");
                        else
                        {
                            if (operationStep_data[i].step_obj == null)
                            {
                                operationStep_data[i].step_obj = Instantiate(operationStep_data[i].step_pre, operationStep_data[i].pre_tr);

                                operationStep_data[i].stepAnim = operationStep_data[i].step_obj.GetComponentInChildren<Animator>();
                                operationStep_data[i].modelModule = operationStep_data[i].step_obj.GetComponent<ModelingTouchModule>();
                                operationStep_data[i].modelModule.modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }
                        }

                        if (operationStep_data[i].useTool_img != null)
                            operationStep_data[i].useTool_img.SetUseToolImage(0);


                        setAuto_TB.active_event.AddListener(() =>
                        {
                            StartCoroutine(AutoAnimator(title, subtitle, opersetting));
                        });


                        setAuto_TB.defalt_event.AddListener(() =>
                        {
                            ManualAnimator();
                        });

                        //최초 시작은 자동모드 진행
                        StartCoroutine(AutoAnimator(title, subtitle, opersetting));

                        break;
                    }
                }
            }
        }
        else if (UM.operation == UIManager.Operation.Unit)
        {
            for (int i = 0; i < operationStep_data.Length; i++)
            {
                if (operationStep_data[i].title == title)
                {
                    InitObject_Operation();

                    setAuto_TB.gameObject.SetActive(true);

                    if (operationStep_data[i].step_pre == null)
                        Debug.Log("NO RESOURCE");
                    else
                    {
                        if (operationStep_data[i].step_obj == null)
                        {
                            operationStep_data[i].step_obj = Instantiate(operationStep_data[i].step_pre, operationStep_data[i].pre_tr);

                            operationStep_data[i].stepAnim = operationStep_data[i].step_obj.GetComponentInChildren<Animator>();
                            operationStep_data[i].modelModule = operationStep_data[i].step_obj.GetComponent<ModelingTouchModule>();
                            operationStep_data[i].modelModule.modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                        }
                    }

                    if (operationStep_data[i].useTool_img != null)
                        operationStep_data[i].useTool_img.SetUseToolImage(0);


                    setAuto_TB.active_event.AddListener(() =>
                    {

                        StartCoroutine(AutoAnimator(title, subtitle, opersetting));
                    });


                    setAuto_TB.defalt_event.AddListener(() =>
                    {
                        ManualAnimator();
                    });

                    //최초 시작은 자동모드 진행
                    StartCoroutine(AutoAnimator(title, subtitle, opersetting));

                    break;
                }
            }
        }
    }



    //정비절차 클릭할때마다 작동(애니메이터 제어)
    public void OperationButtonClick(string title, string subtitle, int step)
    {
        ManualAnimator(); //수동 모드 작동 

        setAuto_TB.SetActive(); //토글 버튼 해제 

        //추후에 SubTitle 사라질 예정 
        if (UM.operation == UIManager.Operation.Field)
        {
            for (int i = 0; i < operationStep_data.Length; i++)
            {
                if (operationStep_data[i].title == title)
                {
                    if (operationStep_data[i].subTitle == subtitle)
                    {
                        if (operationStep_data[i].modelModule != null)
                        {
                            operationStep_data[i].modelModule.ResetTransform();
                        }

                        if (operationStep_data[i].stepAnim != null)
                        {
                            operationStep_data[i].stepAnim.Rebind();
                            if(operationStep_data[i].stepAnim.parameters.Length != 1)
                            {
                                operationStep_data[i].stepAnim.SetInteger("Order", operationStep_data[i].animOrder);
                            }
                            operationStep_data[i].stepAnim.SetInteger("Step", step);
                        }
                        else
                            Debug.Log("NO ANIMATOR");

                        if (operationStep_data[i].useTool_img != null)
                            operationStep_data[i].useTool_img.SetUseToolImage(step);

                        break;
                    }
                }
            }
        }
        else if (UM.operation == UIManager.Operation.Unit)
        {
            for (int i = 0; i < operationStep_data.Length; i++)
            {
                if (operationStep_data[i].title == title)
                {
                    if (operationStep_data[i].modelModule != null)
                    {
                        operationStep_data[i].modelModule.ResetTransform();
                    }

                    if (operationStep_data[i].stepAnim != null)
                    {
                        operationStep_data[i].stepAnim.Rebind();
                        if (operationStep_data[i].stepAnim.parameters.Length != 1)
                        {
                            operationStep_data[i].stepAnim.SetInteger("Order", operationStep_data[i].animOrder);
                        }
                        operationStep_data[i].stepAnim.SetInteger("Step", step);
                    }
                    else
                        Debug.Log("NO ANIMATOR");

                    if (operationStep_data[i].useTool_img != null)
                        operationStep_data[i].useTool_img.SetUseToolImage(step);

                    break;
                }
            }
        }
    }

    //자동 진행
    public IEnumerator AutoAnimator(string title, string subtitle, OperationSetting opersetting)
    {
        if (UM.operation == UIManager.Operation.Field) //야전
        {
            for (int i = 0; i < operationStep_data.Length; i++)
            {
                if (operationStep_data[i].title == title)
                {
                    if (operationStep_data[i].subTitle == subtitle)
                    {
                        for (int j = 0; j < operationStep_data[i].stepAnim_clipCount; j++)
                        {
                            if (operationStep_data[i].modelModule != null)
                            {
                                operationStep_data[i].modelModule.ResetTransform();
                            }

                            if (operationStep_data[i].stepAnim != null)
                            {
                                if (operationStep_data[i].stepAnim.parameters.Length != 1)
                                {
                                    operationStep_data[i].stepAnim.SetInteger("Order", operationStep_data[i].animOrder);
                                }
                                operationStep_data[i].stepAnim.SetInteger("Step", j);
                            }

                            opersetting.ChildFunCheck(j);

                            if (operationStep_data[i].useTool_img != null)
                                operationStep_data[i].useTool_img.SetUseToolImage(j);
                            
                            if (operationStep_data[i].stepAnim != null)
                            {
                                while (true)
                                {
                                    yield return null;

                                    if (operationStep_data[i].stepAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                                    {
                                        break;
                                    }
                                }

                                //마지막 절차가 끝나면 자동재생 버튼 수동으로 변경 
                                if (j == operationStep_data[i].stepAnim_clipCount - 1)
                                {
                                    setAuto_TB.SetActive();
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
        else if (UM.operation == UIManager.Operation.Unit) //부대
        {
            for (int i = 0; i < operationStep_data.Length; i++)
            {
                if (operationStep_data[i].title == title)
                {
                    for (int j = 0; j < operationStep_data[i].stepAnim_clipCount; j++)
                    {
                        if (operationStep_data[i].modelModule != null)
                        {
                            operationStep_data[i].modelModule.ResetTransform();
                        }

                        if (operationStep_data[i].stepAnim != null)
                        {
                            if (operationStep_data[i].stepAnim.parameters.Length != 1)
                            {
                                operationStep_data[i].stepAnim.SetInteger("Order", operationStep_data[i].animOrder);
                            }
                            operationStep_data[i].stepAnim.SetInteger("Step", j);
                        }

                        opersetting.ChildFunCheck(j);

                        if (operationStep_data[i].useTool_img != null)
                            operationStep_data[i].useTool_img.SetUseToolImage(j);

                        if (operationStep_data[i].stepAnim != null)
                        {
                            while (true)
                            {
                                yield return null;

                                if (operationStep_data[i].stepAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                                {
                                    break;
                                }
                            }

                            //마지막 절차가 끝나면 자동재생 버튼 수동으로 변경 
                            if (j == operationStep_data[i].stepAnim_clipCount - 1)
                            {
                                setAuto_TB.SetActive();
                            }
                        }
                    }

                    break;
                }
            }
        }
    }


    //수동 진행
    public void ManualAnimator()
    {
        StopAllCoroutines();
    }
    #endregion

    bool toggleTrigger = false;
    string id, info = "";

    #region FIGURE SETTING 
    public void FigureSetting(string _id, string _info)
    {
        ResetFigureData();

        if (id == "" && info == "") //한번 할당
        {
            toggleTrigger = true;

            id = _id;
            info = _info;
        }
        else
        {
            if (id != _id || info != _info)
            {
                toggleTrigger = false; //초기화

                id = _id;
                info = _info;
            }

            toggleTrigger = !toggleTrigger;
        }

        for (int i = 0; i < figureitems.Length; i++)
        {
            if (figureitems[i].hasSub)
            {
                if ((figureitems[i].id == id && figureitems[i].info == info)
                    || (figureitems[i].id_sub == id && figureitems[i].info_sub == info))
                {
                    if (figureitems[i].itemType == ITEMTYPE.IMAGE)
                    {
                        ImageFigure = figureitems[i].gameObject;

                        ImageFigure.SetActive(toggleTrigger);
                    }
                    else
                    {
                        if (toggleTrigger)
                        {
                            currentItem = Instantiate(figureitems[i], figureitems_rectr);
                            currentItem.GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                        }
                    }

                    if (figureitems[i].listbox != null)
                    {
                        setFullMode_TB.gameObject.SetActive(toggleTrigger);

                        figureitems[i].listbox.gameObject.SetActive(toggleTrigger);
                    }
                    else
                    {
                        setFullMode_TB.gameObject.SetActive(toggleTrigger);
                    }

                    if (toggleTrigger)
                    {
                        if (figureitems[i].GetComponent<DiagramController>())
                            figureitems[i].GetComponent<DiagramController>().FullOffModeSetting();
                    }

                    if (figureitems[i].GetComponent<HeaderController>())
                        figureitems[i].GetComponent<HeaderController>().header_obj.SetActive(false);

                    break;
                }
            }
            else
            {
                if (figureitems[i].id == id && figureitems[i].info == info)
                {
                    if (figureitems[i].itemType == ITEMTYPE.IMAGE)
                    {
                        ImageFigure = figureitems[i].gameObject;

                        ImageFigure.SetActive(toggleTrigger);
                    }
                    else
                    {
                        if (toggleTrigger)
                        {
                            currentItem = Instantiate(figureitems[i], figureitems_rectr);
                            currentItem.GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                        }
                    }

                    if (figureitems[i].listbox != null)
                    {
                        setFullMode_TB.gameObject.SetActive(toggleTrigger);

                        figureitems[i].listbox.gameObject.SetActive(toggleTrigger);
                    }
                    else
                    {
                        setFullMode_TB.gameObject.SetActive(toggleTrigger);
                    }

                    if (toggleTrigger)
                    {
                        if (figureitems[i].GetComponent<DiagramController>())
                            figureitems[i].GetComponent<DiagramController>().FullOffModeSetting();
                    }

                    if (figureitems[i].GetComponent<HeaderController>())
                        figureitems[i].GetComponent<HeaderController>().header_obj.SetActive(false);

                    break;
                }
            }
        }
    }

    #endregion

    //현재 작동된 리스트 박스 제외 비활성화
    void ListBoxSelect()
    {
        for (int i = 0; i < figureitems.Length; i++)
        {
            if (figureitems[i].listbox != null)
            {
                if (figureitems[i].listbox.gameObject.activeInHierarchy)
                    continue;
                else
                    figureitems[i].listbox.gameObject.SetActive(false);
            }
        }
    }

    void ListBoxInit()
    {
        listBox_rectr.localPosition = new Vector3(-530f, 0f, 0f);
    }

    //trigger = true; (확장 모드), trigger = false; (축소 모드)
    public void SetListBoxPosition()
    {
        if (listBox_rectr.localPosition.x == -530f)
            listBox_rectr.localPosition = new Vector3(-900f, 0f, 0f);
        else if (listBox_rectr.localPosition.x == -900f)
            listBox_rectr.localPosition = new Vector3(-530f, 0f, 0f);
    }

    [HideInInspector] GameObject ImageFigure; //삭제는 되지 않는다. 

    #region FULL MODE BUTTON 
    //확장/축소 모드 버튼 설정 
    public void FullModeButtonSetting()
    {
        setFullMode_TB.defalt_event.AddListener(() =>
        {
            detailSettingCanvas_sc.FullScreenOnSetting();

            if (ImageFigure != null)
            {
                if (ImageFigure.GetComponent<DiagramController>())
                    ImageFigure.GetComponent<DiagramController>().FullOnModeSetting();

                if (ImageFigure.GetComponent<HeaderController>())
                    ImageFigure.GetComponent<HeaderController>().header_obj.SetActive(true);
            }
        });

        setFullMode_TB.active_event.AddListener(() =>
        {
            detailSettingCanvas_sc.FullScreenOffSetting();

            if (ImageFigure != null)
            {
                if (ImageFigure.GetComponent<DiagramController>())
                    ImageFigure.GetComponent<DiagramController>().FullOffModeSetting();

                if (ImageFigure.GetComponent<HeaderController>())
                    ImageFigure.GetComponent<HeaderController>().header_obj.SetActive(false);
            }
        });
    }

    #endregion
}
