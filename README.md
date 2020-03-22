# Shaiya Origin

## Notice
This repository is no longer under active development! Please follow current development here: https://github.com/aosyatnik/Imgeneus

## Introduction
Shaiya Origin is an emulator for the Shaiya MMORPG. The core servers are written in C#, with an extendable scripting system written in Python (not yet completed). This project was solely made for learning purposes and is not intended to be used in production.

The project is split into 4 components: 
* The common library

   Contains code shared by multiple servers.

* The database server

   Accepts connections from the login and game servers, and asynchronously processes database requests, and responds with the database.
* The login server

   Connects to the database server, processes login requests, etc. 

* The game server

   Retrieves definitions and player data from the database server, and handles all game related activities.

This project is designed to be fully scalable, so all these servers can be run independently. This means it's very easy to add multiple game servers using the same database/login server.

## Copyright
License: MIT
Read file: [LICENSE](LICENSE.txt)
## Images

![Login server test](https://i.imgur.com/TmF6Jmn.jpg)

![Character list test](https://i.imgur.com/Nr9SNsk.png)

![Game world test](https://i.imgur.com/u4O9s4D.jpg)



