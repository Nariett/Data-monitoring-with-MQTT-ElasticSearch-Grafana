## 🛠 My Tools
<a target="_blank"> <img src="https://img.icons8.com/color/256/visual-studio--v2.png" alt="jenkins" width="46" height="46"/> </a>
<a target="_blank"> <img src="https://developer.community.boschrexroth.com/t5/image/serverpage/image-id/13469i8B924ED49BC58B82/image-size/small?v=v2&px=200" alt="jenkins" width="40" height="40"/> </a> 
<a target="_blank"> <img src="https://img.icons8.com/color/256/docker.png" alt="jenkins" width="48" height="48"/> </a> 
<a target="_blank"> <img src="https://img.icons8.com/color/256/elasticsearch.png" alt="jenkins" width="46" height="46"/> </a> 
<a target="_blank"> <img src="https://img.icons8.com/fluency/256/grafana.png" alt="jenkins" width="46" height="46"/> </a> 

## How work this ?
The SendMQTT file generates class objects, which is converted to JSON and sent to the server test.mosquitto.org . The sent data is received by the GetMQTT file and then sent to ElasticSearch, which is launched in Docker together with Grafana. The data received in ElasticSearch is sent to Grafana, in which the data graph is plotted.