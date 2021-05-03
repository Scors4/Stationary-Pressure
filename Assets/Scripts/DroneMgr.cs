using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMgr : MonoBehaviour
{
    public static DroneMgr inst;

    List<UserDrone> userDrones = new List<UserDrone>();

    void Awake() {
        inst = this;
    }

    public void UserDroneSpawned(UserDrone newDrone) {
        if (!userDrones.Contains(newDrone))
            userDrones.Add(newDrone);
    }

    public void UserDroneDestroyed(UserDrone destroyed) {
        if (userDrones.Contains(destroyed))
            userDrones.Remove(destroyed);
    }

    public List<UserDrone> GetUserDrones() {
        return userDrones;
    }
}
