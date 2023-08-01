# Cybersickness reduction techniques comparison
## Related papers   
This project is closely linked to the projects mentioned below. If you want to get involved in the project, reading these research documents may be a good starting point. 
* [Evaluating discrete viewpoint control to reduce cybersickness 
in virtual reality](https://www.csit.carleton.ca/~rteather/pdfs/VR_2020_cybersickness.pdf)
* [Viewpoint Snapping to Reduce Cybersickness in Virtual Reality](https://www.csit.carleton.ca/~rteather/pdfs/GI_2018_viewpoint_snapping.pdf)
* [Motion Sickness Conditioning to Reduce Cybersickness](https://www.csit.carleton.ca/~rteather/pdfs/SUI21_poster.pdf)

***

## Previous version of the project
The project originally stems from [this version of the project](https://drive.google.com/file/d/1oWPezfK7JtgemdlXkOMTT7fSMHVBTprh/view?usp=share_link ).
This version has the following features :
* Unity 2017.4.30f1
* Unknown version of OVR plugin probably from 2019 
* [VrTunnellingPro](http://www.sigtrapgames.com/VrTunnellingPro/html/index.html) version 1.2.1

This version is old and incompatible with the new Oculus standalone headsets (Meta Quest). It is important to note that this version can be used as a reference point but should no longer be maintained.

***

## Description of the application
The main application is divided into 3 scenes: `Menu`, `TunnellingSnapping` and `End`.
* **Menu** leaves the user in a flat mode, serving as an intermediary world allowing the experimenter to set the virtual environment parameters such as subject ID, group number and day of the experiment.
* **TunnellingSnapping** is the scene where the player is in control. Players are invited to move through the virtual environment with certain conditions affecting their vision or movement. Their objective is to pass through various checkpoints and self-assess their level of discomfort.
* **End** as for only  of the purpose to tell the user and the experimenter that the experiment is over.

***

## Technical glossary 
| Term | Alias | Signification |        
| - | - | - |
| Condition | Experiment parameters | Set of parameters affecting the user's view and movements. This parameter set is made up of: hasRotationSnapping, hasTranslationSnapping, hasTunneling. |
| Mission | N/A | During the experience, the user will carry out several missions. The missions consist of **an objective**: cross the checkpoints, **an end-of-mission objective**: return to the starting point and **an end-of-mission quiz** to help users self-assess their level of discomfort. |
| Snap | Snap rotation - Snap translation | In virtual reality, the term "snap" refers to the abrupt and precise movement or rotation of an object or the user's viewpoint within the virtual environment. It involves quick and accurate repositioning or reorientation without the need for smooth, continuous motion. Snap translation allows for instant and precise relocation from one point to another, while snap rotation enables immediate and accurate changes in the direction faced by the user.  |

***

## Code architecture
```
Dependencies:
- Unity 2021.3.25f1 (+ Android support)
- OculusIntegration plugin v53.1
- VR Tunneling Pro v1.4.0 
```

### OVRPlayerController
It's the code made by Oculus in around 2019 includes in the old OculusIntegration plugin.
While migrating to version 53.1 [I](https://github.com/KilianBonnet) noticed changes on OVRPlayerController script specifically.


[OLD NOTE]<br>
This script has not been updated by now and it still runs on an old version of OculusIntegration.<br>

[NEW NOTE]<br>
The old script had issues with snap translation. Oculus Integration Plugin is (at this time) a big mess. So I decided to update the OVRPlayerController and rewrite the modifications.

Here are the big parts of the modifications 
- Boolean **CanMove**: Enable/Disable movements.
- Boolean **TranslationSnapping**: Enable/Disable the translation snapping
- Float **SnapDistance** : The distance to teleport when using translation snapping
- Float **SnapRotationCooldown** & **SnapTranslationCooldown**: How long the user should wait between two snaps.
- Float **snapRotationTimer** & **snapTranslationTimer**: Internal clocks to proceed cooldowns.
- This scripts invoke an event called `SnappingEvent` every time the user snap rotate & snap translate. It is listened by the ConditionApplier to create a tunneling effect.

Edited code can is marked with "[Custom parameters]" or "[Custom code]" annotations.


### VR Tunneling Pro
This script was made by [SigtrapGames](https://github.com/sigtrapgames/VrTunnellingPro-Unity).
Please note that the version 1.4 is currently only available on GitHub, Unity Asset Store version is still 1.2 and is not compatible. 

This script is used to reduce the user's field of vision depending on the speed of rotation and/or movement of the OVRPlayerController in the space.


### Group
Group is the class associating different group numbers with different order of experimental tests.
If group 1 and 3 are associated with the order `0 1 2 3` and group 2 is associated with the order ` 2 3 1 0` then, at the start of the game, the players of group 1 and 3 will play with the parameters associated with id 0 while the players of group 2 will play with the parameters of id 2.


### GameManager
This class is automatically executed when the project is launched. It is responsible for successively launching the different missions to the player and alternating between the different environments and the different experimental parameters.

### Mission
The missions is represented by:
* **The checkpoints to reach**. The user must traverse the checkpoints one by one. These disappear and appear according to the order of collision. There is always 1 maximum active checkpoint. Checkpoints are represented by `private Transform checkpoints;`. It is important to note that GetChild(0) corresponds to the player's teleport point. The first checkpoint is at index 1.

During a mission, the mission can be in different states that will modify the objectives or the user's movements.
| Mission State | Details |      
| - | - |
| WaitingInit | The mission is waiting to be initialized, and does not read any user input. Related: `InitializeMission()`, `TeleportPlayer()`, `InitializeTunneling()`|
| StartMenu | The mission has been initialized, the player is teleported and must press a button to start the mission. The mission listens to the player's input to know when to start the mission. Related: `CheckForBeginMissionInput()`, `StartMission()` |
| Playing | The user pressed the button to start the mission. The user must now go through the n checkpoints that will appear as the mission progresses. Related: `OnCheckpointReached()`, `UpdatePlayerLogs()`, `ReturnMission()`, `UpdateCheckpointObjective()` |
| WaitingForStartPoint | The user has completed the n checkpoints, and now needs to return to the starting point and press a button to activate the nausea menu. The mission listens to the player's input to determine whether a key has been pressed. Related: `CheckForEvaluateMissionInput()`, `EvaluateMission()` |
| NauseaMenu | The mission activates the nausea menu canvas, and the user must enter its level of discomfort. Once the discomfort level has been selected, the user's average score is recorded and the lead is given to the GameManager. `OnNauseaScoreSelection()`, `UpdatePlayerAverageLogs()` |

### ConditionApplier
This class manage all the condition-related stuff. For instance, enabling/disabling  rotation snapping or translation snapping or even tweak snapping and tunneling parameter.

**Experiment parameters** are given when the `InitializeConditions()` method is call. Experiment parameters represents the different visual effects (such as tunnelling) and movement effects (such as snapping) that are effective or not on the current level.

When `InitializeConditions()` is called it will also read the data stored in the PlayerPrefs to set parameters such as the cooldown between two snap rotation/translation or even the distance and the angle of a snapping.

As mentioned in OVRPlayerController, this script listen to SnappingEvent. Every time the user will snap, the ConditionApplier will, depending on whether the Tunneling is enable or not, tweak the VrTunnellingPro parameters to add a vignette (tunnelling) to the player vision. 


### PlayerDataWriter
PlayerDataWriter is the class used to transform the user collected data into a text file. This text file is saved into the persistentDataPath. `WritePlayerData()` returns the given paths where the data files has been saved.

Related: `MailTo.cs`


### MailTo
MailTo is the class used to email the results of the current experiment.
The recipient's e-mail address and SMTP server settings can be configured from the unity editor. There is a default configuration that should work.

`SendEmail()` is a public function that should be called to end the experiment result by mail.
* **Player** is the given player that completed the experiment.
* **filePaths** is an array of string representing the path to the data file to send attached in the mail.

### Trail
Trail is the class used to draw a trail between the current user's position and a given coordinate. The given coordinate is usually the position of the next checkpoint.
The trail script relies on the NavMeshAgent present by default in Unity. NavMeshAgent uses a modified version of A* path-finding algorithm.

`DrawTrail()` should be called whenever you want to create a trail. It takes a Vector3 in input: its destination point.

`UpdateTrail()` and `Update()` work together, every x seconds (that you can specify on the script parameters) it will update the trail shape based on the new position of the user.

`StopTrail()` stops the trail 👍.