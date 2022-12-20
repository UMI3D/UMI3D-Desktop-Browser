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
using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.Controller;
using umi3d.cdk.collaboration;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using static umi3d.baseBrowser.Controller.BaseCursor;

namespace umi3d.baseBrowser.connection
{
    public partial class BaseGamePanelController
    {
        [Header("Form Loader")]
        public cdk.menu.MenuAsset FormMenu;
        public cdk.menu.view.MenuDisplayManager formMenuDisplay;
        public LoaderFormContainer FormContainer;

        public CustomLoader Loader => GamePanel.Loader;
        public CustomFormScreen Form => Loader.Form;
        public CustomLoadingScreen Loading => Loader.Loading;

        protected virtual void InitLoader()
        {
            Loader.CurrentScreen = LoaderScreens.Loading;
            Loader.ControllerCanProcess = (value) => BaseController.CanProcess = value;
            Loader.SetMovement = (value) => SetMovement(value, CursorMovement.Free);
            Loader.UnSetMovement = (value) => UnSetMovement(value);

            umi3d.cdk.UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded?.AddListener(() =>
            {
                Loader.ControllerCanProcess?.Invoke(true);
                TopArea.InformationArea.EnvironmentName = UMI3DCollaborationClientServer.Instance.environementName;
                Menu.GameData.EnvironmentName = UMI3DCollaborationClientServer.Instance.environementName;
            });

            InitLoader_Loading();
            InitiLoader_FormMenu();
        }

        protected virtual void InitLoader_Loading()
        {
            Loading.Title = "Connection";
            // TODO : for leaving is not working when the environment is loading.
            //Loader.Loading.BackText = "Leave";
            //Loader.Loading.Button_Back.clicked += BaseConnectionProcess.Instance.Leave;
            Loading.LoadingBar.highValue = 1;
        }

        protected virtual void InitiLoader_FormMenu()
        {
            Form.BackText = "Leave";
            Form.Button_Back.clicked += BaseConnectionProcess.Instance.Leave;

            FormContainer.GetContainer = () => Loader.Form.ScrollView;
            FormContainer.InsertDisplayer = (index, displayer) => Loader.Form.Insert(index, displayer);
            FormContainer.RemoveDisplayer = displayer => Loader.Form.Remove(displayer);
        }

        /// <summary>
        /// Asks users some parameters when they join the environment.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="callback"></param>
        protected void GetParameterDtos(common.interaction.FormDto form, System.Action<common.interaction.FormAnswerDto> callback)
        {
            Loader.CurrentScreen = LoaderScreens.Form;

            if (form == null) callback.Invoke(null);
            else
            {
                common.interaction.FormAnswerDto answer = new common.interaction.FormAnswerDto()
                {
                    boneType = 0,
                    hoveredObjectId = 0,
                    id = form.id,
                    toolId = 0,
                    answers = new List<common.interaction.ParameterSettingRequestDto>()
                };

                FormMenu.menu.RemoveAll();
                formMenuDisplay.CreateMenuAndDisplay(true, false);
                FormMenu.menu.Name = form.name;

                foreach (var param in form.fields)
                {
                    var c = cdk.interaction.GlobalToolMenuManager.GetInteractionItem(param);
                    FormMenu.menu.Add(c.Item1);
                    answer.answers.Add(c.Item2);
                }

                ButtonMenuItem send = new ButtonMenuItem() { Name = "Join" };
                UnityEngine.Events.UnityAction<bool> action = (bool b) =>
                {
                    formMenuDisplay.Hide(false);
                    FormMenu.menu.RemoveAll();
                    callback.Invoke(answer);
                    Controller.BaseCursor.SetMovement(this, Controller.BaseCursor.CursorMovement.Center);
                    LocalInfoSender.CheckFormToUpdateAuthorizations(form);
                    m_next = null;
                    Form.ResetSubmitEvent();
                    Form.DisplaySubmitButton = false;
                };
                send.Subscribe(action);

                m_next = () =>
                {
                    Form.Buttond_Submit.Focus();
                    send.NotifyValueChange(true);
                };

                Form.DisplaySubmitButton = true;
                Form.Buttond_Submit.text = "Join";
                Form.SubmitClicked += () => send.NotifyValueChange(true);
                Form.Buttond_Submit.Focus();
                Form.Buttond_Submit.Blur();
                if (FormContainer.Count() >= 1 && FormContainer[0] is TextfieldDisplayer textfield) textfield.Focus();
            }
        }
    }
}
