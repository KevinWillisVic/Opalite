using UnityEngine;

namespace FishAndChips
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		#region -- Properties --
		
		public static T Instance
		{
			get
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						var singletonType = typeof(T);
						_instance = (T)Object.FindAnyObjectByType(typeof(T));
						if (_instance == null)
						{
							GameObject singletonObject = new GameObject();
							_instance = singletonObject.AddComponent<T>();
							singletonObject.name = $"Singleton {typeof(T).ToString()}";
							DontDestroyOnLoad(singletonObject);
						}
					}
					return _instance;
				}
			}
		}
		
		#endregion

		#region -- Public Member Vars --
		public static bool ApplicationIsQuiting;
		#endregion

		#region -- Protected Member Vars --
		protected static bool _initialized;
		protected static T _instance;
		protected static object _lock = new object();
		#endregion

		#region -- Private Methods --
		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this as T;
			}
			else if (_instance != this as T)
			{
				Logger.LogError($"Multiple Singletons of type {typeof(T)} found.");
				Destroy(this);
				return;
			}

			if (IsMarkedAsDontDestroyOnLoad() == true)
			{
				DontDestroyOnLoad(this);
			}
		}
		#endregion

		#region -- Protected Methods --
		protected virtual void OnDestroy()
		{
			Cleanup();
		}

		protected virtual void OnApplicationQuit()
		{
			ApplicationIsQuiting = true;
		}
		#endregion

		#region -- Public Methods --
		public virtual void Initialize()
		{
			_initialized = true;
		}

		public virtual void Cleanup()
		{
			if (_instance != null)
			{
				if (_instance == this as T)
				{
					_instance = null;
				}
			}
		}

		public virtual bool IsMarkedAsDontDestroyOnLoad()
		{
			return true;
		}

		public static bool HasSingletonInstance()
		{
			return _instance != null; 
		}

		public static T TryGetSingleton()
		{
			if (_instance == null)
			{
				_instance = (T)Object.FindAnyObjectByType(typeof(T));
			}
			return _instance;
		}
		#endregion
	}
}
