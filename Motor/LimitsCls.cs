using System;
using System.Collections.Generic;
using System.Text;

namespace Control.Motor.ServoMotor.Limits
{
    [Serializable()]
    public class LimitsCls
    {
        public static LimitsCls Instance = new LimitsCls();
        private int inputAxisThreshold = 500;
        public int InputAxisThreshold
        {
            get { return inputAxisThreshold; }
            set { inputAxisThreshold = value; }
        }
        private int motorAcceleration = 30;
        public int MotorAcceleration
        {
            get { return motorAcceleration; }
            set { motorAcceleration = value; }
        }
        private int motorNormalSpeed = 6800;
        public int MotorNormalSpeed
        {
            get { return motorNormalSpeed; }
            set { motorNormalSpeed = value; }
        }
        private int motorSlowSpeed = 6200;
        public int MotorSlowSpeed
        {
            get { return motorSlowSpeed; }
            set { motorSlowSpeed = value; }
        }
        private int motorMaxSpeed = 1800*4; // 7200
        public int MotorMaxSpeed
        {
            get { return motorMaxSpeed; }
            set { motorMaxSpeed = value; }
        }
        private int motorReverseSpeed = 5400;
        public int MotorReverseSpeed
        {
            get { return motorReverseSpeed; }
            set { motorReverseSpeed = value; }
        }
        private int motorMaxReverseSpeed = 1100*4;
        public int MotorMaxReverseSpeed
        {
            get { return motorMaxReverseSpeed; }
            set { motorMaxReverseSpeed = value; }
        }
        private int motorStopPosition = 6000;
        public int MotorStopPosition
        {
            get { return motorStopPosition; }
            set { motorStopPosition = value; }
        }
        private int rightServoRightPosition = 6200;
        public int RightServoRightPosition
        {
            get { return rightServoRightPosition; }
            set { rightServoRightPosition = value; }
        }
        private int rightServoLeftPosition = 4900;
        public int RightServoLeftPosition
        {
            get { return rightServoLeftPosition; }
            set { rightServoLeftPosition = value; }
        }
        private int rightServoCenterPosition = 5550;
        public int RightServoCenterPosition
        {
            get { return rightServoCenterPosition; }
            set { rightServoCenterPosition = value; }
        }

        private int leftServoRightPosition = 6400;
        public int LeftServoRightPosition
        {
            get { return leftServoRightPosition; }
            set { leftServoRightPosition = value; }
        }
        private int leftServoLeftPosition = 4600;
        public int LeftServoLeftPosition
        {
            get { return leftServoLeftPosition; }
            set { leftServoLeftPosition = value; }
        }
        private int leftServoCenterPosition = 5550;
        public int LeftServoCenterPosition
        {
            get { return leftServoCenterPosition; }
            set { leftServoCenterPosition = value; }
        }

        private int cameraServoRightPosition = 6700;
        public int CameraServoRightPosition
        {
            get { return cameraServoRightPosition; }
            set { cameraServoRightPosition = value; }
        }
        private int cameraServoLeftPosition = 4400;
        public int CameraServoLeftPosition
        {
            get { return cameraServoLeftPosition; }
            set { cameraServoLeftPosition = value; }
        }
        private int cameraServoCenterPosition = 5550;
        public int CameraServoCenterPosition
        {
            get { return cameraServoCenterPosition; }
            set { cameraServoCenterPosition = value; }
        }
    }
}
