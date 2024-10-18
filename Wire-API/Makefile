image = wire-api
version = 0.1.0


build:
	docker build -t $(image):$(version) .

docker-run:
	docker run \
		--detach \
		--publish 8080:8080 \
		-e ASPNETCORE_ENVIRONMENT=Development \
        -e CosmosDBSettings__Account=https://localhost:8081 \
        -e CosmosDBSettings__Key=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw== \
        -e AzureOpenAISettings__Endpoint=https://azure-openai-wire-dev.openai.azure.com/ \
        -e AzureOpenAISettings__Key=1178242fb8c6415aa6395794b333b6d7 \
        -e AzureOpenAISettings__DeploymentName=gpt-4o-mini \
		--name $(image) $(image):$(version)

cosmos:
	docker run \
		--publish 8081:8081 \
		--publish 10251:10251 \
		--name cosmosdb-emulator \
		--detach \
		-e AZURE_COSMOS_EMULATOR_PARTITION_COUNT=5 \
		-e AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=false \
		-e AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE=127.0.0.1 \
		mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest 