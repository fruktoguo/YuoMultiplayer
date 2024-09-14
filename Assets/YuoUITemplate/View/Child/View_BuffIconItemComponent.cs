using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;
using TMPro;

namespace YuoTools.UI
{

	public partial class View_BuffIconItemComponent : UIComponent 
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


		private Image mImage_Mask;

		public Image Image_Mask
		{
			get
			{
				if (mImage_Mask == null)
					mImage_Mask = rectTransform.Find("C_Mask").GetComponent<Image>();
				return mImage_Mask;
			}
		}


		private Image mImage_BuffCountBG;

		public Image Image_BuffCountBG
		{
			get
			{
				if (mImage_BuffCountBG == null)
					mImage_BuffCountBG = rectTransform.Find("C_BuffCountBG").GetComponent<Image>();
				return mImage_BuffCountBG;
			}
		}


		private TextMeshProUGUI mTextMeshProUGUI_BuffCount;

		public TextMeshProUGUI TextMeshProUGUI_BuffCount
		{
			get
			{
				if (mTextMeshProUGUI_BuffCount == null)
					mTextMeshProUGUI_BuffCount = rectTransform.Find("C_BuffCountBG/C_BuffCount").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_BuffCount;
			}
		}



		[FoldoutGroup("ALL")]

		public List<Image> all_Image = new();

		[FoldoutGroup("ALL")]

		public List<TextMeshProUGUI> all_TextMeshProUGUI = new();

		public void FindAll()
		{
				
			all_Image.Add(MainImage);
			all_Image.Add(Image_Icon);
			all_Image.Add(Image_Mask);
			all_Image.Add(Image_BuffCountBG);;
				
			all_TextMeshProUGUI.Add(TextMeshProUGUI_BuffCount);;


		}
	}}
