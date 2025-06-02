using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SimplexNoise;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("OnEnable " + gameObject.name);
    }

    private void Awake()
    {
        /*Debug.Log("Awake " + gameObject.name);

        if (!gameObject.name.Contains("Clone"))
        {
            GameObject newObj = Instantiate(gameObject);
            newObj.SetActive(false);
        }*/
    }

    private void Start()
    {
        Debug.Log("Start " + gameObject.name);

        TaskScheduler t = TaskScheduler.Current;
        TaskScheduler tt = TaskScheduler.Default;

        List<Task> tasks = new List<Task>();

        for (int i = 0; i < 100; i++)
        {
            //tasks.Add(Task.Run(ExpensiveTask));
        }

        //Task.WaitAll(tasks.ToArray());
    }

    private void ExpensiveTask()
    {
        Thread t = Thread.CurrentThread;

        for (int i = 0; i < 10; i++)
        {
            Noise d = new Noise();
            d.Calc3D(100, 100, 100, 1);
        }
    }
}