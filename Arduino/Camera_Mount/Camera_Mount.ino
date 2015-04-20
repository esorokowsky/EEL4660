#include <Servo.h>

#define PITCH_SERVO_PIN 10
#define YAW_SERVO_PIN 9

#define PITCH_MIN 20
#define PITCH_MAX 180
#define YAW_MIN 0
#define YAW_MAX 180

Servo yawServo; //swivel
Servo pitchServo; //up and down

void setup() 
{
  yawServo.attach(YAW_SERVO_PIN); //analog pin 0
  pitchServo.attach(PITCH_SERVO_PIN); //analog pin 0
  
  Serial.begin(9600);
  delay(500);
  Serial.println("Ready");
}

void loop() 
{
  for(int i = 0; i < 180; i++)
  {
    setAngles(i,i);
    delay(5);
  }
  
  for(int i = 180; i >=0; i--)
  {
    setAngles(i,i);
    delay(5);
  }
}

void setAngles(int i_nPitch, int i_nYaw)
{
  setPitch(i_nPitch);
  setYaw(i_nYaw);
  
  //Uncomment to print out the current angle of the servo.
  //Note: this will slow down the system significantly
  //printServoValues();
}

void setPitch(int i_nPitch)
{
  i_nPitch = min(i_nPitch, PITCH_MAX);
  i_nPitch = max(i_nPitch, PITCH_MIN);
  pitchServo.write(i_nPitch);
}

void setYaw(int i_nYaw)
{
  i_nYaw = min(i_nYaw, YAW_MAX);
  i_nYaw = max(i_nYaw, YAW_MIN);
  yawServo.write(i_nYaw);
}

void printServoValues()
{
  Serial.print("Yaw: ");
  Serial.print(yawServo.read());
  Serial.print(", Pitch: ");
  Serial.println(pitchServo.read());
}

