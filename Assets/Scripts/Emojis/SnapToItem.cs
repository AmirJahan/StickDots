using UnityEngine;
using UnityEngine.UI;

public class SnapToItem : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public RectTransform sampleListItem;

    public VerticalLayoutGroup VLG;

    bool isSnapped;

    public float snapForce;
    float snapSpeed;

    private void Start()
    {
        isSnapped = false;
    }

    private void Update()
    {
        int currentItem = Mathf.RoundToInt((0 - contentPanel.localPosition.y / (sampleListItem.rect.height + VLG.spacing)));

        if (scrollRect.velocity.magnitude < 200 && !isSnapped)
        {
            scrollRect.velocity = Vector2.zero;
            snapSpeed += snapForce * Time.deltaTime;

            contentPanel.localPosition = new Vector3(
                contentPanel.localPosition.x,
                Mathf.MoveTowards(contentPanel.localPosition.y, 0 - (currentItem * (sampleListItem.rect.height + VLG.spacing)), snapSpeed),
                contentPanel.localPosition.z);

            if (contentPanel.localPosition.y == 0 - (currentItem * (sampleListItem.rect.height + VLG.spacing)))
            {
                isSnapped = true;
            }
        }

        if (scrollRect.velocity.magnitude > 200)
        {
            isSnapped = false;
            snapSpeed = 0;
        }
    }
}
