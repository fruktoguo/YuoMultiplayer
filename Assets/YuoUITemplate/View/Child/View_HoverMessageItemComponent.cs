using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;
using TMPro;

namespace YuoTools.UI
{

	public partial class View_HoverMessageItemComponent : UIComponent 
	{

		private Image mainImage;

		public Image MainImage
		{
			get
			{
				if (mainImage == null)
					mainImage = rectTransform.GetComponent<Image>();
				return mainImage;
			}
		}

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

		private CanvasGroup mainCanvasGroup;

		public CanvasGroup MainCanvasGroup
		{
			get
			{
				if (mainCanvasGroup == null)
					mainCanvasGroup = rectTransform.GetComponent<CanvasGroup>();
				return mainCanvasGroup;
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


		private TextMeshProUGUI mTextMeshProUGUI_Text;

		public TextMeshProUGUI TextMeshProUGUI_Text
		{
			get
			{
				if (mTextMeshProUGUI_Text == null)
					mTextMeshProUGUI_Text = rectTransform.Find("C_Text").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_Text;
			}
		}


		private ContentSizeFitter mContentSizeFitter_Text;

		public ContentSizeFitter ContentSizeFitter_Text
		{
			get
			{
				if (mContentSizeFitter_Text == null)
					mContentSizeFitter_Text = rectTransform.Find("C_Text").GetComponent<ContentSizeFitter>();
				return mContentSizeFitter_Text;
			}
		}



		[FoldoutGroup("ALL")]
		public List<Image> all_Image = new();

		[FoldoutGroup("ALL")]
		public List<HorizontalLayoutGroup> all_HorizontalLayoutGroup = new();

		[FoldoutGroup("ALL")]
		public List<ContentSizeFitter> all_ContentSizeFitter = new();

		[FoldoutGroup("ALL")]
		public List<CanvasGroup> all_CanvasGroup = new();

		[FoldoutGroup("ALL")]
		public List<TextMeshProUGUI> all_TextMeshProUGUI = new();

		public void FindAll()
		{
				
			all_Image.Add(MainImage);
			all_Image.Add(Image_Icon);;
				
			all_HorizontalLayoutGroup.Add(MainHorizontalLayoutGroup);;
				
			all_ContentSizeFitter.Add(MainContentSizeFitter);
			all_ContentSizeFitter.Add(ContentSizeFitter_Text);;
				
			all_CanvasGroup.Add(MainCanvasGroup);;
				
			all_TextMeshProUGUI.Add(TextMeshProUGUI_Text);;

		}
	}}
