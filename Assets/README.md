Lol I guess this is our README...

Using Player Prefab:

- First, drag in MainCamera prefab.
- Then, drag in PlayerFollowCamera prefab.
- Then, drag in PlayerAimCamera prefab.
- Then, drag in Player prefab.
- Then, drag in Canvas prefab.
- Finally, assign PlayerCameraRoot (inside of Player) to both the the "Follow" and the "Look At" properties of the CinemachineVirtualCamera components for both PlayerAimCamera and PlayerFollowCamera.
