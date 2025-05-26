using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

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

    [Header("Liste des bindings � rebind")]
    public List<BindingUI> bindings;
    private string prefsKey = "rebinds";

    private void Awake()
    {
        // Charge les overrides enregistrés (ne supprime plus jamais la clé !)
        var json = PlayerPrefs.GetString(prefsKey, "");
        if (!string.IsNullOrEmpty(json))
            bindings[0]
              .actionReference
              .action
              .actionMap
              .asset
              .LoadBindingOverridesFromJson(json);
    }

    private void Start()
    {
        foreach (var b in bindings)
        {
            // Ignorer les composites : ne rebinder que les parties simples
            var binding = b.actionReference.action.bindings[b.bindingIndex];
            if (binding.isComposite)
            {
                Debug.LogWarning($"RebindActionsUI: skip composite binding index {b.bindingIndex} for action {b.actionReference.action.name}. Use its parts instead.");
                continue;
            }
            UpdateBindingDisplay(b);
            b.rebindButton.onClick.AddListener(() => StartRebind(b));
        }
    }

    private void UpdateBindingDisplay(BindingUI b)
    {
        var binding = b.actionReference.action.bindings[b.bindingIndex];
        // Affiche le displayName du contrôle correspondant (clavier ou manette)
        var control = b.actionReference.action.controls.FirstOrDefault(c => c.path == binding.effectivePath);
        if (control != null)
        {
            // Si clavier -> juste la touche, sinon affiche périphérique
            if (control.device is UnityEngine.InputSystem.Keyboard)
                b.bindingText.text = control.displayName;
            else
                b.bindingText.text = $"{control.device.displayName}: {control.displayName}";
        }
        else
            b.bindingText.text = InputControlPath.ToHumanReadableString(
                binding.effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private void StartRebind(BindingUI b)
    {
        b.rebindButton.interactable = false;
        b.bindingText.text = "Appuyez sur une touche...";
        // Prépare le rebind en excluant toujours la souris
        var rebindOp = b.actionReference.action
            .PerformInteractiveRebinding(b.bindingIndex)
            .WithControlsExcluding("<Mouse>");
        rebindOp.OnComplete(operation =>
        {
            var control = operation.selectedControl;
            // Si clavier -> juste la touche, sinon affiche périphérique
            if (control.device is UnityEngine.InputSystem.Keyboard)
                b.bindingText.text = control.displayName;
            else
                b.bindingText.text = $"{control.device.displayName}: {control.displayName}";
            SaveBindings();
            b.rebindButton.interactable = true;
            operation.Dispose();
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
