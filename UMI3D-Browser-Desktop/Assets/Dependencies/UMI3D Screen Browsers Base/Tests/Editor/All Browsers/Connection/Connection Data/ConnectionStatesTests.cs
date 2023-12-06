/*
Copyright 2019 - 2023 Inetum

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
using inetum.unityUtils;
using NUnit.Framework;
using System;
using umi3d.browserRuntime.connection;

public class ConnectionStatesTests
{
    IConnectionStateData StatesData
    {
        get
        {
            return new ConnectionStateData();
        }
    }

    IConnectionState GetState(int index)
    {
        if (index < 0 || index > 10)
        {
            System.Random rnd = new System.Random();
            index = rnd.Next(0, 10);
        }

        if (index == 0)
        {
            return new IdleConnectionState();
        }
        else if (index == 1)
        {
            return new InProgressConnectionState();
        }
        else if (index == 2)
        {
            return new MasterServerStartedConnectionState();
        }
        else if (index == 3)
        {
            return new MasterServerSessionConnectionState();
        }
        else if (index == 4)
        {
            return new MasterServerStoppedConnectionState();
        }
        else if (index == 5)
        {
            return new MasterServerFailedConnectionState();
        }
        else if (index == 6)
        {
            return new MediaDTOStartedConnectionState();
        }
        else if (index == 7)
        {
            return new MediaDTOStoppedConnectionState();
        }
        else if (index == 8)
        {
            return new MediaDTOFailedConnectionState();
        }
        else if (index == 9)
        {
            return new MediaDTOFoundConnectionState();
        }
        else
        {
            return new ConnectedConnectionState();
        }
    }

    Guid GetStateId(int index)
    {
        if (index < 0 || index > 10)
        {
            System.Random rnd = new System.Random();
            index = rnd.Next(0, 10);
        }

        var state = GetState(index);

        return (state as ISmartEnumeration).Id;
    }

    // A Test behaves as an ordinary method
    [Test]
    public void AddStates()
    {
        var statesData = StatesData;
        var state = GetState(0);
        var state2 = GetState(1);
        var state3 = GetState(1);

        uint eventRaisedCount = 0;
        statesData.StateAdded += _state =>
        {
            eventRaisedCount++;
        };

        Assert.IsTrue(statesData.Add(state, (state as ISmartEnumeration).Id));
        Assert.IsTrue(statesData.Add(state2, (state2 as ISmartEnumeration).Id), $"{state.GetType().Name}, {state2.GetType().Name}");
        Assert.IsTrue(!statesData.Add(state3, (state3 as ISmartEnumeration).Id), $"{state2.GetType().Name}, {state3.GetType().Name}");

        Assert.AreEqual(statesData[0], state);
        Assert.AreEqual(statesData[1], state2);

        Assert.IsTrue(eventRaisedCount == 2);
    }

    [Test]
    public void ContainStates()
    {
        var statesData = StatesData;
        var state = GetState(0);
        var state2 = GetState(1);
        var state3 = GetState(2);

        Assert.IsTrue(statesData.Add(state, (state as ISmartEnumeration).Id));
        Assert.IsTrue(statesData.Add(state2, (state2 as ISmartEnumeration).Id), $"{state.GetType().Name}, {state2.GetType().Name}");
        Assert.IsTrue(statesData.Add(state3, (state3 as ISmartEnumeration).Id), $"{state2.GetType().Name}, {state3.GetType().Name}");

        Assert.IsTrue(statesData.Contains(GetStateId(0)), $"{((ISmartEnumeration)state).Id}, {((ISmartEnumeration)GetState(0)).Id}");
        Assert.IsTrue(statesData.Contains(GetStateId(1)));
        Assert.IsTrue(statesData.Contains(GetStateId(2)), $"{((ISmartEnumeration)state3).Id}, {((ISmartEnumeration)GetState(2)).Id}");
    }

    [Test]
    public void ClearStates()
    {
        var statesData = StatesData;
        var state = GetState(0);
        var state2 = GetState(1);
        var state3 = GetState(2);

        Assert.IsTrue(statesData.Add(state, (state as ISmartEnumeration).Id));
        Assert.IsTrue(statesData.Add(state2, (state2 as ISmartEnumeration).Id));
        Assert.IsTrue(statesData.Add(state3, (state3 as ISmartEnumeration).Id));

        bool cleared = false;
        statesData.Cleared += () =>
        {
            cleared = true;
        };

        statesData.Clear();

        Assert.IsTrue(!statesData.Contains(GetStateId(0)));
        Assert.IsTrue(!statesData.Contains(GetStateId(1)));
        Assert.IsTrue(!statesData.Contains(GetStateId(2)));
        Assert.IsTrue(cleared);
    }

    [Test]
    public void FullConnection()
    {
        var statesData = StatesData;

        // Start by searching a master server.
        Assert.IsTrue(statesData.Add(new MasterServerStartedConnectionState(), new MasterServerStartedConnectionState().Id));
        // In the same time search a mediaDTO.
        Assert.IsTrue(statesData.Add(new MediaDTOStartedConnectionState(), new MediaDTOStartedConnectionState().Id));
        // A master has been found: stop the search for a mediaDTO.
        Assert.IsTrue(statesData.Add(new MediaDTOStoppedConnectionState(), new MediaDTOStoppedConnectionState().Id));
        Assert.IsTrue(statesData.Add(new MasterServerSessionConnectionState(), new MasterServerSessionConnectionState().Id));

        // The user select a world.
        statesData.Clear();
        // Search for a mediaDTO.
        Assert.IsTrue(statesData.Add(new MediaDTOStartedConnectionState(), new MediaDTOStartedConnectionState().Id));
        Assert.IsTrue(statesData.Add(new MediaDTOFoundConnectionState(), new MediaDTOFoundConnectionState().Id));
    }
}
