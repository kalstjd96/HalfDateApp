using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using TMPro;

    public class SetKeyBoard : MonoBehaviour
    {
        private Vector2 inputOriginalPosition;
        public RectTransform inputFieldRectTransform;

        //모바일 키보드 유무
        bool isKeyboard = false;
        private void Start()
        {
            inputOriginalPosition = inputFieldRectTransform.anchoredPosition;
        }

        private void Update()
        {
            if (TouchScreenKeyboard.visible)
            {
                isKeyboard = true;
                ApplyScrollPosition();
            }
            else
            {
                if (!isKeyboard)
                    return;

                isKeyboard = false;
                inputFieldRectTransform.anchoredPosition = inputOriginalPosition;
            }
        }

    public void ApplyScrollPosition()
    {
        int keyboardHeight = GetHeight(false);
        Vector2 newPosition = inputOriginalPosition;
        newPosition.y = keyboardHeight; // 키보드 높이를 더해주면 위로 이동
        //newPosition.y += keyboardHeight; // 키보드 높이를 더해주면 위로 이동
        inputFieldRectTransform.anchoredPosition = newPosition;
    }

    public static int GetHeight()
    {
        return GetHeight(false);
    }

    public static int GetHeight(bool includeInput)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
		using ( var unityClass = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" ) )
		{
			var currentActivity = unityClass.GetStatic<AndroidJavaObject>( "currentActivity" );
			var unityPlayer = currentActivity.Get<AndroidJavaObject>( "mUnityPlayer" );
			var view = unityPlayer.Call<AndroidJavaObject>( "getView" );

			if ( view == null ) return 0;

			int result;

			using ( var rect = new AndroidJavaObject( "android.graphics.Rect" ) )
			{
				view.Call( "getWindowVisibleDisplayFrame", rect );
				result = Screen.height - rect.Call<int>( "height" );
			}

			if ( !includeInput ) return result;

			var softInputDialog = unityPlayer.Get<AndroidJavaObject>( "mSoftInputDialog" );
			var window = softInputDialog?.Call<AndroidJavaObject>( "getWindow" );
			var decorView = window?.Call<AndroidJavaObject>( "getDecorView" );

			if ( decorView == null ) return result;

			var decorHeight = decorView.Call<int>( "getHeight" );
			result += decorHeight;

			return result;
		}
#else
        var area = TouchScreenKeyboard.area;
        var height = Mathf.RoundToInt(area.height);
        return Screen.height <= height ? 0 : height;
#endif
    }

}

