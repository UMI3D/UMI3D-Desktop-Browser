using System;
using umi3d.baseBrowser.connection;
using umi3d.common.interaction;
using UnityEngine.UIElements;

public class EnvironmentMenu : BaseMenu
{
    private FormScreen _connectionScreen;

    protected override void Start()
    {
        base.Start();

        _connectionScreen = new FormScreen(m_UiDocument.rootVisualElement.Q("Form"));

        BaseConnectionProcess.Instance.ConnectionSucces += e => BaseConnectionProcess.Instance.GetParameterDtos += GetParameterDtos;
    }

    public void GetParameterDtos(umi3d.common.interaction.form.FormDto form, Action<FormAnswerDto> callback)
    {
        _connectionScreen.GetParameterDtos(form, callback);
        _connectionScreen.Show();
    }
}