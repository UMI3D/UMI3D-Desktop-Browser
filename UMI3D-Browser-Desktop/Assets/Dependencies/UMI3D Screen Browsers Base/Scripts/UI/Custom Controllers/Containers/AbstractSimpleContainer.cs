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
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;

/// <summary>
/// A simple container that has only one level and only display Displayers.
/// </summary>
public abstract class AbstractSimpleContainer : AbstractMenuDisplayContainer
{
    public List<AbstractDisplayer> m_displayers = new List<AbstractDisplayer>();

    public override AbstractDisplayer this[int i]
    {
        get => m_displayers[i];
        set => m_displayers[i] = value;
    }

    public override bool Contains(AbstractDisplayer element) => m_displayers.Contains(element);

    public override int Count() => m_displayers.Count;

    public override AbstractMenuDisplayContainer CurrentMenuDisplayContainer() => this;

    public override int GetIndexOf(AbstractDisplayer element) => m_displayers.IndexOf(element);

    public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        => Insert(element, Count(), updateDisplay);

    public override void InsertRange(IEnumerable<AbstractDisplayer> elements, bool updateDisplay = true)
    {
        foreach (var element in elements) Insert(element, updateDisplay);
    }

    public override int IsSuitableFor(AbstractMenuItem menu)
    {
        throw new System.NotImplementedException();
    }

    public override int RemoveAll()
    {
        int count = Count();
        foreach (var element in m_displayers) Remove(element, true);
        return count;
    }

    public override bool RemoveAt(int index, bool updateDisplay = true)
    {
        if (index >= Count()) return false;
        return Remove(this[index], updateDisplay);
    }

    protected override void CollapseImp() => Hide();

    protected override void ExpandAsImp(AbstractMenuDisplayContainer container) => Display();

    protected override void ExpandImp() => Display();

    protected override IEnumerable<AbstractDisplayer> GetDisplayers() => m_displayers;
}
