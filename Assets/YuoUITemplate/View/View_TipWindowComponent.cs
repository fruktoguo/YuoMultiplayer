using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;
using TMPro;

namespace YuoTools.UI
{

	public static partial class ViewType
	{
		public const string TipWindow = "TipWindow";
	}
	public partial class View_TipWindowComponent : UIComponent 
	{

		public static View_TipWindowComponent GetView() => UIManagerComponent.Get.GetUIView<View_TipWindowComponent>();


		private TextMeshProUGUI mTextMeshProUGUI_Content;

		public TextMeshProUGUI TextMeshProUGUI_Content
		{
			get
			{
				if (mTextMeshProUGUI_Content == null)
					mTextMeshProUGUI_Content = rectTransform.Find("Item/BackGround/C_Content").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_Content;
			}
		}


		private Button mButton_Option;

		public Button Button_Option
		{
			get
			{
				if (mButton_Option == null)
					mButton_Option = rectTransform.Find("Item/BackGround/Option/C_Option").GetComponent<Button>();
				return mButton_Option;
			}
		}


		private TextMeshProUGUI mTextMeshProUGUI_Option;

		public TextMeshProUGUI TextMeshProUGUI_Option
		{
			get
			{
				if (mTextMeshProUGUI_Option == null)
					mTextMeshProUGUI_Option = rectTransform.Find("Item/BackGround/Option/C_Option/C_Option").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_Option;
			}
		}



		[FoldoutGroup("ALL")]

		public List<TextMeshProUGUI> all_TextMeshProUGUI = new();

		[FoldoutGroup("ALL")]

		public List<Button> all_Button = new();

		public void FindAll()
		{
				
			all_TextMeshProUGUI.Add(TextMeshProUGUI_Content);
			all_TextMeshProUGUI.Add(TextMeshProUGUI_Option);;
				
			all_Button.Add(Button_Option);;


		}
	}
}
