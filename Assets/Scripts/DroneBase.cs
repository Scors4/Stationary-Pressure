using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBase : MonoBehaviour
{
    public int type = 0;
    public int id = 0;

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
    public float ThrusterForce = 0.2f;
    
    /// The drone's target
    public Transform Target = null;
    /// The speed at which this drone can rotate.
    public float RotationSpeed = 10.0f;
    /// The maximum speed
    public float MaxSpeed = 3.0f;
    
    /// The target rotation
    public Quaternion TargetRotation = Quaternion.identity;
    
    /// Whether the thrusters are activated.
    private bool thrustersActivated = false;
    /// The desired Thruster force
    private float desiredThrusterForce = 0.0f;
    
    /// A queue of commands to execute.
    private Queue<Command> thrusterCommands = new Queue<Command>();
    
    public GameObject LaserEffect;
    
    /// Get this drone's velocity
    public Vector3 GetVelocity() {
        return velocity;
    }
    
    /// Fire the thrusters. 
    /// This clamps the input internally and sets the thrusters to be updated when needed.
    public void FireThrusters(float force) {
        // F = MA
        // A = F/M
        desiredThrusterForce = Mathf.Clamp(force, 0.0f, ThrusterForce);
        
        thrustersActivated = desiredThrusterForce != 0.0f;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        
        // Test target intercept. Remove when this class is complete.
        if(Target != null) {
            
            // This is just to test how it reacts to previous velocities
            velocity.x += 1f;
            velocity.y += 0.5f;
            velocity.z += 1.0f;
            
            // Approach target and stop
            thrusterCommands.Enqueue(new ApproachTargetToRadiusCommand(Target));
            thrusterCommands.Enqueue(new ZeroVelocityCommand(this));
            thrusterCommands.Enqueue(new RotateToPointCommand(Target.transform.position));
            thrusterCommands.Enqueue(new MineAsteroidCommand(Target.gameObject.GetComponent<Asteroid>()));
        }
    }

    void OnEnable()
    {
        DroneMgr.inst.DroneSpawned(this);
    }

    void OnDisable()
    {
        DroneMgr.inst.DroneDestroyed(this);
    }

    // Update is called once per frame
    void Update() {}
    
    // Fixed timestep
    void FixedUpdate() {
        /// Update thruster commands
        while(thrusterCommands.Count > 0 && thrusterCommands.Peek().IsFinished(this)) {
            thrusterCommands.Dequeue();
        }
        
        if(thrusterCommands.Count > 0) {
            thrusterCommands.Peek().Tick(this);
        }
        
        // Rotation
        // If the desired rot and rot are close, just set them equal
        // TODO: Do i need a Mathf.abs here?
        if(Quaternion.Angle(transform.rotation, TargetRotation) > 1.0) {
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, RotationSpeed * Time.fixedDeltaTime);
        } else {
            transform.rotation = TargetRotation;
        }
        
        if(thrustersActivated) {
            float acceleration1D = desiredThrusterForce / weight; 
            acceleration = transform.forward.normalized * acceleration1D;
        }
        
        velocity = Vector3.ClampMagnitude(velocity + (acceleration * Time.fixedDeltaTime), MaxSpeed);
        transform.position += velocity * Time.fixedDeltaTime;
    }
    
    /// Whether the drone is at the target rotation
    public bool isAtTargetRotation() {
        return transform.rotation == TargetRotation;
    }
}

/// A command for a drone
interface Command {
    /// Perform a tick.
    public void Tick(DroneBase drone);
    
    /// Whether this is done executing.
    public bool IsFinished(DroneBase drone);
}

/// A command to rotate to point to a face a point
// TODO: Consider allowing dynamic positions, or make a command that handles dynamic positions.
class RotateToPointCommand : Command {
    /// The target position
    private Vector3 target;
    
    public RotateToPointCommand(Vector3 target) {
        this.target = target;
    }
    
    /// Set the target while in motion.
    public void SetTarget(Vector3 target) {
        this.target = target;
    }
    
     /// Perform a tick
    public void Tick(DroneBase drone) {
        updateTargetRotation(drone);
    }
    
    /// Whether this is done executing.
    public bool IsFinished(DroneBase drone) {
        updateTargetRotation(drone);
        return drone.isAtTargetRotation();
    }
    
    void updateTargetRotation(DroneBase drone) {
        Vector3 positionDiff = target - drone.transform.position;
                
        // Update rotation
        // https://answers.unity.com/questions/254130/how-do-i-rotate-an-object-towards-a-vector3-point.html
        drone.TargetRotation = Quaternion.LookRotation(positionDiff.normalized);
    }
}

/// Command a drone to zero its velocity
class ZeroVelocityCommand : Command {
    private RotateToPointCommand rotateCommand;
    
    public ZeroVelocityCommand(DroneBase drone) {
        rotateCommand = new RotateToPointCommand(drone.transform.position - drone.GetVelocity());
    }
    
    public void Tick(DroneBase drone) {
        rotateCommand.SetTarget(drone.transform.position - drone.GetVelocity());
        
        if(!rotateCommand.IsFinished(drone)) {
            drone.FireThrusters(0.0f);
            rotateCommand.Tick(drone);
            return;
        } 
        
        if(drone.velocity != Vector3.zero) {
            // F = MA
            // A = F/M
            //
            // V(T) = V(0) + AT
            // V = AT
            //
            // V = FT/M
            // F = VM/T
            float desiredForce = drone.GetVelocity().magnitude * drone.weight / Time.fixedDeltaTime;
            drone.FireThrusters(desiredForce);
            
            // Zero velocity to avoid dealing with "drifting".
            // This will lead to unexpected stops, but is far simpler to deal with.
            if(Mathf.Abs(drone.velocity.magnitude) < 0.15) {
                drone.FireThrusters(0.0f);
                drone.velocity = Vector3.zero;
                drone.acceleration = Vector3.zero;
            }
        }
    }
    
    public bool IsFinished(DroneBase drone) {
         return drone.velocity == Vector3.zero;
    }
}

/// A Command to approach a target. It will not stop when it reaches its destination.
class ApproachTargetToRadiusCommand : Command {
    private Transform target;
    private RotateToPointCommand rotateCommand;
    
    public ApproachTargetToRadiusCommand(Transform target) {
        this.target = target;
        rotateCommand = new RotateToPointCommand(target.position);
    }
    
    public void Tick(DroneBase drone) {
        rotateCommand.SetTarget(target.position);
        
        Vector3 positionDiff = target.position - drone.transform.position;
        float positionDiffMag = positionDiff.magnitude;
        
        if(!rotateCommand.IsFinished(drone)) {
            rotateCommand.Tick(drone);
            return;
        } 
        
        if(Mathf.Abs(positionDiffMag) > 5.0f) {
            // F = MA
            // A = F/M
            //
            // V(T) = V(0) + AT
            // V = AT
            //
            // x = VT + 0.5AT^2
            // 2(x - VT) = AT^2
            // A = 2(x - VT) / T^2
            // F/M = 2(x - VT) / T^2
            // F = M * 2(x - VT) / T^2
            Vector3 xVT = positionDiff - (drone.GetVelocity() * Time.fixedDeltaTime);
            float desiredForce = drone.weight * 2.0f * xVT.magnitude / (Time.fixedDeltaTime * Time.fixedDeltaTime);
            drone.FireThrusters(desiredForce);
        } else {
            drone.FireThrusters(0.0f);
        }
    }
    
    public bool IsFinished(DroneBase drone) {
        Vector3 positionDiff = drone.transform.position - target.position;
        return Mathf.Abs(positionDiff.magnitude) < 5.0f;
    }
}

/// A command to mine an asteroid
// TODO: Consider allowing dynamic positions, or make a command that handles dynamic positions.
class MineAsteroidCommand : Command {
    /// The target position
    private Asteroid target;
    
    public MineAsteroidCommand(Asteroid target) {
        this.target = target;
    }
    
     /// Perform a tick
    public void Tick(DroneBase drone) {
        target.Mine();
    }
    
    /// Whether this is done executing.
    public bool IsFinished(DroneBase drone) {
        return target.IsEmpty();
    }
}

/// Execute commands in parallel
class ParallelCommand : Command {
    Command command1;
    Command command2;
    
    public ParallelCommand(Command command1, Command command2) {
        this.command1 = command1;
        this.command2 = command2;
    }
    
    public void Tick(DroneBase drone) {
        command1.Tick(drone);
        command2.Tick(drone);
    }
    
    public bool IsFinished(DroneBase drone) {
        return command1.IsFinished(drone) && command2.IsFinished(drone);
    }
}

/// Execute thruster commands in a series
class SerialCommand : Command {
    Command command1;
    Command command2;
    
    int command = 0;
    
    public SerialCommand(Command command1, Command command2) {
        this.command1 = command1;
        this.command2 = command2;
    }
    
    public void Tick(DroneBase drone) {
        if(command == 0) command1.Tick(drone);
        if(command == 1) command2.Tick(drone);
    }
    
    public bool IsFinished(DroneBase drone) {
        if(command == 0 && command1.IsFinished(drone)) command += 1;
        if(command == 1 && command2.IsFinished(drone)) command += 1; 
        return command == 2;
    }
}