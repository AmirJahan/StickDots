using UnityEngine;
using VoxelBusters.EssentialKit;
using VoxelBusters.CoreLibrary;

public class SocialShare : MonoBehaviour
{
    [SerializeField] public string shareText = "";
    [SerializeField] private ShareSheet _shareSheet = null;

    //ont start create instance
    private void Start()
    {
    }

    private void OnShareSheetClosed(ShareSheetResult result, Error error)
    { 
        // reset instance
        _shareSheet = null;
    }

    //call on game end to automatically take screen shot
    public void TakeScreeShot()
    {
        if (_shareSheet != null) 
        {
            Debug.Log("add text and screenshot");
            _shareSheet.AddText(shareText);
            _shareSheet.AddScreenshot();
        }
    }

    //call when share button is clicked
    public void Share()
    {
        if (_shareSheet != null) { 
            Debug.Log("share");
            _shareSheet.Show();
        }
    }

    //Test Button function
    public void Test()
    {
        var newSheet = ShareSheet.CreateInstance();
        newSheet.SetCompletionCallback(OnShareSheetClosed);
        _shareSheet = newSheet;
        TakeScreeShot();
        Share();
    }
    
}
