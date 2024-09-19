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
		public const string MultiplayerJoinRoom = "MultiplayerJoinRoom";
	}

	public partial class View_MultiplayerJoinRoomComponent : UIComponent 
	{

		public static View_MultiplayerJoinRoomComponent GetView() => UIManagerComponent.Get.GetUIView<View_MultiplayerJoinRoomComponent>();


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
					mButton_Close = rectTransform.Find("Item/Option/C_Close").GetComponent<Button>();
				return mButton_Close;
			}
		}


		private View_RoomItemComponent mChild_RoomItem;

		public View_RoomItemComponent Child_RoomItem
		{
			get
			{
				if (mChild_RoomItem == null)
				{
					mChild_RoomItem = Entity.AddChild<View_RoomItemComponent>();
					mChild_RoomItem.Entity.EntityName = "RoomItem";
					mChild_RoomItem.rectTransform = rectTransform.Find("Item/RoomList/Viewport/Content/D_RoomItem") as RectTransform;
					mChild_RoomItem.RunSystem<IUICreate>();
				}
				return mChild_RoomItem;
			}
		}


		[FoldoutGroup("ALL")]
		public List<RectTransform> all_RectTransform = new();

		[FoldoutGroup("ALL")]
		public List<Button> all_Button = new();

		[FoldoutGroup("ALL")]
		public List<View_RoomItemComponent> all_View_RoomItemComponent = new();

		public void FindAll()
		{
				
			all_RectTransform.Add(MainRectTransform);;
				
			all_Button.Add(Button_Mask);
			all_Button.Add(Button_Close);;
				
			all_View_RoomItemComponent.Add(Child_RoomItem);;

		}
	}}
