using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UnderEquipInfos
{
    public ModelingTouchModule modeling_pre;
    [HideInInspector] public ModelingTouchModule modeling;

    public string headerTitle;

    [TextArea] public string contentTitle;
    public float height; //dm에 연결될때 컨텐츠 높이 
}

public class ARSettingCanvas : MonoBehaviour
{
    //[Header("[ HUMVEE(전시용) ] ")]
    //[SerializeField] public InfoPopupController humvee_info;
    //[SerializeField] public TargetModelingSetting humvee_model_pre;
    //[HideInInspector] public TargetModelingSetting humvee_model;

    //[SerializeField] public UnderEquipInfoPopup humvee_under_info;
    //[SerializeField] public UnderEquipInfos[] humvee_underEquips;

    [Header("[ CHILD RESOURCE ] ")]
    [SerializeField] public InfoPopupController transceiver_info;
    [SerializeField] public InfoPopupController antena_info, powerSupply_info;

    [Header("[ TARGET MODEL ] ")]
    [SerializeField] public RectTransform pre_tr; //처음 생성될 위치 
    [SerializeField] public TargetModelingSetting transceiver_model_pre;
    [SerializeField] public TargetModelingSetting antena_model_pre, powerSupply_model_pre;

    //동적 생성
    [HideInInspector] public TargetModelingSetting transceiver_model;
    [HideInInspector] public TargetModelingSetting antena_model, powerSupply_model;

    [Header("[ UNDER EQUIP MODEL ] ")]
    //하부 구성품
    [SerializeField] public UnderEquipInfoPopup transceiver_under_info;
    [SerializeField] public UnderEquipInfoPopup antena_under_info;
    [SerializeField] public UnderEquipInfoPopup powerSupply_under_info;
    //활성화 비활성화 
    [SerializeField] public UnderEquipInfos[] transceiver_underEquips;
    [SerializeField] public UnderEquipInfos[] antena_underEquips;
    [SerializeField] public UnderEquipInfos[] powerSupply_underEquips;

    [Header("[ USE RESOURCE ] ")]
    [SerializeField] public TargetModelingSetting selected_model; //타겟된 모델링 
    [SerializeField] public ModelingTouchModule selected_underModel; //생성된 하부 구성품 모델링 

    [SerializeField] public ButtonController backgroundOn_btn, backgroundOff_btn, reset_btn;
    [SerializeField] public GameObject background;

    [SerializeField] public GizmoController gizmoController;

    [HideInInspector] public UIManager UM;
    [HideInInspector] public bool isPlaying = false; 

    private void Awake()
    {
        UM = FindObjectOfType<UIManager>();

        gizmoController.gameObject.SetActive(false);
    }

    #region INIT

    public void EquipInfoInit()
    {
        transceiver_info.gameObject.SetActive(false);
        antena_info.gameObject.SetActive(false);
        powerSupply_info.gameObject.SetActive(false);

        //if (UM.isHW) humvee_info.gameObject.SetActive(false);
    }

    public void EquipInit()
    {
        transceiver_info.gameObject.SetActive(false);
        antena_info.gameObject.SetActive(false);
        powerSupply_info.gameObject.SetActive(false);
        //if (UM.isHW) humvee_info.gameObject.SetActive(false);

        if (transceiver_model != null)
            transceiver_model.gameObject.SetActive(false);

        if (antena_model != null)
            antena_model.gameObject.SetActive(false);

        if (powerSupply_model != null)
            powerSupply_model.gameObject.SetActive(false);

        //if (UM.isHW)
        //{
        //    if (humvee_model != null)
        //        humvee_model.gameObject.SetActive(false);
        //}
    }

    public void UnderEquipInit()
    {
        transceiver_under_info.gameObject.SetActive(false);
        antena_under_info.gameObject.SetActive(false);
        powerSupply_under_info.gameObject.SetActive(false);

        //if (UM.isHW) humvee_under_info.gameObject.SetActive(false);

        for (int i = 0; i < transceiver_underEquips.Length; i++)
        {
            if (transceiver_underEquips[i].modeling != null)
                transceiver_underEquips[i].modeling.gameObject.SetActive(false);
        }

        for (int i = 0; i < antena_underEquips.Length; i++)
        {
            if (antena_underEquips[i].modeling != null)
                antena_underEquips[i].modeling.gameObject.SetActive(false);
        }

        for (int i = 0; i < powerSupply_underEquips.Length; i++)
        {
            if (powerSupply_underEquips[i].modeling != null)
                powerSupply_underEquips[i].modeling.gameObject.SetActive(false);
        }

        //if (UM.isHW)
        //{
        //    for (int i = 0; i < humvee_underEquips.Length; i++)
        //    {
        //        if (humvee_underEquips[i].modeling != null)
        //            humvee_underEquips[i].modeling.gameObject.SetActive(false);
        //    }
        //}
    }

    #endregion


    private void OnEnable()
    {
        //카메라 셋팅 
        UM.CameraSetting(UM.arSettingCanvas.gameObject);
        //UM.modelCamera.gameObject.SetActive(true);
        //UM.modelCamera_detail.gameObject.SetActive(false);
        //UM.modelCamera_detail_icon.gameObject.SetActive(false);

        if (selected_model != null) gizmoController.gameObject.SetActive(true);

        BackgroundController(false);
    }

    private void OnDisable()
    {
        if (!isPlaying)
        {
            selected_model = null;
            selected_underModel = null;
        }
    }

    public void SetTargetInfomation(Type type)
    {
        EquipInit();
        UnderEquipInit();

        switch (type)
        {
            case Type.TRANSCEIVER:
                if(transceiver_model == null)
                    transceiver_model = Instantiate(transceiver_model_pre, pre_tr);

                selected_model = transceiver_model;
                gizmoController.gameObject.SetActive(true);
                gizmoController.targetModel_tr = selected_model.transform;

                selected_model.GetComponent<ModelingTouchModule>().modelCamera = UM.modelCamera;
                selected_model.gameObject.SetActive(true);
                selected_model.equip_anim.gameObject.SetActive(true);
                selected_model.InteractionEnd();
                
                transceiver_info.gameObject.SetActive(true);
                transceiver_info.SetDefault();
                transceiver_model.ForceQuit();
                break;

            case Type.ANTENA:
                if (antena_model == null)
                    antena_model = Instantiate(antena_model_pre, pre_tr);

                selected_model = antena_model;
                gizmoController.gameObject.SetActive(true);
                gizmoController.targetModel_tr = selected_model.transform;

                selected_model.GetComponent<ModelingTouchModule>().modelCamera = UM.modelCamera;
                selected_model.gameObject.SetActive(true);
                selected_model.equip_anim.gameObject.SetActive(true);
                selected_model.InteractionEnd();

                antena_info.gameObject.SetActive(true);
                antena_info.SetDefault();
                antena_model.ForceQuit();
                break;

            case Type.POWERSUPPLY:
                if (powerSupply_model == null)
                    powerSupply_model = Instantiate(powerSupply_model_pre, pre_tr);

                selected_model = powerSupply_model;
                gizmoController.gameObject.SetActive(true);
                gizmoController.targetModel_tr = selected_model.transform;

                selected_model.GetComponent<ModelingTouchModule>().modelCamera = UM.modelCamera;
                selected_model.gameObject.SetActive(true);
                selected_model.equip_anim.gameObject.SetActive(true);
                selected_model.InteractionEnd();

                powerSupply_info.gameObject.SetActive(true);
                powerSupply_info.SetDefault();
                powerSupply_model.ForceQuit();
                break;

            //    //HW 전시용
            //case Type.HW:
            //    if (UM.isHW)
            //    {
            //        if (humvee_model == null)
            //            humvee_model = Instantiate(humvee_model_pre, pre_tr);

            //        selected_model = humvee_model;
            //        gizmoController.gameObject.SetActive(true);
            //        gizmoController.targetModel_tr = selected_model.transform;

            //        selected_model.GetComponent<ModelingTouchModule>().modelCamera = UM.modelCamera;
            //        selected_model.gameObject.SetActive(true);
            //        selected_model.equip_anim.gameObject.SetActive(true);
            //        selected_model.InteractionEnd();

            //        humvee_info.gameObject.SetActive(true);
            //        humvee_info.SetDefault();
            //        humvee_model.ForceQuit();
            //    }
            //    break;

            default:
                selected_model = null;
                break;
        }
    }

    public void BackButtonClick()
    {
        UnderEquipInit();
        if (selected_model != null)
        {
            gizmoController.targetModel_tr = selected_model.transform;

            selected_model.touchModule.ResetTransform();
            selected_model.equip_anim.gameObject.SetActive(true);
            selected_model.equipInfoImages.SetActive(true);
            selected_model.UnderEquipInteractionStart();

            if (selected_model == transceiver_model)
            {
                transceiver_info.gameObject.SetActive(true);
            }
            else if (selected_model == antena_model)
            {
                antena_info.gameObject.SetActive(true);
            }
            else if (selected_model == powerSupply_model)
            {
                powerSupply_info.gameObject.SetActive(true);
            }

            //if (UM.isHW)
            //{
            //    if (selected_model == humvee_model)
            //    {
            //        humvee_info.gameObject.SetActive(true);
            //    }
            //}
        }
    }

    public void SetUnderTargetInfomation(int underNum)
    {
        UnderEquipInit();
        if (selected_model != null)
        {
            if (selected_model == transceiver_model)
            {
                if (transceiver_underEquips[underNum].modeling == null)
                    transceiver_underEquips[underNum].modeling = Instantiate(transceiver_underEquips[underNum].modeling_pre, pre_tr);

                selected_underModel = transceiver_underEquips[underNum].modeling;
                gizmoController.targetModel_tr = selected_underModel.transform;

                selected_underModel.gameObject.SetActive(true);
                selected_underModel.modelCamera = UM.modelCamera;

                transceiver_under_info.gameObject.SetActive(true);
                transceiver_under_info.underNum = underNum;
                UnderInfoPopupSetting(transceiver_underEquips[underNum].headerTitle, transceiver_underEquips[underNum].contentTitle);
            }
            else if (selected_model == antena_model)
            {
                if (antena_underEquips[underNum].modeling == null)
                    antena_underEquips[underNum].modeling = Instantiate(antena_underEquips[underNum].modeling_pre, pre_tr);

                selected_underModel = antena_underEquips[underNum].modeling;
                gizmoController.targetModel_tr = selected_underModel.transform;

                selected_underModel.gameObject.SetActive(true);
                selected_underModel.modelCamera = UM.modelCamera;

                antena_under_info.gameObject.SetActive(true);
                antena_under_info.underNum = underNum;
                UnderInfoPopupSetting(antena_underEquips[underNum].headerTitle, antena_underEquips[underNum].contentTitle);
            }
            else if (selected_model == powerSupply_model)
            {
                if (powerSupply_underEquips[underNum].modeling == null)
                    powerSupply_underEquips[underNum].modeling = Instantiate(powerSupply_underEquips[underNum].modeling_pre, pre_tr);

                selected_underModel = powerSupply_underEquips[underNum].modeling;
                gizmoController.targetModel_tr = selected_underModel.transform;

                selected_underModel.gameObject.SetActive(true);
                selected_underModel.modelCamera = UM.modelCamera;

                powerSupply_under_info.gameObject.SetActive(true);
                powerSupply_under_info.underNum = underNum;
                UnderInfoPopupSetting(powerSupply_underEquips[underNum].headerTitle, powerSupply_underEquips[underNum].contentTitle);
            }
            else
            {
                Debug.Log("상위 모델링 리소스가 없습니다.");
            }

            //if (UM.isHW)
            //{
            //    if (selected_model == humvee_model)
            //    {
            //        if (humvee_underEquips[underNum].modeling == null)
            //            humvee_underEquips[underNum].modeling = Instantiate(humvee_underEquips[underNum].modeling_pre, pre_tr);

            //        selected_underModel = humvee_underEquips[underNum].modeling;
            //        gizmoController.targetModel_tr = selected_underModel.transform;

            //        selected_underModel.gameObject.SetActive(true);
            //        selected_underModel.modelCamera = UM.modelCamera;

            //        humvee_under_info.gameObject.SetActive(true);
            //        humvee_under_info.underNum = underNum;
            //        UnderInfoPopupSetting(humvee_underEquips[underNum].headerTitle, humvee_underEquips[underNum].contentTitle);
            //    }
            //}
        }
    }

    void UnderInfoPopupSetting(string header_txt, string content_txt)
    {
        if (selected_model != null)
        {
            if (selected_model == transceiver_model)
            {
                transceiver_under_info.header_Text.text = header_txt;
                transceiver_under_info.content_Text.text = content_txt;
                //transceiver_under_info.dmLinkBtn.dmName = height;
            }
            else if (selected_model == antena_model)
            {
                antena_under_info.header_Text.text = header_txt;
                antena_under_info.content_Text.text = content_txt;
                //antena_under_info.dmLinkBtn.dmName = height;
            }
            else if (selected_model == powerSupply_model)
            {
                powerSupply_under_info.header_Text.text = header_txt;
                powerSupply_under_info.content_Text.text = content_txt;
                //powerSupply_under_info.dmLinkBtn.dmName = height;
            }
            else
            {
                Debug.Log("상위 모델링 리소스가 없습니다.");
            }

            //if (UM.isHW)
            //{
            //    if (selected_model == humvee_model)
            //    {
            //        humvee_under_info.header_Text.text = header_txt;
            //        humvee_under_info.content_Text.text = content_txt;
            //        //powerSupply_under_info.dmLinkBtn.dmName = height;
            //    }
            //}
        }
    }


    //background 제어 true : 켬, false : 끔
    public void BackgroundController(bool boolean)
    {
        backgroundOn_btn.gameObject.SetActive(!boolean);
        backgroundOff_btn.gameObject.SetActive(boolean);

        background.SetActive(boolean);
    }

    public void ResetButtonClick()
    {
        if(selected_model != null && selected_model.equip_anim.gameObject.activeInHierarchy)
        {
            selected_model.touchModule.ResetTransform();
        }
        else if(selected_underModel != null && selected_underModel.gameObject.activeInHierarchy)
        {
            selected_underModel.ResetTransform();
        }
    }
}
