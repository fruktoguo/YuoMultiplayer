using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;
using TMPro;

namespace YuoTools.UI
{

	public partial class View_NumTipComponent : UIComponent 
	{

		private HorizontalLayoutGroup mainHorizontalLayoutGroup;

		public HorizontalLayoutGroup MainHorizontalLayoutGroup
		{
			get
			{
				if (mainHorizontalLayoutGroup == null)
					mainHorizontalLayoutGroup = rectTransform.GetComponent<HorizontalLayoutGroup>();
				return mainHorizontalLayoutGroup;
			}
		}

		private ContentSizeFitter mainContentSizeFitter;

		public ContentSizeFitter MainContentSizeFitter
		{
			get
			{
				if (mainContentSizeFitter == null)
					mainContentSizeFitter = rectTransform.GetComponent<ContentSizeFitter>();
				return mainContentSizeFitter;
			}
		}

		private TextMeshProUGUI mTextMeshProUGUI_Num;

		public TextMeshProUGUI TextMeshProUGUI_Num
		{
			get
			{
				if (mTextMeshProUGUI_Num == null)
					mTextMeshProUGUI_Num = rectTransform.Find("C_Num").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_Num;
			}
		}


		private ContentSizeFitter mContentSizeFitter_Num;

		public ContentSizeFitter ContentSizeFitter_Num
		{
			get
			{
				if (mContentSizeFitter_Num == null)
					mContentSizeFitter_Num = rectTransform.Find("C_Num").GetComponent<ContentSizeFitter>();
				return mContentSizeFitter_Num;
			}
		}


		private Image mImage_Icon;

		public Image Image_Icon
		{
			get
			{
				if (mImage_Icon == null)
					mImage_Icon = rectTransform.Find("C_Icon").GetComponent<Image>();
				return mImage_Icon;
			}
		}



		[FoldoutGroup("ALL")]

		public List<HorizontalLayoutGroup> all_HorizontalLayoutGroup = new();

		[FoldoutGroup("ALL")]

		public List<ContentSizeFitter> all_ContentSizeFitter = new();

		[FoldoutGroup("ALL")]

		public List<TextMeshProUGUI> all_TextMeshProUGUI = new();

		[FoldoutGroup("ALL")]

		public List<Image> all_Image = new();

		public void FindAll()
		{
				
			all_HorizontalLayoutGroup.Add(MainHorizontalLayoutGroup);;
				
			all_ContentSizeFitter.Add(MainContentSizeFitter);
			all_ContentSizeFitter.Add(ContentSizeFitter_Num);;
				
			all_TextMeshProUGUI.Add(TextMeshProUGUI_Num);;
				
			all_Image.Add(Image_Icon);;


		}
	}}
