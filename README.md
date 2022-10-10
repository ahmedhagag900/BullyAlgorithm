# Bully Algorithm


The bully algorithm is a type of Election algorithm which is mainly used for choosing a coordinate. <br>
In a distributed system, we need some election algorithms to get a coordinator that performs functions needed by other processes.<br>
#### refrences: <br>
- https://en.wikipedia.org/wiki/Bully_algorithm <br>
- https://www.geeksforgeeks.org/election-algorithm-and-distributed-processing <br>

## Porpused Solution
- Used the message passing mechanism as way of communication between process
- The pub/sub pattern was used to implement the message passing mechanism
- To implement the pub/sub pattern I used the native C# events [could use sockets insted]
<br><br>
![events](https://www.tutorialsteacher.com/Content/images/csharp/event-model.png)

## Technologies
- C#
- Windows Form
- .NET 6

## How to Run
 - Clone the repo and make the SimulationApp as startup project then run.
 - Eneter number of process to simulate <br> <br>
 ![simulation1](https://user-images.githubusercontent.com/69547439/194932036-2e8b6880-54cd-434f-8994-d2e67b806e25.PNG)
 - Watch the processes communications and control them. <br> <br>
![simulation2](https://user-images.githubusercontent.com/69547439/194932043-2aa64cdb-5590-415c-b938-262ddd0de68c.PNG)

## Author
[Ahmed Hagag](https://github.com/ahmedhagag900)

