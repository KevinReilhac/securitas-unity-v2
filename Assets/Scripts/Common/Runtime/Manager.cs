
using UnityEngine;

/// <summary>
/// A manager is a singleton as a gameobject.
/// </summary>
/// <typeparam name="T">Singleton type</typeparam>
public class Manager<T> : MonoBehaviour where T : MonoBehaviour
{
	private const string MANAGERS_PARENT_NAME = "_MANAGERS_";

	protected static T _instance { get; set; }
	public static bool HasInstance => _instance != null;

	/// <summary>
	/// Access singleton instance through this propriety.
	/// </summary>
	public static T instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (T)FindObjectOfType(typeof(T));

				if (_instance == null)
				{
					CreateInstance();
				}
			}

			return _instance;
		}
	}

	/// <summary>
	/// Create and/or get the managers parent
	/// </summary>
	/// <returns></returns>
	private static Transform GetParent()
	{
		GameObject parent = GameObject.Find(MANAGERS_PARENT_NAME);

		if (parent == null)
		{
			parent = new GameObject(MANAGERS_PARENT_NAME);
			parent.transform.SetSiblingIndex(0);
		}
		return (parent.transform);
	}

	private void OnDestroy()
	{
		if (_instance == this)
			return;
	}

	protected virtual void Awake()
	{
		_instance = null;
		xAwake();
	}

	protected virtual void xAwake() { }

	public virtual void Init() { }

	public static void CreateInstance()
	{
		GameObject singletonObject = new GameObject();
		_instance = singletonObject.AddComponent<T>();
		singletonObject.name = typeof(T).ToString() + " (MANAGER)";
	}

	public static void Dispose()
	{
		Destroy(instance);
		_instance = null;
	}
}