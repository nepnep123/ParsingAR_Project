using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SubTitle
{
    public List<ItemInfo> buttons; //해당 메뉴의 버튼들 
}

public class ItemListSetting : MonoBehaviour
{
    [SerializeField] public ItemInfo Items_pre;
    [SerializeField] public Transform depth_header_tr, depth_scroll_tr;

    [HideInInspector] public ItemInfo[] Items;

    [SerializeField] public SubTitle[] depth__menuList; //메뉴의 모든 0_depth 버튼 할당 //0,1,2,3,4,5

    [SerializeField] public List<ItemInfo> header_items = new List<ItemInfo>(); //헤더 버튼 

    [Header(" [ RESOURCE ] ")]
    [SerializeField] XMLManager xmlManager;
    [SerializeField] UIManager uiManager;

    [Header(" [ ITEM SETTING ] ")]
    [SerializeField] Sprite depth_0_spr_default;
    [SerializeField] Sprite depth_0_spr_active, depth_1_spr_default, depth_1_spr_active;

    [SerializeField] int menuMaxNum; //menu 항목에 맞춰서 포문을 돌린다. 

    [SerializeField] public int clickMenu_num; //현재 클릭한 헤더 넘버 (검색하기위한 인덱스)

    enum Operation { Field, Unit };//개별로 사용되는 변수
    [SerializeField] Operation oper; 

    public void CreateTableList()
    {
        if (oper == Operation.Field)
        {
            uiManager.detailSettingCanvas_Field.contentListSetting.CreatePage(); //개수만큼 페이지 생성 

            Items = new ItemInfo[xmlManager.itemTrees_list_Field.Count];

            menuMaxNum = xmlManager.menuNum;

            int btnID = 0; //버튼마다 ID 값부여

            //생성 및 위치선정, 옵션값 부여, 스타일 설정 
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

                    header_items.Add(Items[i]); //header 추출 

                    //ID값 부여 
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

            // 각 메뉴 별로 관리하기 위한 할당 
            depth__menuList = new SubTitle[menuMaxNum + 1];

            for (int z = 0; z < depth__menuList.Length; z++)
            {
                depth__menuList[z].buttons = new List<ItemInfo>();

                for (int i = 0; i < Items.Length; i++)
                {
                    if (Items[i].menuNum == z)
                    {
                        //header는 따로 관리
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
            uiManager.detailSettingCanvas_Unit.contentListSetting.CreatePage(); //개수만큼 페이지 생성 

            Items = new ItemInfo[xmlManager.itemTrees_list_Unit.Count];

            menuMaxNum = xmlManager.menuNum;

            int btnID = 0; //버튼마다 ID 값부여

            //생성 및 위치선정, 옵션값 부여, 스타일 설정 
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

                    header_items.Add(Items[i]); //header 추출 

                    //ID값 부여 
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

            // 각 메뉴 별로 관리하기 위한 할당 
            depth__menuList = new SubTitle[menuMaxNum + 1];

            for (int z = 0; z < depth__menuList.Length; z++)
            {
                depth__menuList[z].buttons = new List<ItemInfo>();

                for (int i = 0; i < Items.Length; i++)
                {
                    if (Items[i].menuNum == z)
                    {
                        //header는 따로 관리
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

    //각 버튼마다 ID 값을 부여 해서 AR 인식파트에서 상세보기로 넘어올때 메뉴를 펼쳐준다.
    void SetButtonID()
    {
        //헤더 ID 값 부여 
        for (int i = 0; i < header_items.Count; i++)
        {
            header_items[i].id = i.ToString();
        }

        //헤더 제외 
        for (int i = 0; i < depth__menuList.Length; i++)
        {
            int temp = 0; //ID 값 부여 
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

    //버튼 클릭 이벤트 처리 
    void ButtonEventSetting()
    {
        if (oper == Operation.Field)
        {
            //Header 이벤트 등록 
            for (int i = 0; i < header_items.Count; i++)
            {
                int tmp;
                tmp = i;

                //버튼 이벤트 등록 
                header_items[i].itme_btn.onClick.AddListener(() =>
                {
                    SubMenuButtonInit(); //서브 메뉴 초기화 
                    HeaderState(tmp, false); //헤더 버튼 제어 
                    HeaderMenuController(tmp); //눌린 버튼의 서브버튼 셋팅 
                    uiManager.detailSettingCanvas_Field.HeaderButtonClick();
                });
            }

            int contentPageNum = -1; //컨텐츠 넘버 (출력될때마다 + 1)

            //SubMenu 이벤트 등록
            for (int i = 0; i < depth__menuList.Length; i++)
            {
                //메뉴별 체크 (0 : 서론, 1 : 공구,시험장비 및 소모품 물자, 2 : 수령및 취급, 3 : 시험 및 고장탐구, 4 : 정비, 5 : 부분품 도해 명세서)
                for (int j = 0; j < depth__menuList[i].buttons.Count; j++)
                {
                    //폴더 열기 
                    if (depth__menuList[i].buttons[j].hasChild)
                    {
                        int tmp, tmp_1;
                        tmp = i;
                        tmp_1 = j;

                        //버튼 이벤트 등록 
                        depth__menuList[i].buttons[j].itme_btn.onClick.AddListener(() =>
                        {
                            SubFolderController(tmp, tmp_1, false);
                        });
                    }
                    //컨텐츠 열기 
                    else
                    {
                        int tmp, tmp_1;
                        tmp = i;
                        tmp_1 = j;

                        //컨텐츠 넘버 카운트 
                        ++contentPageNum; //0부터 시작 
                        int tmp_2;
                        tmp_2 = contentPageNum;

                        //버튼 이벤트 등록 
                        depth__menuList[i].buttons[j].itme_btn.onClick.AddListener(() =>
                        {
                            SubMenuController(tmp, tmp_1, false); //눌린 버튼 제외 나머지 버튼 상태 초기화 

                            uiManager.detailSettingCanvas_Field.contentListSetting.ContentButtonClick(tmp_2);
                            uiManager.detailSettingCanvas_Field.ContentShow();
                        });
                    }
                }
            }
        }
        else if (oper == Operation.Unit)
        {
            //Header 이벤트 등록 
            for (int i = 0; i < header_items.Count; i++)
            {
                int tmp;
                tmp = i;

                //버튼 이벤트 등록 
                header_items[i].itme_btn.onClick.AddListener(() =>
                {
                    SubMenuButtonInit(); //서브 메뉴 초기화 
                    HeaderState(tmp, false); //헤더 버튼 제어 
                    HeaderMenuController(tmp); //눌린 버튼의 서브버튼 셋팅 
                    uiManager.detailSettingCanvas_Unit.HeaderButtonClick();
                });
            }

            int contentPageNum = -1; //컨텐츠 넘버 (출력될때마다 + 1)

            //SubMenu 이벤트 등록
            for (int i = 0; i < depth__menuList.Length; i++)
            {
                //메뉴별 체크 (0 : 서론, 1 : 공구,시험장비 및 소모품 물자, 2 : 수령및 취급, 3 : 시험 및 고장탐구, 4 : 정비, 5 : 부분품 도해 명세서)
                for (int j = 0; j < depth__menuList[i].buttons.Count; j++)
                {
                    //폴더 열기 
                    if (depth__menuList[i].buttons[j].hasChild)
                    {
                        int tmp, tmp_1;
                        tmp = i;
                        tmp_1 = j;

                        //버튼 이벤트 등록 
                        depth__menuList[i].buttons[j].itme_btn.onClick.AddListener(() =>
                        {
                            SubFolderController(tmp, tmp_1, false);
                        });
                    }
                    //컨텐츠 열기 
                    else
                    {
                        int tmp, tmp_1;
                        tmp = i;
                        tmp_1 = j;

                        //컨텐츠 넘버 카운트 
                        ++contentPageNum; //0부터 시작 
                        int tmp_2;
                        tmp_2 = contentPageNum;

                        //버튼 이벤트 등록 
                        depth__menuList[i].buttons[j].itme_btn.onClick.AddListener(() =>
                        {
                            SubMenuController(tmp, tmp_1, false); //눌린 버튼 제외 나머지 버튼 상태 초기화 

                            uiManager.detailSettingCanvas_Unit.contentListSetting.ContentButtonClick(tmp_2);
                            uiManager.detailSettingCanvas_Unit.ContentShow();
                        });
                    }
                }
            }
        }
    }

    //클릭한 Header 버튼 상태 변경
    void HeaderState(int num, bool autoClick)
    {
        uiManager.DetailPageReset();

        clickMenu_num = num; //클릭한 헤더 번호 기억 

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

        //폴더 넘버 초기화 
        folderNum_1 = -1;
        folderNum_2 = -1; 
    }

    //전부 비활성화 
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

    //헤더 메뉴 관리 
    public void HeaderMenuController(int menu_num)
    {
        SubMenuButtonInit();

        if (oper == Operation.Field)
        {
            uiManager.detailSettingCanvas_Field.InputFieldInit(); //검색 초기화 

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
            uiManager.detailSettingCanvas_Unit.InputFieldInit(); //검색 초기화 

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

    //컨텐츠 열기, 눌린 버튼 제외 모든 기본상태, 단! 이전에 눌럿던 폴더버튼은 제외 처리 (1,2 뎁스 폴더들)
    public void SubMenuController(int menu_num, int click_num, bool autoClick)
    {
        //Debug.Log("menu_num : " + menu_num + " / click_num : " + click_num);
        if (autoClick)
            depth__menuList[menu_num].buttons[click_num].ButtonClickEvent();

        //2depth 컨텐츠 버튼인 경우 이전버튼은 활성화 처리 필요 
        if (depth__menuList[menu_num].buttons[click_num].isSub_1depth && depth__menuList[menu_num].buttons[click_num].isSub_2depth)
        {
            for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
            {
                //버튼 상태 변화 
                //1depth 폴더 제외 
                if (folderNum_1 != -1)
                {
                    if (j == folderNum_1)
                    {
                        continue;
                    }
                }

                //2depth 폴더 제외 
                if (folderNum_2 != -1)
                {
                    if (j == folderNum_2)
                    {
                        continue;
                    }
                }

                //눌린 버튼 제외 
                if (j == click_num)
                    continue;
                else
                {
                    StartCoroutine(depth__menuList[menu_num].buttons[j].SetDefault(true));
                }
            }
        }
        //1depth 컨텐츠 버튼인 경우 
        else
        {
            for (int i = 0; i < depth__menuList[menu_num].buttons.Count; i++)
            {
                //눌린 버튼 제외 
                if (i == click_num)
                    continue;
                else
                {
                    StartCoroutine(depth__menuList[menu_num].buttons[i].SetDefault(true));
                }

                //폴더안에 얘들은 닫아야된다. 
                if (depth__menuList[menu_num].buttons[i].isSub_1depth && depth__menuList[menu_num].buttons[i].isSub_2depth || depth__menuList[menu_num].buttons[i].isSub_3depth)
                {
                    depth__menuList[menu_num].buttons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    int folderNum_1, folderNum_2 = -1; //현재 눌린 폴더 번호
    [SerializeField] float 시험_height; //시험 버튼만 예외처리 
    private IEnumerator 시험테스트(int menu_num, int under_num)
    {
        yield return new WaitForSeconds(0.1f);

        //시험 버튼인 경우는 스크롤뷰를 내려야 보인다. (임의로 내린거라 나중에 설정 필요 할듯? 
        if (depth__menuList[menu_num].buttons[under_num].item_txt.text == "시험")
        {
            depth_scroll_tr.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 시험_height, 0);
        }
    }


    //선택한 폴더 버튼 제어 (autoClick : 외부에서 상세보기로 넘어왔을때 True 값을 줘서 자동 활성화 처리를 해야된다. 
    public void SubFolderController(int menu_num, int under_num, bool autoClick)
    {
        //Debug.Log("menu_num : " + menu_num + " / under_num : " + under_num);
        if (autoClick)
            depth__menuList[menu_num].buttons[under_num].ButtonClickEvent();

        //선택한 폴더는 기억해야된다. SubMenuController에서 사용 
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

        //1. 눌린 버튼 제외 모든 기본상태 
        for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
        {
            //2depth 폴더 제외 
            if (folderNum_2 != -1)
            {
                //2뎁스 폴더면 체크만 하고 1뎁스를 넘긴다. 
                if (j == folderNum_1)
                {
                    continue;
                }
            }

            //눌린 폴더 버튼 제외 
            if (j == under_num)
                continue;
            else
            {
                StartCoroutine(depth__menuList[menu_num].buttons[j].SetDefault(true));
            }
        }

        //1depth에 자식이 있는 경우 
        if (depth__menuList[menu_num].buttons[under_num].isSub_1depth && !depth__menuList[menu_num].buttons[under_num].isSub_2depth
        && !depth__menuList[menu_num].buttons[under_num].isSub_3depth && depth__menuList[menu_num].buttons[under_num].hasChild)
        {
            if (!depth__menuList[menu_num].buttons[under_num].isToggleButton) return; //토글버튼이 아니면 리턴 시킨다. 

            //열려있음
            if (!depth__menuList[menu_num].buttons[under_num].isActive)
            {
                //2, 3뎁스 메뉴 초기화 
                for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth)
                    {
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    }
                    //1depth + 2depth + 3depth는 false;
                    else if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                }

                //해당 페이지 열기 
                for (int j = under_num + 1; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    //1depth + 자식이 있는 경우를 체크해 break;
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && !depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild) break;

                    //1depth + 2depth + 자식이 있거나 없거나  true; 
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(true);
                    else { };
                }
            }
            else
            {
                //2, 3뎁스 메뉴 초기화 
                for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth)
                    {
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    }
                    //1depth + 2depth + 3depth는 false;
                    else if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                }

                //해당 페이지 열기 
                for (int j = under_num + 1; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    //1depth + 자식이 있는 경우를 체크해 break;
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && !depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild) break;

                    //1depth + 2depth + 자식이 있거나 없거나  true; 
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    else { };
                }
            }
        }

        //1depth + 2depth에 자식이 있는 경우 
        if (depth__menuList[menu_num].buttons[under_num].isSub_1depth && depth__menuList[menu_num].buttons[under_num].isSub_2depth
            && !depth__menuList[menu_num].buttons[under_num].isSub_3depth && depth__menuList[menu_num].buttons[under_num].hasChild)
        {
            if (!depth__menuList[menu_num].buttons[under_num].isToggleButton) return; //토글버튼이 아니면 리턴 시킨다. 

            depth_scroll_tr.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

            if (!depth__menuList[menu_num].buttons[under_num].isActive)
            {
                //3 뎁스 메뉴 초기화 
                for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                    {
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    }
                }

                //클릭한 메뉴 범위 찾기 
                int checknum = 0;
                for (int j = under_num; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (j != under_num)
                    {
                        //누른버튼이 아닌 다른 버튼을 제어하기위해 체크 
                        if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                            !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild)
                        {
                            checknum = j;
                            break;
                        }
                    }
                }

                //마지막 순서는 예외 처리 (break 포인트가 없다)
                if (checknum == 0) checknum = depth__menuList[menu_num].buttons.Count;

                //클릭한 3뎁스 메뉴만 열기 
                for (int j = under_num; j < checknum; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(true);

                    //1depth + 2depth + 3depth는 true;
                    else if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(true);
                    else { };
                }
            }
            else
            {
                //3 뎁스 메뉴 초기화 
                for (int j = 0; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                    {
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    }
                }

                //클릭한 메뉴 범위 찾기 
                int checknum = 0;
                for (int j = under_num; j < depth__menuList[menu_num].buttons.Count; j++)
                {
                    if (j != under_num)
                    {
                        //누른버튼이 아닌 다른 버튼을 제어하기위해 체크 
                        if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                            !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild)
                        {
                            checknum = j;
                            break;
                        }
                    }
                }

                //마지막 순서는 예외 처리 (break 포인트가 없다)
                if (checknum == 0) checknum = depth__menuList[menu_num].buttons.Count;

                //클릭한 3뎁스 메뉴만 열기 
                for (int j = under_num; j < checknum; j++)
                {
                    if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        !depth__menuList[menu_num].buttons[j].isSub_3depth && depth__menuList[menu_num].buttons[j].hasChild)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(true);

                    //1depth + 2depth + 3depth는 true;
                    else if (depth__menuList[menu_num].buttons[j].isSub_1depth && depth__menuList[menu_num].buttons[j].isSub_2depth &&
                        depth__menuList[menu_num].buttons[j].isSub_3depth)
                        depth__menuList[menu_num].buttons[j].gameObject.SetActive(false);
                    else { };
                }
            }

        }

        StartCoroutine(시험테스트(menu_num, under_num));
    }


}
