// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TowerOfHanoiGame.cs" company="PG BullsEye">
//   Author: Daniela Betzl
// </copyright>
// <summary>
//   This script is the controller of the game towers of hanoi. It checks and execute the turns.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script is the controller of the game towers of hanoi. It checks and execute the turns.
/// </summary>
public class TowerOfHanoiGame : MonoBehaviour
{
    public const int DirectionLeft = -1;
    public const int DirectionRight = 1;
    public const int StepOne = 1;
    public const int StepTwo = 2;
    public const int AngleError = 35;
    public const double DistanceError = 0.7;

    public Material MaterialNormal;
    public Material MaterialSelected;

    private List<GameObject> plates;
    private List<GameObject> stabs;
    private List<List<GameObject>> stacks;
    private List<float> positionHeights;

    private GameObject invalidMsg;
    private GameObject finishedMsg;
    private GameObject resetMsg;
    private GameObject smallerMsg;
    private GameObject leftMsg;
    private GameObject rightMsg;
    private GameObject platesMsg;

    private GameObject fixedStab;
    private GameObject dimmerSprite;
    private bool gameFinished;
    private bool gestureEnabled;

    /// <summary>
    /// Initialize the script. Setting up all variables and their values.
    /// </summary>
    public void Start()
    {
        this.plates = new List<GameObject>();
        this.stabs = new List<GameObject>();
        this.stacks = new List<List<GameObject>>();
        this.positionHeights = new List<float>();

        for (int i = 0; i <= 2; i++)
        {
            this.stabs.Add(this.gameObject.transform.GetChild(i).gameObject);
            this.stacks.Add(new List<GameObject>());
        }

        for (int i = 3; i <= 6; i++)
        {
            this.plates.Add(this.gameObject.transform.GetChild(i).gameObject);
            this.positionHeights.Add(this.gameObject.transform.GetChild(i).gameObject.transform.position.y);
            this.stacks[0].Add(this.plates.Last());
        }

        this.gameFinished = false;
        this.gestureEnabled = false;
        this.dimmerSprite = GameObject.Find("DimmBackground");
        this.dimmerSprite.SetActive(false);
        this.invalidMsg = GameObject.Find("Invalid");
        this.invalidMsg.SetActive(false);
        this.finishedMsg = GameObject.Find("Finished");
        this.finishedMsg.SetActive(false);
        this.resetMsg = GameObject.Find("TowerReset");
        this.resetMsg.SetActive(false);
        this.smallerMsg = GameObject.Find("InvalidSmaller");
        this.smallerMsg.SetActive(false);
        this.leftMsg = GameObject.Find("InvalidLeft");
        this.leftMsg.SetActive(false);
        this.rightMsg = GameObject.Find("InvalidRight");
        this.rightMsg.SetActive(false);
        this.platesMsg = GameObject.Find("InvalidPlates");
        this.platesMsg.SetActive(false);

        Debug.Log("Towers of Hanois has been initialized");
    }

    /// <summary>
    /// The move plate which is on the top of the selected stab to another stab according to the triggered gesture.
    /// </summary>
    /// <param name="direction">
    /// The direction the plate will moved to.
    /// </param>
    /// <param name="steps">
    /// The amount of steps the plate will moved.
    /// </param>
    public void MovePlate(int direction, int steps)
    {   
        // only move if gesture are enabled and the game is not finished yet
        if (!this.gestureEnabled || this.gameFinished)
        {
            return;
        }

        this.CancelTurn();
        int currPos = this.stabs.IndexOf(this.fixedStab);

        if (this.stacks[currPos].Count > 0)
        {
            switch (direction)
            {
                case DirectionLeft:
                    if (currPos - steps >= 0)
                    {
                        GameObject plateToMove = this.stacks[currPos].Last();
                        bool validTurn = true;

                        // Check if the plate on the top of the stab, which the plate should be move to, is bigger than the moving plate
                        if (this.stacks[currPos - steps].Count > 0)
                        {
                            GameObject plateOnTop = this.stacks[currPos - steps].Last();
                            validTurn = this.plates.IndexOf(plateOnTop) < this.plates.IndexOf(plateToMove);
                        }

                        if (validTurn)
                        {
                            this.stacks[currPos - steps].Add(plateToMove);
                            this.stacks[currPos].Remove(plateToMove);

                            float x = this.stabs[currPos - steps].transform.position.x;
                            float y = this.positionHeights[this.stacks[currPos - steps].IndexOf(plateToMove)];
                            float z = this.stabs[currPos - steps].transform.position.z;
                            plateToMove.transform.position = new Vector3(x, y, z);
                        }
                        else
                        {
                            Debug.LogWarning("[TowerOfHanoiGame] Invalid Turn: You cannot place a plate on a smaller plate!");
                            this.invalidMsg.SetActive(true);
                            this.smallerMsg.SetActive(true);
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("[TowerOfHanoiGame] Invalid Turn: You cannot move the plate that far to the left!");
                        this.invalidMsg.SetActive(true);
                        this.leftMsg.SetActive(true);
                        return;
                    }

                    break;
                case DirectionRight:
                    if (currPos + steps <= 2)
                    {
                        GameObject plateToMove = this.stacks[currPos].Last();
                        bool validTurn = true;

                        // Check if the plate on the top of the stab, which the plate should be move to, is bigger than the moving plate
                        if (this.stacks[currPos + steps].Count > 0)
                        {
                            GameObject plateOnTop = this.stacks[currPos + steps].Last();
                            validTurn = this.plates.IndexOf(plateOnTop) < this.plates.IndexOf(plateToMove);
                        }

                        if (validTurn)
                        {
                            this.stacks[currPos + steps].Add(plateToMove);
                            this.stacks[currPos].Remove(plateToMove);

                            float x = this.stabs[currPos + steps].transform.position.x;
                            float y = this.positionHeights[this.stacks[currPos + steps].IndexOf(plateToMove)];
                            float z = this.stabs[currPos + steps].transform.position.z;
                            plateToMove.transform.position = new Vector3(x, y, z);
                        }
                        else
                        {
                            Debug.LogWarning("[TowerOfHanoiGame] Invalid Turn: You cannot place a plate on a smaller plate!");
                            this.invalidMsg.SetActive(true);
                            this.smallerMsg.SetActive(true);
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("[TowerOfHanoiGame] Invalid Turn: You cannot move the plate that far to the right!");
                        this.invalidMsg.SetActive(true);
                        this.rightMsg.SetActive(true);
                        return;
                    }

                    break;
            }

            // check if tower is correctly build on stab C
            if (this.stacks[2].Count == this.plates.Count)
            {
                this.gameFinished = true;
                for (int i = 0; i < this.plates.Count; i++)
                {
                    if (!this.stacks[2][i].Equals(this.plates[i]))
                    {
                        this.gameFinished = false;
                    }
                }

                if (this.gameFinished)
                {
                    this.finishedMsg.SetActive(true);
                }
            }
        }
        else
        {
            Debug.LogWarning("[TowerOfHanoiGame] Invalid Turn: This stab has no plates on it!");
            this.invalidMsg.SetActive(true);
            this.platesMsg.SetActive(true);
        }
    }

    /// <summary>
    /// This method start the turn. Gesture insertion will be enabled and the selected stab will be highlighted.
    /// </summary>
    /// <param name="go">
    /// The selected stab.
    /// </param>
    public void StartTurn(GameObject go)
    {
        if (this.gestureEnabled)
        {
            return;
        }

        this.fixedStab = go;
        Debug.Log("Fixation received on " + go.name);
        this.gestureEnabled = true;
        GameObject.Find("StabA").GetComponent<FixationOnStab>().UnsubscribeFromService();
        GameObject.Find("StabB").GetComponent<FixationOnStab>().UnsubscribeFromService();
        GameObject.Find("StabC").GetComponent<FixationOnStab>().UnsubscribeFromService();

        // highlight the fixed object
        MeshRenderer gameObjectRenderer = this.fixedStab.GetComponent<MeshRenderer>();
        gameObjectRenderer.material = this.MaterialSelected;
        gameObjectRenderer = this.fixedStab.transform.GetChild(0).GetComponent<MeshRenderer>();
        gameObjectRenderer.material = this.MaterialSelected;

        // disable all current messages
        this.invalidMsg.SetActive(false);
        this.rightMsg.SetActive(false);
        this.smallerMsg.SetActive(false);
        this.leftMsg.SetActive(false);
        this.rightMsg.SetActive(false);
        this.platesMsg.SetActive(false);
        this.resetMsg.SetActive(false);
        this.dimmerSprite.SetActive(true);

        // enable all gestures
        gameObject.GetComponent<GestureLeft1>().SubscribeToService();
        gameObject.GetComponent<GestureLeft2>().SubscribeToService();
        gameObject.GetComponent<GestureRight1>().SubscribeToService();
        gameObject.GetComponent<GestureRight2>().SubscribeToService();
        gameObject.GetComponent<GestureCancelTurn>().SubscribeToService();
        gameObject.GetComponent<GestureResetTowers>().SubscribeToService();
    }

    /// <summary>
    /// This method cancel the current turn. The gesture insertion will be disabled and the highlighting will be removed.
    /// </summary>
    public void CancelTurn()
    {
        if (!this.gestureEnabled)
        {
            return;
        }

        // remove highlighting from the fixed object
        MeshRenderer gameObjectRenderer = this.fixedStab.GetComponent<MeshRenderer>();
        gameObjectRenderer.material = this.MaterialNormal;
        gameObjectRenderer = this.fixedStab.transform.GetChild(0).GetComponent<MeshRenderer>();
        gameObjectRenderer.material = this.MaterialNormal;

        this.dimmerSprite.SetActive(false);
        this.gestureEnabled = false;

        // disable all gestures
        gameObject.GetComponent<GestureLeft1>().UnsubscribeFromService();
        gameObject.GetComponent<GestureLeft2>().UnsubscribeFromService();
        gameObject.GetComponent<GestureRight1>().UnsubscribeFromService();
        gameObject.GetComponent<GestureRight2>().UnsubscribeFromService();
        gameObject.GetComponent<GestureCancelTurn>().UnsubscribeFromService();
        gameObject.GetComponent<GestureResetTowers>().UnsubscribeFromService();

        // enable the fixations for the stabs
        GameObject.Find("StabA").GetComponent<FixationOnStab>().Invoke("SubscribeToService", 2);
        GameObject.Find("StabB").GetComponent<FixationOnStab>().Invoke("SubscribeToService", 2);
        GameObject.Find("StabC").GetComponent<FixationOnStab>().Invoke("SubscribeToService", 2);
    }

    /// <summary>
    /// Reset the towers back to initial state.
    /// </summary>
    public void ResetTowers()
    {
        this.CancelTurn();

        this.stacks[0].Clear();
        this.stacks[1].Clear();
        this.stacks[2].Clear();

        for (int i = 0; i < this.plates.Count; i++)
        {
            this.stacks[0].Add(this.plates[i]);
            float x = this.stabs[0].transform.position.x;
            float y = this.positionHeights[i];
            float z = this.stabs[0].transform.position.z;
            this.plates[i].transform.position = new Vector3(x, y, z);
        }

        this.gameFinished = false;
        this.finishedMsg.SetActive(false);
        this.resetMsg.SetActive(true);
    }
}
