using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectTool : MonoBehaviour {

    private enum ToolType {
        Sword,
        Hook,
        Amulet
    }
    [SerializeField] private ToolType toolType;
    [SerializeField] private float distanceToInteract;

    private void Start() {
        switch (toolType) {
            case ToolType.Sword:
                if (PlayerData.Instance.hasSword) Destroy(gameObject);
                break;
            case ToolType.Hook:
                if (PlayerData.Instance.hasHook) Destroy(gameObject);
                break;
            case ToolType.Amulet:
                if (PlayerData.Instance.hasAmulet) Destroy(gameObject);
                break;
        }
    }

    private void Update() {
        if (Vector3.Distance(transform.position, PlayerData.rb.transform.position) <= distanceToInteract && Input.GetKeyDown(KeyCode.E)) {
            switch (toolType) {
                case ToolType.Sword:
                    PlayerData.Instance.hasSword = true;
                    UserInterface.Instance.swordBtnImage.enabled = true;
                    Destroy(gameObject); // May be changed to a cutscene
                    break;
                case ToolType.Hook:
                    PlayerData.Instance.hasHook = true;
                    UserInterface.Instance.hookBtnImage.enabled = true;
                    break;
                case ToolType.Amulet:
                    PlayerData.Instance.hasAmulet = true;
                    UserInterface.Instance.amuletBtnImage.enabled = true;
                    Destroy(gameObject); // May be changed to a cutsc
                    break;
            }
        }
    }
}
