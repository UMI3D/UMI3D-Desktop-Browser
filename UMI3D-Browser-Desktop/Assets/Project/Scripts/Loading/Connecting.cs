using BrowserDesktop.Controller;
using BrowserDesktop.Cursor;
using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public TMP_InputField login;
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
        identifier.GetIdentityAction = GetIdentifier;
        identifier.GetPinAction = GetLogin;
        identifier.ShouldDownloadLib = shouldDlLibraries;
        identifier.GetParameters = GetParameterDtos;
    }

    Launcher.Data data;

    private void Start()
    {
        UMI3DCollaborationClientServer.Instance.OnConnectionLost.AddListener(OnConnectionLost);
    }


    void GetLogin(Action<string> callback)
    {
        DisplayLoginPassword(true,false, (identity, password) => { DisplayLoginPassword(false,false); callback.Invoke(identity); });
    }

    void GetIdentifier(Action<string,string> callback)
    {
        DisplayLoginPassword(true,true,(identity,password)=> { DisplayLoginPassword(false,true); callback.Invoke(identity,password); });
    }

    void shouldDlLibraries(List<string> ids,Action<bool> callback)
    {
        if (ids.Count == 0) callback.Invoke(true);
        else DisplayAccept(ids.Count,callback);
    }

    void GetParameterDtos(FormDto form,Action<FormAnswerDto> callback)
    {
        if(form == null)
            callback.Invoke(null);
        else
        {
            debugForm(form);
            Menu.menu.RemoveAll();

            var items = form.fields.Select(f => getInteractionItem(f));
            foreach (var item in items)
            {
                Menu.menu.Add(item);
            }
            ButtonMenuItem send = new ButtonMenuItem() { Name = "Send", toggle = false };
            UnityAction<bool> action = (bool b) => {
                FormAnswerDto answer = new FormAnswerDto()
                {
                    boneType = 0,
                    hoveredObjectId = 0,
                    id = form.id,
                    toolId = 0,
                    answers = items.Where(i => i is AbstractInputMenuItem).Select(i => (i as AbstractInputMenuItem).GetParameter()).ToList(),
                };
                MenuDisplayManager.Hide(true); Menu.menu.RemoveAll(); debugForm(form); debugForm(answer); callback.Invoke(answer); CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center); };
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

    void debugForm(FormAnswerDto form)
    {
        form.answers.Select(a=>a.parameter.ToString()).Debug();
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
                b.GetParameterFunc = (x) =>
                {
                    var pararmeterDto = new ParameterSettingRequestDto()
                    {
                        toolId = dto.id,
                        id = booleanParameterDto.id,
                        parameter = x,
                        hoveredObjectId = 0
                    };
                    return pararmeterDto;
                };
                result = b;
                break;
            case FloatRangeParameterDto floatRangeParameterDto:
                var f = new FloatRangeInputMenuItem() { dto = floatRangeParameterDto, max = floatRangeParameterDto.max, min = floatRangeParameterDto.min, value = floatRangeParameterDto.value, increment = floatRangeParameterDto.increment };
                f.Subscribe((x) =>
                {
                    floatRangeParameterDto.value = x;
                }
                );
                f.GetParameterFunc = (x) =>
                {
                    var pararmeterDto = new ParameterSettingRequestDto()
                    {
                        toolId = dto.id,
                        id = floatRangeParameterDto.id,
                        parameter = x,
                        hoveredObjectId = 0
                    };
                    return pararmeterDto;
                };
                result = f;
                break;
            case EnumParameterDto<string> enumParameterDto:
                var en = new DropDownInputMenuItem() { dto = enumParameterDto, options = enumParameterDto.possibleValues };
                en.Subscribe((x) =>
                {
                    enumParameterDto.value = x;
                }
                );
                en.GetParameterFunc = (x) =>
                {
                    var pararmeterDto = new ParameterSettingRequestDto()
                    {
                        toolId = dto.id,
                        id = enumParameterDto.id,
                        parameter = x,
                        hoveredObjectId = 0
                    };
                    return pararmeterDto;
                };
                result = en;
                break;
            case StringParameterDto stringParameterDto:
                var s = new TextInputMenuItem() { dto = stringParameterDto };
                s.Subscribe((x) =>
                {
                    stringParameterDto.value = x;
                }
                );
                s.GetParameterFunc = (x) =>
                {
                    var pararmeterDto = new ParameterSettingRequestDto()
                    {
                        toolId = dto.id,
                        id = stringParameterDto.id,
                        parameter = x,
                        hoveredObjectId = 0
                    };
                    return pararmeterDto;
                };
                result = s;
                break;
            case LocalInfoRequestParameterDto localInfoRequestParameterDto:
                LocalInfoRequestInputMenuItem localReq = new LocalInfoRequestInputMenuItem() { dto = localInfoRequestParameterDto };
                localReq.Subscribe((x) =>
                {
                    localInfoRequestParameterDto.value = x;
                }
                );

                result = localReq;
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
        DisplayLoginPassword(false,true);
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


    void DisplayLoginPassword(bool active, bool askPassword, Action<string,string> callback = null) {
        CursorHandler.SetMovement(this, active ? CursorHandler.CursorMovement.Free : CursorHandler.CursorMovement.Center);
        passwordPannel.SetActive(active);
        button.onClick.RemoveAllListeners();
        password.gameObject.SetActive(askPassword);
        if(active && callback != null)
        {
            login.text = "Login";
            button.onClick.AddListener(()=> { callback.Invoke(login.text,password.text); });
        }
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
