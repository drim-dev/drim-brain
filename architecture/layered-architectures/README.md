* [1. Traditional Architectures](#1-traditional-architectures)
  * [1.1. Layered  Architecture](#11-layered--architecture)
    * [1.1.1. Pros](#111-pros)
  * [1.2. Onion Architecture](#12-onion-architecture)
    * [1.2.1. Statements](#121-statements)
  * [1.3. Hexagonal Architecture](#13-hexagonal-architecture)
  * [1.4. Clean Architecture](#14-clean-architecture)

# 1. Traditional Architectures

## 1.1. Layered  Architecture

![Layered  Architecture](_images/layered-architecture.png)

### 1.1.1. Pros

* Separation of concerns
* Abstraction and encapsulation
* Downward dependency direction
* Single responsibility
* Layer independence

## 1.2. Onion Architecture

Link: https://bit.ly/3WmVjCd

![Onion Architecture](_images/onion-architecture.png)

* __Domain Model + Domain Services.__ This layer contains the core business logic and domain models. It represents the heart of the application and should be independent of any specific technology or infrastructure concerns
* __Application Services.__ The application layer sits outside the domain layer and contains the application-specific logic. It orchestrates the interaction between the domain layer and the infrastructure layer, handling use cases and exposing services
* __Infrastructure.__ This layer is responsible for interacting with external resources and frameworks, such as databases, file systems, or web services
* __User Interface.__ This layer is responsible for presenting information to users and capturing their input. It includes user interfaces, such as web interfaces, desktop applications, or mobile apps

### 1.2.1. Statements

* The domain model is the truth of the organization
* The application is built around an independent object model
* Inner layers define interfaces. Outer layers implement interfaces
* The direction of the coupling is toward the center
* Deeper layers change less often
* All application core code can be compiled and run separately from the infrastructure
* Testability

## 1.3. Hexagonal Architecture

Link: https://bit.ly/3ofxoIp

![Hexagonal Architecture](_images/hexagonal-architecture.png)

* __Domain Model + Domain Services.__ This layer contains the core business logic and domain models. It represents the heart of the application and should be independent of any specific technology or infrastructure concerns
* __Application Services.__ The application layer sits outside the domain layer and contains the application-specific logic. It orchestrates the interaction between the domain layer and the infrastructure layer, handling use cases and exposing services
* __Ports.__ Ports define the interfaces through which the core interacts with the outside world. They are the entry and exit points for data and control flow
* __Adapters.__ Adapters implement the ports and connect the core with the external world. They are responsible for translating and transforming data between the core and the external systems, such as databases, user interfaces, or third-party services

## 1.4. Clean Architecture

Link: https://bit.ly/3BNEqqS

![Clean Architecture](_images/clean-architecture.jpg)

![All of them the same thing](_images/the-same-thing.png)
