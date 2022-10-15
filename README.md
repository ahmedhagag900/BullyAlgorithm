# Bully Algorithm


The bully algorithm is a type of Election algorithm which is mainly used for choosing a coordinate. <br>
In a distributed system, we need some election algorithms to get a coordinator that performs functions needed by other processes.<br>
#### Refrences: <br>
- https://en.wikipedia.org/wiki/Bully_algorithm <br>
- https://www.geeksforgeeks.org/election-algorithm-and-distributed-processing <br>
- https://www.ques10.com/p/36619/explain-bully-election-algorithm-1/? <br>
- https://www.youtube.com/watch?v=kQOy_6_I8i4 <br>
- https://isuruuy.medium.com/electing-master-node-in-a-cluster-using-bully-algorithm-b4e4fa30195c <br>
- https://www.nginx.com/blog/service-discovery-in-a-microservices-architecture/#:~:text=The%20service%20registry%20is%20a,obtained%20from%20the%20service%20registry. <br>

## Porpused Solution
- Used the message passing mechanism as way of communication between process.
- The peer-peer communication was used to implement the message passing communication.
- To implement the peer-peer communication I used socket programing 
<br><br>
![p2p](https://media.geeksforgeeks.org/wp-content/uploads/TCP_new.png))
## Assumptions
- Each process knows the id and address of every other process. [Used service discovery](https://www.nginx.com/blog/service-discovery-in-a-microservices-architecture/#:~:text=The%20service%20registry%20is%20a,obtained%20from%20the%20service%20registry.) <br>
- P sends election message to all process with higher IDS and awaits OK messages.
- If no OK messages, P becomes coordinator and sends coordinator messages to all processes.
- If it receives an OK, it drops out and waits for an coordinator.
- If a process receive an election message.
- Immediately sends coordinator message if it is the process with highest ID.
- Otherwise returns an OK and start election.
- If a process receives a coordinator message, it treats sender as a co-coordinator.
## Technologies
- C#
- Windows Form
- .NET 6
- Socket

## How to Run
 - Clone the repo and make the SimulationApp as startup project then run.
 - Eneter number of process to simulate <br> <br>
 ![simulation1](https://user-images.githubusercontent.com/69547439/194932036-2e8b6880-54cd-434f-8994-d2e67b806e25.PNG)
 - Watch the processes communications and control them. <br> <br>
![simulation2](https://user-images.githubusercontent.com/69547439/195998762-17f65409-7497-48c6-bebd-cc0dd2ba9773.PNG)
## Diagnostices
- If there is any problem faced in running the app clean the sloution and build it again.
- Make sure there is no any processes named ```ClusterServer.exe``` or ```ProcessConsole.exe``` is runing in the background (use task manger to close them)
## Author
[Ahmed Hagag](https://github.com/ahmedhagag900)

