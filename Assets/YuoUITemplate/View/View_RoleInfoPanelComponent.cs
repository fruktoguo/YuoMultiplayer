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
		public const string RoleInfoPanel = "RoleInfoPanel";
	}
	public partial class View_RoleInfoPanelComponent : UIComponent 
	{

		public static View_RoleInfoPanelComponent GetView() => UIManagerComponent.Get.GetUIView<View_RoleInfoPanelComponent>();


		private Image mImage_RoleInfo;

		public Image Image_RoleInfo
		{
			get
			{
				if (mImage_RoleInfo == null)
					mImage_RoleInfo = rectTransform.Find("C_RoleInfo").GetComponent<Image>();
				return mImage_RoleInfo;
			}
		}


		private VerticalLayoutGroup mVerticalLayoutGroup_RoleInfo;

		public VerticalLayoutGroup VerticalLayoutGroup_RoleInfo
		{
			get
			{
				if (mVerticalLayoutGroup_RoleInfo == null)
					mVerticalLayoutGroup_RoleInfo = rectTransform.Find("C_RoleInfo").GetComponent<VerticalLayoutGroup>();
				return mVerticalLayoutGroup_RoleInfo;
			}
		}


		private ContentSizeFitter mContentSizeFitter_RoleInfo;

		public ContentSizeFitter ContentSizeFitter_RoleInfo
		{
			get
			{
				if (mContentSizeFitter_RoleInfo == null)
					mContentSizeFitter_RoleInfo = rectTransform.Find("C_RoleInfo").GetComponent<ContentSizeFitter>();
				return mContentSizeFitter_RoleInfo;
			}
		}


		private Image mImage_Head;

		public Image Image_Head
		{
			get
			{
				if (mImage_Head == null)
					mImage_Head = rectTransform.Find("C_RoleInfo/RoleInfoPanel/HeadFrame/C_Head").GetComponent<Image>();
				return mImage_Head;
			}
		}


		private Image mImage_EquipPanel;

		public Image Image_EquipPanel
		{
			get
			{
				if (mImage_EquipPanel == null)
					mImage_EquipPanel = rectTransform.Find("C_RoleInfo/C_EquipPanel").GetComponent<Image>();
				return mImage_EquipPanel;
			}
		}


		private HorizontalLayoutGroup mHorizontalLayoutGroup_EquipPanel;

		public HorizontalLayoutGroup HorizontalLayoutGroup_EquipPanel
		{
			get
			{
				if (mHorizontalLayoutGroup_EquipPanel == null)
					mHorizontalLayoutGroup_EquipPanel = rectTransform.Find("C_RoleInfo/C_EquipPanel").GetComponent<HorizontalLayoutGroup>();
				return mHorizontalLayoutGroup_EquipPanel;
			}
		}


		private Image mImage_BuffPanel;

		public Image Image_BuffPanel
		{
			get
			{
				if (mImage_BuffPanel == null)
					mImage_BuffPanel = rectTransform.Find("C_RoleInfo/C_BuffPanel").GetComponent<Image>();
				return mImage_BuffPanel;
			}
		}


		private GridLayoutGroup mGridLayoutGroup_BuffPanel;

		public GridLayoutGroup GridLayoutGroup_BuffPanel
		{
			get
			{
				if (mGridLayoutGroup_BuffPanel == null)
					mGridLayoutGroup_BuffPanel = rectTransform.Find("C_RoleInfo/C_BuffPanel").GetComponent<GridLayoutGroup>();
				return mGridLayoutGroup_BuffPanel;
			}
		}


		private ContentSizeFitter mContentSizeFitter_BuffPanel;

		public ContentSizeFitter ContentSizeFitter_BuffPanel
		{
			get
			{
				if (mContentSizeFitter_BuffPanel == null)
					mContentSizeFitter_BuffPanel = rectTransform.Find("C_RoleInfo/C_BuffPanel").GetComponent<ContentSizeFitter>();
				return mContentSizeFitter_BuffPanel;
			}
		}


		private View_RoleInfoNumComponent mChild_RoleInfoNum_Atk;

		public View_RoleInfoNumComponent Child_RoleInfoNum_Atk
		{
			get
			{
				if (mChild_RoleInfoNum_Atk == null)
				{
					mChild_RoleInfoNum_Atk = Entity.AddChild<View_RoleInfoNumComponent>();
					mChild_RoleInfoNum_Atk.Entity.EntityName = "RoleInfoNum_Atk";
					mChild_RoleInfoNum_Atk.rectTransform = rectTransform.Find("C_RoleInfo/RoleInfoPanel/CapacityPanel/D_RoleInfoNum_Atk") as RectTransform;
					mChild_RoleInfoNum_Atk.RunSystem<IUICreate>();
				}
				return mChild_RoleInfoNum_Atk;
			}
		}

		private View_RoleInfoNumComponent mChild_RoleInfoNum_Mag;

		public View_RoleInfoNumComponent Child_RoleInfoNum_Mag
		{
			get
			{
				if (mChild_RoleInfoNum_Mag == null)
				{
					mChild_RoleInfoNum_Mag = Entity.AddChild<View_RoleInfoNumComponent>();
					mChild_RoleInfoNum_Mag.Entity.EntityName = "RoleInfoNum_Mag";
					mChild_RoleInfoNum_Mag.rectTransform = rectTransform.Find("C_RoleInfo/RoleInfoPanel/CapacityPanel/D_RoleInfoNum_Mag") as RectTransform;
					mChild_RoleInfoNum_Mag.RunSystem<IUICreate>();
				}
				return mChild_RoleInfoNum_Mag;
			}
		}

		private View_RoleInfoNumComponent mChild_RoleInfoNum_AttackSpeed;

		public View_RoleInfoNumComponent Child_RoleInfoNum_AttackSpeed
		{
			get
			{
				if (mChild_RoleInfoNum_AttackSpeed == null)
				{
					mChild_RoleInfoNum_AttackSpeed = Entity.AddChild<View_RoleInfoNumComponent>();
					mChild_RoleInfoNum_AttackSpeed.Entity.EntityName = "RoleInfoNum_AttackSpeed";
					mChild_RoleInfoNum_AttackSpeed.rectTransform = rectTransform.Find("C_RoleInfo/RoleInfoPanel/CapacityPanel/D_RoleInfoNum_AttackSpeed") as RectTransform;
					mChild_RoleInfoNum_AttackSpeed.RunSystem<IUICreate>();
				}
				return mChild_RoleInfoNum_AttackSpeed;
			}
		}

		private View_RoleInfoNumComponent mChild_RoleInfoNum_AttackRange;

		public View_RoleInfoNumComponent Child_RoleInfoNum_AttackRange
		{
			get
			{
				if (mChild_RoleInfoNum_AttackRange == null)
				{
					mChild_RoleInfoNum_AttackRange = Entity.AddChild<View_RoleInfoNumComponent>();
					mChild_RoleInfoNum_AttackRange.Entity.EntityName = "RoleInfoNum_AttackRange";
					mChild_RoleInfoNum_AttackRange.rectTransform = rectTransform.Find("C_RoleInfo/RoleInfoPanel/CapacityPanel/D_RoleInfoNum_AttackRange") as RectTransform;
					mChild_RoleInfoNum_AttackRange.RunSystem<IUICreate>();
				}
				return mChild_RoleInfoNum_AttackRange;
			}
		}

		private View_RoleInfoNumComponent mChild_RoleInfoNum_Armor;

		public View_RoleInfoNumComponent Child_RoleInfoNum_Armor
		{
			get
			{
				if (mChild_RoleInfoNum_Armor == null)
				{
					mChild_RoleInfoNum_Armor = Entity.AddChild<View_RoleInfoNumComponent>();
					mChild_RoleInfoNum_Armor.Entity.EntityName = "RoleInfoNum_Armor";
					mChild_RoleInfoNum_Armor.rectTransform = rectTransform.Find("C_RoleInfo/RoleInfoPanel/CapacityPanel/D_RoleInfoNum_Armor") as RectTransform;
					mChild_RoleInfoNum_Armor.RunSystem<IUICreate>();
				}
				return mChild_RoleInfoNum_Armor;
			}
		}

		private View_RoleInfoNumComponent mChild_RoleInfoNum_MagicResist;

		public View_RoleInfoNumComponent Child_RoleInfoNum_MagicResist
		{
			get
			{
				if (mChild_RoleInfoNum_MagicResist == null)
				{
					mChild_RoleInfoNum_MagicResist = Entity.AddChild<View_RoleInfoNumComponent>();
					mChild_RoleInfoNum_MagicResist.Entity.EntityName = "RoleInfoNum_MagicResist";
					mChild_RoleInfoNum_MagicResist.rectTransform = rectTransform.Find("C_RoleInfo/RoleInfoPanel/CapacityPanel/D_RoleInfoNum_MagicResist") as RectTransform;
					mChild_RoleInfoNum_MagicResist.RunSystem<IUICreate>();
				}
				return mChild_RoleInfoNum_MagicResist;
			}
		}

		private View_RoleInfoNumComponent mChild_RoleInfoNum_Critical;

		public View_RoleInfoNumComponent Child_RoleInfoNum_Critical
		{
			get
			{
				if (mChild_RoleInfoNum_Critical == null)
				{
					mChild_RoleInfoNum_Critical = Entity.AddChild<View_RoleInfoNumComponent>();
					mChild_RoleInfoNum_Critical.Entity.EntityName = "RoleInfoNum_Critical";
					mChild_RoleInfoNum_Critical.rectTransform = rectTransform.Find("C_RoleInfo/RoleInfoPanel/CapacityPanel/D_RoleInfoNum_Critical") as RectTransform;
					mChild_RoleInfoNum_Critical.RunSystem<IUICreate>();
				}
				return mChild_RoleInfoNum_Critical;
			}
		}

		private View_RoleInfoNumComponent mChild_RoleInfoNum_CriticalMultiply;

		public View_RoleInfoNumComponent Child_RoleInfoNum_CriticalMultiply
		{
			get
			{
				if (mChild_RoleInfoNum_CriticalMultiply == null)
				{
					mChild_RoleInfoNum_CriticalMultiply = Entity.AddChild<View_RoleInfoNumComponent>();
					mChild_RoleInfoNum_CriticalMultiply.Entity.EntityName = "RoleInfoNum_CriticalMultiply";
					mChild_RoleInfoNum_CriticalMultiply.rectTransform = rectTransform.Find("C_RoleInfo/RoleInfoPanel/CapacityPanel/D_RoleInfoNum_CriticalMultiply") as RectTransform;
					mChild_RoleInfoNum_CriticalMultiply.RunSystem<IUICreate>();
				}
				return mChild_RoleInfoNum_CriticalMultiply;
			}
		}

		private View_EquipItemComponent mChild_EquipItem_1;

		public View_EquipItemComponent Child_EquipItem_1
		{
			get
			{
				if (mChild_EquipItem_1 == null)
				{
					mChild_EquipItem_1 = Entity.AddChild<View_EquipItemComponent>();
					mChild_EquipItem_1.Entity.EntityName = "EquipItem_1";
					mChild_EquipItem_1.rectTransform = rectTransform.Find("C_RoleInfo/C_EquipPanel/D_EquipItem_1") as RectTransform;
					mChild_EquipItem_1.RunSystem<IUICreate>();
				}
				return mChild_EquipItem_1;
			}
		}

		private View_EquipItemComponent mChild_EquipItem_2;

		public View_EquipItemComponent Child_EquipItem_2
		{
			get
			{
				if (mChild_EquipItem_2 == null)
				{
					mChild_EquipItem_2 = Entity.AddChild<View_EquipItemComponent>();
					mChild_EquipItem_2.Entity.EntityName = "EquipItem_2";
					mChild_EquipItem_2.rectTransform = rectTransform.Find("C_RoleInfo/C_EquipPanel/D_EquipItem_2") as RectTransform;
					mChild_EquipItem_2.RunSystem<IUICreate>();
				}
				return mChild_EquipItem_2;
			}
		}

		private View_EquipItemComponent mChild_EquipItem_3;

		public View_EquipItemComponent Child_EquipItem_3
		{
			get
			{
				if (mChild_EquipItem_3 == null)
				{
					mChild_EquipItem_3 = Entity.AddChild<View_EquipItemComponent>();
					mChild_EquipItem_3.Entity.EntityName = "EquipItem_3";
					mChild_EquipItem_3.rectTransform = rectTransform.Find("C_RoleInfo/C_EquipPanel/D_EquipItem_3") as RectTransform;
					mChild_EquipItem_3.RunSystem<IUICreate>();
				}
				return mChild_EquipItem_3;
			}
		}

		private View_EquipItemComponent mChild_EquipItem_4;

		public View_EquipItemComponent Child_EquipItem_4
		{
			get
			{
				if (mChild_EquipItem_4 == null)
				{
					mChild_EquipItem_4 = Entity.AddChild<View_EquipItemComponent>();
					mChild_EquipItem_4.Entity.EntityName = "EquipItem_4";
					mChild_EquipItem_4.rectTransform = rectTransform.Find("C_RoleInfo/C_EquipPanel/D_EquipItem_4") as RectTransform;
					mChild_EquipItem_4.RunSystem<IUICreate>();
				}
				return mChild_EquipItem_4;
			}
		}

		private View_EquipItemComponent mChild_EquipItem_5;

		public View_EquipItemComponent Child_EquipItem_5
		{
			get
			{
				if (mChild_EquipItem_5 == null)
				{
					mChild_EquipItem_5 = Entity.AddChild<View_EquipItemComponent>();
					mChild_EquipItem_5.Entity.EntityName = "EquipItem_5";
					mChild_EquipItem_5.rectTransform = rectTransform.Find("C_RoleInfo/C_EquipPanel/D_EquipItem_5") as RectTransform;
					mChild_EquipItem_5.RunSystem<IUICreate>();
				}
				return mChild_EquipItem_5;
			}
		}

		private View_EquipItemComponent mChild_EquipItem_6;

		public View_EquipItemComponent Child_EquipItem_6
		{
			get
			{
				if (mChild_EquipItem_6 == null)
				{
					mChild_EquipItem_6 = Entity.AddChild<View_EquipItemComponent>();
					mChild_EquipItem_6.Entity.EntityName = "EquipItem_6";
					mChild_EquipItem_6.rectTransform = rectTransform.Find("C_RoleInfo/C_EquipPanel/D_EquipItem_6") as RectTransform;
					mChild_EquipItem_6.RunSystem<IUICreate>();
				}
				return mChild_EquipItem_6;
			}
		}

		private View_BuffIconItemComponent mChild_BuffIconItem;

		public View_BuffIconItemComponent Child_BuffIconItem
		{
			get
			{
				if (mChild_BuffIconItem == null)
				{
					mChild_BuffIconItem = Entity.AddChild<View_BuffIconItemComponent>();
					mChild_BuffIconItem.Entity.EntityName = "BuffIconItem";
					mChild_BuffIconItem.rectTransform = rectTransform.Find("C_RoleInfo/C_BuffPanel/D_BuffIconItem") as RectTransform;
					mChild_BuffIconItem.RunSystem<IUICreate>();
				}
				return mChild_BuffIconItem;
			}
		}

		private View_HPSliderComponent mChild_HPSlider;

		public View_HPSliderComponent Child_HPSlider
		{
			get
			{
				if (mChild_HPSlider == null)
				{
					mChild_HPSlider = Entity.AddChild<View_HPSliderComponent>();
					mChild_HPSlider.Entity.EntityName = "HPSlider";
					mChild_HPSliderVariant = mChild_HPSlider.AddComponent<View_HPSliderVariantComponent>();
					mChild_HPSlider.rectTransform = rectTransform.Find("C_RoleInfo/HpSliderPanel/DV_HPSlider") as RectTransform;
					mChild_HPSliderVariant.rectTransform = mChild_HPSlider.rectTransform;
					mChild_HPSlider.Entity.RunSystem<IUICreate>();;
				}
				return mChild_HPSlider;
			}
		}

		private View_HPSliderVariantComponent mChild_HPSliderVariant;

		public View_HPSliderVariantComponent Child_HPSliderVariant
		{
			get
			{
				return mChild_HPSliderVariant;
			}
		}


		[FoldoutGroup("ALL")]

		public List<Image> all_Image = new();

		[FoldoutGroup("ALL")]

		public List<VerticalLayoutGroup> all_VerticalLayoutGroup = new();

		[FoldoutGroup("ALL")]

		public List<ContentSizeFitter> all_ContentSizeFitter = new();

		[FoldoutGroup("ALL")]

		public List<HorizontalLayoutGroup> all_HorizontalLayoutGroup = new();

		[FoldoutGroup("ALL")]

		public List<GridLayoutGroup> all_GridLayoutGroup = new();

		[FoldoutGroup("ALL")]

		public List<View_RoleInfoNumComponent> all_View_RoleInfoNumComponent = new();

		[FoldoutGroup("ALL")]

		public List<View_EquipItemComponent> all_View_EquipItemComponent = new();

		[FoldoutGroup("ALL")]

		public List<View_BuffIconItemComponent> all_View_BuffIconItemComponent = new();

		[FoldoutGroup("ALL")]

		public List<View_HPSliderComponent> all_View_HPSliderComponent = new();

		public void FindAll()
		{
				
			all_Image.Add(Image_RoleInfo);
			all_Image.Add(Image_Head);
			all_Image.Add(Image_EquipPanel);
			all_Image.Add(Image_BuffPanel);;
				
			all_VerticalLayoutGroup.Add(VerticalLayoutGroup_RoleInfo);;
				
			all_ContentSizeFitter.Add(ContentSizeFitter_RoleInfo);
			all_ContentSizeFitter.Add(ContentSizeFitter_BuffPanel);;
				
			all_HorizontalLayoutGroup.Add(HorizontalLayoutGroup_EquipPanel);;
				
			all_GridLayoutGroup.Add(GridLayoutGroup_BuffPanel);;
				
			all_View_RoleInfoNumComponent.Add(Child_RoleInfoNum_Atk);
			all_View_RoleInfoNumComponent.Add(Child_RoleInfoNum_Mag);
			all_View_RoleInfoNumComponent.Add(Child_RoleInfoNum_AttackSpeed);
			all_View_RoleInfoNumComponent.Add(Child_RoleInfoNum_AttackRange);
			all_View_RoleInfoNumComponent.Add(Child_RoleInfoNum_Armor);
			all_View_RoleInfoNumComponent.Add(Child_RoleInfoNum_MagicResist);
			all_View_RoleInfoNumComponent.Add(Child_RoleInfoNum_Critical);
			all_View_RoleInfoNumComponent.Add(Child_RoleInfoNum_CriticalMultiply);;
				
			all_View_EquipItemComponent.Add(Child_EquipItem_1);
			all_View_EquipItemComponent.Add(Child_EquipItem_2);
			all_View_EquipItemComponent.Add(Child_EquipItem_3);
			all_View_EquipItemComponent.Add(Child_EquipItem_4);
			all_View_EquipItemComponent.Add(Child_EquipItem_5);
			all_View_EquipItemComponent.Add(Child_EquipItem_6);;
				
			all_View_BuffIconItemComponent.Add(Child_BuffIconItem);;
				
			all_View_HPSliderComponent.Add(Child_HPSlider);;


		}
	}}
