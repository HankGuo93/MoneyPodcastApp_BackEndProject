# ELK 環境設定

> 以下設定都記得要開防火牆

## Java

### 安裝OpenJDK

安裝預設版本OpenJDK

```
$ sudo apt-get update
$ sudo apt-get install default-jdk
```

安裝指定版本OpenJDK

```
$ sudo apt-get update
$ sudo apt-get install openjdk-8-jdk
```

安裝指定版本OpenJRE

```
$ sudo apt-get update
$ sudo apt-get install openjdk-8-jre
```



### 設定系統 Java 版本

先查看當前有哪些版本可供設定

```
$ update-alternatives --query java
#或者
$ update-alternatives --display java
```

設定版本

```
$ sudo update-alternatives --config java
```



### 設置Java環境變數

打開/etc/profile這個檔案並且在末尾加入：

```
export JAVA_HOME=/usr/lib/jvm/java-8-openjdk-amd64
export PATH=$PATH:$JAVA_HOME/bin
```



## elasticsearch佈署

elasticsearch是一個分散式的儲存系統，屬於NoSQL資料庫的一種
[elasticsearch介紹](https://www.elastic.co/cn/products/elasticsearch)

```
#下載安裝檔
$ wget https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-7.4.0-amd64.deb
#下載驗證檔
$ wget https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-7.4.0-amd64.deb.sha512
#檢查檔案
$ shasum -a 512 -c elasticsearch-7.4.0-amd64.deb.sha512
#執行安裝
$ sudo dpkg -i elasticsearch-7.4.0-amd64.deb
```



### 修改配置檔案

首先我們要先修改elasticsearch記憶體的使用量
修改elasticsearch使用的記憶體，設定為主機的50%記憶體

```
#/etc/elasticsearch/jvm.options
-Xms1g  # Xms 記憶體使用下限
-Xmx1g  # Xmx 記憶體使用上限
```



### 修改設定檔

```
#/etc/elasticsearch/elasticsearch.yml
#設置服務名稱
cluster.name: elk_elasticsearch
#設置節點名稱
node.name: elk_node
#設置為主要結點
node.master: true
#允許節點儲存數據
node.data: true
#
path.data: /var/lib/elasticsearch
#
path.logs: /var/log/elasticsearch
#綁定來源
network.bind_host: 0.0.0.0
#綁定對外服務端口
http.port: 9200
#綁定節點通信端口
transport.tcp.port: 9300
#啟用資料壓縮
transport.tcp.compress: true
#集群發現節點列表
discovery.seed_hosts: ["127.0.0.1:9300"]
#設置主要節點列表
cluster.initial_master_nodes: ["127.0.0.1"]
```



### 測試

```
#啟動服務
$ service elasticsearch start
#測試
$ curl http://127.0.0.1:9200
{
  "name" : "elk_node",
  "cluster_name" : "elk_service",
  "cluster_uuid" : "SRb6b1RaTaewA_OK2C7fMA",
  "version" : {
    "number" : "7.4.0",
    "build_flavor" : "default",
    "build_type" : "deb",
    "build_hash" : "22e1767283e61a198cb4db791ea66e3f11ab9910",
    "build_date" : "2019-09-27T08:36:48.569419Z",
    "build_snapshot" : false,
    "lucene_version" : "8.2.0",
    "minimum_wire_compatibility_version" : "6.8.0",
    "minimum_index_compatibility_version" : "6.0.0-beta1"
  },
  "tagline" : "You Know, for Search"
}
```



## kibana佈署

kibana是elasticsearch可視化的重要元件
[kibana介紹](https://www.elastic.co/cn/products/kibana)

```
#下載安裝檔
$ wget https://artifacts.elastic.co/downloads/kibana/kibana-7.4.0-amd64.deb
#下載驗證檔
$ wget https://artifacts.elastic.co/downloads/kibana/kibana-7.4.0-amd64.deb.sha512
#檢查檔案
$ shasum -a 512 -c kibana-7.4.0-amd64.deb.sha512
#執行安裝
$ sudo dpkg -i kibana-7.4.0-amd64.deb
```



### 修改配置檔案

```
#/etc/kibana/kibana.yml
#綁定對外服務端口
server.port: 5601
#綁定來源
server.host: "0.0.0.0"
#設置服務名稱
server.name: "elk_kibana"
#設置elasticsearch節點列表
elasticsearch.hosts: ["http://127.0.0.1:9200"]
#設置索引
kibana.index: ".kibana"
設置log路徑
logging.dest: /var/log/kibana/kibana.log
設置中文化
i18n.locale: "zh-CN"
```



### 建立log目錄

```
#建立目錄
$ mkdir /var/log/kibana
#修改使用者權限
$ chown kibana:kibana /var/log/kibana
```



### 測試

```
#啟動服務
$ service kibana start
```



[測試網址http://127.0.0.1:5601](http://127.0.0.1:5601/)

![img](https://pcion123.github.io/2019/10/20/elk-install/kibana1.png)![img](https://pcion123.github.io/2019/10/20/elk-install/kibana2.png)



## logstash佈署

logstash是負責幫我們收集各種log資料的收集器
[logstash介紹](https://www.elastic.co/cn/products/logstash)

```
#下載安裝檔
$ wget https://artifacts.elastic.co/downloads/logstash/logstash-7.4.0.deb
#下載驗證檔
$ wget https://artifacts.elastic.co/downloads/logstash/logstash-7.4.0.deb.sha512
#檢查檔案
$ shasum -a 512 -c logstash-7.4.0.deb.sha512
#執行安裝
$ sudo dpkg -i logstash-7.4.0.deb
```



### 修改配置檔案

```
#/etc/logstash/conf.d/30-log.conf
# Sample Logstash configuration for creating a simple
# Beats -> Logstash -> Elasticsearch pipeline.

input {
  beats {
    port => 5044
  }
}

output {
  elasticsearch {
    hosts => ["http://localhost:9200"]
    index => "%{[@metadata][beat]}-%{[@metadata][version]}-%{+YYYY.MM.dd}"
    #user => "elastic"
    #password => "changeme"
  }
}
```

```
//My podcast_project setting
input {
  beats {
    port => 5044
  }
}

filter{
  if "podcast_project" in [tags]{
    multiline {
      pattern => "^(?!Time)"
      negate => true
      what => "next"
    }
    grok {
      match => {
        "message" => "\A%{NOTSPACE}%{SPACE}%{DATESTAMP}%{SPACE}%{WORD}%{GREEDYDATA:Msg_log}"
      }
    }
    geoip {
      source => "clientip"
    }
  }
}

output {
  elasticsearch {
    hosts => ["http://localhost:9200"]
    index => "filebeat-podcast"
    #index => "%{[@metadata][beat]}-%{[@metadata][version]}-%{+YYYY.MM.dd}"
    #user => "elastic"
    #password => "changeme"
  }
}
```



### 檢查設定檔

```
$ /usr/share/logstash/bin/logstash --config.test_and_exit -f /etc/logstash/conf.d/30-log.conf

...

Configuration OK
```



### 啟動服務

```
#啟動服務
$ service logstash start
```



## filebeat佈署

### 修改配置檔案

https://www.cnblogs.com/xiaobaozi-95/p/9550152.html (配置文件)

```
# ============================== Filebeat inputs ===============================

filebeat.inputs:

# Each - is an input. Most options can be set at the input level, so
# you can use different inputs for various configurations.
# Below are the input specific configurations.

- type: log

  # Change to true to enable this input configuration.
  enabled: true

  # Paths that should be crawled and fetched. Glob based paths.
  paths:
    - /var/www/podcast_project/ErrorLog.txt

  tags: ["podcast_project"]
```

```
# ------------------------------ Logstash Output -------------------------------
output.logstash:
  # The Logstash hosts
  hosts: ["localhost:5044"]

  # Optional SSL. By default is off.
  # List of root certificates for HTTPS server verifications
  #ssl.certificate_authorities: ["/etc/pki/root/ca.pem"]

  # Certificate for SSL client authentication
  #ssl.certificate: "/etc/pki/client/cert.pem"

  # Client Certificate Key
  #ssl.key: "/etc/pki/client/cert.key"
```

安裝多行解析套件

```
# logstash-plugin install logstash-filter-multiline
Validating logstash-filter-multiline
Installing logstash-filter-multiline
Installation successfu
```

https://grokdebug.herokuapp.com/  (檢查grok語法)

https://grokconstructor.appspot.com/do/constructionstep (協助創建grok篩選語法)

https://mmx362003.gitbooks.io/elk-stack-guide/content/logstash_grok.html (grok語法教學)

### 啟動服務

```
#啟動服務
$ service filebeat start
```

## 參考資料

https://lufor129.medium.com/elk-%E5%AF%A6%E4%BD%9C%E5%88%86%E6%95%A3%E5%BC%8Flog%E6%8E%A1%E9%9B%86%E7%B3%BB%E7%B5%B1-d3e729624af4   (ELK教學)

https://pcion123.github.io/2019/10/20/elk-install/ (ELK安裝)