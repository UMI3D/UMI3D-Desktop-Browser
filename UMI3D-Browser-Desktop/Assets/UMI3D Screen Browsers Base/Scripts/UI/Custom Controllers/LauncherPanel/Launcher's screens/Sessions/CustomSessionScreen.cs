/*
Copyright 2019 - 2022 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using BeardedManStudios.Forge.Networking;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomSessionScreen : CustomMenuScreen
{
    public new class UxmlTraits : CustomMenuScreen.UxmlTraits
    {
        

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomSessionScreen;

            custom.Set();
        }
    }

    public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/sessionScreen";
    public override string USSCustomClassName => "session-screen";
    public virtual string USSCustomClassPin => $"{USSCustomClassName}__pin";
    public virtual string USSCustomClassScrollView => $"{USSCustomClassName}__scroll-view";
    public virtual string USSCustomClassButton_Navigation => $"{USSCustomClassName}__button__navigation";
    public virtual string USSCustomClassButton_Submit => $"{USSCustomClassName}__button__submit";

    public override string ShortScreenTitle => "Sessions";
    public CustomTextfield PinTextField;
    public CustomSessionHeader SessionHeader;
    public CustomScrollView Session__ScrollView;
    public CustomButton Buttond_Submit;

    public override void InitElement()
    {
        base.InitElement();

        PinTextField.AddToClassList(USSCustomClassPin);
        Session__ScrollView.AddToClassList(USSCustomClassScrollView);
        Buttond_Submit.AddToClassList(USSCustomClassButton_Navigation);
        Buttond_Submit.AddToClassList(USSCustomClassButton_Submit);

        PinTextField.label = "Pin";

        Add(PinTextField);
        Add(SessionHeader);
        Add(Session__ScrollView);
        Header__Right.Add(Buttond_Submit);

        SessionHeader.Title = "Name";
        SessionHeader.ParticipantsCount = "Participants";
        SessionHeader.MaxParticipants = "Capacity";

        Buttond_Submit.text = "Connect";
        Buttond_Submit.Type = ButtonType.Primary;
    }

    public override void Set()
    {
        m_isSet = false;

        Set("Sessions");

        m_isSet = true;
    }

    public override VisualElement contentContainer => m_isSet ? Session__ScrollView.contentContainer : this;

    #region Implementation

    public System.Action<string, System.Action<MasterServerResponse.Server>> SendDataSession;
    public System.Action<string, string> UpdataCurrentConnectionData;
    public System.Action<string, string> StoreCurrentConnectionDataAndConnect;
    public bool IsSessionSelected { get; protected set; }

    protected List<MasterServerResponse.Server> m_serverResponses = new List<MasterServerResponse.Server>();
    protected string m_ip;
    protected string m_port;

    private void UpdateSessionList()
    {
        Session__ScrollView.Clear();

        foreach (MasterServerResponse.Server session in m_serverResponses)
        {
            CustomSession item = CreateSession();
            Session__ScrollView.Add(item);
            item.Title = session.Name;
            item.ParticipantsCount = session.PlayerCount.ToString();
            item.MaxParticipants = session.MaxPlayers.ToString();
            item.Clicked += DeselectAllSession;
            item.Clicked += () => Select(!item.IsSelected, session.Address, session.Port.ToString());
        }
    }

    public virtual void DeselectAllSession()
    {
        foreach (var child in Session__ScrollView.Children())
        {
            if (!(child is CustomSession session)) return;
            session.IsSelected = false;
        }
    }

    public virtual void Select(bool isSelected, string ip, string port)
    {
        IsSessionSelected = isSelected;
        m_ip = ip;
        m_port = port;
    }

    protected void ConnectWithIPAndPort()
    {
        UpdataCurrentConnectionData(m_ip, m_port);
        StoreCurrentConnectionDataAndConnect(m_ip, m_port);
    }

    protected void ConnectWithPin()
    {
        SendDataSession
            (
                PinTextField.value,
                (ser) =>
                {
                    m_serverResponses.Add(ser);
                    UpdateSessionList();
                }
            );
    }

    protected void Connect()
    {
        if (IsSessionSelected) ConnectWithIPAndPort();
        else ConnectWithPin();
    }

    protected abstract CustomSession CreateSession();

    #endregion
}
