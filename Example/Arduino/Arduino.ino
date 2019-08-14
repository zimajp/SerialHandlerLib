void setup() {
  Serial.begin(115200);
}

void loop() {
  int value = analogRead(0);
  Serial.println(value);
  delay(300);
}
