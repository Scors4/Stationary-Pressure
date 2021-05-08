using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// A class to continually rotate an entity to make it seem more like an asteroid.
public class Asteroid : MonoBehaviour, DroneTargetable
{
    public ResourceSet Resources = new ResourceSet();
    public ResourceSet resourceRatios = new ResourceSet();
    
    // Start is called before the first frame update
    void Start() {
        resourceRatios.Uranium = Random.Range(0.01f, 0.3f);
        resourceRatios.Copper = Random.Range(0.01f, 0.3f);
        resourceRatios.Ice = Random.Range(0.01f, 0.1f);
        resourceRatios.Iron = 1.0f - (resourceRatios.Uranium + resourceRatios.Copper + resourceRatios.Ice);

        int total_resource = Random.Range(30, 150);

        Resources.Uranium = resourceRatios.Uranium * total_resource;
        Resources.Copper = resourceRatios.Copper * total_resource;
        Resources.Iron = resourceRatios.Iron * total_resource;
        Resources.Ice = resourceRatios.Ice * total_resource;


        AsteroidMgr.inst.AsteroidSpawned(this);   
    }

    // Update is called once per frame
    void Update() {}
    
    /// Get the asteroid's position
    public Vector3 getPosition() {
        return transform.position;
    }
    
    /// Try to mine this asteroid
    public void Mine(float power, DroneBase drone, UserDrone uDrone) {
        if (uDrone == null)
            return;

        if (Resources.Iron > 0.0f || Resources.Copper > 0.0f || Resources.Uranium > 0.0f || Resources.Copper > 0.0f) {

            float ironMined = Mathf.Clamp(power * resourceRatios.Iron, 0.0f, Resources.Iron);
            float copperMined = Mathf.Clamp(power * resourceRatios.Copper, 0.0f, Resources.Copper);
            float uraniumMined = Mathf.Clamp(power * resourceRatios.Uranium, 0.0f, Resources.Uranium);
            float iceMined = Mathf.Clamp(power * resourceRatios.Ice, 0.0f, Resources.Ice);

            // TODO: The player currently gains some extra resources from each "mine" op
            // since the iron levels in an asteroid can go below zero. Fix this.
            /*GameMgr.inst.Resources.Iron += ironMined;
            GameMgr.inst.Resources.Copper += copperMined;
            GameMgr.inst.Resources.Uranium += uraniumMined;
            GameMgr.inst.Resources.Ice += iceMined;*/

            uDrone.minedResources.Iron += ironMined;
            uDrone.minedResources.Copper += copperMined;
            uDrone.minedResources.Uranium += uraniumMined;
            uDrone.minedResources.Ice += iceMined;

            drone.AddResourcesStored(ironMined + copperMined + uraniumMined + iceMined);

            Resources.Iron -= ironMined;
            Resources.Copper -= copperMined;
            Resources.Uranium -= uraniumMined;
            Resources.Ice -= iceMined;
        }
        
        if(IsEmpty()) {
            AsteroidMgr.inst.AsteroidDestroyed(this);
            gameObject.GetComponent<Fracture>().FractureObject();
        }
    }
    
    /// Whether this asteroid is empty
    public bool IsEmpty() {
        // TODO: Check for all resources
        return Resources.Iron <= 0.0f && Resources.Copper <= 0.0f && Resources.Uranium <= 0.0f && Resources.Ice <= 0.0f;
    }
}
