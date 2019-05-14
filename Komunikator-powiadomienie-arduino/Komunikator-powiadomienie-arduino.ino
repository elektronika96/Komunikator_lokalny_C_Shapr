void setup() {
  // put your setup code here, to run once:
  pinMode(8,OUTPUT);
  pinMode(7,OUTPUT);
  Serial.begin(9600);
}

void loop() {
  String data = Serial.readStringUntil('\n');
  if (data.length()>0){
    if (data.equals("ledon")) {
      digitalWrite(8, HIGH);
      delay(1000);
      digitalWrite(8, LOW);
       for(int i = 0;i<3;i++) {
          digitalWrite(7, HIGH);
          delay(1000);
          digitalWrite(7, LOW);
          delay(1000);
        }
        
    if (data.equals("led")){
      digitalWrite(7, HIGH);
    }
    
    }
  } 
}
