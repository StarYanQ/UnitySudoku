using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
#if !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace Michsky.MUIP
{
    [RequireComponent(typeof(TMP_InputField))]
    [RequireComponent(typeof(Animator))]
    public class CustomInputField : MonoBehaviour
    {
        [Header("Resources")]
        public TMP_InputField inputText;
        public Animator inputFieldAnimator;

        [Header("Settings")]
        public bool processSubmit = false;
        public bool clearOnSubmit = true;

        [Header("Events")]
        public UnityEvent onSubmit;

        // Hidden variables
        private float cachedDuration = 0.5f;
        private string inAnim = "In";
        private string outAnim = "Out";
        private string instaInAnim = "Instant In";
        private string instaOutAnim = "Instant Out";
        private bool isActive = false;

        void Awake()
        {
            if (inputText == null) { inputText = gameObject.GetComponent<TMP_InputField>(); }
            if (inputFieldAnimator == null) { inputFieldAnimator = gameObject.GetComponent<Animator>(); }

            inputText.onSelect.AddListener(delegate { AnimateIn(); });
            inputText.onEndEdit.AddListener(delegate { HandleEndEdit(); });
            inputText.onValueChanged.AddListener(delegate { UpdateState(); });

            UpdateStateInstant();
        }

        void OnEnable()
        {
            if (inputText == null) { return; }
            if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }

            inputText.ForceLabelUpdate();
            UpdateStateInstant();
        }

        void Update()
        {
            if (processSubmit == false ||
                string.IsNullOrEmpty(inputText.text) == true ||
                EventSystem.current.currentSelectedGameObject != inputText.gameObject)
            { return; }

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Return)) 
            { 
                onSubmit.Invoke();

                if (clearOnSubmit == true) 
                {
                    inputText.text = ""; 
                    UpdateState();
                } 
            }
#elif ENABLE_INPUT_SYSTEM
            if (Keyboard.current.enterKey.wasPressedThisFrame) 
            { 
                onSubmit.Invoke(); 
                
                if (clearOnSubmit == true) 
                { 
                    inputText.text = ""; 
                    UpdateState();
                } 
            }
#endif
        }

        public void AnimateIn() 
        {
            if (inputFieldAnimator.gameObject.activeInHierarchy == true && isActive != true) 
            {
                StopCoroutine("DisableAnimator");
                StartCoroutine("DisableAnimator");

                isActive = true;
                inputFieldAnimator.enabled = true;
                inputFieldAnimator.Play(inAnim);
            }
        }

        public void AnimateOut()
        {
            if (inputFieldAnimator.gameObject.activeInHierarchy == true && inputText.text.Length == 0 && isActive != false)
            {
                StopCoroutine("DisableAnimator");
                StartCoroutine("DisableAnimator");

                isActive = false;
                inputFieldAnimator.enabled = true;
                inputFieldAnimator.Play(outAnim);
            }
        }

        public void UpdateState()
        {
            if (inputText.text.Length == 0) { AnimateOut(); }
            else { AnimateIn(); }
        }

        public void UpdateStateInstant()
        {
            inputFieldAnimator.enabled = true;

            StopCoroutine("DisableAnimator");
            StartCoroutine("DisableAnimator");

            if (inputText.text.Length == 0) { isActive = false; inputFieldAnimator.Play(instaOutAnim);  }
            else { isActive = true; inputFieldAnimator.Play(instaInAnim); }
        }

        void HandleEndEdit()
        {
            if (string.IsNullOrEmpty(inputText.text) && !EventSystem.current.alreadySelecting && EventSystem.current.currentSelectedGameObject == inputText.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            AnimateOut();
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSecondsRealtime(cachedDuration);
            inputFieldAnimator.enabled = false;
        }
    }
}