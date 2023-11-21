# DevopsLearning



## Prometheus + Grafana

|  服務	| Port號 |
|  ----	| ---- |
| telegraf              | 9273 |
| prometheus            | 9090 |
| dotnet-core-exporter  | 8787 |
| pushgateway           | 9091 |


docker build -t dotnet-core-exporter --build-arg ProjectName=dotnet-core-exporter --no-cache .