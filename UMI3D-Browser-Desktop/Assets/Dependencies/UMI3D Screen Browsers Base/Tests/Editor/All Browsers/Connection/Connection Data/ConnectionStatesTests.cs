using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.connection;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.TestTools;

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

    // A Test behaves as an ordinary method
    [Test]
    public void AddStates()
    {
        var statesData = StatesData;
        var state = GetState(0);
        var state2 = GetState(1);
        var state3 = GetState(1);

        Assert.IsTrue(statesData.Add(state));
        Assert.IsTrue(statesData.Add(state2), $"{state.GetType().Name}, {state2.GetType().Name}");
        Assert.IsTrue(!statesData.Add(state3), $"{state2.GetType().Name}, {state3.GetType().Name}");

        Assert.AreEqual(statesData[0], state);
        Assert.AreEqual(statesData[1], state2);
    }

    [Test]
    public void ContainStates()
    {
        var statesData = StatesData;
        var state = GetState(0);
        var state2 = GetState(1);
        var state3 = GetState(2);

        statesData.Add(state);
        statesData.Add(state2);
        statesData.Add(state3);

        Assert.IsTrue(statesData.ContainsStateByType<MasterServerStartedConnectionState>());
        Assert.IsTrue(statesData.ContainsStateByType<MediaDTOFoundConnectionState>());
        Assert.IsTrue(statesData.ContainsStateByType<IdleConnectionState>());
    }

    //[Test]
    //public void Events()
    //{
    //    //var ConnectionEvent = new ConnectionEvents();
    //    var statesData = new ConnectionStateData();
    //    var state = new MasterServerStartedConnectionState();
    //    var state2 = new MediaDTOFoundConnectionState();
    //    var state3 = new IdleConnectionState();

    //    statesData.Add(state);
    //    statesData.Add(state2);
    //    statesData.Add(state3);

    //    Assert.IsTrue(statesData.ContainsStateByType<MasterServerStartedConnectionState>());
    //    Assert.IsTrue(statesData.ContainsStateByType<MediaDTOFoundConnectionState>());
    //    Assert.IsTrue(statesData.ContainsStateByType<IdleConnectionState>());
    //}

    //// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    //// `yield return null;` to skip a frame.
    //[UnityTest]
    //public IEnumerator ConnectionStatesTestsWithEnumeratorPasses()
    //{
    //    // Use the Assert class to test conditions.
    //    // Use yield to skip a frame.
    //    yield return null;
    //}
}
