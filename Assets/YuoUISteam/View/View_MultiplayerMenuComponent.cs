using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;

namespace YuoTools.UI
{
	public static partial class ViewType
	{
		public const string MultiplayerMenu = "MultiplayerMenu";
	}

	public partial class View_MultiplayerMenuComponent : UIComponent 
	{

		public static View_MultiplayerMenuComponent GetView() => UIManagerComponent.Get.GetUIView<View_MultiplayerMenuComponent>();


		private RectTransform mainRectTransform;

		public RectTransform MainRectTransform
		{
			get
			{
				if (mainRectTransform == null)
					mainRectTransform = rectTransform.GetComponent<RectTransform>();
				return mainRectTransform;
			}
		}

		private Button mButton_Mask;

		public Button Button_Mask
		{
			get
			{
				if (mButton_Mask == null)
					mButton_Mask = rectTransform.Find("C_Mask").GetComponent<Button>();
				return mButton_Mask;
			}
		}


		private Button mButton_Close;

		public Button Button_Close
		{
			get
			{
				if (mButton_Close == null)
					mButton_Close = rectTransform.Find("Item/C_Close").GetComponent<Button>();
				return mButton_Close;
			}
		}


		private Button mButton_Create;

		public Button Button_Create
		{
			get
			{
				if (mButton_Create == null)
					mButton_Create = rectTransform.Find("Item/Option/C_Create").GetComponent<Button>();
				return mButton_Create;
			}
		}


		private Button mButton_Join;

		public Button Button_Join
		{
			get
			{
				if (mButton_Join == null)
					mButton_Join = rectTransform.Find("Item/Option/C_Join").GetComponent<Button>();
				return mButton_Join;
			}
		}



		[FoldoutGroup("ALL")]
		public List<RectTransform> all_RectTransform = new();

		[FoldoutGroup("ALL")]
		public List<Button> all_Button = new();

		public void FindAll()
		{
				
			all_RectTransform.Add(MainRectTransform);;
				
			all_Button.Add(Button_Mask);
			all_Button.Add(Button_Close);
			all_Button.Add(Button_Create);
			all_Button.Add(Button_Join);;

		}
	}}
