using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardGameInput
{
    public enum Type
    {
        DEFAULT,
        CLICK,
        DOUBLE_CLICK,
        BEGIN_DRAG,
        CONTINUE_DRAG,
        END_DRAG,
        BEGIN_HOVER,
        END_HOVER,
        RIGHT_CLICK,
        CANCEL,
        CONFIRM
    }

    public Type type { get; private set; }
    public ITargetUI target { get; private set; }

    public List<ITarget>  hoveredTargets { get; private set; }
    public List<DropZone> dropZones { get; private set; }
    public CardGameInput(Type a_type, Transform a_target, PointerEventData a_eventData)
    {
        hoveredTargets = new List<ITarget>();
        dropZones = new List<DropZone>();
        if (a_eventData != null)
        {
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(a_eventData, hits);
            foreach (RaycastResult hit in hits)
            {
                ITargetUI target = hit.gameObject.GetComponent<ITargetUI>();
                if (target != null) { hoveredTargets.Add(target.data); }
                DropZone zone = hit.gameObject.GetComponent<DropZone>();
                if (zone != null) { dropZones.Add(zone); }
            }
        }
        
        type = a_type;
        if (a_target == null) { return; }
        ITargetUI target_ui = a_target.gameObject.GetComponent<ITargetUI>();
        if (target_ui != null)
        {
            target = target_ui;
        }
    }

    public bool Hovering(DropZone.ID a_zoneID)
    {
        foreach (DropZone zone in dropZones)
        {
            if (zone.id == a_zoneID) { return true; }
        }
        if (a_zoneID == DropZone.ID.DEFAULT) { return true; }
        return false;
    }
}
