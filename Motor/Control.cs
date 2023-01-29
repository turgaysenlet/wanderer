
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedTypes
{
        #region Robot Control
        [Serializable()]
        public class ControlRequestCls : ICloneable
        {
            /// <summary>
            /// Maximum steering angle in degrees.
            /// </summary>
            public const float MaximumSteeringAngle = 30.0f;
            /// <summary>
            /// Maximum forward travel speed in meters per second.
            /// </summary>
            public const float MaximumForwardSpeed = 4.1f;
            /// <summary>
            /// Maximum reverse travel speed in meters per second.
            /// </summary>
            public const float MaximumReverseSpeed = -4.1f;
            /// <summary>
            /// Minimum forward travel speed in meters per second.
            /// Below this value motor does not work properly.
            /// </summary>
            public const float MinimumSpeed = 0.2f;
            /// <summary>
            /// Normal travel speed in meters per second.
            /// </summary>
            public const float NormalSpeed = 1.0f;
            /// <summary>
            /// Slow travel speed in meters per second.
            /// </summary>
            public const float SlowSpeed = 0.45f;
            /// <summary>
            /// Extra slow travel speed in meters per second.
            /// </summary>
            public const float ExtraSlowSpeed = 0.28f;
            /// <summary>
            /// Fast travel speed in meters per second.
            /// </summary>
            public const float FastSpeed = 2.0f;
            /// <summary>
            /// Extra fast travel speed in meters per second. Not suitable for indoors.
            /// </summary>
            public const float ExtraFastSpeed = 4.0f;
            /// <summary>
            /// Stop speed.
            /// </summary>
            public const float StopSpeed = 0.0f;

            #region Ignition
            /// <summary>
            /// Ignition step 1 speed in meters per second. 
            /// Can be too high, do not use it for running the robot.
            /// Use it only for less than 1 seconds.
            /// </summary>
            public const float IgnitionStep1Speed = 0.0f;
            /// <summary>
            /// Ignition step 2 speed in meters per second. 
            /// Can be too high, do not use it for running the robot.
            /// Use it only for less than 1 seconds.
            /// </summary>
            public const float IgnitionStep2Speed = -4;
            /// <summary>
            /// Ignition step 3 speed in meters per second. 
            /// Can be too high, do not use it for running the robot.
            /// Use it only for less than 1 seconds.
            /// </summary>
            public const float IgnitionStep3Speed = 0.0f;
            #endregion

            private bool requestSteeringAngleChange = false;
            public bool SteeringAngleChangeRequested { get { return requestSteeringAngleChange; } }
            private float steeringAngle = 0;
            /// <summary>
            /// Desired steering angle in degrees. Left is negative, Right is positive.
            /// Values exceeding the limits are truncated to the limits, see MaximumSteeringAngle.
            /// </summary>
            public float SteeringAngle
            {
                get { return steeringAngle; }
                set
                {
                    if (value <= MaximumSteeringAngle)
                    {
                        if (value >= -MaximumSteeringAngle)
                        {
                            steeringAngle = value;
                            requestSteeringAngleChange = true;
                            IdleSteeringServos = false;
                        }
                        else
                        {
                            steeringAngle = -MaximumSteeringAngle;
                            requestSteeringAngleChange = true;
                            IdleSteeringServos = false;
                        }
                    }
                    else
                    {
                        steeringAngle = MaximumSteeringAngle;
                        requestSteeringAngleChange = true;
                        IdleSteeringServos = false;
                    }
                }
            }
            private bool requestSpeedChange = false;
            public bool SpeedChangeRequested { get { return requestSpeedChange; } }
            /// <summary>
            /// Speed in meters per second.
            /// </summary>
            private float speed = 0;
            /// <summary>
            /// Desired travel speed in meters per second. Forward is positive, Reverse is negative.
            /// Values exceeding the limits are truncated to the limits, see MaximumForwardSpeed and MaximumReverseSpeed.
            /// Values below MinimumSpeed are set to zero.
            /// </summary>
            public float SpeedMeterPerSecond
            {
                get { return speed; }
                set
                {
                    if (speed == ControlRequestCls.IgnitionStep1Speed || speed == ControlRequestCls.IgnitionStep2Speed || speed == ControlRequestCls.IgnitionStep3Speed)
                    {
                        speed = value;
                        requestSpeedChange = true;
                    }
                    else if (value <= MaximumForwardSpeed)
                    {
                        if (value >= MaximumReverseSpeed)
                        {
                            if (value < MinimumSpeed && value > -MinimumSpeed)
                            {
                                speed = 0;
                                requestSpeedChange = true;
                            }
                            else
                            {
                                speed = value;
                                requestSpeedChange = true;
                            }
                        }
                        else
                        {
                            // Do not set unsure speeds to maximum, just set it to fast
                            speed = -FastSpeed;
                            requestSpeedChange = true;
                        }
                    }
                    else
                    {
                        // Do not set unsure speeds to maximum, just set it to fast
                        speed = FastSpeed;
                        requestSpeedChange = true;
                    }
                }
            }
            /// Desired travel speed in miles per hour. Forward is positive, Reverse is negative.
            /// Values exceeding the limits are truncated to the limits, see MaximumForwardSpeed and MaximumReverseSpeed in meters per second.
            /// Values below MinimumSpeed are set to zero.
            public float SpeedMilesPerHour
            {
                get { return speed * 2.2236936292054402f; }
                set
                {
                    SpeedMeterPerSecond = value / 2.2236936292054402f;
                }
            }
            /// Desired travel speed in kilometers per hour. Forward is positive, Reverse is negative.
            /// Values exceeding the limits are truncated to the limits, see MaximumForwardSpeed and MaximumReverseSpeed in meters per second.
            /// Values below MinimumSpeed are set to zero.
            public float SpeedKilometersPerHour
            {
                get { return speed * 3.6f; }
                set
                {
                    SpeedMeterPerSecond = value / 3.6f;
                }
            }
            private bool requestServo1AngleChange = false;
            public bool Servo1AngleChangeRequested { get { return requestServo1AngleChange; } }
            private float servo1Angle = 0;
            /// <summary>
            /// Desired servo 1 angle in degrees. Left is negative, Right is positive.
            /// </summary>
            public float Servo1Angle
            {
                get { return servo1Angle; }
                set
                {
                    servo1Angle = SetAngle(value);
                    requestServo1AngleChange = true;
                    IdleServo1 = false;
                }
            }

            private float SetAngle(float value)
            {
                float angle = value;
                angle %= 360.0f;
                return angle;
            }
            private bool requestServo2AngleChange = false;
            public bool Servo2AngleChangeRequested { get { return requestServo2AngleChange; } }
            private float servo2Angle = 0;
            /// <summary>
            /// Desired servo 2 angle in degrees. Left is negative, Right is positive.
            /// </summary>
            public float Servo2Angle
            {
                get { return servo2Angle; }
                set
                {
                    servo2Angle = SetAngle(value);
                    requestServo2AngleChange = true;
                    IdleServo2 = false;
                }
            }
            private bool requestServo3AngleChange = false;
            public bool Servo3AngleChangeRequested { get { return requestServo3AngleChange; } }
            private float servo3Angle = 0;
            /// <summary>
            /// Desired servo 2 angle in degrees. Left is negative, Right is positive.
            /// </summary>
            public float Servo3Angle
            {
                get { return servo2Angle; }
                set
                {
                    servo3Angle = SetAngle(value);
                    requestServo3AngleChange = true;
                    IdleServo3 = false;
                }
            }
            public bool IdleServo1 { get; set; }
            public bool IdleServo2 { get; set; }
            public bool IdleServo3 { get; set; }
            public bool IdleSteeringServos { get; set; }

            private bool runMotor = false;
            public bool RunMotor { get { return runMotor; } set { runMotor = value; } }

            #region ICloneable Members

            public object Clone()
            {
                ControlRequestCls c = new ControlRequestCls();
                c.IdleServo1 = IdleServo1;
                c.IdleServo2 = IdleServo2;
                c.IdleServo3 = IdleServo3;
                c.IdleSteeringServos = IdleSteeringServos;
                c.requestServo1AngleChange = requestServo1AngleChange;
                c.requestServo2AngleChange = requestServo2AngleChange;
                c.requestServo3AngleChange = requestServo3AngleChange;
                c.requestSpeedChange = requestSpeedChange;
                c.requestSteeringAngleChange = requestSteeringAngleChange;
                c.runMotor = runMotor;
                c.servo1Angle = servo1Angle;
                c.servo2Angle = servo2Angle;
                c.servo3Angle = servo3Angle;
                c.speed = speed;
                c.steeringAngle = steeringAngle;
                return c;
            }

            #endregion
        }

        [Serializable()]
        public class VehicleStatusCls : ICloneable
        {
            private static float cutOffSpeed = 0.01f;
            /// <summary>
            /// Speed to be treated as zero.
            /// </summary>
            public static float CutOffSpeed
            {
                get { return cutOffSpeed; }
                set { cutOffSpeed = value; }
            }

            private static float speedConstant = 0.004f;
            /// <summary>
            /// Conversion from revolutions to meters per second.
            /// Unit is 1/meters
            /// </summary>
            public static float SpeedConstant
            {
                get { return speedConstant; }
                set { speedConstant = value; }
            }
            private static bool smoothSpeed = true;
            public static bool SmoothSpeed
            {
                get { return smoothSpeed; }
                set { smoothSpeed = value; }
            }
            private static float alpha = 0.4f;
            public static float Alpha
            {
                get { return alpha; }
                set { alpha = value; }
            }
            private DateTime leftPreviousUpdateTime = DateTime.MinValue;
            public DateTime LeftPreviousUpdateTime
            {
                get { return leftPreviousUpdateTime; }
            }
            private DateTime leftCurrentUpdateTime = DateTime.MinValue;
            public DateTime LeftCurrentUpdateTime
            {
                get { return leftCurrentUpdateTime; }
            }
            private int leftEncoderCount = 0;
            public int LeftEncoderCount
            {
                get { return leftEncoderCount; }
                set
                {
                    leftPreviousUpdateTime = leftCurrentUpdateTime;
                    previousLeftEncoderCount = leftEncoderCount;
                    leftCurrentUpdateTime = DateTime.Now;
                    leftEncoderCount = value;
                    CalculateLeftActualSpeed();
                }
            }
            private DateTime rightPreviousUpdateTime = DateTime.MinValue;
            public DateTime RightPreviousUpdateTime
            {
                get { return rightPreviousUpdateTime; }
            }
            private DateTime rightCurrentUpdateTime = DateTime.MinValue;
            public DateTime RightCurrentUpdateTime
            {
                get { return rightCurrentUpdateTime; }
            }
            private int rightEncoderCount = 0;
            public int RightEncoderCount
            {
                get { return rightEncoderCount; }
                set
                {
                    rightPreviousUpdateTime = rightCurrentUpdateTime;
                    previousRightEncoderCount = rightEncoderCount;
                    rightCurrentUpdateTime = DateTime.Now;
                    rightEncoderCount = value;
                    CalculateRightActualSpeed();
                }
            }
            private int previousLeftEncoderCount = 0;
            public int PreviousLeftEncoderCount
            {
                get { return previousLeftEncoderCount; }
            }
            private int previousRightEncoderCount = 0;
            public int PreviousRightEncoderCount
            {
                get { return previousRightEncoderCount; }
            }
            /// <summary>
            /// Actual speed in meters per second
            /// </summary>
            public float ActualSpeed
            {
                get { return (LeftActualSpeed + RightActualSpeed) / 2.0f; }
            }
            /// <summary>
            /// Actual speed of left tire in meters per second
            /// </summary>
            public float LeftActualSpeed
            {
                get
                {
                    return leftActualSpeed;
                }
            }
            private void CalculateLeftActualSpeed()
            {
                float left = 0.0f;
                // Wrap around, use previous speed as constant
                if (leftEncoderCount < previousLeftEncoderCount)
                {
                    previousLeftEncoderCount = (int)(leftEncoderCount - leftActualSpeed * SpeedConstant);
                }
                if (smoothSpeed == true)
                {
                    left = leftActualSpeed * alpha + (leftEncoderCount - previousLeftEncoderCount) / SpeedConstant * (1 - alpha);
                }
                else
                {
                    left = (leftEncoderCount - previousLeftEncoderCount) / SpeedConstant;
                }
                if (leftCurrentUpdateTime != DateTime.MinValue && leftPreviousUpdateTime != DateTime.MinValue)
                {
                    left = left / (float)(leftCurrentUpdateTime - leftPreviousUpdateTime).TotalMilliseconds;
                }
                else
                {
                    left = 0;
                }
                if (left < CutOffSpeed)
                {
                    left = 0;
                }
                leftActualSpeed = left;
            }
            /// <summary>
            /// Actual speed of right tire in meters per second
            /// </summary>
            public float RightActualSpeed
            {
                get
                {
                    return rightActualSpeed;
                }
            }
            private void CalculateRightActualSpeed()
            {
                float right = 0.0f;
                // Wrap around, use previous speed as constant
                if (rightEncoderCount < previousRightEncoderCount)
                {
                    previousRightEncoderCount = (int)(rightEncoderCount - rightActualSpeed * SpeedConstant);
                }
                if (smoothSpeed == true)
                {
                    right = rightActualSpeed * alpha + (rightEncoderCount - previousRightEncoderCount) / SpeedConstant * (1 - alpha);
                }
                else
                {
                    right = (rightEncoderCount - previousRightEncoderCount) / SpeedConstant;
                }
                if (rightCurrentUpdateTime != DateTime.MinValue && rightPreviousUpdateTime != DateTime.MinValue)
                {
                    right = right / (float)(rightCurrentUpdateTime - rightPreviousUpdateTime).TotalMilliseconds;
                }
                else
                {
                    right = 0;
                }
                if (right < CutOffSpeed)
                {
                    right = 0;
                }
                rightActualSpeed = right;
            }
            private float leftActualSpeed = 0;
            private float rightActualSpeed = 0;
            #region ICloneable Members

            public object Clone()
            {
                VehicleStatusCls c = new VehicleStatusCls();
                c.leftEncoderCount = leftEncoderCount;
                c.rightEncoderCount = rightEncoderCount;
                c.previousLeftEncoderCount = previousLeftEncoderCount;
                c.previousRightEncoderCount = previousRightEncoderCount;
                return c;
            }

            #endregion
        }
        #endregion

        #region Vehicle autonomous state
        [Serializable()]
        public class AutonomousStateCls : ICloneable
        {
            /// <summary>
            /// Decided BEST steering angle to have the most clear pathway.
            /// In degrees.
            /// </summary>
            public float MaximumDistanceAngle = 0;
            /// <summary>
            /// Length of the longest open pathway that the robot can fit.
            /// In meters.
            /// </summary>
            public float MaximumDistance = 0;
            /// <summary>
            /// Decided BEST steering angle to have the most clear pathway.
            /// In degrees.
            /// </summary>
            public float MinimumDistanceAngle = 0;
            /// <summary>
            /// Distance of the closest obstacle.
            /// In meters.
            /// </summary>
            public float MinimumDistance = 0;
            /// <summary>
            /// Decided BEST steering angle to avoid obstacles that are close.
            /// Does not explicitely calculate a clear pathway.
            /// In degrees.
            /// </summary>
            public float MinimumProximityAngle = 0;
            public float MinimumProximityValue = 0;
            /// <summary>
            /// Worst steering angle to avoid obstacles that are close.
            /// Does not explicitely calculate a clear pathway.
            /// In degrees.
            /// </summary>
            public float MaximumProximityAngle = 0;
            public float MaximumProximityValue = 0;

            #region ICloneable Members
            public object Clone()
            {
                AutonomousStateCls obj = new AutonomousStateCls();
                obj.MaximumDistanceAngle = MaximumDistanceAngle;
                obj.MaximumDistance = MaximumDistance;
                obj.MinimumDistanceAngle = MinimumDistanceAngle;
                obj.MinimumDistance = MinimumDistance;
                obj.MinimumProximityAngle = MinimumProximityAngle;
                obj.MinimumProximityValue = MinimumProximityValue;
                obj.MaximumProximityAngle = MaximumProximityAngle;
                obj.MaximumProximityValue = MaximumProximityValue;
                return obj;
            }
            #endregion
        }
        #endregion
    namespace Control
    {
        #region Servo Motor
        [Serializable()]
        public class ServoPositionInfoCls
        {
            protected double acceleration = 0;
            public double Acceleration
            {
                get
                {
                    return acceleration;
                }
                set
                {
                    acceleration = value;
                }
            }
            protected double speed = 0;
            public double Speed
            {
                get
                {
                    return speed;
                }
                set
                {
                    speed = value;
                }
            }
            /// <summary>
            /// Fill this field to move the servo
            /// </summary>
            protected double target = 0;
            public double Target
            {
                get
                {
                    return target;
                }
                set
                {
                    target = value;
                }
            }
            /// <summary>
            /// Observe this field to see the servo position
            /// </summary>
            protected double position = 0;
            public double Position
            {
                get
                {
                    return position;
                }
            }
            public ServoNoEnu ServoNo = ServoNoEnu.Unknown;

            // Explicit mutator for position field, which is actualy read-only
            public void ReportPosition(double p)
            {
                position = p;
            }
        }
        public enum ServoNoEnu : int { Servo0 = 0, Servo1 = 1, Servo2 = 2, Servo3 = 3, Servo4 = 4, Servo5 = 5, Unknown = 255 };
        #endregion

        #region Joystick
        [Serializable()]
        public class InputStateCls
        {
            public InputStateCls()
            {
            }

            private bool connected = false;
            public bool Connected
            {
                get { return connected; }
                set { connected = value; }
            }

            private int lx = 0;
            public int Lx
            {
                get { return lx; }
                set { lx = value; }
            }

            private int ly = 0;
            public int Ly
            {
                get { return ly; }
                set { ly = value; }
            }

            private int rx = 0;
            public int Rx
            {
                get { return rx; }
                set { rx = value; }
            }
            private int ry = 0;
            public int Ry
            {
                get { return ry; }
                set { ry = value; }
            }
            private int tR = 0;
            /// Right trigger
            public int TR
            {
                get { return tR; }
                set { tR = value; }
            }
            private int tL = 0;
            /// <summary>
            /// Left trigger
            /// </summary>
            public int TL
            {
                get { return tL; }
                set { tL = value; }
            }
            private bool[] buttons = new bool[20];
            public bool[] Buttons
            {
                get { return buttons; }
                set { buttons = value; }
            }
        }
        #endregion


        #region Robot State
        [Serializable()]
        public class StateChartCls<T> where T : class, IConvertible, new()
        {
            public Type StateEnumType
            {
                get
                {
                    return typeof(T).GetType();
                }
            }
            private T currentState = new T();
            public T CurrentState
            {
                get
                {
                    return currentState;
                }
                set
                {
                    int v = value.ToInt32(null);
                    int c = currentState.ToInt32(null);

                    if (c != v)
                    {
                        previousState = currentState;
                        if (BeforeStateTransition != null)
                        {
                            BeforeStateTransition(this, value, previousState);
                        }
                        currentState = DoStateTransition(this, value);
                        if (AfterStateTransition != null)
                        {
                            AfterStateTransition(this, currentState, previousState);
                        }
                    }
                }
            }

            private T previousState = new T();
            public T PreviousState
            {
                get
                {
                    return previousState;
                }
            }
            /// <summary>
            /// Checks in current state is or in the given state.
            /// </summary>
            /// <param name="stateOrMacroState"></param>
            /// <returns>If the current state is the given state or a substate of the given state.</returns>
            public bool IsInState(T stateOrMacroState)
            {
                return IsInState(stateOrMacroState, currentState);
            }
            public static bool IsInState(T stateOrMacroState, T currentState)
            {
                bool result = false;
                if (currentState.ToInt32(null) == stateOrMacroState.ToInt32(null))
                {
                    result = true;
                }
                else
                {
                    int s = stateOrMacroState.ToInt32(null);
                    int c = currentState.ToInt32(null);
                    if ((s & 0x0FF) == 0x00)
                    {
                        if ((c & s & 0xF00) != 0x00)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else if ((s & 0x00F) == 0x0)
                    {
                        if ((c & s & 0xF00) != 0x0 && (c & s & 0x0F0) != 0x0)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
                return result;
            }
            public event StateTransitionDlt BeforeStateTransition;
            public event StateTransitionDlt AfterStateTransition;
            public event DoStateTransitionDlt DoStateTransition;
            public delegate void StateTransitionDlt(StateChartCls<T> stateChart, T current, T previous);
            public delegate T DoStateTransitionDlt(StateChartCls<T> stateChart, T value);
        }
        public enum RobotStateEnu
        {
            UnknownState = 0x000,
            InitializeState = 0x100,
            AutonomousState = 0x200,
            Autonomous_CheckStuckState = 0x210,
            Autonomous_RunState = 0x220,
            Autonomous_Run_SlowState = 0x221, Autonomous_Run_NormalState = 0x222, Autonomous_Run_FastState = 0x224, //Autonomous_Run_FaceFoundState = 0x228,
            Autonomous_InitialStuckStopState = 0x240,
            Autonomous_StuckState = 0x280,
            Autonomous_Stuck_WaitState = 0x281, Autonomous_Stuck_BackState = 0x282,
            RemoteControlState = 0x400,
            RemoteControl_ControlState = 0x410, RemoteControl_IgnitionState = 0x420,

            StopState = 0x800
        };
        #endregion

        #region External events
        [Serializable()]
        public class ExternalEventRequestCls : ICloneable
        {
            private ExternalEventTypeEnu eventType = ExternalEventTypeEnu.Unknown;
            public ExternalEventTypeEnu EventType
            {
                get { return eventType; }
                set { eventType = value; }
            }
            private double value1;
            public double Value1
            {
                get { return value1; }
                set { value1 = value; }
            }
            private double value2;
            public double Value2
            {
                get { return value2; }
                set { value2 = value; }
            }
            private double value3;
            public double Value3
            {
                get { return value3; }
                set { value3 = value; }
            }
            private double value4;
            public double Value4
            {
                get { return value4; }
                set { value4 = value; }
            }
            private double value5;
            public double Value5
            {
                get { return value5; }
                set { value5 = value; }
            }
            public ExternalEventRequestCls()
            {
                eventType = ExternalEventTypeEnu.Unknown;
                value1 = 0;
                value2 = 0;
                value3 = 0;
                value4 = 0;
                value5 = 0;
            }
            public ExternalEventRequestCls(ExternalEventTypeEnu eventtype)
            {
                eventType = eventtype;
                this.value1 = 0;
                this.value2 = 0;
                this.value3 = 0;
                this.value4 = 0;
                this.value5 = 0;
            }
            public ExternalEventRequestCls(ExternalEventTypeEnu eventtype, double value1)
            {
                eventType = eventtype;
                this.value1 = value1;
                this.value2 = 0;
                this.value3 = 0;
                this.value4 = 0;
                this.value5 = 0;
            }
            public ExternalEventRequestCls(ExternalEventTypeEnu eventtype, double value1, double value2)
            {
                eventType = eventtype;
                this.value1 = value1;
                this.value2 = value2;
                this.value3 = 0;
                this.value4 = 0;
                this.value5 = 0;
            }
            public ExternalEventRequestCls(ExternalEventTypeEnu eventtype, double value1, double value2, double value3)
            {
                eventType = eventtype;
                this.value1 = value1;
                this.value2 = value2;
                this.value3 = value3;
                this.value4 = 0;
                this.value5 = 0;
            }
            public ExternalEventRequestCls(ExternalEventTypeEnu eventtype, double value1, double value2, double value3, double value4)
            {
                eventType = eventtype;
                this.value1 = value1;
                this.value2 = value2;
                this.value3 = value3;
                this.value4 = value4;
                this.value5 = 0;
            }
            public ExternalEventRequestCls(ExternalEventTypeEnu eventtype, double value1, double value2, double value3, double value4, double value5)
            {
                eventType = eventtype;
                this.value1 = value1;
                this.value2 = value2;
                this.value3 = value3;
                this.value4 = value4;
                this.value5 = value5;
            }
            #region ICloneable Members
            public object Clone()
            {
                ExternalEventRequestCls obj = new ExternalEventRequestCls(eventType, value1, value2, value3, value4, value5);
                return obj;
            }
            #endregion

        }
        public enum ExternalEventTypeEnu { Unknown, Stop, Ignition, RunAround, Slow, Fast, GoBack, Follow, Track, Wait, Move, Home };
        #endregion
    }
}