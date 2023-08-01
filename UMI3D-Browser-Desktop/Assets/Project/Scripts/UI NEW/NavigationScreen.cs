using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class NavigationScreen : BaseScreen
{
    private const string k_navigationItemPath = "Assets/Project/Resources/UI NEW/NavigationItem.uxml";

    private VisualElement _homeIntermediateArea;
    private VisualElement _intermediateArea;
    private TextElement _title;
    private VisualElement _elements;

    private bool _isHome;

    public TextElement Title => _title;
    public VisualElement Elements => _elements;

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

        _title = _root.Q<TextElement>("Title");
        _elements = _root.Q("Elements");

        IsHome = true;
    }

    public void AddElement(string name, Action callback)
    {
        var itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_navigationItemPath);
        var item = itemAsset.Instantiate();

        item.Q<TextElement>("Name").text = name;

        if (callback != null)
            item.Q<Button>("ButtonElement").clicked += callback;

        _elements.Add(item);
    }
}