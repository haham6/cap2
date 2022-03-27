using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class PlayerUI : MonoBehaviour
    {
        #region Private Fields
        PlayerManager target;

        [Tooltip("UI Text to display Player's Name")]
        [SerializeField]
        public Text playerNameText;

        #endregion

        #region Private Fields Messages
        float characterControllerHeight = 0f;
        Transform targetTransform;
        Vector3 targetPosition;
        #endregion

        #region Public Fields
        [Tooltip("Pixel offset from the player target")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);
        #endregion




        #region MonoBehaviour CallBacks
        void Update()
        {
            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }
        }
        
        void Awake()
        {
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        }
        #endregion


        #region Public Methods
        public void SetTarget(PlayerManager _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }

            // Cache references for efficiency
            target = _target;
            targetTransform = target.transform;

            if (playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }

            CharacterController _characterController = _target.GetComponent<CharacterController>();

            // Get data from the Player that won't change during the lifetime of this Component
            if (_characterController != null)
            {
                characterControllerHeight = _characterController.height;
            }
        }
        void LateUpdate()
        {
            Debug.Log(targetTransform);
            // #Critical
            // Follow the Target GameObject on screen.
            if (targetTransform != null)
            {
                Debug.Log(targetTransform);
                targetPosition = targetTransform.position;
                Debug.Log(targetPosition);
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }
        #endregion



    }
}