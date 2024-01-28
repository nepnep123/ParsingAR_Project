using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ر���ǰ ���ҽ� 
[System.Serializable]
public struct EquipUnderResource
{
    public string objectName;

    //���������� ���� ���� ������ �ִ� ��찡 �ִ�. 
    public ObjectInfoList[] object_pre; //���� ���� 
    public RectTransform obj_tr;

    [SerializeField] public ObjectInfoList[] object_active; //MODEL �Ҵ�
}

//���� ���� ���ҽ� 
[System.Serializable]
public struct OperationStepPrefab
{
    public string title;
    public string subTitle;

    public GameObject step_pre;
    public Transform pre_tr;

    [SerializeField] public GameObject step_obj;
    [SerializeField] public Animator stepAnim; //Ŭ���ҽ� �Ҵ�Ǵ� ��ü 

    public int animOrder; //���� �ִϸ����͸� �����ϴ°�� ��� ��ȣ 
    [HideInInspector] public ModelingTouchModule modelModule; //��� ���� 

    public int stepAnim_clipCount; //�ڵ������ ���� 

    public OperObjectUseTool useTool_img; //��� ���� 
}

public class ObjectListSetting : MonoBehaviour
{
    [SerializeField] public EquipUnderResource[] subComponent_data;

    [SerializeField] public OperationStepPrefab[] operationStep_data;

    [SerializeField] public FigureItem[] figureitems;
    [HideInInspector] public FigureItem currentItem;
 
    [SerializeField] public RectTransform figureitems_rectr;


    [Header(" [ CONTROL BUTTON ] ")]
    [SerializeField] public ToggleButtonController setAuto_TB; //���� ���������� ��� �Ѵ�. 
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

        ListBoxInit(); //��ġ ���� 
    }

    #region INIT
    public void InitObject() //���� ����ǰ 
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
                //�޸� ���� 
                subComponent_data[i].object_active = null;
            }

        }
    }

    public void InitObject_Operation() //�������� 
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

    //Figure(�̹���, �𵨸�) ���ҽ� �ʱ�ȭ 
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

    //������ ���� �� �ʱ�ȭ 
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



    #region ���� ����ǰ 
    //���� �޴� ��ư Ŭ�� (ī�װ� ���̺� �ʿ��� �𵨸��� ��ũ���ش�)
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

        //            //���������� ���� ���� ������ �ִ� ��찡 �ִ�. 
        //            for (int i = 0; i < subComponent_data[0].object_active.Length; i++)
        //            {
        //                subComponent_data[0].object_active[i] = Instantiate(subComponent_data[0].object_pre[i], subComponent_data[0].obj_tr);
        //                subComponent_data[0].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
        //            }

        //            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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
                //�δ����� 
                case "0":
                    if (subsubSystemCode == "0")
                    {
                        //�δ����� �ۼ���ó����ġ ������ü  
                        if (assyCode == "W631")
                        {
                            subComponent_data[0].object_active = new ObjectInfoList[subComponent_data[0].object_pre.Length];

                            //���������� ���� ���� ������ �ִ� ��찡 �ִ�. 
                            for (int i = 0; i < subComponent_data[0].object_active.Length; i++)
                            {
                                subComponent_data[0].object_active[i] = Instantiate(subComponent_data[0].object_pre[i], subComponent_data[0].obj_tr);
                                subComponent_data[0].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
                            //catalog.link_pre = subComponent_data[0].object_active[0].gameObject;
                        }
                        //�δ����� ����������ġ ������ü  
                        else if (assyCode == "W651")
                        {
                            subComponent_data[1].object_active = new ObjectInfoList[subComponent_data[0].object_pre.Length];

                            //���������� ���� ���� ������ �ִ� ��찡 �ִ�. 
                            for (int i = 0; i < subComponent_data[1].object_active.Length; i++)
                            {
                                subComponent_data[1].object_active[i] = Instantiate(subComponent_data[1].object_pre[i], subComponent_data[1].obj_tr);
                                subComponent_data[1].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
                            //catalog.link_pre = subComponent_data[1].object_active[0].gameObject;
                        }
                    }
                    break;
                //��������
                //���׳� ��ġ 
                case "2":
                    if (assyCode == "0000")
                    {
                        subComponent_data[0].object_active = new ObjectInfoList[subComponent_data[0].object_pre.Length];

                        //���������� ���� ���� ������ �ִ� ��찡 �ִ�. 
                        for (int i = 0; i < subComponent_data[0].object_active.Length; i++)
                        {
                            subComponent_data[0].object_active[i] = Instantiate(subComponent_data[0].object_pre[i], subComponent_data[0].obj_tr);
                            subComponent_data[0].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                        }

                        //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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

                        //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
                        //catalog.link_pre = subComponent_data[1].object_active[0].gameObject;
                    }
                    break;
                //�ۼ��� ó�� ��ġ 
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

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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

                //����������ġ
                case "6":
                    if (subsubSystemCode == "0")
                    {
                        //����������ġ
                        if (assyCode == "0000")
                        {
                            subComponent_data[8].object_active = new ObjectInfoList[subComponent_data[8].object_pre.Length];

                            for (int i = 0; i < subComponent_data[8].object_active.Length; i++)
                            {
                                subComponent_data[8].object_active[i] = Instantiate(subComponent_data[8].object_pre[i], subComponent_data[8].obj_tr);
                                subComponent_data[8].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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
                        //����������ü
                        else if (assyCode == "0100")
                        {
                            subComponent_data[9].object_active = new ObjectInfoList[subComponent_data[9].object_pre.Length];

                            for (int i = 0; i < subComponent_data[9].object_active.Length; i++)
                            {
                                subComponent_data[9].object_active[i] = Instantiate(subComponent_data[9].object_pre[i], subComponent_data[9].obj_tr);
                                subComponent_data[9].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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
                        //�����������ü
                        else if (assyCode == "0200")
                        {
                            subComponent_data[10].object_active = new ObjectInfoList[subComponent_data[10].object_pre.Length];

                            for (int i = 0; i < subComponent_data[10].object_active.Length; i++)
                            {
                                subComponent_data[10].object_active[i] = Instantiate(subComponent_data[10].object_pre[i], subComponent_data[10].obj_tr);
                                subComponent_data[10].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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
                        //�Էº�ȣ����ü
                        else if (assyCode == "0300")
                        {
                            subComponent_data[11].object_active = new ObjectInfoList[subComponent_data[11].object_pre.Length];

                            for (int i = 0; i < subComponent_data[11].object_active.Length; i++)
                            {
                                subComponent_data[11].object_active[i] = Instantiate(subComponent_data[11].object_pre[i], subComponent_data[11].obj_tr);
                                subComponent_data[11].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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
                        //������������ü
                        else if (assyCode == "0400")
                        {
                            subComponent_data[12].object_active = new ObjectInfoList[subComponent_data[12].object_pre.Length];

                            for (int i = 0; i < subComponent_data[12].object_active.Length; i++)
                            {
                                subComponent_data[12].object_active[i] = Instantiate(subComponent_data[12].object_pre[i], subComponent_data[12].obj_tr);
                                subComponent_data[12].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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
                        //������ü
                        if (assyCode == "0000")
                        {
                            subComponent_data[13].object_active = new ObjectInfoList[subComponent_data[13].object_pre.Length];

                            for (int i = 0; i < subComponent_data[13].object_active.Length; i++)
                            {
                                subComponent_data[13].object_active[i] = Instantiate(subComponent_data[13].object_pre[i], subComponent_data[13].obj_tr);
                                subComponent_data[13].object_active[i].GetComponent<ModelingTouchModule>().modelCamera = detailSettingCanvas_sc.UM.modelCamera_detail;
                            }

                            //ù������ ���� ���Ŀ� īŻ�α׵����Ϳ� ���� 
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
                    Debug.Log("SUBSYSTEMCODE : " + subSystemCode + " �����ϴ� �𵨸� ���ҽ��� �����ϴ� ! ");
                    break;
            }
        //}
    }

    //������Ʈ�� Ű���� �޾� �ش� �̺�Ʈ�� �߻� ��Ų��. 
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
            //�𵨸��� �ִٴ� ���� 
            switch (subSystemCode)
            {
                //�δ�����
                case "0":
                    if (subsubSystemCode == "0")
                    {
                        //�δ����� �ۼ���ó����ġ ������ü  
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
                        else //�δ����� ����������ġ ������ü  
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


                //��������
                //���׳���ġ
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

                //�ۼ��� ó�� ��ġ
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

                //�ۼ��� ó�� ��ġ
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
                    Debug.Log("SUBSYSTEMCODE : " + subSystemCode + " FIGURENUMBER : " + figureNumber + " �����ϴ� �𵨸� ���ҽ��� �����ϴ� ! ");
                    break;
            }
        //}
    }
    #endregion

    //�������� ���� ��ü
    //GameObject tranceive_CardOff, tranceive_UnderOff, powerFeeder_FieldOper, CableTableOff;

    #region ���� ���� 
    //ó�� �⺻ ���� ���� ó�� (���É������� �����ȴ�.)
    public void OperationReady(string title, string subtitle, OperationSetting opersetting)
    {
        InitFigure();
        //Debug.Log("Title : " + title + " / subTitle : " + subtitle);

        //���Ŀ� SubTitle ����� ���� 
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

                        //���� ������ �ڵ���� ����
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

                    //���� ������ �ڵ���� ����
                    StartCoroutine(AutoAnimator(title, subtitle, opersetting));

                    break;
                }
            }
        }
    }



    //�������� Ŭ���Ҷ����� �۵�(�ִϸ����� ����)
    public void OperationButtonClick(string title, string subtitle, int step)
    {
        ManualAnimator(); //���� ��� �۵� 

        setAuto_TB.SetActive(); //��� ��ư ���� 

        //���Ŀ� SubTitle ����� ���� 
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

    //�ڵ� ����
    public IEnumerator AutoAnimator(string title, string subtitle, OperationSetting opersetting)
    {
        if (UM.operation == UIManager.Operation.Field) //����
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

                                //������ ������ ������ �ڵ���� ��ư �������� ���� 
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
        else if (UM.operation == UIManager.Operation.Unit) //�δ�
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

                            //������ ������ ������ �ڵ���� ��ư �������� ���� 
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


    //���� ����
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

        if (id == "" && info == "") //�ѹ� �Ҵ�
        {
            toggleTrigger = true;

            id = _id;
            info = _info;
        }
        else
        {
            if (id != _id || info != _info)
            {
                toggleTrigger = false; //�ʱ�ȭ

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

    //���� �۵��� ����Ʈ �ڽ� ���� ��Ȱ��ȭ
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

    //trigger = true; (Ȯ�� ���), trigger = false; (��� ���)
    public void SetListBoxPosition()
    {
        if (listBox_rectr.localPosition.x == -530f)
            listBox_rectr.localPosition = new Vector3(-900f, 0f, 0f);
        else if (listBox_rectr.localPosition.x == -900f)
            listBox_rectr.localPosition = new Vector3(-530f, 0f, 0f);
    }

    [HideInInspector] GameObject ImageFigure; //������ ���� �ʴ´�. 

    #region FULL MODE BUTTON 
    //Ȯ��/��� ��� ��ư ���� 
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
