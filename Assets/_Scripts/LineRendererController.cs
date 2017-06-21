using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour {
	public GameObject lineRendererGO;
	private LineRenderer lineRenderer;

	private const float TOUCHPAD_RADIUS = 0.012f;
	private const float TOUCHPAD_POINT_Y_OFFSET = 0.035f;
	private const float TOUCHPAD_POINT_ELEVATION = 0.0025f;

	void Start () {
		lineRenderer = lineRendererGO.GetComponent<LineRenderer> ();
		lineRenderer.enabled = false;
	}

	void Update () {
		if (GvrController.ClickButtonDown) {
			lineRenderer.enabled = true;
			float x = (GvrController.TouchPos.x - 0.5f) * 2.0f * TOUCHPAD_RADIUS;
			float y = (GvrController.TouchPos.y - 0.5f) * 2.0f * TOUCHPAD_RADIUS;
			Vector3 startPosition = new Vector3 (-x, TOUCHPAD_POINT_Y_OFFSET - y, 0);
			lineRenderer.SetPosition(0, startPosition);
		}

		if (GvrController.ClickButton) {
			float x = (GvrController.TouchPos.x - 0.5f) * 2.0f * TOUCHPAD_RADIUS;
			float y = (GvrController.TouchPos.y - 0.5f) * 2.0f * TOUCHPAD_RADIUS;
			Vector3 endPosition = new Vector3 (-x, TOUCHPAD_POINT_Y_OFFSET - y, 0);
			lineRenderer.SetPosition(1, endPosition);
		}

		if (GvrController.ClickButtonUp) {
			lineRenderer.enabled = false;
		}
	}
}
