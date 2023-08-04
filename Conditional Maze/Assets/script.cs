using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class script : MonoBehaviour {
    public KMBombInfo bomb;
    public KMAudio bombAudio;
    public KMBombModule module;
    public KMSelectable moduleSelectable;

    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool moduleSolved = false;

    public KMSelectable[] buttons;
    string[] directionNames = new string[4] { "Up", "Right", "Down", "Left" };
    int[] modifiedDirections = new int[4] { 0, 1, 2 ,3 };

    int timePressed;
    int durationHeld;
    int timeReleased;
    int timeBetween = -1;
    bool buttonWasPressed;
    int lastButtonPressed = -1;
    bool timeLimitForMoving; //between waiting
    int timeLimit;
    bool didDefocus; //defocus
    bool afterFirstPress; //this makes the defocus work how i want it to
    bool trackingMultiplePresses; //multiple presses
    bool differentButton;
    int remainingRepeatPresses; 
    int repeatPressesQuota = 0;

    bool[,] allConditions;
    bool[,] conditionLights;
    string[,] maze = new string[4, 4];
    bool foundUniqueMaze;
    int playerPositionX;
    int playerPositionY;
    int initialPlayerPositionX;
    int initialPlayerPositionY;
    int goalPositionX;
    int goalPositionY;

    /*Condition[,] fullConditionsTable = new Condition[10, 10] {
        { new Condition(0, 0), new Condition(0, 0), new Condition(Condition.Types.TimerOnes, 2), new Condition(Condition.Types.MultiplePresses, 2), new Condition(Condition.Types.BetweenMinimum, 1), new Condition(0, 0), new Condition(Condition.Types.HoldMaximum, 7), new Condition(Condition.Types.HoldMinimum, 1), new Condition(Condition.Types.MultiplePresses, 3), new Condition(0, 0) },
        { new Condition(Condition.Types.TimerTens, 3), new Condition(Condition.Types.HoldMaximum, 9), new Condition(0, 0), new Condition(0, 0), new Condition(Condition.Types.ControlsFlipHorizontal, 0), new Condition(Condition.Types.TimerTens, 5), new Condition(Condition.Types.BetweenMaximum, 10), new Condition(0, 0), new Condition(0, 0), new Condition(Condition.Types.BetweenMinimum, 3) },
        { new Condition(0, 0), new Condition(Condition.Types.BetweenMinimum, 3), new Condition(Condition.Types.HoldMinimum, 3), new Condition(Condition.Types.HoldMaximum, 5), new Condition(Condition.Types.HoldMinimum, 1), new Condition(0, 0), new Condition(Condition.Types.MultiplePresses, 3), new Condition(Condition.Types.HoldMaximum, 5), new Condition(Condition.Types.HoldMinimum, 3), new Condition(Condition.Types.ControlsRotate, 2) },
        { new Condition(Condition.Types.MultiplePresses, 2), new Condition(Condition.Types.ControlsFlipVertical, 0), new Condition(0, 0), new Condition(Condition.Types.BetweenMaximum, 7), new Condition(0, 0), new Condition(Condition.Types.ControlsRotate, 3), new Condition(Condition.Types.TimerOnes, 1), new Condition(0, 0), new Condition(Condition.Types.ControlsFlipVertical, 0), new Condition(Condition.Types.BetweenMaximum, 7) },
        { new Condition(Condition.Types.TimerOnes, 4), new Condition(0, 0), new Condition(Condition.Types.HoldMaximum, 7), new Condition(Condition.Types.HoldMinimum, 1), new Condition(Condition.Types.BetweenMinimum, 2), new Condition(0, 0), new Condition(Condition.Types.ControlsFlipHorizontal, 0), new Condition(0, 0), new Condition(Condition.Types.HoldMinimum, 2), new Condition(0, 0) },
        { new Condition(Condition.Types.TimerTens, 1), new Condition(Condition.Types.ControlsRotate, 1), new Condition(Condition.Types.MultiplePresses, 2), new Condition(0, 0), new Condition(Condition.Types.HoldMinimum, 2), new Condition(Condition.Types.TimerTens, 2), new Condition(0, 0), new Condition(Condition.Types.ControlsRotate, 1), new Condition(Condition.Types.MultiplePresses, 3), new Condition(Condition.Types.TimerTens, 4) },
        { new Condition(Condition.Types.HoldMinimum, 1), new Condition(0, 0), new Condition(Condition.Types.BetweenMaximum, 7), new Condition(0, 0), new Condition(Condition.Types.ControlsFlipHorizontal, 0), new Condition(Condition.Types.BetweenMaximum, 10), new Condition(0, 0), new Condition(Condition.Types.HoldMaximum, 9), new Condition(0, 0), new Condition(Condition.Types.BetweenMaximum, 10) },
        { new Condition(0, 0), new Condition(Condition.Types.BetweenMinimum, 2), new Condition(Condition.Types.HoldMinimum, 3), new Condition(Condition.Types.HoldMaximum, 7), new Condition(Condition.Types.MultiplePresses, 4), new Condition(0, 0), new Condition(Condition.Types.ControlsFlipVertical, 0), new Condition(Condition.Types.HoldMinimum, 2), new Condition(0, 0), new Condition(Condition.Types.TimerOnes, 5) },
        { new Condition(Condition.Types.MultiplePresses, 4), new Condition(0, 0), new Condition(Condition.Types.ControlsRotate, 3), new Condition(Condition.Types.BetweenMaximum, 7), new Condition(0, 0), new Condition(Condition.Types.TimerOnes, 0), new Condition(Condition.Types.BetweenMinimum, 1), new Condition(0, 0), new Condition(Condition.Types.HoldMaximum, 5), new Condition(0, 0) },
        { new Condition(0, 0), new Condition(Condition.Types.TimerOnes, 7), new Condition(Condition.Types.HoldMaximum, 9), new Condition(Condition.Types.HoldMinimum, 1), new Condition(0, 0), new Condition(Condition.Types.TimerTens, 0), new Condition(Condition.Types.MultiplePresses, 3), new Condition(Condition.Types.HoldMinimum, 3), new Condition(Condition.Types.ControlsRotate, 2), new Condition(0, 0) } };*/
    Condition[,] fullConditionsTable = new Condition[10, 10] {
        { new Condition(0, 0), new Condition(0, 0), new Condition(Condition.Types.TimerOnes, 2), new Condition(Condition.Types.MultiplePresses, 2), new Condition(Condition.Types.BetweenMinimum, 1), new Condition(0, 0), new Condition(Condition.Types.Defocus, 0), new Condition(Condition.Types.HoldMinimum, 1), new Condition(Condition.Types.ControlsFlipLeftDiagonal, 0), new Condition(0, 0) },
        { new Condition(Condition.Types.TimerTens, 3), new Condition(Condition.Types.ControlsFlipLeftDiagonal, 0), new Condition(Condition.Types.Defocus, 0), new Condition(0, 0), new Condition(Condition.Types.HoldMaximum, 7), new Condition(Condition.Types.TimerTens, 5), new Condition(Condition.Types.HoldMaximum, 10), new Condition(Condition.Types.BetweenMinimum, 3), new Condition(0, 0), new Condition(Condition.Types.TimerOnes, 6) },
        { new Condition(0, 0), new Condition(Condition.Types.BetweenMinimum, 3), new Condition(Condition.Types.HoldMinimum, 3), new Condition(Condition.Types.ControlsFlipRightDiagonal, 0), new Condition(Condition.Types.HoldMinimum, 1), new Condition(0, 0), new Condition(Condition.Types.MultiplePresses, 4), new Condition(Condition.Types.HoldMaximum, 5), new Condition(Condition.Types.HoldMinimum, 3), new Condition(Condition.Types.ControlsRotate, 2) },
        { new Condition(Condition.Types.HoldMinimum, 2), new Condition(Condition.Types.ControlsFlipVertical, 0), new Condition(0, 0), new Condition(Condition.Types.BetweenMaximum, 7), new Condition(Condition.Types.HoldMaximum, 9), new Condition(Condition.Types.ControlsRotate, 3), new Condition(Condition.Types.TimerOnes, 1), new Condition(0, 0), new Condition(Condition.Types.ControlsFlipVertical, 0), new Condition(Condition.Types.BetweenMaximum, 7) },
        { new Condition(Condition.Types.TimerOnes, 4), new Condition(0, 0), new Condition(Condition.Types.HoldMaximum, 7), new Condition(Condition.Types.Defocus, 0), new Condition(Condition.Types.BetweenMinimum, 2), new Condition(0, 0), new Condition(Condition.Types.ControlsFlipHorizontal, 0), new Condition(Condition.Types.Defocus, 0), new Condition(Condition.Types.HoldMinimum, 2), new Condition(0, 0) },
        { new Condition(Condition.Types.TimerTens, 1), new Condition(Condition.Types.Defocus, 0), new Condition(Condition.Types.MultiplePresses, 2), new Condition(Condition.Types.ControlsFlipRightDiagonal, 0), new Condition(Condition.Types.HoldMinimum, 2), new Condition(Condition.Types.TimerTens, 2), new Condition(0, 0), new Condition(Condition.Types.ControlsRotate, 1), new Condition(Condition.Types.MultiplePresses, 3), new Condition(Condition.Types.TimerTens, 4) },
        { new Condition(Condition.Types.TimerOnes, 3), new Condition(Condition.Types.ControlsRotate, 1), new Condition(Condition.Types.HoldMinimum, 1), new Condition(0, 0), new Condition(Condition.Types.ControlsFlipHorizontal, 0), new Condition(Condition.Types.BetweenMaximum, 10), new Condition(Condition.Types.Defocus, 0), new Condition(Condition.Types.HoldMaximum, 9), new Condition(0, 0), new Condition(Condition.Types.BetweenMaximum, 10) },
        { new Condition(0, 0), new Condition(Condition.Types.BetweenMinimum, 2), new Condition(Condition.Types.HoldMinimum, 2), new Condition(Condition.Types.HoldMaximum, 7), new Condition(Condition.Types.TimerOnes, 0), new Condition(0, 0), new Condition(Condition.Types.ControlsFlipVertical, 0), new Condition(Condition.Types.HoldMinimum, 2), new Condition(Condition.Types.Defocus, 0), new Condition(Condition.Types.TimerOnes, 5) },
        { new Condition(Condition.Types.MultiplePresses, 4), new Condition(0, 0), new Condition(Condition.Types.ControlsRotate, 3), new Condition(Condition.Types.BetweenMaximum, 7), new Condition(Condition.Types.Defocus, 0), new Condition(Condition.Types.MultiplePresses, 3), new Condition(Condition.Types.BetweenMinimum, 1), new Condition(0, 0), new Condition(Condition.Types.HoldMaximum, 5), new Condition(0, 0) },
        { new Condition(0, 0), new Condition(Condition.Types.TimerOnes, 7), new Condition(Condition.Types.HoldMaximum, 9), new Condition(Condition.Types.HoldMinimum, 1), new Condition(0, 0), new Condition(Condition.Types.TimerTens, 0), new Condition(Condition.Types.HoldMaximum, 5), new Condition(Condition.Types.HoldMaximum, 3), new Condition(Condition.Types.ControlsRotate, 2), new Condition(0, 0) } };
    Condition[,] mazeConditionsTable;
    List<Condition> currentConditions = new List<Condition>();
    bool[,] visitedSquares = new bool[4, 4];

    public GameObject[] mazeObjects;
    public SpriteRenderer[] mazeSprites;
    float playerRotation;

    public SpriteRenderer[] horizontalWalls;
    public SpriteRenderer[] verticalWalls;

    public GameObject goalObject;

    private double Voltage() // shamelessly stolen from Access Codes, thanks GhostSalt
    {
        if (bomb.QueryWidgets("volt", "").Count() != 0)
        {
            double TempVoltage = double.Parse(bomb.QueryWidgets("volt", "")[0].Substring(12).Replace("\"}", ""));
            return TempVoltage;
        }
        return -1d;
    }

    void Start () {
        int batteries = bomb.GetBatteryCount();
        int batteryHolders = bomb.GetBatteryHolderCount();
        int aaBatteries = 2 * (batteries - batteryHolders);
        int dBatteres = batteries - aaBatteries;

        List<string> indicators = bomb.GetIndicators().ToList();
        List<string> litIndicators = bomb.GetOnIndicators().ToList();
        List<string> unlitIndicators = bomb.GetOffIndicators().ToList();

        List<string> ports = bomb.GetPorts().ToList();

        string serialNumber = bomb.GetSerialNumber();

        double voltage = Voltage();

        allConditions = new bool[10, 10] {
            { false, aaBatteries < 4, false, false, false, ports.Contains("RJ45"), false, false, false, voltage >= 0d && voltage <= 2d },
            { false, false, false, litIndicators.Contains("BOB"), false, false, false, false, unlitIndicators.Contains("IND"), false },
            { ports.Contains("StereoRCA"), false, false, false, false, indicators.Contains("CLR"), false, false, false, false },
            { false, false, voltage >= 2d && voltage <= 5d, false, false, false, false, dBatteres > 2, false, false },
            { false, litIndicators.Count < unlitIndicators.Count, false, false, false, ports.Contains("Serial"), false, false, false, ports.Contains("DVI") },
            { false, false, false, false, false, false, char.IsLetter(serialNumber[0]), false, false, false },
            { false, false, false, aaBatteries > dBatteres, false, false, false, false, litIndicators.Count > unlitIndicators.Count, false },
            { indicators.Contains("SIG"), false, false, false, false, char.IsNumber(serialNumber[1]), false, false, false, false },
            { false, voltage >= 8d && voltage <= 10d, false, false, false, false, false, ports.Contains("Parallel"), false, voltage >= 5d && voltage <= 8d },
            { ports.Contains("PS2"), false, false, false, batteryHolders < 3, false, false, false, false, false }
        };

        string lightsLogging = "\n";
        conditionLights = findUniqueLightsPosition(); //this also grabs the conditions to put in the 4x4 condition table because it's way easier to do this way
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                if(conditionLights[j,i])
                {
                    mazeSprites[j + (i * 4)].color = Color.white;
                    lightsLogging += '#';
                }
                else
                    lightsLogging += '-';
            }
            lightsLogging += '\n';
        }
        Log("Shown lights:" + lightsLogging);

        generateMaze(); //the goal position and player position deciding is also in this function just an fyi
        renderMaze();

        playerPositionX = Random.Range(1,3);
        playerPositionY = Random.Range(1,3);
        initialPlayerPositionX = playerPositionX;
        initialPlayerPositionY = playerPositionY;
        visitedSquares[playerPositionX, playerPositionY] = true;
        goalPositionX = (playerPositionX + 2) % 4;
        goalPositionY = (playerPositionY + 2) % 4;
        goalObject.transform.position = mazeObjects[goalPositionX + goalPositionY * 4].transform.position;
        Log("Player's coordinate in the maze: " + (playerPositionX + 1) + ", " + (playerPositionY + 1) + ".");
        Log("Goal coordinate in the maze: " + (goalPositionX + 1) + ", " + (goalPositionY + 1) + ".");

        currentConditions.Add(mazeConditionsTable[playerPositionX, playerPositionY]);
        Log("Starting condition: " + LogCondition(currentConditions[0]));
        applyControlModifications(currentConditions[0]);

        StartCoroutine(AnimatePlayer());
        StartCoroutine(AnimateGoal());
    }

    void Awake()
    {
        ModuleId = ModuleIdCounter++;

        for (int i = 0; i < 4; i++)
        {
            int dummy = i;
            buttons[i].OnInteract += delegate () { buttonPressed(dummy); return false; };
            buttons[i].OnInteractEnded += delegate () { buttonReleased(dummy);};
        }
        moduleSelectable.OnDefocus += delegate () { didDefocus = true; };
    }

    void buttonPressed(int pressedButton)
    {
        if (moduleSolved)
        {
            return;
        }

        GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
        buttons[pressedButton].AddInteractionPunch(.4f);

        if (timeLimitForMoving)
        {
            StopAllCoroutines();
            StartCoroutine(AnimatePlayer());
            StartCoroutine(AnimateGoal());
        }

        timePressed = (int)bomb.GetTime();
        if (buttonWasPressed)
        {
            timeBetween = Mathf.Abs(timeReleased - timePressed);
        }
        else
        {
            buttonWasPressed = true;
        }

        string pressedMessage = " ";
        if (lastButtonPressed == pressedButton)
        {
            differentButton = false;
        }
        else
        {
            differentButton = true;
            lastButtonPressed = pressedButton;
        }

        int seconds = timePressed % 60;
        int minutes = (timePressed - seconds) / 60;
        Log(directionNames[pressedButton] +  " button pressed at " + minutes + ":" + seconds.ToString("00") + "." + pressedMessage);
    }

    void buttonReleased(int pressedButton)
    {
        timeReleased = (int)bomb.GetTime();
        int seconds = timeReleased % 60;
        int minutes = (timeReleased - seconds) / 60;
        durationHeld = Mathf.Abs(timePressed - timeReleased);
        Log("Button released at " + minutes + ":" + seconds.ToString("00") + ". It was held for " + durationHeld + " timer ticks.");

        if (timeLimitForMoving)
        {
            StartCoroutine(WaitForTimeLimit(timeLimit));
        }

        bool repeatingPresses = false;
        bool quotaMet = false;
        if (trackingMultiplePresses)
        {
            if ((differentButton || remainingRepeatPresses <= 0))
            {
                if (remainingRepeatPresses > 0)
                {
                    Log("Defuser didn't press the previous button " + repeatPressesQuota + " times.");
                    module.HandleStrike();
                    moduleReset();
                    return;
                }
                else
                {
                    remainingRepeatPresses = repeatPressesQuota - 1;
                }
            }
            else
            {
                remainingRepeatPresses--;
                if(remainingRepeatPresses <= 0)
                {
                    quotaMet = true;
                }
                repeatingPresses = true;
            }
        }

        if (!repeatingPresses)
        {
            int oldPlayerPositionX = playerPositionX;
            int oldPlayerPositionY = playerPositionY;
            switch (modifiedDirections[pressedButton])
            {
                case 0:
                    if (playerPositionY > 0)
                    {
                        if (maze[playerPositionX, playerPositionY].Contains('U'))
                        {
                            playerPositionY--;
                        }
                        else
                        {
                            Log("Defuser hit a wall while going up.");
                            module.HandleStrike();
                            moduleReset();
                            return;
                        }
                    }
                    break;
                case 1:
                    if (playerPositionX < 3)
                    {
                        if (maze[playerPositionX, playerPositionY].Contains('R'))
                        {
                            playerPositionX++;
                        }
                        else
                        {
                            Log("Defuser hit a wall while going right.");
                            module.HandleStrike();
                            moduleReset();
                            return;
                        }
                    }
                    break;
                case 2:
                    if (playerPositionY < 3)
                    {
                        if (maze[playerPositionX, playerPositionY].Contains('D'))
                        {
                            playerPositionY++;
                        }
                        else
                        {
                            Log("Defuser hit a wall while going down.");
                            module.HandleStrike();
                            moduleReset();
                            return;
                        }
                    }
                    break;
                case 3:
                    if (playerPositionX > 0)
                    {
                        if (maze[playerPositionX, playerPositionY].Contains('L'))
                        {
                            playerPositionX--;
                        }
                        else
                        {
                            Log("Defuser hit a wall while going left.");
                            module.HandleStrike();
                            moduleReset();
                            return;
                        }
                    }
                    break;
            }

            if (oldPlayerPositionX != playerPositionX || oldPlayerPositionY != playerPositionY)
            {
                mazeObjects[oldPlayerPositionX + oldPlayerPositionY * 4].transform.localScale = new Vector3(.3f, .3f, 1f);
                mazeObjects[oldPlayerPositionX + oldPlayerPositionY * 4].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }
        }

        if (followedAllConditions())
        {
            if (!visitedSquares[playerPositionX, playerPositionY] && (!repeatingPresses || quotaMet))
            {
                currentConditions.Add(mazeConditionsTable[playerPositionX, playerPositionY]);
                Log("Added condition: " + LogCondition(currentConditions[currentConditions.Count - 1]));

                applyControlModifications(currentConditions[currentConditions.Count - 1]);
            }
        }
        else
        {
            module.HandleStrike();
            moduleReset();
            return;
        }

        didDefocus = false;
        afterFirstPress = true;
        visitedSquares[playerPositionX, playerPositionY] = true;

        if (playerPositionX == goalPositionX && playerPositionY == goalPositionY)
        {
            module.HandlePass();
            StopAllCoroutines();

            foreach(SpriteRenderer sprite in mazeSprites)
            {
                StartCoroutine(TurnGreen(sprite));
            }
            foreach (SpriteRenderer sprite in horizontalWalls)
            {
                StartCoroutine(TurnGreen(sprite));
            }
            foreach (SpriteRenderer sprite in verticalWalls)
            {
                StartCoroutine(TurnGreen(sprite));
            }

            bombAudio.PlaySoundAtTransform("congratulation", transform);
            StartCoroutine(AnimateGoal());
            StartCoroutine(SolveAnimation());
        }
    }

    IEnumerator WaitForTimeLimit(int secs)
    {
        yield return new WaitForSeconds(secs);
        Log("Didn't press a button under " + secs + " seconds.");
        module.HandleStrike();
        moduleReset();
    }

    void moduleReset()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateGoal());
        StartCoroutine(AnimatePlayer());

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int dummyi = i;
                int dummyj = j;
                if (conditionLights[j, i])
                {
                    mazeSprites[dummyj + (dummyi * 4)].color = Color.white;
                }
                else
                {
                    mazeSprites[dummyj + (dummyi * 4)].color = new Color(94/255f, 94/255f, 94/255f);
                }
            }
        }

        foreach (SpriteRenderer sprite in mazeSprites)
        {
            StartCoroutine(StrikeAnimation(sprite));
        }
        foreach (SpriteRenderer sprite in horizontalWalls)
        {
            sprite.color = Color.white;
            StartCoroutine(StrikeAnimation(sprite));
        }
        foreach (SpriteRenderer sprite in verticalWalls)
        {
            sprite.color = Color.white;
            StartCoroutine(StrikeAnimation(sprite));
        }

        Log("Module reset.");

        mazeObjects[playerPositionX + playerPositionY * 4].transform.localScale = new Vector3(.3f, .3f, 1f);
        mazeObjects[playerPositionX + playerPositionY * 4].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        playerPositionX = initialPlayerPositionX;
        playerPositionY = initialPlayerPositionY;
        currentConditions.Clear();

        timeBetween = -1;
        buttonWasPressed = false;
        lastButtonPressed = -1;
        timeLimitForMoving = false;
        didDefocus = false;
        trackingMultiplePresses = false;
        remainingRepeatPresses = 0;

        modifiedDirections = new int[4] { 0, 1, 2, 3 };

        currentConditions.Add(mazeConditionsTable[playerPositionX, playerPositionY]);
        applyControlModifications(currentConditions[0]);

        visitedSquares = new bool[4, 4];
        visitedSquares[playerPositionX, playerPositionY] = true;
    }

    bool[,] findUniqueLightsPosition() //returns the top left coordinate of the maze if it is able to
    {
        List<int> uniqueLights = new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48 };

        while(uniqueLights.Count > 0 && !foundUniqueMaze)
        {
            int chosenPosition = uniqueLights[Random.Range(0,uniqueLights.Count)];
            int chosenPositionX = Mathf.FloorToInt(chosenPosition / 7);
            int chosenPositionY = chosenPosition % 7;
            bool[,] chosenPositionLights = new bool[4, 4] { 
                { allConditions[chosenPositionX, chosenPositionY], allConditions[chosenPositionX + 1, chosenPositionY], allConditions[chosenPositionX + 2, chosenPositionY], allConditions[chosenPositionX + 3, chosenPositionY]},
                { allConditions[chosenPositionX, chosenPositionY + 1], allConditions[chosenPositionX + 1, chosenPositionY + 1], allConditions[chosenPositionX + 2, chosenPositionY + 1], allConditions[chosenPositionX + 3, chosenPositionY + 1]},
                { allConditions[chosenPositionX, chosenPositionY + 2], allConditions[chosenPositionX + 1, chosenPositionY + 2], allConditions[chosenPositionX + 2, chosenPositionY + 2], allConditions[chosenPositionX + 3, chosenPositionY + 2]},
                { allConditions[chosenPositionX, chosenPositionY + 3], allConditions[chosenPositionX + 1, chosenPositionY + 3], allConditions[chosenPositionX + 2, chosenPositionY + 3], allConditions[chosenPositionX + 3, chosenPositionY + 3]} };

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if(i != chosenPositionX || j != chosenPositionY)
                    {
                        bool[,] testPosition = new bool[4, 4] {
                        { allConditions[i, j], allConditions[i + 1, j], allConditions[i + 2, j], allConditions[i + 3, j]},
                        { allConditions[i, j + 1], allConditions[i + 1, j + 1], allConditions[i + 2, j + 1], allConditions[i + 3, j + 1]},
                        { allConditions[i, j + 2], allConditions[i + 1, j + 2], allConditions[i + 2, j + 2], allConditions[i + 3, j + 2]},
                        { allConditions[i, j + 3], allConditions[i + 1, j + 3], allConditions[i + 2, j + 3], allConditions[i + 3, j + 3]} };

                        if(sameBoolThings(chosenPositionLights, testPosition))
                        {
                            uniqueLights.Remove(chosenPosition);
                            i = 7;
                            j = 7;
                        }
                    }
                    if (i == 6 && j == 6)
                    {
                        foundUniqueMaze = true;
                        Log("Top left coordinate of maze: " + (chosenPositionY + 1) + ", " + (chosenPositionX + 1) + ".");
                        mazeConditionsTable = new Condition[4, 4] {
                        { fullConditionsTable[chosenPositionX, chosenPositionY], fullConditionsTable[chosenPositionX + 1, chosenPositionY], fullConditionsTable[chosenPositionX + 2, chosenPositionY], fullConditionsTable[chosenPositionX + 3, chosenPositionY]},
                        { fullConditionsTable[chosenPositionX, chosenPositionY + 1], fullConditionsTable[chosenPositionX + 1, chosenPositionY + 1], fullConditionsTable[chosenPositionX + 2, chosenPositionY + 1], fullConditionsTable[chosenPositionX + 3, chosenPositionY + 1]},
                        { fullConditionsTable[chosenPositionX, chosenPositionY + 2], fullConditionsTable[chosenPositionX + 1, chosenPositionY + 2], fullConditionsTable[chosenPositionX + 2, chosenPositionY + 2], fullConditionsTable[chosenPositionX + 3, chosenPositionY + 2]},
                        { fullConditionsTable[chosenPositionX, chosenPositionY + 3], fullConditionsTable[chosenPositionX + 1, chosenPositionY + 3], fullConditionsTable[chosenPositionX + 2, chosenPositionY + 3], fullConditionsTable[chosenPositionX + 3, chosenPositionY + 3]}
                        };
                        return chosenPositionLights;
                    }
                }
            }
        }

        return null;
    }

    void generateMaze()
    {
        int generatorPositionX = Random.Range(0, 3);
        int generatorPositionY = Random.Range(0, 3);

        bool[,] visitedSquares = new bool[4, 4];
        visitedSquares[generatorPositionX, generatorPositionY] = true;
        int visitedSquaresCount = 1;

        List<int> squareQueueX = new List<int>() { generatorPositionX };
        List<int> squareQueueY = new List<int>() { generatorPositionY };

        List<int> deadEndSquaresX = new List<int>() { generatorPositionX };
        List<int> deadEndSquaresY = new List<int>() { generatorPositionY };

        while (visitedSquaresCount < 16)
        {
            int direction = Random.Range(0, 4);
            int attempts = 0;
            bool succeeded = false;

            GenRetry:
            switch (direction)
            {
                case 0: //up
                    SilentLog("Attempting to go up.");
                    if (generatorPositionY > 0)
                    {
                        if (!visitedSquares[generatorPositionX, generatorPositionY - 1])
                        {
                            SilentLog("Went up.");
                            succeeded = true;
                            maze[generatorPositionX, generatorPositionY] += 'U';
                            generatorPositionY--;
                            maze[generatorPositionX, generatorPositionY] += 'D';
                            visitedSquaresCount++;
                            squareQueueX.Add(generatorPositionX);
                            squareQueueY.Add(generatorPositionY);
                            visitedSquares[generatorPositionX, generatorPositionY] = true;
                        }
                    }

                    if (!succeeded)
                    {
                        if (attempts == 4)
                        {
                            break;
                        }
                        direction++;
                        attempts++;
                        goto GenRetry;
                    }
                    break;
                case 1: //right
                    SilentLog("Attempting to go right.");
                    if (generatorPositionX < 3)
                    {
                        if (!visitedSquares[generatorPositionX + 1, generatorPositionY])
                        {
                            SilentLog("Went right.");
                            succeeded = true;
                            maze[generatorPositionX, generatorPositionY] += 'R';
                            generatorPositionX++;
                            maze[generatorPositionX, generatorPositionY] += 'L';
                            visitedSquaresCount++;
                            squareQueueX.Add(generatorPositionX);
                            squareQueueY.Add(generatorPositionY);
                            visitedSquares[generatorPositionX, generatorPositionY] = true;
                        }
                    }

                    if (!succeeded)
                    {
                        if (attempts == 4)
                        {
                            break;
                        }
                        direction++;
                        attempts++;
                        goto GenRetry;
                    }
                    break;
                case 2: //down
                    SilentLog("Attempting to go down.");
                    if (generatorPositionY < 3)
                    {
                        if (!visitedSquares[generatorPositionX, generatorPositionY + 1])
                        {
                            SilentLog("Went down.");
                            succeeded = true;
                            maze[generatorPositionX, generatorPositionY] += 'D';
                            generatorPositionY++;
                            maze[generatorPositionX, generatorPositionY] += 'U';
                            visitedSquaresCount++;
                            squareQueueX.Add(generatorPositionX);
                            squareQueueY.Add(generatorPositionY);
                            visitedSquares[generatorPositionX, generatorPositionY] = true;
                        }
                    }

                    if (!succeeded)
                    {
                        if (attempts == 4)
                        {
                            break;
                        }
                        direction++;
                        attempts++;
                        goto GenRetry;
                    }
                    break;
                case 3: //left
                    SilentLog("Attempting to go left.");
                    if (generatorPositionX > 0)
                    {
                        if (!visitedSquares[generatorPositionX - 1, generatorPositionY])
                        {
                            SilentLog("Went left.");
                            succeeded = true;
                            maze[generatorPositionX, generatorPositionY] += 'L';
                            generatorPositionX--;
                            maze[generatorPositionX, generatorPositionY] += 'R';
                            visitedSquaresCount++;
                            squareQueueX.Add(generatorPositionX);
                            squareQueueY.Add(generatorPositionY);
                            visitedSquares[generatorPositionX, generatorPositionY] = true;
                        }
                    }

                    if (!succeeded)
                    {
                        if (attempts == 4)
                        {
                            break;
                        }
                        direction = 0;
                        attempts++;
                        goto GenRetry;
                    }
                    break;
            }

            if(!succeeded)
            {
                deadEndSquaresX.Add(generatorPositionX);
                deadEndSquaresY.Add(generatorPositionY);

                SilentLog("Going backwards. Stack count: " + squareQueueX.Count);
                generatorPositionX = squareQueueX[squareQueueX.Count - 2];
                generatorPositionY = squareQueueY[squareQueueY.Count - 2];
                squareQueueX.RemoveAt(squareQueueX.Count - 1);
                squareQueueY.RemoveAt(squareQueueY.Count - 1);
            }
        }

        int extraPaths = 1;
        for(int i = 0; i < extraPaths; i++)
        {
            Reset:
            int randomSquareX = Random.Range(0, 4);
            int randomSquareY = Random.Range(0, 4);
            int direction = Random.Range(0,4);
            char[] directionNames = new char[] { 'U', 'R', 'D', 'L' };
            int attempts = 0;

            Retry:
            if(maze[randomSquareX, randomSquareY].Contains(directionNames[direction]) || (direction == 0 && randomSquareY == 0) || (direction == 1 && randomSquareX == 3) || (direction == 2 && randomSquareY == 3) || (direction == 3 && randomSquareX == 0))
            {
                if(attempts == 4)
                {
                    goto Reset;
                }

                attempts++;
                direction = (direction + 1) % 4;
                goto Retry;
            }
            else
            {
                switch (direction)
                {
                    case 0: //up
                        maze[randomSquareX, randomSquareY] += 'U';
                        maze[randomSquareX, randomSquareY - 1] += 'D';
                        break;
                    case 1: //right
                        maze[randomSquareX, randomSquareY] += 'R';
                        maze[randomSquareX + 1, randomSquareY] += 'L';
                        break;
                    case 2: //down
                        maze[randomSquareX, randomSquareY] += 'D';
                        maze[randomSquareX, randomSquareY + 1] += 'U';
                        break;
                    case 3: //left
                        maze[randomSquareX, randomSquareY] += 'L';
                        maze[randomSquareX - 1, randomSquareY] += 'R';
                        break;
                }
            }
        }

        /*deadEndSquaresX.Add(generatorPositionX);
        deadEndSquaresY.Add(generatorPositionY);

        int playerIndex = Random.Range(0, deadEndSquaresX.Count);
        playerPositionX = deadEndSquaresX[playerIndex];
        playerPositionY = deadEndSquaresY[playerIndex];
        int goalIndex = Random.Range(0, deadEndSquaresX.Count);
        while(goalIndex == playerIndex)
        {
            goalIndex = Random.Range(0, deadEndSquaresX.Count);
        }
        goalPositionX = deadEndSquaresX[goalIndex];
        goalPositionY = deadEndSquaresY[goalIndex];*/
    }

    void renderMaze()
    {
        for(int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (maze[j,i].Contains('R'))
                {
                    horizontalWalls[i * 3 + j].sprite = null;
                }
                if (maze[j, i].Contains('D'))
                {
                    verticalWalls[i * 4 + j].sprite = null;
                }
            }
        }
    }

    bool followedAllConditions()
    {
        for(int i = 0; i < currentConditions.Count; i++)
        {
            Condition.Types type = currentConditions[i].Type;
            switch (type)
            {
                case Condition.Types.TimerTens:
                    if(Mathf.FloorToInt((timePressed % 60) / 10) != currentConditions[i].Variable)
                    {
                        Log("Defuser did not press the button when the seconds were " + currentConditions[i].Variable + "X.");
                        return false;
                    }
                    break;
                case Condition.Types.TimerOnes:
                    if (Mathf.FloorToInt(timePressed % 10) != currentConditions[i].Variable)
                    {
                        Log("Defuser did not press the button when the seconds were X" + currentConditions[i].Variable + ".");
                        return false;
                    }
                    break;
                case Condition.Types.HoldMinimum:
                    if (durationHeld < currentConditions[i].Variable)
                    {
                        Log("Defuser did not hold the button for " + currentConditions[i].Variable + " timer ticks.");
                        return false;
                    }
                    break;
                case Condition.Types.HoldMaximum:
                    if (durationHeld > currentConditions[i].Variable)
                    {
                        Log("Defuser held the button for over " + currentConditions[i].Variable + " timer ticks.");
                        return false;
                    }
                    break;
                case Condition.Types.BetweenMinimum:
                    if (timeBetween < currentConditions[i].Variable && timeBetween != -1)
                    {
                        Log("There was under " + currentConditions[i].Variable + " timer ticks between the last button press and this one.");
                        return false;
                    }
                    break;
                case Condition.Types.Defocus:
                    if (!didDefocus && afterFirstPress)
                    {
                        Log("The defuser did not defocus the module.");
                        return false;
                    }
                    didDefocus = false;
                    break;
                default:
                    break;
            }
        }
        return true;
    }

    void applyControlModifications(Condition condition)
    {
        if (condition.Type == Condition.Types.ControlsRotate)
        {
            for (int i = 0; i < 4; i++)
            {
                modifiedDirections[i] = (modifiedDirections[i] - currentConditions[currentConditions.Count - 1].Variable + 4) % 4;
            }
        }

        if (condition.Type == Condition.Types.ControlsFlipHorizontal)
        {
            int oldLeft = modifiedDirections[1];
            modifiedDirections[1] = modifiedDirections[3];
            modifiedDirections[3] = oldLeft;
        }
        if (condition.Type == Condition.Types.ControlsFlipVertical)
        {
            int oldUp = modifiedDirections[0];
            modifiedDirections[0] = modifiedDirections[2];
            modifiedDirections[2] = oldUp;
        }
        if (condition.Type == Condition.Types.ControlsFlipLeftDiagonal)
        {
            int[] oldDirections = new int[4];
            for (int i = 0; i < 4; i++)
            {
                oldDirections[i] = modifiedDirections[i];
            }
            modifiedDirections[0] = oldDirections[3];
            modifiedDirections[1] = oldDirections[2];
            modifiedDirections[2] = oldDirections[1];
            modifiedDirections[3] = oldDirections[0];
        }
        if (condition.Type == Condition.Types.ControlsFlipRightDiagonal)
        {
            int[] oldDirections = new int[4];
            for (int i = 0; i < 4; i++)
            {
                oldDirections[i] = modifiedDirections[i];
            }
            modifiedDirections[0] = oldDirections[1];
            modifiedDirections[1] = oldDirections[0];
            modifiedDirections[2] = oldDirections[3];
            modifiedDirections[3] = oldDirections[2];
        }

        if(condition.Type == Condition.Types.BetweenMaximum)
        {
            timeLimitForMoving = true;
            timeLimit = condition.Variable;
        }

        if (condition.Type == Condition.Types.MultiplePresses)
        {
            trackingMultiplePresses = true;
            repeatPressesQuota = currentConditions[currentConditions.Count - 1].Variable;
        }

        //print(modifiedDirections[0] + "" + modifiedDirections[1] + modifiedDirections[2] + modifiedDirections[3]);
    }

    IEnumerator AnimatePlayer()
    {
        while (true)
        {
            mazeObjects[playerPositionX + playerPositionY * 4].transform.localScale = new Vector3(.5f, .5f, .5f);
            playerRotation += 1f;
            mazeObjects[playerPositionX + playerPositionY * 4].transform.localEulerAngles = new Vector3(90f, playerRotation, 0f);
            yield return new WaitForSeconds(.05f);
        }
    }

    IEnumerator AnimateGoal()
    {
        while (true)
        {
            goalObject.transform.localEulerAngles = new Vector3(90f, goalObject.transform.localEulerAngles.y - 1.333f, 0f);
            yield return new WaitForSeconds(.05f);
        }
    }

    IEnumerator StrikeAnimation(SpriteRenderer sprite)
    {
        Color originalColor = sprite.color;
        sprite.color = Color.red;
        yield return new WaitForSeconds(.5f);
        float t = 0;
        while (t < 1)
        {
            sprite.color = Color.Lerp(Color.red, originalColor, t);
            t += .05f;
            yield return new WaitForSeconds(.05f);
        }
        sprite.color = originalColor;
    }

    IEnumerator SolveAnimation()
    {
        int player = playerPositionX + playerPositionY * 4;

        mazeObjects[player].transform.localScale = new Vector3(.5f, .5f, .5f);
        float t = 0f;
        float rotation = Random.Range(31f, 50f);
        while (t < 1)
        {
            mazeObjects[player].transform.localEulerAngles = new Vector3(90f, (mazeObjects[player].transform.localEulerAngles.y + rotation) % 360, 0f);
            yield return new WaitForSeconds(.01f);
            t += .01f;
        }
        t = 0;
        float newRotation = 23f;
        while (t < 1)
        {
            mazeObjects[player].transform.localEulerAngles = new Vector3(90f, (mazeObjects[player].transform.localEulerAngles.y + newRotation) % 360, 0f);
            yield return new WaitForSeconds(.01f);
            newRotation = Mathf.Lerp(rotation, 0, t);
            t += .01f;
        }
    }

    IEnumerator TurnGreen(SpriteRenderer sprite)
    {
        Color originalColor = new Color();
        originalColor = sprite.color;
        float t = 0;
        while(t < 1)
        {
            sprite.color = Color.Lerp(originalColor, Color.green, t);
            t += .01f;
            yield return new WaitForSeconds(.01f);
        }
    }

    bool sameBoolThings(bool[,] boolThing1, bool[,] boolThing2)
    {
        for(int k = 0; k < 4; k++)
        {
            for (int l = 0; l < 4; l++)
            {
                if(boolThing1[k,l] != boolThing2[k, l])
                {
                    return false;
                }
            }
        }
        return true;
    }

    /*private bool isCommandValid(string cmd)
    {
        string[] validbtns = { "1","2","3","4" };

        var parts = cmd.ToLowerInvariant().Split(new[] { ' ' });

        foreach (var btn in parts)
        {
            if (!validbtns.Contains(btn.ToLower()))
            {
                return false;
            }
        }
        return true;
    }

    public string TwitchHelpMessage = "Use !{0} press 1 13 to press the first button 13 times.";
    IEnumerator ProcessTwitchCommand(string cmd)
    {
        var parts = cmd.ToLowerInvariant().Split(new[] { ' ' });

        if (isCommandValid(cmd))
        {
            yield return null;
            for (int i = 0; i < parts.Count(); i++)
            {
                if (parts[i] == "1")
                {
                    yield return new KMSelectable[] { buttons[0] };
                }
                else if (parts[i] == "2")
                {
                    yield return new KMSelectable[] { buttons[1] };
                }
                else if (parts[i] == "3")
                {
                    yield return new KMSelectable[] { buttons[2] };
                }
                else if (parts[i] == "4")
                {
                    yield return new KMSelectable[] { buttons[3] };
                }
            }
        }
        else
        {
            yield break;
        }
    }*/

    void Log(string msg)
    {
        Debug.LogFormat("[Conditional Maze #{0}] {1}", ModuleId, msg);
    }

    string LogCondition(Condition condition)
    {
        switch(condition.Type)
        {
            case Condition.Types.TimerOnes:
                return "Press the buttons when the last timer digit is " + condition.Variable + ".";
            case Condition.Types.TimerTens:
                return "Press the buttons when the second to last timer digit is " + condition.Variable + ".";
            case Condition.Types.HoldMinimum:
                return "Hold the buttons for " + condition.Variable + " timer ticks.";
            case Condition.Types.HoldMaximum:
                return "Do NOT hold the buttons for over " + condition.Variable + " timer ticks.";
            case Condition.Types.BetweenMinimum:
                return "Wait " + condition.Variable + " timer ticks before pressing buttons.";
            case Condition.Types.BetweenMaximum:
                return "Press the buttons within " + condition.Variable + " timer ticks.";
            case Condition.Types.MultiplePresses:
                return "Press the buttons " + condition.Variable + " times.";
            case Condition.Types.ControlsRotate:
                return "The buttons have rotated " + condition.Variable * 90 + " degrees clockwise.";
            case Condition.Types.ControlsFlipHorizontal:
                return "The buttons have been flipped along the Y axis.";
            case Condition.Types.ControlsFlipVertical:
                return "The buttons have been flipped along the X axis.";
            case Condition.Types.ControlsFlipLeftDiagonal:
                return "The buttons have been flipped along the top left-bottom right diagonal.";
            case Condition.Types.ControlsFlipRightDiagonal:
                return "The buttons have been flipped along the top right-bottom left diagonal.";
            case Condition.Types.Defocus:
                return "Defocus from the module before pressing buttons.";
            default:
                return "No condition.";
        }
    }

    void SilentLog(string msg)
    {
        Debug.LogFormat("<Conditional Maze #{0}> {1}", ModuleId, msg);
    }
}
