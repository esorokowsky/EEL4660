void setup()
{
  Serial.begin(9600);
}

void loop()
{
  unsigned long time;
  time = millis();
  int nChannelOne = analogRead(A0);
  int nChannelTwo = analogRead(A1);
  
  Serial.print(time);
  Serial.print(",");
  if(nChannelOne > 0)
  {
    Serial.println(nChannelOne);
  }
  else
  {
    Serial.println(-nChannelTwo);
  }
}
