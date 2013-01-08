using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void ObjectLoaderCallback(AssetBundle obj);

public class AssetLoader : MonoBehaviour {
	
	private static int MAX_THREADS = 10;
	private static AssetLoader _instance = null;
	
	private struct Callback {
		public ObjectLoaderCallback callback;
		public string url;
	}
	
	private struct CacheRecord {
		public AssetBundle obj;
		public string url;
	}
	
	private static List<WWW> _loaders = new List<WWW>();
	private static List<CacheRecord> _cache = new List<CacheRecord>();
	private static List<Callback> _callback = new List<Callback>();
	private static List<Callback> _waiting = new List<Callback>();
	
	public void load(string fileName, ObjectLoaderCallback callback) {
		for (int i = 0; i < _cache.Count; i++) {
			if (_cache[i].url == fileName) {

				if (callback != null) {
					try {
						callback(_cache[i].obj);
					} catch {
						Debug.Log("AssetLoader: callback error for file " + fileName);
					}
				}
				return;
			}
		}
		
		for (int i = 0; i < _loaders.Count; i++) {
			if (_loaders[i].url == fileName) {
				if (callback != null) {
					Callback rec = new Callback();
					rec.callback = callback;
					rec.url = fileName;
					_callback.Add(rec);
				}
				return;
			}
		}
		for (int i = 0; i < _waiting.Count; i++) {
			if (_waiting[i].url == fileName) {
				if (callback != null) {
					Callback rec = new Callback();
					rec.callback = callback;
					rec.url = fileName;
					_callback.Add(rec);
				}
				return;
			}
		}
		
		StartCoroutine(wwwLoad(fileName, callback));
	}
	
	private IEnumerator wwwLoad(string fileName, ObjectLoaderCallback callback) {
		if (_loaders.Count >= MAX_THREADS) {
			Callback waitingRec;
			waitingRec.callback = callback;
			waitingRec.url = fileName;
			_waiting.Add(waitingRec);
			return true;
		}

		WWW www = new WWW(fileName);
		_loaders.Add(www);
		yield return www;
		_loaders.Remove(www);
		
		AssetBundle assetData = null;
		if (www.error != null) {
			Debug.LogWarning("AssetLoader: " + www.error);
		} else {
			assetData = www.assetBundle;
			CacheRecord cacheRec = new CacheRecord();
			cacheRec.url = www.url;
			cacheRec.obj = assetData;
			_cache.Add(cacheRec);
		}
		
		if (callback != null) {
			try {
				callback(assetData);
			} catch {
				Debug.Log("AssetLoader: callback error for file " + fileName);
			}
		}
		
		for (int i = 0; i < _callback.Count; i++) {
			if (_callback[i].url == fileName) {
				try {
					_callback[i].callback(assetData);
				} catch {
					Debug.Log("AssetLoader: callback error for file " + fileName);
				}
				_callback.RemoveAt(i);
				i--;
			}
		}
		
		if (_waiting.Count > 0) {
			Callback waitingRec = _waiting[0];
			_waiting.RemoveAt(0);
			StartCoroutine(wwwLoad(waitingRec.url, waitingRec.callback));
		}
	}
	
	public static AssetLoader instance {
		get {
			if(_instance == null) {
				GameObject asl = new GameObject("AssetLoader");
				DontDestroyOnLoad(asl);
				_instance = asl.AddComponent<AssetLoader>();
			}
			
			return _instance;
		}
	}
}
