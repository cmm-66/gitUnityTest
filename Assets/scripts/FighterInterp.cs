using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterInterp : MonoBehaviour
{
    private Controls controls;
    private GameObject playerCharacter;
    private float speed;
    //records the moment a movement key is released
    private float dashShot;
    private int dashDirection;
    private ArrayList buffer;
    private enum playerStates
    {
        Standing,
        Walking,
        Dashing,
        Jumping,
        Grounded,
        Airborne,
        Attacking,
        Defending
    }
    private int currentState;

    // Start is called before the first frame update
    void Start()
    {
        dashShot = 0f;
        dashDirection = 0;
        speed = 3.0f;
        playerCharacter = GameObject.Find("Capsule");
        controls.Simple.Attack.performed += _ => Attack();
        controls.Simple.Block.performed += _ => Block();
        controls.Simple.Jump.performed += _ => Jump();
        controls.Simple.Left.performed += _ => MoveLeft();
        controls.Simple.Left.performed += _ => DashCheck();
        controls.Simple.Left.canceled += _ => BufferAdd(14);
        controls.Simple.Left.canceled += _ => StopDash();
        controls.Simple.Right.performed += _ => MoveRight();
        controls.Simple.Right.performed += _ => DashCheck();
        controls.Simple.Right.canceled += _ => BufferAdd(16);
        controls.Simple.Right.canceled += _ => StopDash();
        controls.Simple.Up.performed += _ => MoveUp();
        controls.Simple.Up.performed += _ => DashCheck();
        controls.Simple.Up.canceled += _ => BufferAdd(18);
        controls.Simple.Up.canceled += _ => StopDash();
        controls.Simple.Down.performed += _ => MoveDown();
        controls.Simple.Down.performed += _ => DashCheck();
        controls.Simple.Down.canceled += _ => BufferAdd(12);
        controls.Simple.Down.canceled += _ => StopDash();

        currentState = (int) playerStates.Standing;
        //buffer will recieve inputs and store it. Which will be used as context for when multi-button inputs are used.
        /**
         * 2 = down
         * 4 = left
         * 6 = right
         * 8 = up
         * 1 = jump
         * 3 = defend
         * 5 = attack
         * put a 1 before the actual number to notify that this is a release, not a pressdown
         * put a 2 before the actual number to notify that this is a hold down
         */
        buffer = new ArrayList(){0,0,0,0,0,0,0,0,0};
    }

    private void BufferAdd(int x)
    {
        buffer.Insert(0, x);
    }
    private void Attack()
    {
        //in the future send a check to 'current character' to do a certain animation/input, or do something completely different
        //if attack is held down, check to see if 'current character' has a certain animation/input that is performed and execute it.

        Debug.Log(playerCharacter.name + " attacked");
    }

    private void Jump()
    {
        //in the future send a check to 'current character' to do a certain animation/input, or do something completely different
        //by default jump makes you jump a certain amount in the air, if you're holding a directional input then your jump makes u go in that direction
        //if you're dashing the jump angle will be less steep
        //if you're holding down jump while Jump() is called, check if the current character has a charge jump input, if not just default to normal jump


        //add force to make them go up
        //also check in update to see if they held down the jump button for super jump
        Debug.Log(playerCharacter.name + " jumped");
    }

    private void Block()
    {
        //in the future send a check to 'current character' to do a certain animation/input, or do something completely different
        //still send block checks even if they can't block, soley to check to see if the character has a special input/animation that is done by hitting S during another animation
        
        //check if the 'current character' can air block, if not, just do a default check(by default u cannot block in the air)
        //the only ways a default character can block is from standing or dashing
        if(currentState == (int) playerStates.Dashing || currentState == (int) playerStates.Standing || currentState == (int)playerStates.Walking)
        {
            currentState = (int)playerStates.Defending;
            Debug.Log(playerCharacter.name + " blocked");
        }
    }

    private void StopDash()
    {
        if(!(controls.Simple.Left.IsPressed() ||
             controls.Simple.Right.IsPressed() ||
             controls.Simple.Down.IsPressed() ||
             controls.Simple.Up.IsPressed()))
        {
            if (currentState == (int)playerStates.Dashing) { currentState = (int)playerStates.Standing; speed = 3.0f; Debug.Log("No longer dashing"); }
        }
        
    }
    //players cannot move while jumping, blocking, attacking, being on the ground or airborne
    //if we are dashing, maintain the dashing
    private void MoveLeft()
    {
        if(currentState == (int)playerStates.Walking ||
           currentState == (int)playerStates.Standing)
        {
            currentState = (int)playerStates.Walking;
            playerCharacter.transform.Translate(Vector3.left * speed * Time.deltaTime);
            if ((int)(buffer[0]) != 24 && controls.Simple.Left.IsPressed()) buffer.Insert(0, 24);
            dashDirection = 14;
        }
        if(currentState == (int)playerStates.Dashing)
        {
            playerCharacter.transform.Translate(Vector3.left * speed * Time.deltaTime);
            if ((int)(buffer[0]) != 24 && controls.Simple.Left.IsPressed()) buffer.Insert(0, 24);
            dashDirection = 14;
        }
        
        
    }
    private void MoveRight()
    {
        if (currentState == (int)playerStates.Walking ||
           currentState == (int)playerStates.Dashing ||
           currentState == (int)playerStates.Standing)
        {
            currentState = (int)playerStates.Walking;
            playerCharacter.transform.Translate(Vector3.right * speed * Time.deltaTime);
            if ((int)(buffer[0]) != 26 && controls.Simple.Right.IsPressed()) buffer.Insert(0, 26);
            dashDirection = 16;
        }

        if (currentState == (int)playerStates.Dashing)
        {
            playerCharacter.transform.Translate(Vector3.right * speed * Time.deltaTime);
            if ((int)(buffer[0]) != 26 && controls.Simple.Right.IsPressed()) buffer.Insert(0, 26);
            dashDirection = 16;
        }
    }

    private void MoveDown()
    {
        if (currentState == (int)playerStates.Walking ||
           currentState == (int)playerStates.Dashing ||
           currentState == (int)playerStates.Standing)
        {
            currentState = (int)playerStates.Walking;
            playerCharacter.transform.Translate(Vector3.back * speed * Time.deltaTime);
            if ((int)(buffer[0]) != 22 && controls.Simple.Down.IsPressed()) buffer.Insert(0, 22);
            dashDirection = 12;
        }
        if(currentState == (int)playerStates.Dashing)
        {
            playerCharacter.transform.Translate(Vector3.back * speed * Time.deltaTime);
            if ((int)(buffer[0]) != 22 && controls.Simple.Down.IsPressed()) buffer.Insert(0, 22);
            dashDirection = 12;
        }
    }

    private void MoveUp()
    {
        if (currentState == (int)playerStates.Walking ||
           currentState == (int)playerStates.Dashing ||
           currentState == (int)playerStates.Standing)
        {
            currentState = (int)playerStates.Walking;
            playerCharacter.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if ((int)(buffer[0]) != 28 && controls.Simple.Up.IsPressed()) buffer.Insert(0, 28);
            dashDirection = 18;
        }
        if (currentState == (int)playerStates.Dashing)
        {
            playerCharacter.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if ((int)(buffer[0]) != 28 && controls.Simple.Up.IsPressed()) buffer.Insert(0, 28);
            dashDirection = 18;
        }
    }
    //by default, players may dash if they are standing or walking
    //airdashes and mid-attack dashes will be added later, as shown by the commented code within the if (attacking/airborne) states check
    private void DashCheck()
    {
        if(currentState == (int)playerStates.Walking ||
            currentState == (int)playerStates.Standing)
        {
            if (currentState == (int)playerStates.Dashing)
            {

            }
            else
            {
                if (dashDirection == (int)buffer[1] && (Time.time - dashShot) < 0.5)
                {
                    Debug.Log("Dashed!");
                    currentState = (int)playerStates.Dashing;
                    speed = 4.0f;
                }
                else
                {
                    dashShot = Time.time;
                    Debug.Log("dash failed time recorded");

                }

                string wholset = "";
                foreach (int item in buffer)
                {
                    wholset += item.ToString() + ", ";

                }
                //Debug.Log(wholset);
            }
        }
       if(currentState == (int)playerStates.Jumping)
        {
            /**airdash check
             * if (dashDirection == (int)buffer[1] && (Time.time - dashShot) < 0.5)
                {
                    Debug.Log("Dashed!");
                    currentState = (int)playerStates.Dashing;
                    speed = 4.0f;
                }
                else
                {
                    dashShot = Time.time;
                    Debug.Log("dash failed time recorded");

                }
             */
        }
        if (currentState == (int)playerStates.Attacking)
        {
            /**attack check
             * if (dashDirection == (int)buffer[1] && (Time.time - dashShot) < 0.5)
                {
                    Debug.Log("Dashed!");
                    currentState = (int)playerStates.Dashing;
                    speed = 4.0f;
                }
                else
                {
                    dashShot = Time.time;
                    Debug.Log("dash failed time recorded");

                }
             */
        }
        if (currentState == (int)playerStates.Defending)
        {
            /**this is for rolling out of block or something
             * if (dashDirection == (int)buffer[1] && (Time.time - dashShot) < 0.5)
                {
                    Debug.Log("Dashed!");
                    currentState = (int)playerStates.Dashing;
                    speed = 4.0f;
                }
                else
                {
                    dashShot = Time.time;
                    Debug.Log("dash failed time recorded");

                }
             */
        }




    }
    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        if (controls.Simple.Left.IsPressed()) MoveLeft();
        if (controls.Simple.Right.IsPressed()) MoveRight();
        if (controls.Simple.Up.IsPressed()) MoveUp();
        if (controls.Simple.Down.IsPressed()) MoveDown();
        if(currentState == (int)playerStates.Defending && controls.Simple.Block.IsPressed()) Debug.Log(playerCharacter.name + " is still blocking.");
        if (currentState == (int)playerStates.Jumping && controls.Simple.Jump.IsPressed()) Debug.Log(playerCharacter.name + " super jumps");
        /**foreach(int item in buffer)
        {
            Debug.Log(item.ToString());
        }**/
    }
}
