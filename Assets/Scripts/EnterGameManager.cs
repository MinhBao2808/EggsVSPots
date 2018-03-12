using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class EnterGameManager : MonoBehaviour {
	public static EnterGameManager instance = null;
	private int bestScore;

	void Awake () {
		if (instance == null) {
			instance = this;
		}
		if (!FB.IsInitialized) {
			FB.Init (InitCallback, OnHideUnity);
		} else {
			FB.ActivateApp ();
		}
	}

	void Start() {
		Invoke ("CallAdsBanner", 0.01f);
		bestScore = PlayerPrefs.GetInt ("bestscore", bestScore);
	}

	private void InitCallback () {
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity (bool isGameShown) {
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void FBShare() {
		FB.ShareLink(
			new System.Uri("https://itunes.apple.com/us/app/eggs-vs-pot/id1255156868?ls=1&mt=8"),
			"Egg's N Pot\n" + "Can you beat my best score " + bestScore + "?",
			"Can you beat my best score " + bestScore + "?",
			new System.Uri("http://i.imgur.com/3a5LToh.png"),
			ShareCallback);
	}

	private void ShareCallback (IShareResult result) {
		if (result.Cancelled || !string.IsNullOrEmpty(result.Error)) {
			Debug.Log("ShareLink Error: "+result.Error);
		} else if (!string.IsNullOrEmpty(result.PostId)) {
			// Print post identifier of the shared content
			Debug.Log(result.PostId);
		} else {
			// Share succeeded without postID
			Debug.Log("ShareLink success!");
		}
	}



	void CallAdsBanner(){
		AdsController.instance.HideBanner ();
		AdsController.instance.RequestBanner ();
	}

	public void LoadGamePlay () {
		SceneManager.LoadScene (2);
	}

	public void GotoShop () {
		SceneManager.LoadScene (3);
	}

	public void OnRateGameClick(){
		Application.OpenURL ("itms-apps://itunes.apple.com/app/id1255156868");
	}
}
