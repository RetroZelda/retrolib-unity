using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class SuggestionButton : MonoBehaviour 
{
	[SerializeField]
	private Text _Text;

	private Button _Butt;

	public string Text { get { return _Text.text; } set { _Text.text = value; } }
	public Button Butt { get { return _Butt; } }

	public Action<SuggestionButton> OnButtSlapped;

	protected void Awake()
	{
		_Butt = GetComponent<Button>();
		_Butt.onClick.AddListener(OnButtClicked);
	}

	private void OnButtClicked()
	{
		if(OnButtSlapped != null)
		{
			OnButtSlapped.Invoke(this);
		}
	}
}
