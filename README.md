# Introduction
This repo demonstrates a working implementation of how we can run Azure Storage and Cosmos DB related integration tests with GitHub Actions.

This step installs and run the CosmosDB Emulator.
```
- uses: southpolesteve/cosmos-emulator-github-action@v1
```

This step installs and run the Storage Emulator known as azurite. Note that it is running just the Table endpoint which the integration tests relies on.
```
- run: npm install -g azurite
- shell: bash
run: azurite-table &
```