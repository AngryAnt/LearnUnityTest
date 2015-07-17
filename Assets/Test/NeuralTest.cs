using UnityEngine;
using System.Collections;
using Learn;
using Network = Learn.Network;


public abstract class NeuralTest : MonoBehaviour
{
	[SerializeField] CurveVisualisation m_LearningCurve = null;
	[SerializeField] LogView m_LogView = null;

	[SerializeField, Space (15)] bool m_Step = false;
	[SerializeField] bool m_DebugGUI = false;

	[SerializeField, Space (15)] bool m_Randomize = true;
	[SerializeField] float m_TargetError = 0f;


	protected abstract double[][] Input {get;}
	protected abstract double[][] Output {get;}
	protected abstract int InputCount {get;}
	protected abstract int[] LayerCounts {get;}
	protected virtual double LearningRate { get { return .1; } }
	protected virtual double Momentum { get { return .1; } }


	protected Network m_Network;
	protected bool m_Running;


	IEnumerator Start ()
	{
		m_Running = true;

		Debug.Log ("Starting test");

		yield return null;

		Debug.Log ("Setup");

		m_Network = new Network (InputCount, LayerCounts);
		if (m_Randomize)
		{
			m_Network.RandomizeWeights ();
		}

		BackPropagation teacher = new BackPropagation (m_Network)
		{
			LearningRate = this.LearningRate,
			Momentum = this.Momentum
		};

		if (m_LearningCurve != null)
		{
			m_LearningCurve.Curve.keys = new Keyframe[] {};
		}

		if (m_Step)
		{
			Debug.Break ();
		}

		yield return null;

		Debug.Log ("Start training");

		while (m_Running && enabled && Application.isPlaying)
		{
			double error = teacher.TrainSet (Input, Output);

			LogResult ("Training run error: " + error);

			if (m_LearningCurve != null)
			{
				m_LearningCurve.Curve.AddKey (Time.time, (float)error);
			}

			if (error < m_TargetError)
			{
				Debug.Log (string.Format ("Achieved error {0}, beating target {1}", error, m_TargetError));
				yield break;
			}

			if (m_Step)
			{
				Debug.Break ();
			}

			yield return null;
		}
	}


	void LogResult (string message)
	{
		if (m_LogView != null)
		{
			m_LogView.Log (message);
		}
		else
		{
			Debug.Log (message);
		}
	}


	void OnGUI ()
	{
		if (!m_DebugGUI)
		{
			return;
		}

		GUILayout.Box ("Training data");
		for (int setIndex = 0; setIndex < Input.Length; ++setIndex)
		{
			GUILayout.BeginHorizontal (GUI.skin.box);
				GUILayout.BeginVertical (GUI.skin.box);
					for (int inputIndex = 0; inputIndex < Input[setIndex].Length; ++inputIndex)
					{
						GUILayout.Box (Input[setIndex][inputIndex].ToString ("f2"));
					}
				GUILayout.EndVertical ();
				GUILayout.BeginVertical (GUI.skin.box);
					for (int outputIndex = 0; outputIndex < Output[setIndex].Length; ++outputIndex)
					{
						GUILayout.Box (Output[setIndex][outputIndex].ToString ("f2"));
					}
				GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
		}

		GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();
				GUILayout.Box ("Network");
				if (m_Network == null)
				{
					GUILayout.Box ("Awaiting network");
				}
				else
				{
					m_Network.OnGUI ();
				}
			GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();
	}
}


static class NNDebugGUI
{
	public static void OnGUI (this Network network)
	{
		GUILayout.BeginHorizontal (GUI.skin.box);
			foreach (NeuronLayer layer in network)
			{
				layer.OnGUI ();
			}
		GUILayout.EndHorizontal ();
	}


	public static void OnGUI (this NeuronLayer layer)
	{
		GUILayout.BeginVertical (GUI.skin.box);
			GUILayout.FlexibleSpace ();
			foreach (Neuron neuron in layer)
			{
				neuron.OnGUI ();
			}
			GUILayout.FlexibleSpace ();
		GUILayout.EndVertical ();
	}


	public static void OnGUI (this Neuron neuron)
	{
		GUILayout.BeginHorizontal (GUI.skin.box);
			GUILayout.BeginVertical ();
				for (int index = 0; index < neuron.InputCount; ++index)
				{
					GUILayout.Box (neuron.GetWeight (index).ToString ("f2"));
				}
			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();
				GUILayout.Box (neuron.BiasWeight.ToString ("f2"));
			GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();
	}
}
