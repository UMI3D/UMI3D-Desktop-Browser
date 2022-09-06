using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    async void  Start()
    {
        Debug.Log("Pomme");

        var tasks = new List<string>();

        for (int i = 0; i < 50; i++)
        {
            tasks.Add(i.ToString());
        }
        var tasks2 = tasks.Select(async s => await FunctionTest(s)).ToList();

        try
        {
            await Task.WhenAll(tasks2);
        }
        catch (Exception e)
        {
            Debug.Log("TOTAL");
        }

        if (tasks2.Find(t => t.Status == TaskStatus.Faulted) != null)
            Debug.LogError("OHHH");

        Debug.Log("Poire");

    }

    async Task FunctionTest(string str)
    {
        await Task.Yield();

        try
        {
            throw new System.NotImplementedException();
        }
         catch  (Exception e)
        {
            Debug.Log("Exception " + str);
            throw new System.NotImplementedException();
        }
    }
}
