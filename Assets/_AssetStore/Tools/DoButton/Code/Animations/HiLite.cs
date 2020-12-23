using DG.Tweening;
using UnityEngine;

namespace FuguFirecracker.UI
{
    public class HiLite : MonoBehaviour, IAnimatable
    {
        //Set In Inspector
        public Color32 HiLiteColor;
        public Color32 HiLiteToo;
        
        [Range(0.5f, 1.5f)] public float Growth;

        // assign a variable to our Tweener so that we can Kill and reassign as required
        private Tween _tween;

        // I want to capture the color I've assigned to the background Image only once,
        // so I use an initial dummy color to check against such that I don't assign
        // a transitional color to the sprite while the color is changing.
        private static readonly Color32 DummyColor = new Color32(42, 42, 42, 42);
        private Color32 _originalcolor = DummyColor;

        private bool _isOverButton;

		public void OnStart(DoButton doButton)
		{
			for(int i = 0; i < doButton.Images.Length; i++)
				_tween = doButton.Images[i].DOColor(new Color(1, 1, 1, 0.3f), 0.1f);
		}

        public void OnEnter(DoButton doButton)
        {
            _isOverButton = true;
            
            if (_originalcolor.Equals(DummyColor)) _originalcolor = doButton.Images[0].color;
            // For any other Handler to fire, OnEnter HAS to have occured,
            // Therefore we only need check for null here
            if (_tween != null && _tween.IsActive()) _tween.Kill();
			//_tween = doButton.Images[0].DOColor(HiLiteColor, 0.42f);
			for (int i = 0; i < doButton.Images.Length; i++)
				_tween = doButton.Images[i].DOColor(new Color(1, 1, 1, 0.3f), 0.1f);
		}

        public void OnExit(DoButton doButton)
        {
            _isOverButton = false;
            
            _tween.Kill();
			//_tween = doButton.Images[0].DOColor(_originalcolor, 0.2f);
			for (int i = 0; i < doButton.Images.Length; i++)
				_tween = doButton.Images[i].DOColor(new Color(1, 1, 1, 0.3f), 0.1f);
		}

        public void OnDown(DoButton doButton)
        {
			var growthVector = new Vector3(1 + Growth * 0.1f, 1 + Growth * 0.1f, 1);
			_tween.Kill();
			for (int i = 0; i < doButton.Images.Length; i++)
			{
				_tween = DOTween.Sequence()
					.Join(doButton.Images[i].transform.DOScale(growthVector, 0.2f))
					.Join(doButton.Images[i].DOColor(new Color(1, 1, 1, 1f), 0.1f));
				//.Join(doButton.Images[0].DOColor(HiLiteToo, 0.2f));
			}
		}

        // On up is special... We call back to doButton to check for a clickEvent
        // That is, if Up happened when the mouse (or finger) was over the button... That's a CLICK.
        // doButton will Pop() the Stack and Do() the Action
        public void OnUp(DoButton doButton)
        {
			_tween.Kill();
			for (int i = 0; i < doButton.Images.Length; i++)
			{
				_tween = DOTween.Sequence()
					.Join(doButton.Images[i].transform.DOScale(Vector3.one, 0.2f))
					.Join(doButton.Images[i].DOColor(new Color(1, 1, 1, 0.3f), 0.1f))
					.OnComplete(doButton.Execute);
				//.Join(doButton.Images[0].DOColor(DecideColor(), 0.2f))
			}
		}

        private Color32 DecideColor()
        {
            return _isOverButton ? HiLiteColor : _originalcolor;
        }

       
    }
}