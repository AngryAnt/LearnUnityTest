using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class LogView : MonoBehaviour
{
	[SerializeField] Text m_Text;
	[SerializeField] int m_MaxLength = 10;


	List<string> m_Log = new List<string> ();
	bool m_Update = false;


	public void Log (string message)
	{
		m_Log.Add (string.Format ("{0:D8}: {1}", Time.frameCount, message));

		m_Update = true;
	}


	void Reset ()
	{
		m_Text = GetComponent<Text> ();
	}


	void OnEnable ()
	{
		Application.logMessageReceived += OnLog;
	}


	void OnDisable ()
	{
		Application.logMessageReceived -= OnLog;
	}


	void OnLog (string message, string stack, LogType type)
	{
		Log (message);
	}


	void LateUpdate ()
	{
		if (!m_Update)
		{
			return;
		}

		if (m_Log.Count > m_MaxLength)
		{
			m_Log.RemoveRange (0, m_Log.Count - m_MaxLength);
		}

		m_Text.text = string.Join ("\n", m_Log.ToArray ());

		m_Update = false;
	}
}
