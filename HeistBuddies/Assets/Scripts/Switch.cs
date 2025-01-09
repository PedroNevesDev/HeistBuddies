

using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;

public class Switch : MonoBehaviour
{

    [SerializeField] Color p1color;
    [SerializeField] Color p2color;

    [SerializeField] Image backimage1;
    [SerializeField] Image backimage2;

    [SerializeField] Transform leftPos;
    [SerializeField] Transform rightPos;

    [SerializeField] Transform movingImage1;
    [SerializeField] Transform movingImage2;
    bool isOnLeft;
    SelectionPersistantData selectionPersistantData;
    [SerializeField] InputSystemUIInputModule inputModule;
    
    void Start()
    {
        selectionPersistantData = SelectionPersistantData.Instance;
        CheckStuff();
    }
    private void OnDisable() 
    {
        inputModule.submit.action.performed -= ChangeScene;
    }

    private void OnEnable() 
    {
        inputModule.submit.action.performed += ChangeScene;     
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.D))
        {
            SwitchStuff();
        }
        if (Gamepad.current != null)
        {
            if (Gamepad.current.dpad.left.wasPressedThisFrame)
            {
                SwitchStuff();
            }
            if (Gamepad.current.dpad.right.wasPressedThisFrame)
            {
                SwitchStuff();
            }
        }
    }

    void ChangeScene(InputAction.CallbackContext context)
    {

        SceneManager.LoadScene("LevelX");
        this.enabled =false;
    }
    void SwitchStuff()
    {
        isOnLeft = !isOnLeft;
        CheckStuff();
    }

    void CheckStuff()
    {
        if(!isOnLeft)
        {
            movingImage1.transform.position = leftPos.position;
            movingImage2.transform.position = rightPos.position;

            backimage1.color = p1color;           
            backimage2.color = p2color;           

            selectionPersistantData.LockModel = 1;
            selectionPersistantData.PickModel = 2;
        }
        else
        {
            movingImage1.transform.position = rightPos.position;
            movingImage2.transform.position = leftPos.position;

            backimage1.color = p2color;           
            backimage2.color = p1color;       

            selectionPersistantData.LockModel = 2;
            selectionPersistantData.PickModel = 1;   
        }
    }
}
