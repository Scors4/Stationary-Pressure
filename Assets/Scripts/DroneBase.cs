using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBase : MonoBehaviour
{
    /// The transform
    new private Transform transform = null;
    /// The 3D velocity vector
    public Vector3 velocity = Vector3.zero;
    /// The 3D acceleration vector
    public Vector3 acceleration = Vector3.zero;
    
    /// The "weight" of the drone. 
    /// Changes how force is applied to the acceleration.
    /// We default to 1.0 to prevent divide-by-zero issues.
    // TODO: Consider making private to ensure weight is only changed by drone components.
    public float weight = 1.0f; 
    /// The amount of force a thruster can provide.
    public float ThrusterForce = 0.00000002f;
    
    /// The drone's target. The drone will attempt to approach the target until it gets within the `targetRadius`.
    public Transform Target = null;
    /// The radius at which the drone will try to maintain distance from the target.
    public float TargetRadius = 1.0f;
    /// The speed at which this drone can rotate.
    public float RotationSpeed = 10.0f;
    
    // /// Whether the thrusters are activated.
    // public bool thrustersActivated = false;
    
    /// A queue of commands to execute for the thrusters.
    private Queue<ThrusterCommand> thrusterCommands = new Queue<ThrusterCommand>();
    
    /// Get this drone's velocity
    public Vector3 GetVelocity() {
        return velocity;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        
        if(Target != null) {
            thrusterCommands.Enqueue(new RotateToPointThrusterCommand(Target.position));
            thrusterCommands.Enqueue(new ZeroVelocityThrusterCommand(this));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Fixed timestep
    void FixedUpdate() {
        // If there is a target, try to intercept
        if(Target != null) {
            // Vector3 targetPosition = Target.getPosition();
            // Vector3 targetPosition = Target.position;
            // Vector3 positionDiff = targetPosition - transform.position;
            
        } else {
            acceleration = Vector3.zero;
        }
        
        /// Update thruster commands
        while(thrusterCommands.Count > 0 && thrusterCommands.Peek().IsFinished(this)) {
            thrusterCommands.Dequeue();
        }
        
        if(thrusterCommands.Count > 0) {
            thrusterCommands.Peek().Tick(this);
        }
        
        velocity += acceleration * Time.fixedDeltaTime;
        transform.position += velocity * Time.fixedDeltaTime;
    }
}

/// A command that controls a drone's thrusters
interface ThrusterCommand {
    /// Perform a tick.
    public void Tick(DroneBase drone);
    
    /// Whether this is done executing.
    public bool IsFinished(DroneBase drone);
}

/// A command to rotate to point to a face a point
class RotateToPointThrusterCommand : ThrusterCommand {
    /// The target position
    private Vector3 target;
    
    public RotateToPointThrusterCommand(Vector3 target) {
        this.target = target;
    }
    
    /// Set the target while in motion.
    public void SetTarget(Vector3 target) {
        this.target = target;
    }
    
     /// Perform a tick
    public void Tick(DroneBase drone) {
        Vector3 positionDiff = target - drone.transform.position;
                
        // Update rotation
        // https://answers.unity.com/questions/254130/how-do-i-rotate-an-object-towards-a-vector3-point.html
        Quaternion lookRotation = Quaternion.LookRotation(positionDiff.normalized);
        
        // If the desired rot and rot are close, just set them equal
        // TODO: Do i need a Mathf.abs here?
        if(Quaternion.Angle(drone.transform.rotation, lookRotation) > 1.0) {
            drone.transform.rotation = Quaternion.Slerp(drone.transform.rotation, lookRotation, drone.RotationSpeed * Time.fixedDeltaTime);
        } else {
            drone.transform.rotation = lookRotation;
        }
    }
    
    /// Whether this is done executing.
    public bool IsFinished(DroneBase drone) {
        Vector3 positionDiff = target - drone.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(positionDiff.normalized);
        
        return drone.transform.rotation == lookRotation;
    }
}

/// Command a drone to zero its velocity
class ZeroVelocityThrusterCommand : ThrusterCommand {
    private RotateToPointThrusterCommand rotateCommand;
    
    public ZeroVelocityThrusterCommand(DroneBase drone) {
        rotateCommand = new RotateToPointThrusterCommand(drone.transform.position - drone.GetVelocity());
    }
    
    public void Tick(DroneBase drone) {
        rotateCommand.SetTarget(drone.transform.position - drone.GetVelocity());
        
        if(!rotateCommand.IsFinished(drone)) {
            rotateCommand.Tick(drone);
        } else if(drone.velocity != Vector3.zero) {
            // F = MA
            // A = F/M
            //
            // V(T) = V(0) + AT
            // V = AT
            //
            // V = FT/M
            // F = VM/T
            float desiredForce = Mathf.Clamp(drone.GetVelocity().magnitude * drone.weight / Time.fixedDeltaTime, 0.0f, drone.ThrusterForce);
            float acceleration1D = desiredForce / drone.weight; 
            drone.acceleration = drone.transform.forward.normalized * acceleration1D;
            
            if(Mathf.Abs(drone.velocity.magnitude) < 0.15) {
                drone.velocity = Vector3.zero;
                drone.acceleration = Vector3.zero;
            }
        }
    }
    
    public bool IsFinished(DroneBase drone) {
         return rotateCommand.IsFinished(drone) && drone.velocity == Vector3.zero;
    }
}