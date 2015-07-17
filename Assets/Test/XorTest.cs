using UnityEngine;


public class XorTest : NeuralTest
{
	[SerializeField] double m_LearningRate = .1, m_Momentum = .1;


	const int kInputCount = 2;
	readonly int[]
		kLayerCounts = new[]
		{
			2,
			1
		};
	readonly double[][]
		kInput = new double[][]
		{
			new[] {0.0, 0.0},
			new[] {0.0, 1.0},
			new[] {1.0, 0.0},
			new[] {1.0, 1.0}
		},
		kOutput = new double[][]
		{
			new[] {0.0},
			new[] {1.0},
			new[] {1.0},
			new[] {0.0}
		};


	protected override int InputCount
	{
		get
		{
			return kInputCount;
		}
	}


	protected override int[] LayerCounts
	{
		get
		{
			return kLayerCounts;
		}
	}


	protected override double[][] Input
	{
		get
		{
			return kInput;
		}
	}


	protected override double[][] Output
	{
		get
		{
			return kOutput;
		}
	}


	protected override double LearningRate
	{
		get
		{
			return m_LearningRate;
		}
	}


	protected override double Momentum
	{
		get
		{
			return m_Momentum;
		}
	}
}
