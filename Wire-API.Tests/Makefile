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