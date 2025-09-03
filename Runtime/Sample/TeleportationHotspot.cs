using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using MMUCAVE;
using UnityEngine.Serialization;

public class TeleportationHotspot : InteractionObject
{
    [FormerlySerializedAs("objectToTeleport")]
    [Tooltip("A reference to the CAVE")]
    [SerializeField] private GameObject CAVE;

    [Tooltip("The rotation to teleport to relative to this GameObject")]
    [SerializeField] private Vector3 rotationOffset;

    public override void OnTouch()
    {
        if (CAVE == null)
        {
            Debug.LogError("Object to teleport is null");
            return;
        }

        CAVE.transform.position = transform.position;
        CAVE.transform.rotation = Quaternion.Euler(rotationOffset);

    }

}
