#include <Servo.h>

Servo servo; 
int increment;
int curAngle;

void setup() 
{
  increment = 1;
  curAngle = 0;
  
  //pinMode(12, INPUT);
  
  //pinMode(1,OUTPUT);
  servo.attach(9); //analog pin 0
  //servo1.setMaximumPulse(2000);
  //servo1.setMinimumPulse(700);
  
  Serial.begin(9600);
  Serial.println("Ready");

}

void loop() 
{
  int retVal = analogRead(A0);
  Serial.println(retVal);
  
  retVal = map(retVal, 0, 1023, 0, 180);
  //stepAngle();
  
  servo.write(retVal);
}

void stepAngle()
{
  if(curAngle > 180)
  {
    increment = -1;
  }
  if(curAngle <= 0)
  {
    increment = 1;
  }
  
  curAngle = curAngle + increment;
}

