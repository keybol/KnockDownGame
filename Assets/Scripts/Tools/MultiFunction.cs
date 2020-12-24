using MoreMountains.NiceVibrations;
using System.Collections.Generic;
using UnityEngine;

public static class MultiFunction
{
	// This needs to be added to a public static class to be used like an extension
	public static void CopyToClipboard(this string s)
	{
		TextEditor te = new TextEditor();
		te.text = s;
		te.SelectAll();
		te.Copy();
	}

	public static void Shuffle<T>(this IList<T> ts)
	{
		var count = ts.Count;
		var last = count - 1;
		Random.seed = System.DateTime.Now.Millisecond;
		for (var i = 0; i < last; i += 1)
		{
			var r = UnityEngine.Random.Range(i, count);
			var tmp = ts[i];
			ts[i] = ts[r];
			ts[r] = tmp;
		}
	}

	public static Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position)
	{
		//Vector position (percentage from 0 to 1) considering camera size.
		//For example (0,0) is lower left, middle is (0.5,0.5)
		Vector2 temp = camera.WorldToViewportPoint(position);

		//Calculate position considering our percentage, using our canvas size
		//So if canvas size is (1100,500), and percentage is (0.5,0.5), current value will be (550,250)
		temp.x *= canvas.sizeDelta.x;
		temp.y *= canvas.sizeDelta.y;

		//The result is ready, but, this result is correct if canvas recttransform pivot is 0,0 - left lower corner.
		//But in reality its middle (0.5,0.5) by default, so we remove the amount considering cavnas rectransform pivot.
		//We could multiply with constant 0.5, but we will actually read the value, so if custom rect transform is passed(with custom pivot) , 
		//returned value will still be correct.

		temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
		temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

		return temp;
	}

	public static void DoVibrate(HapticTypes hapticType)
	{
		MMVibrationManager.Haptic(hapticType, false, true);
	}

	public static void LoadScene(string scenename)
	{
		//ScreenFadeManager.Instance.onFadeEndDelegate = () =>
		//{
		//	LoadingScreenManager.Instance.Show(true);
		//	SceneLoader.Instance.LoadScene(scenename);
		//};

		//SceneLoader.Instance.onLoadSceneProgressDeletage = (progress) =>
		//{
		//	LoadingScreenManager.Instance.UpdateProgress(progress);
		//};

		//SceneLoader.Instance.onLoadSceneEndDeletage = (progress) =>
		//{
		//	LoadingScreenManager.Instance.Hide();
		//	ScreenFadeManager.Instance.FadeIn();
		//};

		//ScreenFadeManager.Instance.FadeOut();
	}
}