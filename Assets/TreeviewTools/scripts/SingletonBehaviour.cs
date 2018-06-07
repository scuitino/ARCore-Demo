using UnityEngine;

// This is a singleton
public class SingletonBehaviourKeepOld<T> : MonoBehaviour where T : SingletonBehaviourKeepOld<T>
{
	private static T _instance;
	public static T Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType<T>();
			}
			return _instance;
		}
	}


    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Singleton<"+typeof(T).Name+"> multiple instance > destroying gameobject.");
            Destroy(gameObject);
            return;
        }

        // Force Instantiation in Awake
        if (Instance == null)
        {
            Debug.LogError("Singleton cant instantiate! " + typeof(T).Name);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}


public class SingletonBehaviourKeepLast<T> : MonoBehaviour where T : SingletonBehaviourKeepLast<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }


    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Singleton<" + typeof(T).Name + "> multiple instance > destroying old gameobject.");
            Destroy(_instance.gameObject);
            _instance = FindObjectOfType<T>();
            Debug.LogError("Singleton<" + typeof(T).Name + "> new instance is: "+_instance.gameObject.name);
            return;
        }

        // Force Instantiation in Awake
        if (Instance == null)
        {
            Debug.LogError("Singleton cant instantiate! " + typeof(T).Name);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
