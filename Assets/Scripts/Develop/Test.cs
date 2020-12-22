using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SimplexNoise;
using UnityEngine;

public class Test : MonoBehaviour
{
	void OnEnable()
	{
		Debug.Log("OnEnable " + gameObject.name);
	}

	void Awake()
	{
		/*Debug.Log("Awake " + gameObject.name);

		if (!gameObject.name.Contains("Clone"))
		{
			GameObject newObj = Instantiate(gameObject);
			newObj.SetActive(false);
		}*/
	}

	void Start()
	{
		Debug.Log("Start " + gameObject.name);

		var t = TaskScheduler.Current;
		var tt = TaskScheduler.Default;

		List<Task> tasks = new List<Task>();

		for (int i = 0; i < 100; i++)
		{
			//tasks.Add(Task.Run(ExpensiveTask));
		}

		//Task.WaitAll(tasks.ToArray());
	}

	private void ExpensiveTask()
	{
		var t = Thread.CurrentThread;

		for (int i = 0; i < 10; i++)
		{
			SimplexNoise.Noise d = new Noise();
			d.Calc3D(100, 100, 100, 1);
		}
	}
}