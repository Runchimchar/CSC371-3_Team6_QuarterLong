Lol I guess this is our README...

Using Player Prefab:

- First, drag in MainCamera prefab.
- Then, drag in PlayerFollowCamera prefab.
- Then, drag in PlayerAimCamera prefab.
- Then, drag in Player prefab.
- Then, drag in Canvas prefab.
- Finally, assign PlayerCameraRoot (inside of Player) to both the the "Follow" and the "Look At" properties of the CinemachineVirtualCamera components for both PlayerAimCamera and PlayerFollowCamera.



Using Respawn System:

    Setting up respawn controller:
        1. Drag in RespawnSystem prefab.
        2. Position included Checkpoint object away from play area for now.
        3. Click on RespawnController object (inside RespawnSYSTEM object) and drag Player object into the "Pm" field.
        Now you can play the scene and press "/" to respawn at original checkpoint.
    
    Setting up Respawn Triggers:
        1. Follow above to set up respawn controller.
        2. Drag in RespawnOnEnter prefab(s) and position them where you want it.
        3. That should be it lol

    Setting up Checkpoints:
        1. Follow above to set up RespawnSystem prefab.
        2. Position included Checkpoint object such that the center of the collider is on the floor where you want the 
            player to respawn. See below for tips on placing Checkpoints.
        3. Click on RespawnController object (inside RespawnSYSTEM object) and click plus on the Checkpoints list. Drag the 
            Checkpoint object into the new slot.
        4. For any additonal checkpoints, duplicate the given Checkpoint object and repeat steps 2 and 3. Make sure they 
            are in the correct order in the RespawnController list (order that the player will pass through them)
        
        Notes on placing Checkpoints:
        - Reset your RespawnSystem to (0,0,0). Then you can just set the desired respawn location as the position of the
            Checkpoint object.
        - When resizing the Checkpoint trigger, you can adjust the Size field to maintain the same center.
        - The player will actually spawn 1 unit above the center of the location, so its important that you keep the center
            of the trigger on the ground.
        - If its too difficult, tell Angela and she can change it to a seperate trigger for passing through and a seperate
            empty to indicate desired respawn location x.x