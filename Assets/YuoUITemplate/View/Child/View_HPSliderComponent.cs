using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;

namespace YuoTools.UI
{

	public partial class View_HPSliderComponent : UIComponent 
	{

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

		private Image mImage_BG;

		public Image Image_BG
		{
			get
			{
				if (mImage_BG == null)
					mImage_BG = rectTransform.Find("HP/C_BG").GetComponent<Image>();
				return mImage_BG;
			}
		}


		private Image mImage_Transition;

		public Image Image_Transition
		{
			get
			{
				if (mImage_Transition == null)
					mImage_Transition = rectTransform.Find("HP/C_Transition").GetComponent<Image>();
				return mImage_Transition;
			}
		}


		private Image mImage_Value;

		public Image Image_Value
		{
			get
			{
				if (mImage_Value == null)
					mImage_Value = rectTransform.Find("HP/C_Value").GetComponent<Image>();
				return mImage_Value;
			}
		}


		private Image mImage_MpValue;

		public Image Image_MpValue
		{
			get
			{
				if (mImage_MpValue == null)
					mImage_MpValue = rectTransform.Find("MP/C_MpValue").GetComponent<Image>();
				return mImage_MpValue;
			}
		}



		[FoldoutGroup("ALL")]

		public List<RectTransform> all_RectTransform = new();

		[FoldoutGroup("ALL")]

		public List<Image> all_Image = new();

		public void FindAll()
		{
				
			all_RectTransform.Add(MainRectTransform);;
				
			all_Image.Add(Image_BG);
			all_Image.Add(Image_Transition);
			all_Image.Add(Image_Value);
			all_Image.Add(Image_MpValue);;


		}
	}}
