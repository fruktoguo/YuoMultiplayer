using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;
using TMPro;

namespace YuoTools.UI
{

	public partial class View_HPSliderVariantComponent : UIComponent 
	{

		private TextMeshProUGUI mTextMeshProUGUI_HpText;

		public TextMeshProUGUI TextMeshProUGUI_HpText
		{
			get
			{
				if (mTextMeshProUGUI_HpText == null)
					mTextMeshProUGUI_HpText = rectTransform.Find("HP/CV_HpText").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_HpText;
			}
		}


		private TextMeshProUGUI mTextMeshProUGUI_MpText;

		public TextMeshProUGUI TextMeshProUGUI_MpText
		{
			get
			{
				if (mTextMeshProUGUI_MpText == null)
					mTextMeshProUGUI_MpText = rectTransform.Find("MP/CV_MpText").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_MpText;
			}
		}



		[FoldoutGroup("ALL")]

		public List<TextMeshProUGUI> all_TextMeshProUGUI = new();

		public void FindAll()
		{
				
			all_TextMeshProUGUI.Add(TextMeshProUGUI_HpText);
			all_TextMeshProUGUI.Add(TextMeshProUGUI_MpText);;


		}
	}}
