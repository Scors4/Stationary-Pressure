using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Something that can be targeted by a drone.
public interface DroneTargetable {
    public Vector3 getPosition();
}
