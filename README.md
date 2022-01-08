# Hider and Seeker with Reinforcement Learning (Unity ML Agents)

<a href="https://ibb.co/1vMB99c"><img src="https://i.ibb.co/YRXJttg/image-20220107130341677.png" alt="image-20220107130341677" border="0"></a>

There  are  two  main  types  of  agents  the  hiders  and  the  seekers.  Each has equal rights of movement and to move objects in the environment.  The  agents  are  3D  figures  taken from Sketchfab after  characters  from  the  famous animated science fiction series “Rick and Morty”, Mr. Meeseeks. The development of each agent starts with defining them as game objects,for the initial implementation we use box-like agents with rigid bodies (green for the Seeker and blue for the Hider).  The vision abilities of the agents are approximated by implementing raycasting component, which help the agents locate objects within their visual field.  For the implementation each object of interest needs to be tagged in order to be returned as an valid response when the raycast hit it.

For more, take a look at our paper [here](https://drive.google.com/file/d/12wwRcMHF5alRkOfA5pQJiq3aoQJM1pZX/view?usp=sharing)
