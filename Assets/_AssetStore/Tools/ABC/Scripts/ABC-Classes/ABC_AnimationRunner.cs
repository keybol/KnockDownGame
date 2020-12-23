using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Animation runner component will allow for animation clips to play without the use of the animator. 
/// ABC will attach clips to the component and run them. Requires an Animator component to be attached to the object however no animations 
/// are required to be setup on the Animator. If an Animator component doesn't exist one is added 
/// </summary>
public class ABC_AnimationRunner : MonoBehaviour{

    // ********************* Settings ********************
    #region Settings

    /// <summary>
    /// The animation clip to run
    /// </summary>
    public AnimationClip animationClip;

    /// <summary>
    /// If enabled then the animation will play, used for debugging
    /// </summary>
    public bool Play = false;

    #endregion

    // ********************* Variables ********************

    #region Variables

    /// <summary>
    /// Animator Component - needs to exist but doesn't need to have the animations
    /// </summary>
    private Animator meAni;

    /// <summary>
    /// The playable graph used to run the animation
    /// </summary>
    private PlayableGraph playableGraph;

    /// <summary>
    /// Tracks the run animation method currently running the animation clip, used to interupt the method if another clip is played
    /// </summary>
    private List<IEnumerator> aniRunCoroutines = new List<IEnumerator>();

    /// <summary>
    /// Tracks the stop animation method currently stopping the animation clip, used to interupt the method if another clip is ended
    /// </summary>
    private List<IEnumerator> aniStopCoroutines = new List<IEnumerator>();

    /// <summary>
    /// Tracks the run and stop animation method currently starting and ending the animation clip in one call, used to interupt the method if another clip is played
    /// </summary>
    private List<IEnumerator> aniRunStopCoroutines = new List<IEnumerator>();

    #endregion

    // ********************* Public Methods ********************

    #region Public Methods

    /// <summary>
    /// Will start and stop an animation using the clip provided after the delay and duration set. 
    /// </summary>
    /// <param name="Animation">Animation clip to play</param>
    /// <param name="Delay">Delay before animation clip runs</param>
    /// <param name="Speed">The speed of the animation clip</param>
    /// <param name="Duration">The duration before the animation ends, if 0 is provided then the duration will last for the whole animation clip</param>
    /// <param name="InterruptCurrentAnimation">(Optional) If true then this animation will interrupt any animations currently playing, else if false then the animation won't play if one is currently running</param>
    public void PlayAnimation(AnimationClip Animation, float Delay = 0f, float Speed = 1f, float Duration = 0f, bool InterruptCurrentAnimation = true) {

        //If set to not interrupt any current animations and one is playing then return here
        if (InterruptCurrentAnimation == false && this.playableGraph.IsValid() && this.playableGraph.IsPlaying()){
            return; 
        }


        //If any run and stop animations are currently in progress then stop them ready for the new one
        if (this.aniRunStopCoroutines.Count > 0) {
        this.aniRunStopCoroutines.ForEach(c => StopCoroutine(c));
        this.aniRunStopCoroutines.Clear();
        }

        //Track the animation run and stop method which is about to activate
        IEnumerator newCoroutine = null;

        //Run animation and then stop it after duration 
        StartCoroutine((newCoroutine = RunAndStopAnimation(Animation, Delay, Speed, Duration)));

        //Add the method to the tracker to remove early if required (another animation may start early) 
        this.aniRunStopCoroutines.Add(newCoroutine);

    }

    /// <summary>
    /// Will start the animation clip after the delay
    /// </summary>
    /// <param name="Animation">Animation clip to play</param>
    /// <param name="Delay">Delay before animation clip runs</param>
    /// <param name="Speed">(Optional) The speed of the animation clip</param>
    /// <param name="DontInterrupt">(Optional) If true then this animation will interrupt any animations currently playing, else if false then the animation won't play if one is currently running</param>
    public void StartAnimation(AnimationClip Animation, float Delay, float Speed = 1f){


        if (this.aniRunCoroutines.Count > 0) {
            this.aniRunCoroutines.ForEach(c => StopCoroutine(c));
            this.aniRunCoroutines.Clear();
        }

        //If any run and stop animations are currently in progress then stop them ready for the new one
        if (this.aniStopCoroutines.Count > 0) {
            this.aniStopCoroutines.ForEach(c => StopCoroutine(c));
            this.aniStopCoroutines.Clear();
        }


        //Show in editor what animation is playing
        this.animationClip = Animation; 

        //Track the animation run  method which is about to activate
        IEnumerator newCoroutine = null;

        //Run animation 
        StartCoroutine(newCoroutine = RunAnimation(Animation, Delay, Speed));

        //Add the method to the tracker to remove early if required (another animation may start early) 
        this.aniRunCoroutines.Add(newCoroutine);
    }


    /// <summary>
    /// Will end the animation that is currently playing, unless another one has started
    /// </summary>
    /// <param name="Animation">Animation clip to end</param>
    /// <param name="Delay">Delay before animation clip stops</param>
    public void EndAnimation(AnimationClip Animation, float Delay = 0f) {

        //If the animation currently playing does not match the animaiton we are ending
        //then end here as another animation has already started
        if (Animation != this.animationClip)
            return;

        //If any run and stop animations are currently in progress then stop them ready for the new one
        if (this.aniStopCoroutines.Count > 0)
        {
            this.aniStopCoroutines.ForEach(c => StopCoroutine(c));
            this.aniStopCoroutines.Clear();
        }

        //Track the animation run and stop method which is about to activate
        IEnumerator newCoroutine = null;

        //Run animation and then stop it after duration 
        StartCoroutine((newCoroutine = this.StopAnimation(Delay)));

        //Add the method to the tracker to remove early if required (another animation may start early) 
        this.aniStopCoroutines.Add(newCoroutine);
    }

    /// <summary>
    /// Will interrupt the current animation that is playing 
    /// </summary>
    public void InterruptCurrentAnimation() {

        //Stop all coroutines 
        this.aniRunCoroutines.ForEach(c => StopCoroutine(c));
        this.aniRunCoroutines.Clear();

        this.aniStopCoroutines.ForEach(c => StopCoroutine(c));
        this.aniStopCoroutines.Clear();

        this.aniRunStopCoroutines.ForEach(c => StopCoroutine(c));
        this.aniRunStopCoroutines.Clear();


        StartCoroutine(this.StopAnimation());
    }

    #endregion

    // ********************* Private Methods ********************

    #region Private Methods

    /// <summary>
    /// This method is the code to run the animation - it will create the clip and playable graph and then start the animation
    /// </summary>
    /// <param name="Animation">Animation clip to play</param>
    /// <param name="Delay">Delay before animation clip runs</param>
    /// <param name="Speed">The speed of the animation clip</param>
    private IEnumerator RunAnimation(AnimationClip Animation, float Delay,  float Speed = 1f){

        //If animation clip has not been provided then end here
        if (Animation == null)
            yield return null;

        //If a delay has been provided then wait 
        if (Delay > 0)
            yield return new WaitForSeconds(Delay);

        //If a previous animation clip is already playing then stop it 
        if (this.playableGraph.IsValid() && this.playableGraph.IsPlaying())
            this.playableGraph.Stop();

        //Create the playable graph and set time update mode
        this.playableGraph = PlayableGraph.Create();
        this.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        //Create the animator animation playable output 
        var playableOutput = AnimationPlayableOutput.Create(this.playableGraph, "ABC_Animation", this.meAni);

        // create and wrap the animation clip in a playable
        var clipPlayable = AnimationClipPlayable.Create(playableGraph, Animation);

        //Set the properties of the animation (speed, footOK)
        clipPlayable.SetSpeed(Speed);
        clipPlayable.SetApplyFootIK(true);

        // Connect the Playable to an output
        playableOutput.SetSourcePlayable(clipPlayable);

        // Plays the Graph/Animation.
        this.playableGraph.Play();
    }

    /// <summary>
    /// Will perform the code to stop the animation that is currently playing 
    /// </summary>
    /// <param name="Delay">Delay before animation clip runs</param>
    private IEnumerator StopAnimation(float Delay = 0f){

        yield return new WaitForSeconds(Delay);

        if (this.playableGraph.IsValid() && this.playableGraph.IsPlaying())
            this.playableGraph.Stop();
    }

    /// <summary>
    /// Will start and stop an animation using the clip provided after the delay and duration set. 
    /// </summary>
    /// <param name="Animation">Animation clip to play</param>
    /// <param name="Delay">Delay before animation clip runs</param>
    /// <param name="Speed">The speed of the animation clip</param>
    /// <param name="Duration">The duration before the animation ends, if 0 is provided then the duration will last for the whole animation clip</param>
    private IEnumerator RunAndStopAnimation(AnimationClip Animation, float Delay, float Speed = 1f, float Duration = 0) {

        //Start the animation
        this.StartAnimation(Animation, Delay, Speed);

        //Wait for duration (if 0 duration is set then use the animations full duration)
        if (Duration == 0)
            yield return new WaitForSeconds((float)Animation.length + Delay);
        else
            yield return new WaitForSeconds(Duration + Delay);

        //Stop the animation
        this.EndAnimation(Animation);
        
      
        
    }

    #endregion

    // ********************** Game ******************

    #region Game

    private void Awake(){

        //If animator component doesn't exist on the object add it and track
        if (this.meAni == null){
            this.meAni = gameObject.GetComponent<Animator>();

            if (this.meAni == null)
                Debug.LogError("Animator not found on object. Please add one to the object to play animation clips using ABC's Animation Runner");
        }


        //Stop all coroutines 
        this.aniRunCoroutines.ForEach(c => StopCoroutine(c));
        this.aniRunCoroutines.Clear();

        this.aniStopCoroutines.ForEach(c => StopCoroutine(c));
        this.aniStopCoroutines.Clear();

        this.aniRunStopCoroutines.ForEach(c => StopCoroutine(c));
        this.aniRunStopCoroutines.Clear();


    }


    void Update(){

        //For debugging will play the animation using the inspector 
        if (this.Play == true) {
            this.PlayAnimation(this.animationClip);
            this.Play = false;
        }
    }

  

    void OnDisable() {
   
        //Destroy the playable graph 
        if (this.playableGraph.IsValid())
        this.playableGraph.Destroy();
    }

    #endregion

}