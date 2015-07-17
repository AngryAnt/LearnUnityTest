using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class CurveVisualisation : MonoBehaviour
{
	[SerializeField] RawImage m_Image = null;
	[SerializeField] Color
		m_BackgroundColor = Color.black,
		m_CurveColor = Color.green;
	[SerializeField] Text
		m_MinXLabel = null,
		m_MaxXLabel = null,
		m_MinYLabel = null,
		m_MaxYLabel = null;


	AnimationCurve m_Curve;
	Texture2D m_Texture;
	Rect m_ValueScope = new Rect ();
	Vector2 m_Offset, m_Scale;


	public AnimationCurve Curve
	{
		get
		{
			return m_Curve;
		}
	}


	void Reset ()
	{
		m_Image = GetComponent<RawImage> ();
	}


	void Start ()
	{
		m_Curve = new AnimationCurve ();
		RectTransform rect = m_Image.rectTransform;
		m_Texture = new Texture2D ((int)rect.sizeDelta.x, (int)rect.sizeDelta.y, TextureFormat.ARGB32, false);
		m_Image.texture = m_Texture;
	}


	void Update ()
	{
		Render (); // TODO: Implement a frequency variable and use this to control the render frequency via a coroutine
	}


	void Render ()
	{
		// Clear
		m_Texture.SetPixels (Enumerable.Repeat (m_BackgroundColor, m_Texture.width * m_Texture.height).ToArray ());

		// Determine value scope

		m_ValueScope.xMin = m_ValueScope.yMin = Mathf.Infinity;
		m_ValueScope.xMax = m_ValueScope.yMax = Mathf.NegativeInfinity;

		foreach (Keyframe key in m_Curve.keys)
		{
			m_ValueScope.xMin = Mathf.Min (m_ValueScope.xMin, key.time);
			m_ValueScope.yMin = Mathf.Min (m_ValueScope.yMin, key.value);
			m_ValueScope.xMax = Mathf.Max (m_ValueScope.xMax, key.time);
			m_ValueScope.yMax = Mathf.Max (m_ValueScope.yMax, key.value);
		}

		// Set labels if configured

		string xLabelFormat = "F2", yLabelFormat = "F2"; // TODO: Dynamically scale these

		if (m_MinXLabel != null)
		{
			m_MinXLabel.text = m_ValueScope.xMin.ToString (xLabelFormat);
		}

		if (m_MaxXLabel != null)
		{
			m_MaxXLabel.text = m_ValueScope.xMax.ToString (xLabelFormat);
		}

		if (m_MinYLabel != null)
		{
			m_MinYLabel.text = m_ValueScope.yMin.ToString (yLabelFormat);
		}

		if (m_MaxYLabel != null)
		{
			m_MaxYLabel.text = m_ValueScope.yMax.ToString (yLabelFormat);
		}

		// Translate to pixel space

		m_Offset = new Vector2 (m_ValueScope.xMin * -1.0f, m_ValueScope.yMin * -1.0f);
		m_Scale = new Vector2 (m_Texture.width / m_ValueScope.width, m_Texture.height / m_ValueScope.height);

		// Draw

		for (int x = 1; x < m_Texture.width; ++x)
		{
			float
				indexA = (x - 1) / m_Scale.x - m_Offset.x,
				indexB = x / m_Scale.x - m_Offset.x;
			float
				yA = (m_Curve.Evaluate (indexA) + m_Offset.y) * m_Scale.y,
				yB = (m_Curve.Evaluate (indexB) + m_Offset.y) * m_Scale.y;

			Line (m_Texture, x - 1, (int)yA, x, (int)yB, m_CurveColor);
		}

		m_Texture.Apply ();
	}


	static void Line (Texture2D texture, int aX, int aY, int bX, int bY, Color color)
	// Source: http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
	{
		aX = Mathf.Clamp (aX, 0, texture.width - 1);
		bX = Mathf.Clamp (bX, 0, texture.width - 1);
		aY = Mathf.Clamp (aY, 0, texture.height - 1);
		bY = Mathf.Clamp (bY, 0, texture.height - 1);

		int
			deltaX = Mathf.Abs (bX - aX),
			deltaY = Mathf.Abs (bY - aY);

		if (deltaX + deltaY <= 2)
		{
			texture.SetPixel (aX, aY, color);
			texture.SetPixel (bX, bY, color);
			return;
		}

		int
			stepX = aX < bX ? 1 : -1,
			stepY = aY < bY ? 1 : -1;
		int error = deltaX - deltaY;

		while (true)
		{
			texture.SetPixel (aX, aY, color);

			if (aX == bX && aY == bY)
			{
				break;
			}

			int error2 = error * 2;

			if (error2 > -deltaY)
			{
				error -= deltaY;
				aX += stepX;
			}

			if (error2 < deltaX)
			{
				error += deltaX;
				aY += stepY;
			}
		}
	}
}
