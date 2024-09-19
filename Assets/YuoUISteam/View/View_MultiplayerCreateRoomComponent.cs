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
		public const string MultiplayerCreateRoom = "MultiplayerCreateRoom";
	}

	public partial class View_MultiplayerCreateRoomComponent : UIComponent 
	{

		public static View_MultiplayerCreateRoomComponent GetView() => UIManagerComponent.Get.GetUIView<View_MultiplayerCreateRoomComponent>();


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


		private TMP_InputField mTMP_InputField_RoomName;

		public TMP_InputField TMP_InputField_RoomName
		{
			get
			{
				if (mTMP_InputField_RoomName == null)
					mTMP_InputField_RoomName = rectTransform.Find("Item/Option/RoomName/C_RoomName").GetComponent<TMP_InputField>();
				return mTMP_InputField_RoomName;
			}
		}


		private Toggle mToggle_UsePassword;

		public Toggle Toggle_UsePassword
		{
			get
			{
				if (mToggle_UsePassword == null)
					mToggle_UsePassword = rectTransform.Find("Item/Option/Password/C_UsePassword").GetComponent<Toggle>();
				return mToggle_UsePassword;
			}
		}


		private TMP_InputField mTMP_InputField_Password;

		public TMP_InputField TMP_InputField_Password
		{
			get
			{
				if (mTMP_InputField_Password == null)
					mTMP_InputField_Password = rectTransform.Find("Item/Option/Password/C_Password").GetComponent<TMP_InputField>();
				return mTMP_InputField_Password;
			}
		}


		private TMP_InputField mTMP_InputField_MaxPlayer;

		public TMP_InputField TMP_InputField_MaxPlayer
		{
			get
			{
				if (mTMP_InputField_MaxPlayer == null)
					mTMP_InputField_MaxPlayer = rectTransform.Find("Item/Option/MaxPlayer/C_MaxPlayer").GetComponent<TMP_InputField>();
				return mTMP_InputField_MaxPlayer;
			}
		}


		private Toggle mToggle_Public;

		public Toggle Toggle_Public
		{
			get
			{
				if (mToggle_Public == null)
					mToggle_Public = rectTransform.Find("Item/Option/Create/Public/C_Public").GetComponent<Toggle>();
				return mToggle_Public;
			}
		}


		private Button mButton_Create;

		public Button Button_Create
		{
			get
			{
				if (mButton_Create == null)
					mButton_Create = rectTransform.Find("Item/Option/Create/C_Create").GetComponent<Button>();
				return mButton_Create;
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


		private RawImage mRawImage_Avatar;

		public RawImage RawImage_Avatar
		{
			get
			{
				if (mRawImage_Avatar == null)
					mRawImage_Avatar = rectTransform.Find("Item/C_Avatar").GetComponent<RawImage>();
				return mRawImage_Avatar;
			}
		}



		[FoldoutGroup("ALL")]
		public List<RectTransform> all_RectTransform = new();

		[FoldoutGroup("ALL")]
		public List<Button> all_Button = new();

		[FoldoutGroup("ALL")]
		public List<TMP_InputField> all_TMP_InputField = new();

		[FoldoutGroup("ALL")]
		public List<Toggle> all_Toggle = new();

		[FoldoutGroup("ALL")]
		public List<RawImage> all_RawImage = new();

		public void FindAll()
		{
				
			all_RectTransform.Add(MainRectTransform);;
				
			all_Button.Add(Button_Mask);
			all_Button.Add(Button_Create);
			all_Button.Add(Button_Close);;
				
			all_TMP_InputField.Add(TMP_InputField_RoomName);
			all_TMP_InputField.Add(TMP_InputField_Password);
			all_TMP_InputField.Add(TMP_InputField_MaxPlayer);;
				
			all_Toggle.Add(Toggle_UsePassword);
			all_Toggle.Add(Toggle_Public);;
				
			all_RawImage.Add(RawImage_Avatar);;

		}
	}}
