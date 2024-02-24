* [Cohesion](#cohesion)

# Cohesion

## Levels of Cohesion

__(From best to worst)!__

* __Functional cohesion__ - It is considered to be the highest degree of cohesion, and it is highly expected. Elements of module in functional cohesion are grouped because they all contribute to a single well-defined function. It can also be reused.
* __Sequential cohesion__ - When elements of module are grouped because the output of one element serves as input to another and so on.
* __Communicational cohesion__ - When elements of module are grouped together, which are executed sequentially and work on same data. Grouped because of common I/O source.
* __Procedural cohesion__ - Grouped to ensure order of use. Grouped together, which are executed sequentially in order to perform a task.
* __Temporal cohesion__ - Grouped by time of use.
* __Logical cohesion__ - Grouped by function but not related. Logically categorized elements are put together.
* __Coincidental cohesion__ - Grouped by chance. It is unplanned and random cohesion, which might be the result of breaking the program into smaller modules for the sake of modularization

## Cohesion Metrics

* Lack of Cohesion in Methods (LCOM)
* Cohesion Among Methods in a Class (CAM)
* Responsibility Driven Design Metrics
* Cyclomatic Complexity
* Semantic Cohesion
* Component Dependency Metrics
* Coupling Metrics
