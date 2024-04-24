
<img src="https://github.com/LuddeM/LuddeM.github.io/blob/master/img/procedurally-generated-trees.baea6c68.png" width="560px"/>

# Procedurally Generated Trees, Created in Unity (C#)

Procedurally generated trees project created in Unity, based on a paper called <a href='https://www.researchgate.net/publication/221314843_Modeling_Trees_with_a_Space_Colonization_Algorithm'>Modeling Trees with a Space Colonization Algorithm</a>.

Was created for the course [Procedural Methods for Images](https://studieinfo.liu.se/en/kurs/TNM084/ht-2024) during my studies at [Master of Science in Media Technology and Engineering](https://studieinfo.liu.se/en/program/6cmen/#overview). A computer-science program with focus the technology of images, videos, and computer graphics, as well as user experience.

The project was nominated for the Norrköping Visualization Center's C-Awards 2018 in the **<i>Technical Excellence</i>** category.

## About

- A project which generates trees from GameObjects placed in the world, using an adaptation of the above mentioned algorithm.

- The project had a strong focus on writing **<i>clean code</i>** and following **<i>good coding practices</i>**.

- It also contains a system for capturing images of the algorithm progress, to be able to visually show how it works.

- The algorithm uses various parameters and can, depending no the settings, create a great variation of tree shapes 
    - The 'Random Seed' makes it possible to recreate the same result each time.
    - What each setting means is best understood by reading the scientific paper and looking at the implementation.

<p align="center">
  <img src="https://github.com/LuddeM/LuddeM.github.io/blob/master/img/standard-tree-settings.015e19b2.png" width="420px"/>
</p>

## Building the Project

Project uses Unity version **5.6.1f1**, which by now is fairly old. Until I update it to a newer version (which I have considered doing) I would recommend to instead look at the images and video on my website's [project page](https://luddem.github.io/#/projects).

## Future Work

- Make it possible to use other shapes than spheres as basis for the attractor point generation.
- Use more Unity features, for example ScriptableObjects to save tree settings.
- Use assets from the Asset Store to make a better looking demo scene for the trees and improve how the generated trees look.
- Make the project into an editor tool that could be used to place various trees, instead of generating them when game is run.
