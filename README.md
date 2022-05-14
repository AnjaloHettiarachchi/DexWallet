# DexWallet
A Distributed Cryptocurrency Exchange Wallet on Cloud.

## Motivation
This project is meant as an implementation for the assignment in **[SE6020] Architecting Enterprise Cloud Solutions** module conducted as a
part of **Master of Sc. IT (Enterprise Application Development)** program offered by **Sri Lanka Institute of
Information Technology, Sri Lanka**.

## Use Case
DexLK is a distributed cryptocurrency exchange based in Sri Lanka. They are planning on building an API outside of the blockchain for users to easily interact with the DEX. This solution architected for this specific scenario.

## Features
- Distributed computing with `Microservice` architecture.
- Instant and universal deployment with `Docker` integration.
- A `cloud-ready` implementation with necessary requirements (like `health-check` endpoints).
- Proper deployment strategy with `Amazon CloudFormation` templates.

## Services
- **Identity Service** `DexWallet.Identity`: Provides necessary authentication and authorization requirements for other services.
- **Core (Wallet) Service** `DexWallet.Core`: Handles the core resource (Wallets) within the application. It can be used to perform basic action on the resource such as creation, retrieval, and store funds within specific resource when needed.
- **Exchange Service** `DexWallet.Exchange`: Designed to facilitate the exchange between cryptocurrencies and regular currencies and storing them in a specific Wallet resource.

### Other Resources
- `DexWallet.Common`: Hosts the common (shared) implementations of helper classes and other resources needed for the core services.
- `DexWallet.Infrastructure`: Contains the `Amazon CloudFormation` templates needs to deploy the core services into a `Amazon ECS (Fargate mode)` cluster.

## Technologies
- `.Net Framework 6 (C# 10)`
- `AWS .Net SDK`
- `AWS CLI tool`
- `Docker`

