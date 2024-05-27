using UnityEngine;
using UnityEngine.EventSystems;

public class InGameUIpopUp : MonoBehaviour, IPointerClickHandler // 이건 플레이어한테 달아줄 것
{
    [SerializeField] PlayerClickUI playerClickUI;
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerClickUI ui = Manager.UI.ShowInGameUI(playerClickUI);
        ui.SetTarget(transform);
        ui.SetOffset(new Vector3(0, 150, 0));
    }
    // 이거 다음에 플레이어쪽 viewID 보는곳에다 플레이어 레이어 찍고 그 ViewID 가 내가 아니면 이 UI 뜨게 하면되고,
    // 또 그버튼 누르면 매니저의 tradeUserID 저 Viewid들어오게 하고, 내 ViewID 보내서 거래창 켜는 것과 동시에 상대의 tradeUserUD 를 내 Viewid로 바꾸는 버튼
    // 거래UI까지만 뜬 다음엔 양쪽다 수락이면 각 템 교환되게 하는 스크립트도 필요하네 왤케 복잡해
}
