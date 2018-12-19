# Unity-IA
This project is created for implement the AI game theory in Unity3D. All done by Eloi Salles Torres.

This repository contains:
* Steering Behaviors.
* Combined Steering Behaviors.
* Pathfinding.
  * Blind Search.
  * Informed Search.
* Decision Making.
  * Reaction Based.
* Others

## Steering Behaviors
It used for simulate a movement of the agent in a wolrd. 

We can find the most basics Steering Behaviors:
* Seek.
* Flee.
* Arrive.
* Pursue.
* Evade.
* Wander.
* Area Avoidence.
* Simple Path Following.
* Predicted Path Following.

## Combined Steering Behaviors
It combines some previous Steering Behaviors.

* Flocking (is a combined with weights).
* Combination1 (Area Avoidance, Predicted Path Following & Wander).

## Pathfinding
It contains algorithms is used for find goal. In the examples the goal is representated in world space.

### Blind Search Method

* Breadth First Search.
* Dijkstra.

### Informed Search Method

* Greedy Best First Search.
* A*.

## Decision Making
How the agent make decision at what it does.

### Reaction Based
* Hardcode Finite State Machine.
* Point Style Finite State Machine (using polymorfism).

## Others
Implement other AI concept using or not the Unity AI tools.

* Navigator Mesh (working).