 #include <Servo.h> 
 
Servo myservo;  // create servo object to control a servo 
                // a maximum of eight servo objects can be created 
 
void setup() 
{ 
  myservo.attach(9);  // attaches the servo on pin 9 to the servo object 
  Serial.begin(9600);
} 
 
void loop()
{
  for(int i = 0; i <= 180; i++)
  {
    myservo.write(i);
    int val = analogRead(A0);
    val = map(val, 150, 470, 0, 170);
    Serial.println(val);
    delay(150);
  }
  for(int i = 180; i >= 0; i--)
  {
    myservo.write(i);
    int val = analogRead(A0);
    val = map(val, 150, 470, 0, 170);
    Serial.println(val);
    delay(150);
  }
}
