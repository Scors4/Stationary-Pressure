using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DroneStatFields
{
    HEALTH,
    POWER,
    FUEL,
    STORAGE
}

public struct DroneStatSet
{
    public float Health;
    public float Power;
    public float Fuel;
    public float Storage;
}

public enum OWNERS
{
    PLAYER,
    RAIDER,
    NEUTRAL
}

public class DroneBase : MonoBehaviour {
    DroneStatSet droneStats;
    DroneStatSet currentStats;

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
    public float thrusterForce = 1f;
    
    /// The speed at which this drone can rotate.
    public float rotationSpeed = 10.0f;
    /// The maximum speed
    public float maxSpeed = 3.0f;
    
    /// The target rotation
    public Quaternion targetRotation = Quaternion.identity;
    
    /// Whether the thrusters are activated.
    private bool thrustersActivated = false;
    /// The desired Thruster force
    private float desiredThrusterForce = 0.0f;
    
    /// A queue of commands to execute.
    private Queue<Command> commands = new Queue<Command>();
    
    public GameObject laserEffect;
    
    public float health = 100.0f;
    public float damage = 3.0f;

    OWNERS owner;
    public bool isRadarHidden = false;
    
    /// Get this drone's position
    public Vector3 GetPosition() {
        return transform.position;
    }
    
    /// Get this drone's velocity
    public Vector3 GetVelocity() {
        return velocity;
    }
    
    /// Fire the thrusters. 
    /// This clamps the input internally and sets the thrusters to be updated when needed.
    public void FireThrusters(float force) {
        // F = MA
        // A = F/M
        desiredThrusterForce = Mathf.Clamp(force, 0.0f, thrusterForce);
        
        thrustersActivated = desiredThrusterForce != 0.0f;
    }
    
    // Start is called before the first frame update
    void Start() 
    {
        currentStats = new DroneStatSet();
    }

    public void SetDroneStats(DroneStatSet stats) {
        droneStats = stats;
        currentStats.Health = stats.Health;
    }

    // Update is called once per frame
    void Update() {}
    
    // Fixed timestep
    void FixedUpdate() {
        /// Update thruster commands
        while (!isIdle() && commands.Peek().IsFinished(this)) 
            commands.Dequeue();
        
        if (!isIdle()) 
            commands.Peek().Tick(this);
        
        
        // Rotation
        // If the desired rot and rot are close, just set them equal
        // TODO: Do i need a Mathf.abs here?
        if(Quaternion.Angle(transform.rotation, targetRotation) > 1.0) {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        } else {
            transform.rotation = targetRotation;
        }
        
        if(thrustersActivated) {
            float acceleration1D = desiredThrusterForce / weight; 
            acceleration = transform.forward.normalized * acceleration1D;
        }
        
        velocity = Vector3.ClampMagnitude(velocity + (acceleration * Time.fixedDeltaTime), maxSpeed);
        transform.position += velocity * Time.fixedDeltaTime;
    }
    
    /// Whether the drone is at the target rotation
    public bool isAtTargetRotation() {
        return transform.rotation == targetRotation;
    }
    
    /// Whether this drone is idle
    public bool isIdle() {
        return commands.Count == 0;
    }
    
    /// Add a command to be executed
    public void addCommand(Command c) {
        commands.Enqueue(c);
    }
    
    /// Clear all current commands
    public void ClearCommands() {
        commands.Clear();
    }
    
    public void DoDamage(float power) {
        health -= power;
        
        // Delegate the actual reaction to negative health to the caller since the caller needs to handle deregistering the user/raider drone.
    }

    public void SetOwner(OWNERS newOwner)
    {
        owner = newOwner;
    }

    public OWNERS GetOwner()
    {
        return owner;
    }

    public DroneStatSet GetDroneMaxStats()
    {
        return droneStats;
    }

    public DroneStatSet GetDroneCurrentStats()
    {
        return currentStats;
    }

    public void AddResourcesStored(float amount)
    {
        currentStats.Storage = Mathf.Clamp(currentStats.Storage + amount, 0.0f, droneStats.Storage);
    }

    public bool HasStorageAvailable()
    {
        return (droneStats.Storage - currentStats.Storage) > 0.0f;
    }
}

/// A command for a drone
public interface Command {
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
        drone.targetRotation = Quaternion.LookRotation(positionDiff.normalized);
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
            drone.FireThrusters(drone.thrusterForce);
            
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
    
    private float radius = 10.0f;
    
    public ApproachTargetToRadiusCommand(Transform target) {
        this.target = target;
        rotateCommand = new RotateToPointCommand(target.position);
    }
    
    public void Tick(DroneBase drone) {
        if(target == null) return;
        
        rotateCommand.SetTarget(target.position);
        
        Vector3 positionDiff = target.position - drone.transform.position;
        float positionDiffMag = positionDiff.magnitude;
        
        if(!rotateCommand.IsFinished(drone)) {
            rotateCommand.Tick(drone);
            return;
        } 
        
       float velocityMag = drone.velocity.magnitude;
       float timeToIntercept = positionDiffMag / velocityMag;
       float maxAcceleration = drone.thrusterForce / drone.weight;
       float timeToSlow = drone.velocity.magnitude / maxAcceleration;
       float timeToTarget = (positionDiffMag + radius + 10.0f) / drone.velocity.magnitude;
        
        if(timeToSlow < timeToTarget) {
            drone.FireThrusters(drone.thrusterForce);
        } else {
            drone.FireThrusters(0.0f);
        }
    }
    
    public bool IsFinished(DroneBase drone) {
        if(target == null) return true;
        
        Vector3 positionDiff = drone.transform.position - target.position;
        float maxAcceleration = drone.thrusterForce / drone.weight;
        return Mathf.Abs(positionDiff.magnitude) < radius;
    }
}

/// A command to mine an asteroid
class MineAsteroidCommand : Command {
    /// The target position
    private Asteroid target;
    
    public MineAsteroidCommand(Asteroid target) {
        this.target = target;
    }
    
     /// Perform a tick
    public void Tick(DroneBase drone) {
        UserDrone uDrone = drone.GetComponent<UserDrone>();
        if (uDrone == null)
            return;

        if(drone.HasStorageAvailable())
            target.Mine(drone.damage, drone, uDrone);
    }
    
    /// Whether this is done executing.
    public bool IsFinished(DroneBase drone) {
        return target.IsEmpty();
    }
}

/// A Command to pursue a target. It will not stop when it reaches its destination.
class PursueTargetToRadiusCommand : Command {
    private DroneBase target;
    private RotateToPointCommand rotateCommand;
    
    private float radius = 25.0f;
    
    public PursueTargetToRadiusCommand(DroneBase target) {
        this.target = target;
        rotateCommand = new RotateToPointCommand(target.transform.position);
    }
    
    public void Tick(DroneBase drone) {
        if(target == null) return;
        
        rotateCommand.SetTarget(target.transform.position);
        
        Vector3 positionDiff = target.transform.position - drone.transform.position;
        float positionDiffMag = positionDiff.magnitude;
        
        if(!rotateCommand.IsFinished(drone)) {
            rotateCommand.Tick(drone);
            return;
        } 
        
       float velocityMag = drone.velocity.magnitude;
       float timeToIntercept = positionDiffMag / velocityMag;
       float maxAcceleration = drone.thrusterForce / drone.weight;
       float timeToSlow = drone.velocity.magnitude / maxAcceleration;
       float timeToTarget = (positionDiffMag + radius + 10.0f) / drone.velocity.magnitude;
        
        if(timeToSlow < timeToTarget) {
            drone.FireThrusters(drone.thrusterForce);
        } else {
            drone.FireThrusters(0.0f);
        }
    }
    
    public bool IsFinished(DroneBase drone) {
        return target == null;
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