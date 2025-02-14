using System.Collections;
using System.Collections.Generic;
using CruFramework.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

public class SampleScrollGridItem : ScrollGridItem
{

	[SerializeField]
	private Image image = null;
	
	private  bool isSelected = false;
	
	private void UpdateColor()
	{
		image.color = isSelected ? Color.green : Color.white;
	}

	protected override void OnSetView(object value)
	{
		UpdateColor();
	}

	protected override void OnSelectedItem()
	{
		isSelected = true;
		UpdateColor();
	}

	protected override void OnDeselectItem()
	{
		isSelected = false;
		UpdateColor();
	}
}
