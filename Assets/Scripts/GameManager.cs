using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class GameManager : MonoBehaviour {
	[SerializeField] private AudioClip eggjump;
	[SerializeField] private AudioClip gameover;
	[SerializeField] private AudioSource gameSound;
	[SerializeField] private Text resultScoretext;
	[SerializeField] private GameObject gameOverMenu;
	[SerializeField] private Text bestScoreText;
	[SerializeField] private Text textScore;
	[SerializeField] private Sprite[] eggsSprite;
	[SerializeField] private SpriteRenderer gameObjectSprite;

	public static GameManager instance = null;
	private int gamePoint = 0;
	public int bestScore;
	private bool gameOver = false;
	private bool eggFall = false;
	private int eggColorIndex;
	private bool playAgain = false;
	private bool playerPlayGame = false;
	static int timeToPlay;

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

	void Start () {
		eggColorIndex = PlayerPrefs.GetInt ("eggName", eggColorIndex);
		gameObjectSprite.sprite = eggsSprite [eggColorIndex];
		bestScore = PlayerPrefs.GetInt ("bestscore", bestScore);
		//		Invoke ("CallAdsBanner", 0.01f);
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


//	void CallAdsBanner(){
//		AdsController.instance.HideBanner ();
//		AdsController.instance.RequestBanner ();
//	}
	public bool GameOver {
		get { return gameOver; }
	}

	public int GamePoint {
		get { return gamePoint;}
	}

	public bool EggFall {
		get { return eggFall; }
	}

	public bool PlayerPlayGame {
		get { return playerPlayGame; }
	}

	public void CaclulatePoint () {
		gamePoint = gamePoint + 1;
		textScore.text = "" + gamePoint;
		if (bestScore < gamePoint) {
			bestScore = gamePoint;
			PlayerPrefs.SetInt ("bestscore", bestScore);
		}
	}

	public void EggDied () {
		gameOver = true;
		gameOverMenu.SetActive (true);
		gameSound.PlayOneShot (gameover);
		resultScoretext.text = "" + gamePoint;
		bestScoreText.text = "" + bestScore;
		timeToPlay++;
		if (timeToPlay % 8 == 0) {
			UnityAdsController.instance.ShowSkipAd ();
		}
		else if (timeToPlay % 8 == 4) {
			AdsController.instance.ShowInterstitial ();
			AdsController.instance.RequestInterstitial ();
		}
	}

	public void PlayGameAgain () {
		
		PlayerPrefs.SetInt ("colorIndex", eggColorIndex);
		SceneManager.LoadScene (2);
	}

	public void GotoShop() {
		SceneManager.LoadScene (3);
	}

	public void EggFalling () {
		eggFall = true;
	}

	public void EggJumping () {
		eggFall = false;
		gameSound.PlayOneShot (eggjump);
	}

	public void PlayGame() {
		playerPlayGame = true;
	}

	public void OnRateGameClick(){
		Application.OpenURL ("itms-apps://itunes.apple.com/app/id1255156868");
	}
}
