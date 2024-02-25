* [Architecture](#architecture)
  * [What is architecture?](#what-is-architecture)
  * [Architecture Definition](#architecture-definition)
  * [Expectations of Architect](#expectations-of-architect)
  * [Application Architecture Goals](#application-architecture-goals)
* [Architecture vs. Design](#architecture-vs-design)
* [Laws of Software Architecture](#laws-of-software-architecture)
  * [First Law](#first-law)
  * [Second Law](#second-law)
* [Architecture Quantum](#architecture-quantum)
  * [Independently deployable](#independently-deployable)
  * [High Functional Cohesion](#high-functional-cohesion)

# Architecture

## What is architecture?

Software architecture is about making fundamental structural choices which are costly to change once implemented.

Architecture Definition

## Architecture Definition

Software architecture consists of:

* Structure
* Architecture characteristics
  * Availability, Reliability, Testability, Scalability, Security, Agility, Fault tolerance, Elasticity, Recoverability, Performace, Deployability, Learnability
* Architecture decisions
  * [Architecture Decision Records - ADR - README.md](ADR/README.md)
* Design principles

## Expectations of Architect

There are core expectations placed on a software architect, irrespective of any given role, title, or job description:

* Make architecture decisions
* Continually analyze the architecture
* Keep current with the latest trends
* Ensure compliance with decisions
* Have business domain knowledge
* Diverse exposure and experience
* Possess interpersonal skills
* Understand and navigate politics

## Application Architecture Goals

* Managing complexity (reducing accidental complexity)
* Application maintainability
* Accidental complexity vs evolvability
* Knowledge communication
* Managing obsolescence
* Reusability
* Development speed up

# Architecture vs. Design

Architecture is the __logical__ implementation of the system.

Design is the __physical__ implementation of the system.

# Laws of Software Architecture

## First Law

__Everything in software architecture is a trade-off.__

## Second Law

___Why_ is more important than _how_.__

# Architecture Quantum

__Architecture quantum__ is an __independently deployable__ artifact with __high functional cohesion, low static coupling__, and some form of [__dynamic coupling__](modularity/coupling/README.md#dynamic-coupling).

_Architecture quantum is an independent part of the system that can evolve independently of other parts._

Examples are here: [styles/README.md](styles/README.md)

## Independently deployable

Independently deployable implies several different aspects of an architecture quantum—each quantum represents a separate deployable unit within a particular architecture. Thus, a __monolithic architecture__ — one that is deployed as a single unit—is, by definition, a __single architecture quantum__. Within a distributed architecture such as microservices, developers tend toward the ability to deploy services independently, often in a highly automated way. Thus, from an independently deployable standpoint, __a service within a microservices architecture represents an architecture quantum__.

## High Functional Cohesion

High functional cohesion refers structurally to the proximity of related elements: classes, components, services, and so on. Throughout history, computer scientists defined a variety of types of cohesion, scoped in this case to the generic __module__, which may be represented as __classes__ or __components__, depending on the platform. From a domain standpoint, the technical definition of high functional cohesion overlaps with the goals of the __bounded context__ in domain-driven design: behavior and data that implement a particular domain workflow.
