// original shader by @bgolus, modified slightly by @alexanderameye for URP, thanks Ben
// https://twitter.com/bgolus
// https://medium.com/@bgolus/the-quest-for-very-wide-outlines-ba82ed442cd9

// URP pass/feature code written by @alexanderameye



== USAGE ==
1. Add outline renderer feature to URP forward renderer
2. Choose layer 
3. On your object, in the mesh renderer component, in the 'rendering layer mask' setting
	you need to select that same layer as you set in the renderer feature, this object will then receive the outline



If you have issue, you can reach me on Twitter @alexanderameye but apologies in advance if I don't get the time to respond :/