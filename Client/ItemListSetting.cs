using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SubTitle
{
    public List<ItemInfo> buttons; //�ش� �޴��� ��ư�� 
}

public class ItemListSetting : MonoBehaviour
{
    [SerializeField] public ItemInfo Items_pre;
    [SerializeField] public Transform depth_header_tr, depth_scroll_tr;

    [HideInInspector] public ItemInfo[] Items;

    [SerializeField] public SubTitle[] depth__menuList; //�޴��� ��� 0_depth ��ư �Ҵ� //0,1,2,3,4,5

    [SerializeField] public List<ItemInfo> header_items = new List<ItemInfo>(); //��� ��ư 

    [Header(" [ RESOURCE ] ")]
    [SerializeField] XMLManager xmlManager;
    [SerializeField] UIManager uiManager;

    [Header(" [ ITEM SETTING ] ")]
    [SerializeField] Sprite depth_0_spr_default;
    [SerializeField] Sprite depth_0_spr_active, depth_1_spr_default, depth_1_spr_active;

    [SerializeField] int menuMaxNum; //menu �׸� ���缭 ������ ������. 

    [SerializeField] public int clickMenu_num; //���� Ŭ���� ��� �ѹ� (�˻��ϱ����� �ε���)

    enum Operation { Field, Unit };//������ ���Ǵ� ����
    [SerializeField] Operation oper; 

    public void CreateTableList()
    {
        if (oper == Operation.Field)
        {
            uiManager.detailSettingCanvas_Field.contentListSetting.CreatePage(); //������ŭ ������ ���� 

            Items = new ItemInfo[xmlManager.itemTrees_list_Field.Count];

            menuMaxNum = xmlManager.menuNum;

            int btnID = 0; //��ư���� ID ���ο�

            //���� �� ��ġ����, �ɼǰ� �ο�, ��Ÿ�� ���� 
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = Instantiate(Items_pre);

                Items[i].menuNum = xmlManager.itemTrees_list_Field[i].menuNum;
                Items[i].item_txt.text = xmlManager.itemTrees_list_Field[i].title;
                Items[i].isSub_1depth = xmlManager.itemTrees_list_Field[i].isSub_1depth;
                Items[i].isSub_2depth = xmlManager.itemTrees_list_Field[i].isSub_2depth;
                Items[i].isSub_3depth = xmlManager.itemTrees_list_Field[i].isSub_3depth;
                Items[i].hasChild = xmlManager.itemTrees_list_Field[i].hasChild;

                if (!Items[i].isSub_1depth && !Items[i].isSub_2depth && !Items[i].isSub_3depth) //Header
                {
                    Items[i].transform.SetParent(depth_header_tr, false);

                    header_items.Add(Items[i]); //header ���� 

                    //ID�� �ο� 
                    Items[i].id = btnID.ToString();
                    ++btnID;

                    Items[i].child_icon_default.gameObject.SetActive(false);
                    Items[i].child_icon_active.gameObject.SetActive(false);
                    Items[i].child_icon_default.sprite = null;
                    Items[i].child_icon_active.sprite = null;
                }
                else if (Items[i].isSub_1depth && !Items[i].isSub_2depth && !Items[i].isSub_3depth && Items[i].hasChild)
                {
                    Items[i].transform.SetParent(depth_scroll_tr, false);

                    Items[i].child_icon_default.gameObject.SetActive(true);
                    Items[i].child_icon_active.gameObject.SetActive(false);
                    Items[i].child_icon_default.sprite = depth_0_spr_default;
                    Items[i].child_icon_active.sprite = depth_0_spr_active;
                    Items[i].isToggleButton = true;
                }
                else if (Items[i].isSub_1depth && Items[i].isSub_2depth && !Items[i].isSub_3depth && Items[i].hasChild)
                {
                    Items[i].transform.SetParent(depth_scroll_tr, false);

                    Items[i].child_icon_default.gameObject.SetActive(true);
                    Items[i].child_icon_active.gameObject.SetActive(false);
                    Items[i].child_icon_default.sprite = depth_1_spr_default;
                    Items[i].child_icon_active.sprite = depth_1_spr_active;
                    Items[i].isToggleButton = true;

                    Items[i].child_icon_default.GetComponent<RectTransform>().sizeDelta = new Vector2(28.0f, 28.0f);
                    Items[i].child_icon_active.GetComponent<RectTransform>().sizeDelta = new Vector2(28.0f, 28.0f);
                }
                else
                {
                    Items[i].transform.SetParent(depth_scroll_tr, false);

                    Items[i].child_icon_default.gameObject.SetActive(false);
                    Items[i].child_icon_active.gameObject.SetActive(false);
                    Items[i].child_icon_default.sprite = null;
                    Items[i].child_icon_active.sprite = null;
                }
            }

            // �� �޴� ���� �����ϱ� ���� �Ҵ� 
            depth__menuList = new SubTitle[menuMaxNum + 1];

            for (int z = 0; z < depth__menuList.Length; z++)
            {
                depth__menuList[z].buttons = new List<ItemInfo>();

                for (int i = 0; i < Items.Length; i++)
                {
                    if (Items[i].menuNum == z)
                    {
                        //header�� ���� ����
                        if (!Items[i].isSub_1depth && !Items[i].isSub_2depth && !Items[i].isSub_3depth) //Header
                        {
                            continue;
                        }
                        else
                        {
                            depth__menuList[z].buttons.Add(Items[i]);
                        }
                    }
                }
            }
        }
        else if (oper == Operation.Unit)
        {
            uiManager.detailSettingCanvas_Unit.contentListSetting.CreatePage(); //������ŭ ������ ���� 

            Items = new ItemInfo[xmlManager.itemTrees_list_Unit.Count];

            menuMaxNum = xmlManager.menuNum;

            int btnID = 0; //��ư���� ID ���ο�

            //���� �� ��ġ����, �ɼǰ� �ο�, ��Ÿ�� ���� 
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = Instantiate(Items_pre);

                Items[i].menuNum = xmlManager.itemTrees_list_Unit[i].menuNum;
                Items[i].item_txt.text = xmlManager.itemTrees_list_Unit[i].title;
                Items[i].isSub_1depth = xmlManager.itemTrees_list_Unit[i].isSub_1depth;
                Items[i].isSub_2depth = xmlManager.itemTrees_list_Unit[i].isSub_2depth;
                Items[i].isSub_3depth = xmlManager.itemTrees_list_Unit[i].isSub_3depth;
                Items[i].hasChild = xmlManager.itemTrees_list_Unit[i].hasChild;

                if (!Items[i].isSub_1depth && !Items[i].isSub_2depth && !Items[i].isSub_3depth) //Header
                {
                    Items[i].transform.SetParent(depth_header_tr, false);

                    header_items.Add(Items[i]); //header ���� 

                    //ID�� �ο� 
                    Items[i].id = btnID.ToString();
                    ++btnID;

                    Items[i].child_icon_default.gameObject.SetActive(false);
                    Items[i].child_icon_active.gameObject.SetActive(false);
                    Items[i].child_icon_default.sprite = null;
                    Items[i].child_icon_active.sprite = null;
                }
                else if (Items[i].isSub_1depth && !Items[i].isSub_2depth && !Items[i].isSub_3depth && Items[i].hasChild)
                {
                    Items[i].transform.SetParent(depth_scroll_tr, false);

                    Items[i].child_icon_default.gameObject.SetActive(true);
                    Items[i].child_icon_active.gameObject.SetActive(false);
                    Items[i].child_icon_default.sprite = depth_0_spr_default;
                    Items[i].child_icon_active.sprite = depth_0_spr_active;
                    Items[i].isToggleButton = true;
                }
                else if (Items[i].isSub_1depth && Items[i].isSub_2depth && !Items[i].isSub_3depth && Items[i].hasChild)
                {
                    Items[i].transform.SetParent(depth_scroll_tr, false);

                    Items[i].child_icon_default.gameObject.SetActive(true);
                    Items[i].child_icon_active.gameObject.SetActive(false);
                    Items[i].child_icon_default.sprite = depth_1_spr_default;
                    Items[i].child_icon_active.sprite = depth_1_spr_active;
                    Items[i].isToggleButton = true;

                    Items[i].child_icon_default.GetComponent<RectTransform>().sizeDelta = new Vector2(28.0f, 28.0f);
                    Items[i].child_icon_active.GetComponent<RectTransform>().sizeDelta = new Vector2(28.0f, 28.0f);
                }
                else
                {
                    Items[i].transform.SetParent(depth_scroll_tr, false);

                    Items[i].child_icon_default.gameObject.SetActive(false);
                    Items[i].child_icon_active.gameObject.SetActive(false);
                    Items[i].child_icon_default.sprite = null;
                    Items[i].child_icon_active.sprite = null;
                }
            }

            // �� �޴� ���� �����ϱ� ���� �Ҵ� 
            depth__menuList = new SubTitle[menuMaxNum + 1];

            for (int z = 0; z < depth__menuList.Length; z++)
            {
                depth__menuList[z].buttons = new List<ItemInfo>();

                for (int i = 0; i < Items.Length; i++)
                {
                    if (Items[i].menuNum == z)
                    {
                        //header�� ���� ����
                        if (!Items[i].isSub_1depth && !Items[i].isSub_2depth && !Items[i].isSub_3depth) //Header
                        {
                            continue;
                        }
                        else
                        {
                            depth__menuList[z].buttons.Add(Items[i]);
                        }
                    }
                }
            }

        }

        ButtonEventSetting();
        SetButtonID();
    }

    //�� ��ư���� ID ���� �ο� �ؼ� AR �ν���Ʈ���� �󼼺���� �Ѿ�ö� �޴��� �����ش�.
    void SetButtonID()
    {
        //��� ID �� �ο� 
        for (int i = 0; i < header_items.Count; i++)
        {
            header_items[i].id = i.ToString();
        }

        //��� ���� 
        for (int i = 0; i < depth__menuList.Length; i++)
        {
            int temp = 0; //ID �� �ο� 
            for (int j = 0; j < depth__menuList[i].buttons.Count; j++)
            {
                //1depth + 1depth folder
                if (depth__menuList[i].buttons[j].isSub_1depth && !depth__menuList[i].buttons[j].isSub_2depth &&
                    !depth__menuList[i].buttons[j].isSub_3depth)
                {
                    depth__menuList[i].buttons[j].id = header_items[i].id + "_" + temp.ToString();
                    ++temp;
                }

               //2depth + 2depth folder
                else if(depth__menuList[i].buttons[j].isSub_1depth && depth__menuList[i].buttons[j].isSub_2depth &&
                    !depth__menuList[i].buttons[j].isSub_3depth)
                {
                    depth__menuList[i].buttons[j].id = header_items[i].id + "_" + temp.ToString();
                    ++temp;
                }

                else if (depth__menuList[i].buttons[j].isSub_1depth && depth__menuList[i].buttons[j].isSub_2depth &&
                    depth__menuList[i].buttons[j].isSub_3depth)
                {
                    depth__menuList[i].buttons[j].id = header_items[i].id + "_" + temp.ToString();
                    ++temp;
                }
                else
                {};
            }
        }
    }

    //��ư Ŭ�� �̺�Ʈ ó�� 
    void ButtonEventSetting()
    {
        if (oper == Operation.Field)
        {
            //Header �̺�Ʈ ��� 
            for (int i = 0; i < header_items.Count; i++)
            {
                int tmp;
                tmp = i;

                //��ư �̺�Ʈ ��� 
                header_items[i].itme_btn.onClick.AddListener(() =>
                {
                    SubMenuButtonInit(); //���� �޴� �ʱ�ȭ 
                    HeaderState(tmp, false); //��� ��ư ���� 
                    HeaderMenuController(tmp); //���� ��ư�� �����ư ���� 
                    uiManager.detailSettingCanvas_Field.HeaderButtonClick();
                });
            }

            int contentPageNum = -1; //������ �ѹ� (��µɶ����� + 1)

            //SubMenu �̺�Ʈ ���
            for (int i = 0; i < depth__menuList.Length; i++)
            {
                //�޴��� üũ (0 : ����, 1 : ����,������� �� �Ҹ�ǰ ����, 2 : ���ɹ� ���, 3 : ���� �� ����Ž��, 4 : ����, 5 : �κ�ǰ ���� ����)
                for (int j = 0; j < depth__menuList[i].buttons.Count; j++)
                {
                    //���� ���� 
                    if (depth__menuList[i].buttons[j].hasChild)
                    {
                        int tmp, tmp_1;
                        tmp = i;
                        tmp_1 = j;

                        //��ư �̺�Ʈ ��� 
                        depth__menuList[i].buttons[j].itme_btn.onClick.AddListener(() =>
                        {
                            SubFolderController(tmp, tmp_1, false);
                        });
                    }
                    //������ ���� 
                    else
                    {
                        int tmp, tmp_1;
                        tmp = i;
                        tmp_1 = j;

                        //������ �ѹ� ī��Ʈ 
                        ++contentPageNum; //0���� ���� 
                        int tmp_2;
                        tmp_2 = contentPageNum;

                        //��ư �̺�Ʈ ��� 
                        depth__menuList[i].buttons[j].itme_btn.onClick.AddListener(() =>
                        {
                            SubMenuController(tmp, tmp_1, false); //���� ��ư ���� ������ ��ư ���� �ʱ�ȭ 

                            uiManager.detailSettingCanvas_Field.contentListSetting.ContentButtonClick(tmp_2);
                            uiManager.detailSettingCanvas_Field.ContentShow();
                        });
                    }
                }
            }
        }
        else if (oper == Operation.Unit)
        {
            //Header �̺�Ʈ ��� 
            for (int i = 0; i < header_items.Count; i++)
            {
                int tmp;
                tmp = i;

                //��ư �̺�Ʈ ��� 
                header_items[i].itme_btn.onClick.AddListener(() =>
                {
                    SubMenuButtonInit(); //���� �޴� �ʱ�ȭ 
                    HeaderState(tmp, false); //��� ��ư ���� 
                    HeaderMenuController(tmp); //���� ��ư�� �����ư ���� 
                    uiManager.detailSettingCanvas_Unit.HeaderButtonClick();
                });
            }

            int contentPageNum = -1; //������ �ѹ� (��µɶ����� + 1)

            //SubMenu �̺�Ʈ ���
            for (int i = 0; i < depth__menuList.Length; i++)
            {
                //�޴��� üũ (0 : ����, 1 : ����,������� �� �Ҹ�ǰ ����, 2 : ���ɹ� ���, 3 : ���� �� ����Ž��, 4 : ����, 5 : �κ�ǰ ���� ����)
                for (int j = 0; j < depth__menuList[i].buttons.Count; j++)
                {
                    //���� ���� 
                    if (depth__menuList[i].buttons[j].hasChild)
                    {
                        int tmp, tmp_1;
                        tmp = i;
                        tmp_1 = j;

                        //��ư �̺�Ʈ ��� 
                        depth__menuList[i].buttons[j].itme_btn.onClick.AddListener(() =>
                        {
                            SubFolderController(tmp, tmp_1, false);
                        });
                    }
                    //������ ���� 
                    else
                    {
                        int tmp, tmp_1;
                        tmp = i;
                        tmp_1 = j;

                        //������ �ѹ� ī��Ʈ 
                        ++contentPageNum; //0���� ���� 
                        int tmp_2;
                        tmp_2 = contentPageNum;

                        //��ư �̺�Ʈ ��� 
                        depth__menuList[i].buttons[j].itme_btn.onClick.AddListener(() =>
                        {
                            SubMenuController(tmp, tmp_1, false); //���� ��ư ���� ������ ��ư ���� �ʱ�ȭ 

                            uiManager.detailSettingCanvas_Unit.contentListSetting.ContentButtonClick(tmp_2);
                            uiManager.detailSettingCanvas_Unit.ContentShow();
                        });
                    }
                }
            }
        }
    }

    //Ŭ���� Header ��ư ���� ����
    void HeaderState(int num, bool autoClick)
    {
        uiManager.DetailPageReset();

        clickMenu_num = num; //Ŭ���� ��� ��ȣ ��� 

        for (int i = 0; i < header_items.Count; i++)
        {
            if (i == num)
            {
                if(autoClick)
                    header_items[i].ButtonClickEvent();
                continue;
            }
            StartCoroutine(header_items[i].SetDefault(true));
        }

        //���� �ѹ� �ʱ�ȭ 
        folderNum_1 = -1;
        folderNum_2 = -1; 
    }

    //���� ��Ȱ��ȭ 
    public void SubMenuButtonInit()
    {
        for (int i = 0; i < depth__menuList.Length; i++)
        {
            for (int j = 0; j < depth__menuList[i].buttons.Count; j++)
            {
                depth__menuList[i].buttons[j].isSearchCall = false;
                depth__menuList[i].buttons[j].gameObject.SetActive(false);
            }
        }
    }

    //��� �޴� ���� 
    public void HeaderMenuController(int menu_num)
    {
        SubMenuButtonInit();

        if (oper == Operation.Field)
        {
            uiManager.detailSettingCanvas_Field.InputFieldInit(); //�˻� �ʱ�ȭ 

            for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
            {
                if (depth__menuList[menu_num].buttons[j].isSub_1depth && !depth__menuList[menu_num].buttons[j].isSub_2depth && !depth__menuList[menu_num].buttons[j].isSub_3depth)
                {
                    if (depth__menuList[menu_num].buttons[j].gameObject.activeInHierarchy)
                    {
                        StartCoroutine(depth__menuList[menu_num].buttons[j].SetDefault(true));
                    }
                    else
                    {
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(true);
                    }
                }
                else
                {
                    depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                }
            }
        }
        else if (oper == Operation.Unit)
        {
            uiManager.detailSettingCanvas_Unit.InputFieldInit(); //�˻� �ʱ�ȭ 

            for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
            {
                if (depth__menuList[menu_num].buttons[j].isSub_1depth && !depth__menuList[menu_num].buttons[j].isSub_2depth && !depth__menuList[menu_num].buttons[j].isSub_3depth)
                {
                    if (depth__menuList[menu_num].buttons[j].gameObject.activeInHierarchy)
                    {
                        StartCoroutine(depth__menuList[menu_num].buttons[j].SetDefault(true));
                    }
                    else
                    {
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(true);
                    }
                }
                else
                {
                    depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                }
            }
        }
    }

    //������ ����, ���� ��ư ���� ��� �⺻����, ��! ������ ������ ������ư�� ���� ó�� (1,2 ���� ������)
    public void SubMenuController(int menu_num, int click_num, bool autoClick)
    {
        //Debug.Log("menu_num : " + menu_num + " / click_num : " + click_num);
        if (autoClick)
            depth__menuList[menu_num].buttons[click_num].ButtonClickEvent();

        //2depth ������ ��ư�� ��� ������ư�� Ȱ��ȭ ó�� �ʿ� 
        if (depth__menuList[menu_num].buttons[click_num].isSub_1depth && depth__menuList[menu_num].buttons[click_num].isSub_2depth)
        {
            for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
            {
                //��ư ���� ��ȭ 
                //1depth ���� ���� 
                if (folderNum_1 != -1)
                {
                    if (j == folderNum_1)
                    {
                        continue;
                    }
                }

                //2depth ���� ���� 
                if (folderNum_2 != -1)
                {
                    if (j == folderNum_2)
                    {
                        continue;
                    }
                }

                //���� ��ư ���� 
                if (j == click_num)
                    continue;
                else
                {
                    StartCoroutine(depth__menuList[menu_num].buttons[j].SetDefault(true));
                }
            }
        }
        //1depth ������ ��ư�� ��� 
        else
        {
            for (int i = 0; i < depth__menuList[menu_num].buttons.Count; i++)
            {
                //���� ��ư ���� 
                if (i == click_num)
                    continue;
                else
                {
                    StartCoroutine(depth__menuList[menu_num].buttons[i].SetDefault(true));
                }

                //�����ȿ� ����� �ݾƾߵȴ�. 
                if (depth__menuList[menu_num].buttons[i].isSub_1depth && depth__menuList[menu_num].buttons[i].isSub_2depth || depth__menuList[menu_num].buttons[i].isSub_3depth)
                {
                    depth__menuList[menu_num].buttons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    int folderNum_1, folderNum_2 = -1; //���� ���� ���� ��ȣ
    [SerializeField] float ����_height; //���� ��ư�� ����ó�� 
    private IEnumerator �����׽�Ʈ(int menu_num, int under_num)
    {
        yield return new WaitForSeconds(0.1f);

        //���� ��ư�� ���� ��ũ�Ѻ並 ������ ���δ�. (���Ƿ� �����Ŷ� ���߿� ���� �ʿ� �ҵ�? 
        if (depth__menuList[menu_num].buttons[under_num].item_txt.text == "����")
        {
            depth_scroll_tr.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, ����_height, 0);
        }
    }


    //������ ���� ��ư ���� (autoClick : �ܺο��� �󼼺���� �Ѿ������ True ���� �༭ �ڵ� Ȱ��ȭ ó���� �ؾߵȴ�. 
    public void SubFolderController(int menu_num, int under_num, bool autoClick)
    {
        //Debug.Log("menu_num : " + menu_num + " / under_num : " + under_num);
        if (autoClick)
            depth__menuList[menu_num].buttons[under_num].ButtonClickEvent();

        //������ ������ ����ؾߵȴ�. SubMenuController���� ��� 
        if (depth__menuList[menu_num].buttons[under_num].isSub_1depth && !depth__menuList[menu_num].buttons[under_num].isSub_2depth
        && !depth__menuList[menu_num].buttons[under_num].isSub_3depth)
        {
            folderNum_1 = under_num;
        }
        else if (depth__menuList[menu_num].buttons[under_num].isSub_1depth && depth__menuList[menu_num].buttons[under_num].isSub_2depth
           && !depth__menuList[menu_num].buttons[under_num].isSub_3depth)
        {
            folderNum_2 = under_num;
        }

        //1. ���� ��ư ���� ��� �⺻���� 
        for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
        {
            //2depth ���� ���� 
            if (folderNum_2 != -1)
            {
                //2���� ������ üũ�� �ϰ� 1������ �ѱ��. 
                if (j == folderNum_1)
                {
                    continue;
                }
            }

            //���� ���� ��ư ���� 
            if (j == under_num)
                continue;
            else
            {
                StartCoroutine(depth__menuList[menu_num].buttons[j].SetDefault(true));
            }
        }

        //1depth�� �ڽ��� �ִ� ��� 
        if (depth__menuList[menu_num].buttons[under_num].isSub_1depth && !depth__menuList[menu_num].buttons[under_num].isSub_2depth
        && !depth__menuList[menu_num].buttons[under_num].isSub_3depth && depth__menuList[menu_num].buttons[under_num].hasChild)
        {
            if (!depth__menuList[menu_num].buttons[under_num].isToggleButton) return; //��۹�ư�� �ƴϸ� ���� ��Ų��. 

            //��������
            if (!depth__menuList[menu_num].buttons[under_num].isActive)
            {
                //2, 3���� �޴� �ʱ�ȭ 
                for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth)
                    {
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    }
                    //1depth + 2depth + 3depth�� false;
                    else if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                }

                //�ش� ������ ���� 
                for (int j = under_num + 1; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    //1depth + �ڽ��� �ִ� ��츦 üũ�� break;
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && !depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild) break;

                    //1depth + 2depth + �ڽ��� �ְų� ���ų�  true; 
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(true);
                    else { };
                }
            }
            else
            {
                //2, 3���� �޴� �ʱ�ȭ 
                for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth)
                    {
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    }
                    //1depth + 2depth + 3depth�� false;
                    else if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                }

                //�ش� ������ ���� 
                for (int j = under_num + 1; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    //1depth + �ڽ��� �ִ� ��츦 üũ�� break;
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && !depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild) break;

                    //1depth + 2depth + �ڽ��� �ְų� ���ų�  true; 
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    else { };
                }
            }
        }

        //1depth + 2depth�� �ڽ��� �ִ� ��� 
        if (depth__menuList[menu_num].buttons[under_num].isSub_1depth && depth__menuList[menu_num].buttons[under_num].isSub_2depth
            && !depth__menuList[menu_num].buttons[under_num].isSub_3depth && depth__menuList[menu_num].buttons[under_num].hasChild)
        {
            if (!depth__menuList[menu_num].buttons[under_num].isToggleButton) return; //��۹�ư�� �ƴϸ� ���� ��Ų��. 

            depth_scroll_tr.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

            if (!depth__menuList[menu_num].buttons[under_num].isActive)
            {
                //3 ���� �޴� �ʱ�ȭ 
                for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                    {
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    }
                }

                //Ŭ���� �޴� ���� ã�� 
                int checknum = 0;
                for (int j = under_num; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (j != under_num)
                    {
                        //������ư�� �ƴ� �ٸ� ��ư�� �����ϱ����� üũ 
                        if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                            !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild)
                        {
                            checknum = j;
                            break;
                        }
                    }
                }

                //������ ������ ���� ó�� (break ����Ʈ�� ����)
                if (checknum == 0) checknum = depth__menuList[menu_num].buttons.Count;

                //Ŭ���� 3���� �޴��� ���� 
                for (int j = under_num; j < checknum; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(true);

                    //1depth + 2depth + 3depth�� true;
                    else if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(true);
                    else { };
                }
            }
            else
            {
                //3 ���� �޴� �ʱ�ȭ 
                for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                    {
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    }
                }

                //Ŭ���� �޴� ���� ã�� 
                int checknum = 0;
                for (int j = under_num; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (j != under_num)
                    {
                        //������ư�� �ƴ� �ٸ� ��ư�� �����ϱ����� üũ 
                        if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                            !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild)
                        {
                            checknum = j;
                            break;
                        }
                    }
                }

                //������ ������ ���� ó�� (break ����Ʈ�� ����)
                if (checknum == 0) checknum = depth__menuList[menu_num].buttons.Count;

                //Ŭ���� 3���� �޴��� ���� 
                for (int j = under_num; j < checknum; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(true);

                    //1depth + 2depth + 3depth�� true;
                    else if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    else { };
                }
            }

        }

        StartCoroutine(�����׽�Ʈ(menu_num, under_num));
    }


}
