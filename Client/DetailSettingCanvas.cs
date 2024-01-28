using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetailSettingCanvas : MonoBehaviour
{
    //sdsdsd
    [Header(" [ BUTTON ] ")] 
    [SerializeField] public ButtonController search_btn;
    [SerializeField] public ButtonController close_btn;

    [Header("[ RESOURCE ] ")]
    [SerializeField] public UIManager UM;
    [SerializeField] public ItemListSetting itemListSetting;
    [SerializeField] public ContentListSetting contentListSetting;
    [SerializeField] public ObjectListSetting objectListSetting;

    [Header("[ CONTROL BACKGROUND ] ")]
    [SerializeField] public GameObject main_bg; //0_depth버튼 누르기전에 활성화 
    [SerializeField] public GameObject content_bg; //1_2_depth 버튼 누르기전에 활성화 
    [SerializeField] public GameObject contentArea_bg; //컨텐츠 영역 배경 

    [SerializeField] public TMP_InputField search_Inputfield;
    
    [Header("[ FULL MODE CONTROL RESOURCE ] ")]
    [SerializeField] public GameObject content_obj; //확대/축소 버튼 눌렀을때 제어 
    [SerializeField] public GameObject fullScreenBackground_obj;
    [SerializeField] public RectTransform figureResource, 도해구성품;

    enum Operation { Field, Unit }; //개별로 사용되는 변수
    [SerializeField] Operation oper; 

    private void Start()
    {
        UM = FindObjectOfType<UIManager>();

        //확대/축소 버튼 셋팅
        objectListSetting.FullModeButtonSetting();
    }

    private void OnEnable()
    {
        //카메라 셋팅 
        UM.CameraSetting(gameObject);
        //UM.modelCamera.gameObject.SetActive(false);
        //UM.modelCamera_detail.gameObject.SetActive(true);
        //UM.modelCamera_detail_icon.gameObject.SetActive(true);

        main_bg.SetActive(true);
        content_bg.SetActive(true);

        itemListSetting.clickMenu_num = -1; //헤더 클릭 안한 상태 

        FullScreenOnSetting();
    }

    #region INIT
    void InitIntroButton()
    {
        search_btn.SetDefault();
        close_btn.SetDefault();
    }
    #endregion


    #region BUTTON EVENT

    public void ButtonClickEvent(ButtonController button)
    {
        InitIntroButton();

        if (button == search_btn)
        {
            button.ButtonClickEvent();

            //구현 필요
        }
        else if (button == close_btn)
        {
            button.ButtonClickEvent();

            UM.operation = UIManager.Operation.None;

            //AR 인식 타겟팅이 있는 상태에서 버튼을 눌렀을때 
            if (UM.arSettingCanvas.isPlaying)
            {
                UM.InitialState();
                UM.InitCamera();
                UM.ar_btn.ButtonClickEvent(); //클릭처리 

                UM.arSettingCanvas.gameObject.SetActive(true);
            }
            //AR 인식 타겟팅이 없는 상태에서 버튼을 눌렀을때
            else
            {
                UM.InitialState();
                UM.InitCamera();
            }
        }
    }

    #endregion

    //컨텐츠 버튼 눌렀을때 
    public void ContentShow()
    {
        //UM.modelCamera_detail.gameObject.SetActive(true);
        //UM.modelCamera_detail_icon.gameObject.SetActive(true);

        main_bg.SetActive(false);
        content_bg.SetActive(false);
    }

    //헤더 버튼 눌렀을때 (컨텐츠 시작)
    public void HeaderButtonClick()
    {
        main_bg.SetActive(false);
        content_bg.SetActive(true);

        FullScreenOffSetting();
        objectListSetting.setFullMode_TB.gameObject.SetActive(false);
    }

    #region 검색 기능

    //인풋필드 눌럿을때 현재 적힌 얘들 삭제 
    public void InputFieldInit()
    {
        search_Inputfield.text = "";
    }

    //검색 버튼 클릭 (이벤트)
    public void SearchMenuItem()
    {
        if (itemListSetting.clickMenu_num == -1) return;

        var search_split = search_Inputfield.text.ToString();

        //검색한 기록이 없는 상태로 클릭했을때 
        if (search_split.Length == 0)
            itemListSetting.HeaderMenuController(itemListSetting.clickMenu_num);
        else
        {
            //버튼 아이템 개수
            for (int i = 0; i < itemListSetting.depth__menuList[itemListSetting.clickMenu_num].buttons.Count; i++)
            {
                itemListSetting.depth__menuList[itemListSetting.clickMenu_num].buttons[i].isSearchCall = true;

                string items_sp = itemListSetting.depth__menuList[itemListSetting.clickMenu_num].buttons[i].item_txt.text.ToString();
                bool trigger = false;

                for (int j = 0; j < items_sp.Length - (search_split.Length - 1); j++)
                {
                    string test = "";

                    for (int z = j; z < j + search_split.Length; z++)
                    {
                        test += items_sp[z];
                    }

                    if (test == search_split)
                    {
                        //합격 출력
                        if (!itemListSetting.depth__menuList[itemListSetting.clickMenu_num].buttons[i].hasChild)
                            itemListSetting.depth__menuList[itemListSetting.clickMenu_num].buttons[i].gameObject.SetActive(true);
                        else
                            itemListSetting.depth__menuList[itemListSetting.clickMenu_num].buttons[i].gameObject.SetActive(false);

                        trigger = true;
                        break;
                    }
                }

                if (!trigger)
                {
                    itemListSetting.depth__menuList[itemListSetting.clickMenu_num].buttons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    #endregion

    #region FULL SCREEN CONTROL
    public void FullScreenOnSetting()
    {
        //컨텐츠 영역 확장 (컨텐츠 텍스트 영역 비활성화)
        content_obj.SetActive(false);
        contentArea_bg.SetActive(false);
        fullScreenBackground_obj.SetActive(true);

        //사용중인 이미지, 모델링(리소스) 중앙 정렬 및 사이즈 확장
        UM.modelCamera_detail.rect = new Rect(0.21f, 0, 1, 0.9f);
        figureResource.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        //도해구성품.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    public void FullScreenOffSetting()
    {
        //컨텐츠 영역 축소 (컨텐츠 텍스트 영역 활성화)
        content_obj.SetActive(true);
        contentArea_bg.SetActive(true);
        fullScreenBackground_obj.SetActive(false);

        //사용중인 이미지, 모델링(리소스) 위치 및 사이즈 복귀(디폴트 값)
        UM.modelCamera_detail.rect = new Rect(0.62f, 0, 0.38f, 0.9f);
        figureResource.localScale = new Vector3(1, 1, 1);
        //도해구성품.localScale = new Vector3(1, 1, 1);

    }
    #endregion

}
