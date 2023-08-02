using inetum.unityUtils;
using System;
using umi3d.baseBrowser.connection;
using UnityEngine;
using UnityEngine.UIElements;

public class NavigationScreen : BaseScreen
{
    private const string k_navigationItemPath = "UI NEW/NavigationItem";

    private VisualElement _homeIntermediateArea;
    private VisualElement _intermediateArea;
    private TextField _portalUrl;
    private TextElement _title;
    private VisualElement _elements;

    private bool _isHome;

    public TextElement Title => _title;
    public VisualElement Elements => _elements;
    public TextField PortalUrl => _portalUrl;

    public bool IsHome
    {
        get => _isHome;
        set
        {
            _isHome = value;
            if (_isHome )
            {
                _homeIntermediateArea.RemoveFromClassList("hidden");
                _intermediateArea.AddToClassList("hidden");
            } 
            else
            {
                _homeIntermediateArea.AddToClassList("hidden");
                _intermediateArea.RemoveFromClassList("hidden");
            }
        }
    }

    public NavigationScreen(VisualElement element) : base(element)
    {
        _homeIntermediateArea = _root.Q("HomeIntermediateArea");
        _intermediateArea = _root.Q("IntermediateArea");
        _portalUrl = _homeIntermediateArea.Q<TextField>("Url");
        _portalUrl.SetPlaceholderText("example.fr");

        _title = _root.Q<TextElement>("Title");
        _elements = _root.Q("Elements");

        IsHome = true;
    }

    public void AddElement(string name, Action callback)
    {
        var itemAsset = Resources.Load<VisualTreeAsset>(k_navigationItemPath);
        var item = itemAsset.Instantiate();

        item.Q<TextElement>("Name").text = name;

        if (callback != null)
            item.Q<Button>("ButtonElement").clicked += callback;

        _elements.Add(item);
    }
}