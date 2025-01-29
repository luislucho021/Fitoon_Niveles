using UnityEngine;

namespace Unity.Netcode.Components
{
    [RequireComponent(typeof(NetworkTransform))]
    [AddComponentMenu("Netcode/Network Rotator Translator")]
    public class NetworkRotatorTranslator : NetworkBehaviour
    {
        private bool hasAuthority;

        private void Awake()
        {
            if (GetComponent<SimpleRotator>() != null) GetComponent<SimpleRotator>().enabled = true;
            if (GetComponent<Rotator>() != null) GetComponent<Rotator>().enabled = true;
            if (GetComponent<Translator>() != null) GetComponent<Translator>().enabled = true;
        }

        private void UpdateOwnershipAuthority()
        {
            hasAuthority = NetworkManager.IsServer;

            if (GetComponent<SimpleRotator>() != null) GetComponent<SimpleRotator>().enabled = hasAuthority;
            if (GetComponent<Rotator>() != null) GetComponent<Rotator>().enabled = hasAuthority;
            if (GetComponent<Translator>() != null) GetComponent<Translator>().enabled = hasAuthority;
        }

        public override void OnNetworkSpawn()
        {
            UpdateOwnershipAuthority();
        }

    }

}
