* [1. Feature-based architecture](#1-feature-based-architecture)
  * [1.1. Screaming Architecture](#11-screaming-architecture)
  * [1.2. Vertical Slices](#12-vertical-slices)
    * [1.2.1. Statements](#121-statements)
    * [1.2.2. Pros](#122-pros)
    * [1.2.3. Cons](#123-cons)

# 1. Feature-based architecture

## 1.1. Screaming Architecture

The basic idea behind Screaming Architecture is to create an architecture that clearly reflects the business domain and requirements of the system. It aims to make the architecture "scream" the essential features, behaviors, and rules of the system, making it easily understandable and maintainable.

## 1.2. Vertical Slices

Link: https://bit.ly/3BNpDMU

<img alt="Vertical Slices" src="images/vertical-slices.png" />

### 1.2.1. Statements

* Code that is changed together should live together. Proximity Principle

### 1.2.2. Pros

* Low barrier to entry
* Resilient to technology changes
* Level of testability can be per feature
* Easy to work on by many teams

### 1.2.3. Cons

* Hard to decide what to have as Shared code vs Feature code
* Each feature can be written in a different way leading to cognitive load when switching
