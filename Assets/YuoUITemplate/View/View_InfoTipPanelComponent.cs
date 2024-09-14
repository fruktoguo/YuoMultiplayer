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
		public const string InfoTipPanel = "InfoTipPanel";
	}
	public partial class View_InfoTipPanelComponent : UIComponent 
	{

		public static View_InfoTipPanelComponent GetView() => UIManagerComponent.Get.GetUIView<View_InfoTipPanelComponent>();


		private RectTransform mRectTransform_Item;

		public RectTransform RectTransform_Item
		{
			get
			{
				if (mRectTransform_Item == null)
					mRectTransform_Item = rectTransform.Find("C_Item").GetComponent<RectTransform>();
				return mRectTransform_Item;
			}
		}


		private TextMeshProUGUI mTextMeshProUGUI_Info;

		public TextMeshProUGUI TextMeshProUGUI_Info
		{
			get
			{
				if (mTextMeshProUGUI_Info == null)
					mTextMeshProUGUI_Info = rectTransform.Find("C_Item/InfoPanel/C_Info").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_Info;
			}
		}


		private ContentSizeFitter mContentSizeFitter_Info;

		public ContentSizeFitter ContentSizeFitter_Info
		{
			get
			{
				if (mContentSizeFitter_Info == null)
					mContentSizeFitter_Info = rectTransform.Find("C_Item/InfoPanel/C_Info").GetComponent<ContentSizeFitter>();
				return mContentSizeFitter_Info;
			}
		}



		[FoldoutGroup("ALL")]

		public List<RectTransform> all_RectTransform = new();

		[FoldoutGroup("ALL")]

		public List<TextMeshProUGUI> all_TextMeshProUGUI = new();

		[FoldoutGroup("ALL")]

		public List<ContentSizeFitter> all_ContentSizeFitter = new();

		public void FindAll()
		{
				
			all_RectTransform.Add(RectTransform_Item);;
				
			all_TextMeshProUGUI.Add(TextMeshProUGUI_Info);;
				
			all_ContentSizeFitter.Add(ContentSizeFitter_Info);;


		}
	}}
