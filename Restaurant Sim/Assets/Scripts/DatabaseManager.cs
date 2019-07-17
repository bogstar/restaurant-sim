using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : Singleton<DatabaseManager>
{
	public GameObject trayPrefab;
	public Sprite defaultSprite;
	public ModelSetup defaultModel;
	public GameObject workSpaceCanvasPrefab;
	public GameObject packagePrefab;
	DatabaseEntryScriptableObject[] allItems;

	protected override void Awake()
	{
		base.Awake();

		var allItems = Resources.LoadAll<DatabaseEntryScriptableObject>("Database");
		Dictionary<string, int> idCountMap = new Dictionary<string, int>();

		var list = new List<DatabaseEntryScriptableObject>();

		foreach (var item in allItems)
		{
			var strungId = item.GetDefiningFeatures();
			if (idCountMap.ContainsKey(strungId))
			{
				idCountMap[strungId]++;
				item.SetId(strungId + "_" + idCountMap[strungId]);
			}
			else
			{
				item.SetId(strungId + "_" + 1);
				idCountMap.Add(strungId, 1);
			}

			print(item.id);
		}

		/*
		var it = Resources.LoadAll<DatabaseEntryScriptableObject>("Database/Items");

		string t = Application.dataPath;
		string a = Application.persistentDataPath;
		string b = System.IO.Path.Combine(a, "Resources");
		System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(b);

		List<DatabaseEntryScriptableObject> allObjects = new List<DatabaseEntryScriptableObject>();

		GetAllDirectories(ApplicationManager.Paths.Database, allObjects);

		allItems = allObjects.ToArray();*/
	}

	private void Start()
	{
		//FindObjectOfType<Pult>().PlaceDownTray(Instantiate(trayPrefab).GetComponent<Tray>());
	}

	public static DatabaseEntryScriptableObject GetItem(string id)
	{
		if (id == null || id.Trim() == "")
		{
			return null;
		}

		if (Instance.allItems == null)
		{
			//Debug.LogError("Invalid item type" + typeof(T).ToString());
			return null;
		}

		foreach (var item in Instance.allItems)
		{
			if (item.id == id)
			{
				return Instantiate(item);
			}
		}

		Debug.LogError("No " /*+ typeof(T).ToString()*/ + " with id " + id + " found.");
		return null;
	}

	public static DatabaseEntryScriptableObject[] GetAllItems()
	{
		if (Instance.allItems == null)
		{
			//Debug.LogError("Invalid item type" + typeof(T).ToString());
			return null;
		}

		return (DatabaseEntryScriptableObject[])Instance.allItems.Clone();
	}
}