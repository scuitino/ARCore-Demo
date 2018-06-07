using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneExtension : MonoBehaviour
{

	public static List<GraphicRaycaster> UiRaycasters = new List<GraphicRaycaster>();

	public static  bool IsAnyTouchOverUI()
	{
		foreach (var ray in UiRaycasters)
		{
			GraphicRaycaster gr = ray;
			PointerEventData ped = new PointerEventData(null);
			ped.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult>();
			gr.Raycast(ped, results);
			foreach (var r in results)
			{
				//Debug.Log("I touched ui object return  : " + r.gameObject.name);
				return true;
			}
		}

		return false;
	}

	public static void CacheRaycasters()
	{
		
		GetAllGameObjectsOfType<GraphicRaycaster>(out UiRaycasters);
	}

	public static void GetAllGameObjectsOfType<T>(out List<T> list)
	{
		Scene scene = SceneManager.GetActiveScene();
		GameObject[] roots = scene.GetRootGameObjects();
		List<Transform> t = new List<Transform>();
		foreach (var g in roots)
		{
			List<Transform> tmp = g.GetComponentsInChildren<Transform>().ToList();
			t.AddRange(tmp);
			

		}

		List<T> temp = new List<T>();
		foreach (var tr in t)
		{
			List<T> l = tr.GetComponentsInChildren<T>().ToList();
			if (l.Count > 0)
			{
				temp.AddRange(l);
			}
		}

		list = temp;
	}
}
