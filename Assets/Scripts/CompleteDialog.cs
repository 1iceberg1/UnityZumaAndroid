using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CompleteDialog : Dialog
{
	public Animator starsAnim;

	public Animator scoreAnim;

	public Text scoreText;

	public GameObject complete;

	public GameObject fail;

	public int numStars;

	public int score;

	public bool isWin;

    public AdmobController admob;

	protected override void Start()
	{
		base.Start();
		if (isWin)
		{
			StartCoroutine(StarAnimation());
		}
		StartCoroutine(ScoreAnimation());
		complete.SetActive(isWin);
		fail.SetActive(!isWin);
        admob=GameObject.Find("AdmobController").GetComponent<AdmobController>();
        
        admob.ShowAds();
    }

	private IEnumerator StarAnimation()
	{
		yield return new WaitForSeconds(0.7f);
		string trigger = (numStars == 0) ? null : ((numStars == 1) ? "one" : ((numStars != 2) ? "three" : "two"));
		if (trigger != null)
		{
			starsAnim.SetTrigger(trigger);
		}
	}

	private IEnumerator ScoreAnimation()
	{
		yield return new WaitForSeconds(0.4f);
		int count = 52;
		int step = score / count;
		int current = 0;
		while (current < score)
		{
			current += step;
			if (current > score)
			{
				current = score;
			}
			scoreText.text = current.ToString();
			yield return new WaitForSeconds(0.03f);
		}
		scoreAnim.SetTrigger("animate");
	}
}
