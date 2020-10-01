using BrowserDesktop.Controller;
using BrowserDesktop.Cursor;
using System;
using System.Collections.Generic;
using TMPro;
using umi3d;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Connecting : Singleton<Connecting>
{
    public GameObject passwordPannel;
    public TMP_InputField password;
    public Button button;

    public DialogBox DialogBoxPrefab;

    public ClientPCIdentifier identifier;

    public LoadingScreen LoadingScreen;

    public MenuAsset Menu;
    public MenuDisplayManager MenuDisplayManager;

    public string scene;
    public string thisScene;

    protected override void Awake()
    {
        base.Awake();
        passwordPannel.SetActive(false);
        identifier.GetPasswordAction = GetPassword;
        identifier.ShouldDownloadLib = shouldDlLibraries;
        identifier.GetParameters = GetParameterDtos;
    }

    Launcher.Data data;

    private void Start()
    {
        UMI3DCollaborationClientServer.Instance.OnConnectionLost.AddListener(OnConnectionLost);
    }


    void GetPassword(Action<string> callback)
    {
        DisplayLoginPassword(true,(s)=> { DisplayLoginPassword(false); callback.Invoke(s); });
    }

    void shouldDlLibraries(List<string> ids,Action<bool> callback)
    {
        if (ids.Count == 0) callback.Invoke(true);
        else DisplayAccept(ids.Count,callback);
    }

    void GetParameterDtos(FormDto form,Action<FormDto> callback)
    {
        if(form == null)
            callback.Invoke(form);
        else
        {
            debugForm(form);
            Menu.menu.RemoveAll();
            foreach(var param in form.Fields)
            {
                Menu.menu.Add(getInteractionItem(param));
            }
            ButtonMenuItem send = new ButtonMenuItem() { Name = "Send", toggle = false };
            UnityAction<bool> action = (bool b) => { MenuDisplayManager.Hide(true); Menu.menu.RemoveAll(); debugForm(form); callback.Invoke(form); CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center); };
            send.Subscribe(action);
            Menu.menu.Add(send);
            MenuDisplayManager.Display(true);
            CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
        }
    }

    void debugForm(FormDto form)
    {
        //foreach(var dto in form.interactions)
        //    switch (dto)
        //    {
        //        case BooleanParameterDto booleanParameterDto:
        //            Debug.Log(booleanParameterDto.value);
        //            break;
        //        case FloatRangeParameterDto floatRangeParameterDto:
        //            Debug.Log(floatRangeParameterDto.value);
        //            break;
        //        case EnumParameterDto<string> enumParameterDto:
        //            Debug.Log(enumParameterDto.value);
        //            break;
        //        case StringParameterDto stringParameterDto:
        //            Debug.Log(stringParameterDto.value);
        //            break;
        //        default:
        //            Debug.Log(dto);
        //            break;
        //    }
        }

    static MenuItem getInteractionItem(AbstractInteractionDto dto)
    {
        MenuItem result = null;
        switch (dto)
        {
            case BooleanParameterDto booleanParameterDto:
                var b = new BooleanInputMenuItem() { dto = booleanParameterDto };
                b.Subscribe((x) =>
                {
                    booleanParameterDto.value = x;
                }
                );
                result = b;
                break;
            case FloatRangeParameterDto floatRangeParameterDto:
                var f = new FloatRangeInputMenuItem() { dto = floatRangeParameterDto, max = floatRangeParameterDto.Max, min = floatRangeParameterDto.Min, value = floatRangeParameterDto.value, increment = floatRangeParameterDto.Increment };
                f.Subscribe((x) =>
                {
                    floatRangeParameterDto.value = x;
                }
                );
                result = f;
                break;
            case EnumParameterDto<string> enumParameterDto:
                var en = new DropDownInputMenuItem() { dto = enumParameterDto, options = enumParameterDto.PossibleValues };
                en.Subscribe((x) =>
                {
                    enumParameterDto.value = x;
                }
                );
                result = en;
                break;
            case StringParameterDto stringParameterDto:
                var s = new TextInputMenuItem() { dto = stringParameterDto };
                s.Subscribe((x) =>
                {
                    stringParameterDto.value = x;
                }
                );
                result = s;
                break;
            default:
                result = new MenuItem();
                result.Subscribe(() => Debug.Log("hellooo 2"));
                break;
        }
        result.Name = dto.name;
        //icon;
        return result;
    }

    public void Connect(Launcher.Data data)
    {
        this.data = data;
        LoadingScreen.OnProgressChange(0);
        DisplayLoginPassword(false);
        string url = "http://" + data.ip + ":" + data.port + UMI3DNetworkingKeys.media;
        UMI3DCollaborationClientServer.GetMedia(url, GetMediaSucces, GetMediaFailed);
    }

    void GetMediaFailed(string error) 
    {
        var d = Instantiate(DialogBoxPrefab, transform);
        d.Setup($"Server Unreachable", "Leave or Leave ?", "Leave", "also Leave", (b) => {
            Leave();
            Destroy(d.gameObject);
        });
    }

    void GetMediaSucces(MediaDto media)
    {
        UMI3DCollaborationClientServer.Connect();
    }


    void DisplayLoginPassword(bool active, Action<string> callback = null) {
        CursorHandler.SetMovement(this, active ? CursorHandler.CursorMovement.Free : CursorHandler.CursorMovement.Center);
        passwordPannel.SetActive(active);
        button.onClick.RemoveAllListeners();
        if(callback != null)
            button.onClick.AddListener(()=> { callback.Invoke(password.text); });
    }

    void DisplayAccept(int count, Action<bool> callback)
    {
        var d = Instantiate(DialogBoxPrefab, transform);
        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
        d.Setup($"{count} AssetLibrary are needed.", "Download libraries and connect to the server ?", "Accept","Denied",(b)=> {
            callback.Invoke(b);
            Destroy(d.gameObject);
            CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
        });
    }

    void OnConnectionLost() {
        MouseAndKeyboardController.CanProcess = false;
        Action<bool> callback = (b) => {
            if (b) Connect(data);
            else Leave();
            MouseAndKeyboardController.CanProcess = true;
        };
        OnConnectionLost(callback);
    }

    void OnConnectionLost(Action<bool> callback)
    {
        var d = Instantiate(DialogBoxPrefab, transform);
        d.Setup($"Connection to the server lost", "Leave to the connection menu or try again ?", "Leave", "Try Again", (b) => {
            callback.Invoke(b);
            Destroy(d.gameObject);
        });
    }

    public void Leave()
    {
        UMI3DEnvironmentLoader.Clear();
        UMI3DResourcesManager.Instance.ClearCache();
        UMI3DCollaborationClientServer.Logout(()=> { GameObject.Destroy(UMI3DClientServer.Instance.gameObject); },null);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }


}
