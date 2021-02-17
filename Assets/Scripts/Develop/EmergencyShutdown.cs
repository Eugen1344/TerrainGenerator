using System.Collections;
using UnityEngine;

public class EmergencyShutdown : MonoBehaviour
{
	public static EmergencyShutdown Instance;

	public bool EnableShutdownTimer;
	public bool EnableShutdownCoroutine;
	public int ShutdownTimerSeconds;

	private void Awake()
	{
		Instance = this;

		if (EnableShutdownTimer && EnableShutdownCoroutine)
			StartCoroutine(WaitAndShutdown());
	}

	private IEnumerator WaitAndShutdown()
	{
		yield return new WaitForSecondsRealtime(ShutdownTimerSeconds);

		Application.Quit();
	}

	public bool CheckForShutdownTimer()
	{
		if (!EnableShutdownTimer)
			return false;

		if (Time.realtimeSinceStartup > ShutdownTimerSeconds)
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
			return true;
		}

		return false;
	}
}