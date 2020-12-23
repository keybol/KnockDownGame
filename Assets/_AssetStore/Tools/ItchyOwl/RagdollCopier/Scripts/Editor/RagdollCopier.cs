using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using ItchyOwl.Extensions;

namespace ItchyOwl.Editor
{
    public class RagdollCopier : EditorWindow
    {
        private bool verbose;
        private bool useHumanoidMapping;
        private bool autoConnectToParent = true;
        private PhysicMaterial physicMaterial;
        private GameObject source;
        private int targetCount = 1;
        private static List<GameObject> targets = new List<GameObject>();
        private string ignoreTransformsWithName;

        #region GUI
        [MenuItem("GameObject/Copy Ragdoll")]
        public static void OpenWindow()
        {
            var window = GetWindow<RagdollCopier>(true, "RagdollCopier");
            window.Show();
        }

        public void OnGUI()
        {
            source = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Source", "Drag here the source object that has been ragdolled. You can use the built-in Ragdoll Wizard or a custom implementation for creating the ragdoll."), source, typeof(GameObject), true);
            physicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField("Physic Material", physicMaterial, typeof(PhysicMaterial), false);
            ignoreTransformsWithName = EditorGUILayout.TextField(new GUIContent("Ignore Transforms With", "Case insensitive. Not used for humanoid mapping."), ignoreTransformsWithName);
            useHumanoidMapping = EditorGUILayout.Toggle(new GUIContent("Use Humanoid Mapping", "Use the avatar definition to match the transforms. Useful for example, when you have modified the transform hierarchy."), useHumanoidMapping);
            autoConnectToParent = EditorGUILayout.Toggle(new GUIContent("Auto Connect Joints", "Automatically maps connected body properties of joint components to parents. If disabled, the connection will be null."), autoConnectToParent);
            targetCount = EditorGUILayout.IntSlider(new GUIContent("Target Count", "Drag the slider to adjust the target count."), targetCount, 1, 10);
            if (targetCount > targets.Count)
            {
                for (int i = targets.Count; i < targetCount; i++)
                {
                    targets.Add(null);
                }
            }
            else if (targetCount < targets.Count)
            {
                for (int i = targets.Count; i > targetCount; i--)
                {
                    targets.Remove(targets.LastOrDefault());
                }
            }
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i] = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Target " + (i + 1), "Drag here the target object(s)."), targets[i], typeof(GameObject), true);
            }
            verbose = EditorGUILayout.Toggle("Verbose", verbose);
            if (source != null && targets.Any(t => t != null))
            {
                if (GUILayout.Button("Copy"))
                {
                    string log = CopyRagdoll();
                    TextPopup.Display(log);
                }
            }
            if (GUILayout.Button("Clear Targets (Joints, Rigidbodies, Colliders)"))
            {
                ClearTargets();
                TextPopup.Display("Targets cleared.");
            }
        }
        #endregion

        #region Core functionality
        private void SetPhysicMaterials()
        {
            foreach (var collider in source.GetComponentsInChildren<Collider>())
            {
                collider.sharedMaterial = physicMaterial;
            }
        }

        private string CopyRagdoll()
        {
            if (physicMaterial != null)
            {
                SetPhysicMaterials();
            }
            ClearTargets();
            foreach (var target in targets.Where(t => t != null))
            {
                bool bothAreHumanoids = false;
                var sourceAnimator = source.GetComponentInChildren<Animator>();
                var targetAnimator = target.GetComponent<Animator>();
                bool animatorsFound = sourceAnimator != null && targetAnimator != null;
                if (animatorsFound)
                {
                    bothAreHumanoids = sourceAnimator.isHuman && targetAnimator.isHuman;
                    if (useHumanoidMapping)
                    {
                        if (sourceAnimator.avatar == null)
                        {
                            string msg = "The source avatar is null.Please define the avatar in order to use the humanoid mapping.";
                            Debug.LogError("[RagodollCopier] " + msg);
                            return msg;
                        }
                        else if (!sourceAnimator.avatar.isValid)
                        {
                            string msg = "The source avatar is not valid. Please check the avatar configuration.";
                            Debug.LogError("[RagodollCopier] " + msg);
                            return msg;
                        }
                        if (targetAnimator.avatar == null)
                        {
                            string msg = string.Format("The avatar of target {0} is null. Please define the avatar in order to use the humanoid mapping", target.name);
                            Debug.LogError("[RagodollCopier] " + msg);
                            return msg;
                        }
                        else if (!targetAnimator.avatar.isValid)
                        {
                            string msg = string.Format("The avatar of target {0} is not valid. Please check the avatar configuration.", target.name);
                            Debug.LogError("[RagodollCopier] " + msg);
                            return msg;
                        }
                    }
                }
                else
                {
                    if (useHumanoidMapping)
                    {
                        string msg = "Cannot find animators on both the source and the target. Humanoid definition is not possible.";
                        Debug.LogError("[RagdollCopier] " + msg);
                        return msg;
                    }
                }
                if (bothAreHumanoids && useHumanoidMapping)
                {
                    if (verbose)
                    {
                        Debug.Log("[RagdollCopier] Using humanoid mapping to copy the ragdolls.");
                    }
                    var sourceTransforms = new List<Transform>();
                    var targetTransforms = new List<Transform>();
                    var humanBones = Enum.GetValues(typeof(HumanBodyBones)) as HumanBodyBones[];
                    foreach (var boneType in humanBones)
                    {
                        if (boneType == HumanBodyBones.LastBone)
                        {
                            if (verbose)
                            {
                                Debug.Log("[RagdollCopier] Skipping " + boneType);
                            }
                            continue;
                        }
                        if (verbose)
                        {
                            Debug.Log("[RagdollCopier] Checking " + boneType);
                        }
                        var sourceBone = sourceAnimator.GetBoneTransform(boneType);
                        var targetBone = targetAnimator.GetBoneTransform(boneType);
                        if (sourceBone != null && targetBone != null)
                        {
                            sourceTransforms.Add(sourceBone);
                            targetTransforms.Add(targetBone);
                        }
                        else if (verbose)
                        {
                            Debug.LogWarningFormat("[RagdollCopier] Skipping {0} because it cannot be found in both objects.", boneType.ToString());
                        }
                    }
                    if (!Copy(sourceTransforms.ToArray(), targetTransforms.ToArray()))
                    {
                        return "Copy Error";
                    }
                }
                else
                {
                    if (verbose)
                    {
                        Debug.Log("[RagdollCopier] Trying to copy the ragdoll by comparing the transform arrays.");
                    }
                    var sourceTransforms = source.GetComponentsInChildren<Transform>(true);
                    var targetTransforms = target.GetComponentsInChildren<Transform>(true);
                    if (!string.IsNullOrEmpty(ignoreTransformsWithName))
                    {
                        ignoreTransformsWithName = ignoreTransformsWithName.ToLowerInvariant();
                        targetTransforms = targetTransforms.Where(t => !t.name.ToLowerInvariant().Contains(ignoreTransformsWithName)).ToArray();
                        sourceTransforms = sourceTransforms.Where(t => !t.name.ToLowerInvariant().Contains(ignoreTransformsWithName)).ToArray();
                    }
                    if (!Copy(sourceTransforms, targetTransforms))
                    {
                        return "Copy Error";
                    }
                }
                if (verbose)
                {
                    Debug.LogFormat("[RagdollCopier] {0} ready.", target.name);
                }
            }
            if (verbose)
            {
                Debug.Log("[RagdollCopier] Ready.");
            }
            return "Ragdoll successfully copied";
        }

        private bool Copy(Transform[] sourceTransforms, Transform[] targetTransforms)
        {
            if (sourceTransforms.Length != targetTransforms.Length)
            {
                Debug.LogErrorFormat("[RagdollCopier] Uneven number of transforms (source: {0}, target: {1}). Use humanoid mapping, if possible.", sourceTransforms.Length, targetTransforms.Length);
                return false;
            }
            for (int i = 0; i < sourceTransforms.Length; i++)
            {
                var targetT = targetTransforms[i];
                var sourceT = sourceTransforms[i];
                if (verbose)
                {
                    Debug.Log("[RagdollCopier] Checking child " + i);
                }
                // 3D
                GetAndCopyIfExists<Rigidbody>(sourceT, targetT);
                foreach (var joint in sourceT.GetComponents<Joint>())
                {
                    var charJ = CopyIfExists(joint as CharacterJoint, targetT) as Joint;
                    SetupJoint(charJ, targetT);
                    var configJ = CopyIfExists(joint as ConfigurableJoint, targetT) as Joint;
                    SetupJoint(configJ, targetT);
                    var fixedJ = CopyIfExists(joint as FixedJoint, targetT) as Joint;
                    SetupJoint(fixedJ, targetT);
                    var hingeJ = CopyIfExists(joint as HingeJoint, targetT) as Joint;
                    SetupJoint(hingeJ, targetT);
                    var springJ = CopyIfExists(joint as SpringJoint, targetT) as Joint;
                    SetupJoint(springJ, targetT);
                    if (charJ == null && configJ == null && fixedJ == null && hingeJ == null && springJ == null)
                    {
                        Debug.LogWarningFormat("[RagdollCopier] {0} cannot be copied, because the joint type {1} is not implemented. This should be easy to fix by adding CopyIfExists(joint as {1}, targetT) above this message.", joint, joint.GetType());
                    }
                }
                foreach (var sourceC in sourceT.GetComponents<Collider>())
                {
                    var boxC = CopyIfExists(sourceC as BoxCollider, targetT);
                    var sphereC = CopyIfExists(sourceC as SphereCollider, targetT);
                    var capsuleC = CopyIfExists(sourceC as CapsuleCollider, targetT);
                    var wheelC = CopyIfExists(sourceC as WheelCollider, targetT);
                    var meshC = CopyIfExists(sourceC as MeshCollider, targetT);
                    if (boxC == null && sphereC == null && capsuleC == null && wheelC == null && meshC == null)
                    {
                        Debug.LogWarningFormat("[RagdollCopier] {0} cannot be copied, because the collider type {1} is not implemented. This should be easy to fix by adding CopyIfExists(joint as {1}, targetT) above this message.", sourceC, sourceC.GetType());
                    }
                }
                // 2D
                GetAndCopyIfExists<Rigidbody2D>(sourceT, targetT);
                foreach (var joint in sourceT.GetComponents<Joint2D>())
                {
                    var fixedJ = CopyIfExists(joint as FixedJoint2D, targetT) as Joint2D;
                    SetupJoint(fixedJ, targetT);
                    var hingeJ = CopyIfExists(joint as HingeJoint2D, targetT) as Joint2D;
                    SetupJoint(hingeJ, targetT);
                    var springJ = CopyIfExists(joint as SpringJoint2D, targetT) as Joint2D;
                    SetupJoint(springJ, targetT);
                    if (fixedJ == null && hingeJ == null && springJ == null)
                    {
                        Debug.LogWarningFormat("[RagdollCopier] {0} cannot be copied, because the joint type {1} is not implemented. This should be easy to fix by adding CopyIfExists(joint as {1}, targetT) above this message.", joint, joint.GetType());
                    }
                }
                foreach (var sourceC in sourceT.GetComponents<Collider2D>())
                {
                    var boxC = CopyIfExists(sourceC as BoxCollider2D, targetT);
                    var circleC = CopyIfExists(sourceC as CircleCollider2D, targetT);
                    var capsuleC = CopyIfExists(sourceC as CapsuleCollider2D, targetT);
                    var compositeC = CopyIfExists(sourceC as CompositeCollider2D, targetT);
                    var edgeC = CopyIfExists(sourceC as EdgeCollider2D, targetT);
                    var polygonC = CopyIfExists(sourceC as PolygonCollider2D, targetT);
                    if (boxC == null && circleC == null && capsuleC == null && compositeC == null && edgeC == null && polygonC == null)
                    {
                        Debug.LogWarningFormat("[RagdollCopier] {0} cannot be copied, because the collider type {1} is not implemented. This should be easy to fix by adding CopyIfExists(joint as {1}, targetT) above this message.", sourceC, sourceC.GetType());
                    }
                }
            }
            return true;
        }

        private void ClearTargets()
        {
            Debug.Log("[RagdollCopier] Removing all Joints, rigidbodies, and colliders from the target objects...");
            foreach (var target in targets.Where(t => t != null))
            {
                int joint2dCount = target.RemoveComponentsInChildren<Joint2D>(isUsedInEditor: true);
                int jointCount = target.RemoveComponentsInChildren<Joint>(isUsedInEditor: true);
                int rbCount = target.RemoveComponentsInChildren<Rigidbody>(isUsedInEditor: true);
                int rb2dCount = target.RemoveComponentsInChildren<Rigidbody2D>(isUsedInEditor: true);
                int colliderCount = target.RemoveComponentsInChildren<Collider>(isUsedInEditor: true);
                if (verbose)
                {
                    Debug.LogFormat("Joint2Ds removed from {0}: {1}", target.name, joint2dCount);
                    Debug.LogFormat("Joints removed from {0}: {1}", target.name, jointCount);
                    Debug.LogFormat("Rigidbodies removed from {0}: {1}", target.name, rbCount);
                    Debug.LogFormat("Rigidbody2Ds removed from {0}: {1}", target.name, rb2dCount);
                    Debug.LogFormat("Colliders removed from {0}: {1}", target.name, colliderCount);
                }
            }
        }
        #endregion

        #region Helpers
        private void SetupJoint(Joint joint, Transform targetT)
        {
            if (joint != null)
            {
                if (autoConnectToParent)
                {
                    SetConnectedBody(joint, targetT);
                }
                else
                {
                    // Clear the reference, in case it's invalid.
                    joint.connectedBody = null;
                }
            }
        }

        private void SetupJoint(Joint2D joint, Transform targetT)
        {
            if (joint != null)
            {
                if (autoConnectToParent)
                {
                    SetConnectedBody(joint, targetT);
                }
                else
                {
                    // Clear the reference, in case it's invalid.
                    joint.connectedBody = null;
                }
            }
        }

        private void SetConnectedBody(Joint2D joint, Transform targetT)
        {
            var parentBody = targetT.GetComponentsOnlyInParents<Rigidbody2D>(true).FirstOrDefault();
            if (parentBody == null)
            {
                Debug.LogWarningFormat("[RagdollCopier] Cannot find rigidbody2d for {0} from the parent transform {1}", targetT.name, targetT.parent.name);
            }
            else
            {
                joint.connectedBody = parentBody;
                if (verbose) { Debug.LogFormat("[RagdollCopier] {0} connected to {1}", joint, parentBody); }
            }
        }

        private void SetConnectedBody(Joint joint, Transform targetT)
        {
            var parentBody = targetT.GetComponentsOnlyInParents<Rigidbody>(true).FirstOrDefault();
            if (parentBody == null)
            {
                Debug.LogWarningFormat("[RagdollCopier] Cannot find rigidbody for {0} from the parent transform {1}", targetT.name, targetT.parent.name);
            }
            else
            {
                joint.connectedBody = parentBody;
                if (verbose) { Debug.LogFormat("[RagdollCopier] {0} connected to {1}", joint, parentBody); }
            }
        }

        private UnityEngine.Object GetAndCopyIfExists<T>(Transform sourceT, Transform targetT) where T : Component
        {
            return CopyIfExists(sourceT.GetComponent<T>(), targetT);
        }

        private UnityEngine.Object CopyIfExists<T>(T source, Transform targetT) where T : Component
        {
            if (source != null)
            {
                var copy = targetT.gameObject.GetOrAddComponent<T>();
                EditorUtility.CopySerialized(source, copy);
                if (verbose) { Debug.LogFormat("[RagdollCopier] Copy of {0} created.", copy); }
                return copy;
            }
            return null;
        }
        #endregion
    }
}