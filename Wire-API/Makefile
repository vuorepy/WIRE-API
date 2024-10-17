image = wire-api
version = 0.1.0


build:
	docker build -t $(image):$(version) .

docker-run:
	docker run -d -p 8080:80 --name $(image) -e ASPNETCORE_ENVIRONMENT=Development $(image):$(version)

cosmos:
	docker run \
		--publish 8081:8081 \
		--publish 10251:10251 \
		--name cosmosdb-emulator \
		--detach \
		-e AZURE_COSMOS_EMULATOR_PARTITION_COUNT=2 \
		-e AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=false \
		-e AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE=127.0.0.1 \
		mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest  