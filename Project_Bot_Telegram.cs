String ssid     = "Simulator Wifi";  // SSID to connect to
String password = ""; // Our virtual wifi has no password 
const int httpPort   = 80;
String host = "api.telegram.org";

String uri = "/bot6028953721:AAFyhvJivR1d73lFamzP-mbk6_Cxi6D9_BA/sendMessage?chat_id=-970233807&text=";

int setupESP8266(void) {
  // Start our ESP8266 Serial Communication
  Serial.begin(115200);   // Serial connection over USB to computer
  Serial.println("AT");   // Serial connection on Tx / Rx port to ESP8266
  delay(10);        // Wait a little for the ESP to respond
  if (!Serial.find("OK")) return 1;
    
  // Connect to 123D Circuits Simulator Wifi
  Serial.println("AT+CWJAP=\"" + ssid + "\",\"" + password + "\"");
  delay(10);        // Wait a little for the ESP to respond
  if (!Serial.find("OK")) return 2;
  
  // Open TCP connection to the host:
  Serial.println("AT+CIPSTART=\"TCP\",\"" + host + "\"," + httpPort);
  delay(50);        // Wait a little for the ESP to respond
  if (!Serial.find("OK")) return 3;
  
  return 0;
}

void setup() {
  Serial.begin(115200);
  delay(1000);
  pinMode(A0, INPUT);
  pinMode(A1, INPUT);
  pinMode(A2, INPUT);
  setupESP8266();
}

void loop() {
  delay(10000);
  sendToTelegram();
}

void sendToTelegram(void) {
  int temperatura = map(analogRead(A0),20,358,-40,125);
  int umidade = map(analogRead(A1),20,358,-40,125);
  int luminosidade = map(analogRead(A2),20,358,-40,125);
  
  String str = " temperatura:" + String(temperatura);
  str = str + " | umidade: " + String(umidade);
  str = str + " | luminosidade:" + String(luminosidade);
  
  String httpPacket = "POST " + uri + str + " HTTP/1.1\r\nHost: " + host + "\r\n\r\n";
  
  int length = httpPacket.length();
  
  Serial.print("AT+CIPSEND=");
  Serial.println(length);
  delay(10);

  Serial.print(httpPacket);
  delay(10);
  if (!Serial.find("SEND OK\r\n")) return;
}