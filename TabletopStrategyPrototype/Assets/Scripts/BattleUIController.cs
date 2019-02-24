using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    public static BattleUIController instance;

    public CameraControls cameraControls;

    public Unit selectedUnit
    {
        get
        {
            return _selectedUnit;
        }
        set
        {
            _selectedUnit = value;
            cameraControls.focusOnPoint(BattleUIController.instance.selectedUnit.transform.position);
        }

    }
    private Unit _selectedUnit;

    public GameObject topLevelMenu, actionsMenu, movingMenu, attackingMenu;
    public Button moveButton, attackButton;


    public GameObject rangeIndicator;

    private bool waitingForClickInput = false;
    private GameObject clickedObject;
    private Vector3 clickedPoint;

    private enum MenuStates
    {
        TopLevel,
        Moving,
        Actions,
        Attacking,
        Abilities,
        Spells
    }

    private Stack<MenuStates> menuStack = new Stack<MenuStates>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            menuStack.Push(MenuStates.TopLevel);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForClickInput && Input.GetMouseButtonDown(0))
        {
            // shoot a ray to find where they clicked
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                clickedObject = hit.transform.gameObject;
                clickedPoint = hit.point;
            }
        }

        topLevelMenu.SetActive(false);
        actionsMenu.SetActive(false);
        movingMenu.SetActive(false);
        attackingMenu.SetActive(false);
        rangeIndicator.SetActive(false);

        if (selectedUnit != null)
        {
            // TODO: update buttons based on capabilities of unit
            moveButton.interactable = selectedUnit.movementRemaining >= 1f;
            attackButton.interactable = selectedUnit.attacksRemaining > 0;

            switch (menuStack.Peek())
            {
                case MenuStates.TopLevel:
                    topLevelMenu.SetActive(true);
                    break;
                case MenuStates.Moving:
                    rangeIndicator.SetActive(true);
                    movingMenu.SetActive(true);

                    if (clickedObject != null)
                    {
                        if (clickedObject.tag == "Terrain")
                        {
                            if (Vector3.Distance(selectedUnit.transform.position, clickedPoint) <= selectedUnit.movementRemaining)
                            {
                                selectedUnit.moveTo(clickedPoint);
                                clickedObject = null;
                                waitingForClickInput = false;
                                menuStack.Pop();
                            }
                        }
                    }
                    break;
                case MenuStates.Actions:
                    actionsMenu.SetActive(true);
                    break;
                case MenuStates.Attacking:
                    rangeIndicator.SetActive(true);
                    attackingMenu.SetActive(true);

                    if (clickedObject != null)
                    {
                        if (clickedObject.tag == "Unit")
                        {
                            if (Vector3.Distance(selectedUnit.transform.position, clickedObject.transform.position) <= selectedUnit.attackRange + 0.5f) // +0.5f to account for unit radius approx
                            {
                                selectedUnit.hitRoll(clickedObject.GetComponent<Unit>());
                                clickedObject = null;

                                if (selectedUnit.attacksRemaining == 0)
                                {
                                    waitingForClickInput = false;
                                    menuStack.Pop();
                                }
                            }
                        }
                    }
                    break;
                case MenuStates.Abilities:
                    break;
                case MenuStates.Spells:
                    break;
                default:
                    break;
            }
        }
        else
        {
            print("No unit selected.");
        }
    }

    public void moveButtonPressed()
    {
        rangeIndicator.transform.position = new Vector3(selectedUnit.transform.position.x, rangeIndicator.transform.position.y, selectedUnit.transform.position.z);
        rangeIndicator.transform.localScale = new Vector3(selectedUnit.movementRemaining * 2, 1, selectedUnit.movementRemaining * 2);
        waitingForClickInput = true;
        menuStack.Push(MenuStates.Moving);
    }

    public void actionButtonPressed()
    {
        menuStack.Push(MenuStates.Actions);
    }

    public void attackButtonPressed()
    {
        rangeIndicator.transform.position = new Vector3(selectedUnit.transform.position.x, rangeIndicator.transform.position.y, selectedUnit.transform.position.z);
        rangeIndicator.transform.localScale = new Vector3(selectedUnit.attackRange * 2, 1, selectedUnit.attackRange * 2);
        waitingForClickInput = true;
        menuStack.Push(MenuStates.Attacking);
    }

    public void spellButtonPressed()
    {

    }

    public void backButtonPressed()
    {
        menuStack.Pop();
    }

    public void endTurnButtonPressed()
    {
        selectedUnit.endTurn();
        BattleManager.instance.nextTurn();
    }
}
