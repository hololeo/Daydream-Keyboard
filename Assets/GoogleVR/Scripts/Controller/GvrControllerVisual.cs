// Copyright 2016 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// The controller is not available for versions of Unity without the
// GVR native integration.

using UnityEngine;
using System.Collections;

/// Provides visual feedback for the daydream controller.
[RequireComponent (typeof(Renderer))]
public class GvrControllerVisual : MonoBehaviour
{
	#if UNITY_HAS_GOOGLEVR && (UNITY_ANDROID || UNITY_EDITOR)
	/// Units are in meters.
	private static readonly Vector3 TOUCHPAD_POINT_DIMENSIONS = new Vector3 (0.003f, 0.0002f, 0.003f);
	private const float TOUCHPAD_RADIUS = 0.012f;
	private const float TOUCHPAD_POINT_Y_OFFSET = 0.035f;
	private const float TOUCHPAD_POINT_ELEVATION = 0.0025f;
	private const float TOUCHPAD_POINT_SCALE_DURATION_SECONDS = 0.15f;

	private Renderer controllerRenderer;
	private Renderer touchRenderer;
	private float elapsedScaleTimeSeconds;
	private bool wasTouching;
	private MaterialPropertyBlock materialPropertyBlock;
	private int colorId;

	public GameObject touchPoint;
	public Material material_idle;
	public Material material_app;
	public Material material_system;
	public Material material_touchpad;
	public Material touchTransparent;
	public Material touchOpaque;
	public Material green;

	/// The suggested rendering alpha value of the controller.
	/// This is to prevent the controller from intersecting face.
	public static float AlphaValue  { get; set; }

	void Awake ()
	{
		AlphaValue = 1.0f;
		controllerRenderer = GetComponent<Renderer> ();
		touchRenderer = touchPoint.GetComponent<Renderer> ();
		materialPropertyBlock = new MaterialPropertyBlock ();
		colorId = Shader.PropertyToID ("_Color");

		// Makes it so the touchPoint is initialized at the correct scale
		elapsedScaleTimeSeconds = TOUCHPAD_POINT_SCALE_DURATION_SECONDS;
	}

	void Start ()
	{
		if (GvrArmModel.Instance != null) {
			GvrArmModel.Instance.OnArmModelUpdate += OnArmModelUpdate;
		} else {
			Debug.LogError ("Unable to find GvrArmModel.");
		}
	}

	void OnDestroy ()
	{
		if (GvrArmModel.Instance != null) {
			GvrArmModel.Instance.OnArmModelUpdate -= OnArmModelUpdate;
		}
	}

	private void OnArmModelUpdate ()
	{
		// Choose the appropriate material to render based on button states.
		// Change material to reflect button presses.
		if (GvrController.AppButton) {
			controllerRenderer.material = material_app;
		} else if (GvrController.Recentering) {
			controllerRenderer.material = material_system;
		} else {
			controllerRenderer.material = material_idle;
		}

		// Draw the touch point and animate the scale change.
		touchPoint.SetActive (true);
		if (GvrController.IsTouching || GvrController.ClickButton) {
			// Reset the elapsedScaleTime when we start touching.
			// This flag is necessary because
			// GvrController.TouchDown sometimes becomes true a frame after GvrController.Istouching
			if (!wasTouching) {
				wasTouching = true;
				elapsedScaleTimeSeconds = 0.0f;
			}

			float x = (GvrController.TouchPos.x - 0.5f) * 2.0f * TOUCHPAD_RADIUS;
			float y = (GvrController.TouchPos.y - 0.5f) * 2.0f * TOUCHPAD_RADIUS;
			Vector3 scale = Vector3.Lerp (Vector3.zero,
				                    TOUCHPAD_POINT_DIMENSIONS,
				                    elapsedScaleTimeSeconds / TOUCHPAD_POINT_SCALE_DURATION_SECONDS);

			touchPoint.transform.localScale = scale;
			touchPoint.transform.localPosition = new Vector3 (-x, TOUCHPAD_POINT_Y_OFFSET - y, TOUCHPAD_POINT_ELEVATION);
		} else {
			// Reset the elapsedScaleTime when we stop touching.
			// This flag is necessary because
			// GvrController.TouchDown sometimes becomes true a frame after GvrController.Istouching
			if (wasTouching) {
				wasTouching = false;
				elapsedScaleTimeSeconds = 0.0f;
			}

			Vector3 scale = Vector3.Lerp (TOUCHPAD_POINT_DIMENSIONS,
				                    Vector3.zero,
				                    elapsedScaleTimeSeconds / TOUCHPAD_POINT_SCALE_DURATION_SECONDS);
			
			touchPoint.transform.localScale = scale;
		}

		elapsedScaleTimeSeconds += Time.deltaTime;

		// Adjust transparency.
		Color color = controllerRenderer.material.color;
		color.a = AlphaValue;
		controllerRenderer.GetPropertyBlock (materialPropertyBlock);
		materialPropertyBlock.SetColor (colorId, color);
		controllerRenderer.SetPropertyBlock (materialPropertyBlock);
		if (AlphaValue < 1.0f) {
			touchRenderer.material = touchTransparent;
			touchRenderer.GetPropertyBlock (materialPropertyBlock);
			materialPropertyBlock.SetColor (colorId, color);
			touchRenderer.SetPropertyBlock (materialPropertyBlock);
		} else {
			if (GvrController.ClickButton) {
				touchRenderer.material = green;
			} else {
				touchRenderer.material = touchOpaque;
			}
		}
	}

	#endif  // UNITY_HAS_GOOGLEVR && (UNITY_ANDROID || UNITY_EDITOR)
}
