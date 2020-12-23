v. 1.1

Thank you for downloading Ragdoll Copier.

If you have any questions, don't hesitate to mail me at contact@itchyowl.com!

INSTRUCTIONS
- GameObject -> Copy Ragdoll.
- Use humanoid mapping for non-identical rigs. Both objects has to have an animator component with a valid avatar. The animator is only required for mapping the transforms. It can be removed after the ragdoll has been copied.
- Disable "Auto Connect To Parent", if you want to set Joint.connectedBodies null for any reason. By default, all the joints are connected to the rigidbody found in the parent transform.
- If a physic material is defined, it will be assigned to all colliders.
- Filter out transforms by adding a phrase in the field "Ignore Transforms with" (In the video, labeled as "Filter Transforms with"). This is handy, when you cannot use humanoid mapping and there are certain parts in the source that are missing from the target(s). Another use case would be that you have defined custom colliders inside the source object that you don't want to copy to the targets.
- All the work is executed by RagdollCopier script. It can be found in ItchyOwl/RagdollCopier/Scripts/Editor/ folder. Besides that, there are only some extension methods found in ItchyOwl/_Common/Extensions. These files might be shared with my other assets.

NOTE
- You can use any method for creating the ragdoll (Default wizard or a custom system).
- Scaling, removing, or adding transforms does not break the system. But if the transform hierachy is not identical, you have to enable humanoid mapping to get the desired results.
- Copying a ragdoll between non-identical rigs may not work properly. Rather than fixing this by hand, it may be easier to create the ragdoll individually for each model (once). It depends, how different the rigs are.
- Copying a ragdoll between originally identical rigs that have been modified in Unity should work.

CHANGELOG
v.1.1
- Fixed humanoid mapping in Unity 2018.3