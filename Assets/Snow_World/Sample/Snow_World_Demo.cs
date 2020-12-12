using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Snow_World_Demo : MonoBehaviour {

	List<string> models = new List<string>();

	int currentModelIndex = 0;
	GameObject currentModel = null;
	float angle = 0;
	float speed = 10f;
	float buttonSize = 60f;

	string prefabsFolder = "Snow_World_Prefabs";

	// Use this for initialization
	void Start () {
		// fill models list
		Object[] objs = Resources.LoadAll(prefabsFolder, typeof(GameObject));
		foreach(Object obj in objs) {
			models.Add(obj.name);
		}

		RecreateModel();
	}
	
	// Update is called once per frame
	void Update () {
		MouseController();
		KeyboardController();
		RecalcPos();
	}

	void RecalcPos() {
		angle += Time.deltaTime * speed;
		if (currentModel != null) {
			currentModel.transform.localRotation = Quaternion.Euler(0, angle, 0);
		}
	}

	void MouseController() {
		if (Input.GetMouseButton(1) == false)
			return;
		float mouseX = Input.GetAxis("Mouse X");
		angle += mouseX * speed;
	}

	void KeyboardController() {
		float keyX = 0f;
		if (Input.GetKey(KeyCode.LeftArrow)) {
			keyX = -Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.RightArrow)) {
			keyX = Time.deltaTime;
		}
		angle += keyX * speed * 3;
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			SafeIncCurrectModelIndex();
			RecreateModel();
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			SafeDecCurrectModelIndex();
			RecreateModel();
		}
	}

	void OnGUI(){
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(" < ", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize))) {
			SafeDecCurrectModelIndex();
			RecreateModel();
		};

		GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
		labelStyle.fontSize = 30;
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.normal.textColor = Color.red;
		GUILayout.Label(models[currentModelIndex].Replace("Personage_", "").Replace("Object_", ""), labelStyle, GUILayout.Width(buttonSize * 5), GUILayout.Height(buttonSize));

		if (GUILayout.Button(" > ", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize))) {
			SafeIncCurrectModelIndex();
			RecreateModel();
		};
		GUILayout.EndHorizontal();
   }

	void SafeIncCurrectModelIndex() {
		currentModelIndex++;
		if (currentModelIndex >= models.Count) {
			currentModelIndex = 0;
		}
	}

	void SafeDecCurrectModelIndex() {
		currentModelIndex--;
		if (currentModelIndex < 0) {
			currentModelIndex = models.Count - 1;
		}
	}

	void RecreateModel() {
		if (currentModel != null) {
			DestroyObject(currentModel);
			currentModel = null;
		}
		currentModel = Instantiate(Resources.Load(prefabsFolder + "/" + models[currentModelIndex], typeof(GameObject))) as GameObject;
	}
}
