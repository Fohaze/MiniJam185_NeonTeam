using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RebindActionsUI : MonoBehaviour
{
    [System.Serializable]
    public class BindingUI
    {
        [Tooltip("Nom descriptif de l'action (ex: " + "Haut" + ")")]
        public string label;
        public InputActionReference actionReference;
        [Tooltip("Index du binding dans l'InputAction (0=action simple, 1..n=composite parts)")]
        public int bindingIndex;
        public Button rebindButton;
        public Text bindingText;
    }

    [Header("Liste des bindings à rebind")]
    public List<BindingUI> bindings;
    private string prefsKey = "rebinds";

    private void Awake()
    {
        // Charger les overrides sauvegardés
        var json = PlayerPrefs.GetString(prefsKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            // l'asset est le même pour toutes les actions
            var asset = bindings[0].actionReference.action.actionMap.asset;
            asset.LoadBindingOverridesFromJson(json);
        }
    }

    private void Start()
    {
        foreach (var b in bindings)
        {
            UpdateBindingDisplay(b);
            b.rebindButton.onClick.AddListener(() => StartRebind(b));
        }
    }

    private void UpdateBindingDisplay(BindingUI b)
    {
        var path = b.actionReference.action.bindings[b.bindingIndex].effectivePath;
        b.bindingText.text = InputControlPath.ToHumanReadableString(
            path,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private void StartRebind(BindingUI b)
    {
        b.rebindButton.interactable = false;
        b.bindingText.text = "Appuyez sur une touche...";

        b.actionReference.action
         .PerformInteractiveRebinding(b.bindingIndex)
         .WithControlsExcluding("<Mouse>")
         .OnComplete(operation =>
         {
             operation.Dispose();
             UpdateBindingDisplay(b);
             SaveBindings();
             b.rebindButton.interactable = true;
         })
         .Start();
    }

    private void SaveBindings()
    {
        var asset = bindings[0].actionReference.action.actionMap.asset;
        var json = asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(prefsKey, json);
        PlayerPrefs.Save();
    }
}
