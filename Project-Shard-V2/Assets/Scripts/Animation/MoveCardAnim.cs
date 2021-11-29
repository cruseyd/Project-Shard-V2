using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCardAnim : CardGameAnimation
{
    private bool _faceUp;
    private CardZoneUI _dest;
    private CardUI _card;

    public CardUI card { get { return _card; } }

    public MoveCardAnim(CardUI a_card, CardZoneUI a_zone, bool a_faceUp)
    {
        _faceUp = a_faceUp;
        _card = a_card;
        _dest = a_zone;
    }
    public override IEnumerator PlayNow()
    {
        CardZoneUI prevZone = _card.zone;
        _card.transform.SetParent(_dest.transform);
        prevZone?.Organize();
        _card.FaceUp(_faceUp);
        yield return _dest.DoOrganize();
        yield return new WaitForSeconds(CardGameParams.cardAnimationRate);
    }
}
