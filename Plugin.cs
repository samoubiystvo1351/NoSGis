using BepInEx;

namespace BepInEx5.PluginTemplate
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Update() { 
            // Only run  on the master client
            // If you are not the master client you cannot use the PhotonNetwork.CloseConnection call.
            if (!PhotonNetwork.isMasterClient) { return; }

            // We look for all network players in the scene
            // This is not the best way of doing it, ideally you would patch the NetworkPlayer or something with Harmony.
            // Then do the work there to avoid searching every frame, but the performance impact is negligible for this use case.
            NetworkPlayer[] networkPlayers = FindObjectsOfType<NetworkPlayer>();
            foreach (NetworkPlayer player in networkPlayers) {
                // Ignore the local player to prevent weirdness
                // mPhotonView is a private field, but made accessible via the BepInEx Assembly Publicizer
                // This is the same photon view that the PlayerHandler for this player uses.
                if (player.mPhotonView.isMine) { continue; }

                // The IS_RED value gets passed on instantiation to the player object, this is how you retrieve it.
                bool isRed = (bool)player.mPhotonView.instantiationData[0];
                if (!isRed) { continue; } // the problematic players will never be on the blue team bcs of how there clients are edited.
                
                // Cache the owner for easier access
                // PhotonView.owner is a property that does PhotonPlayer.Find(this.ownerId) internally.
                // So caching it is more efficient.
                PhotonPlayer owner = player.mPhotonView.owner;
                
                // Final check for the specific nickname, disconnect them if all conditions are met.
                if (owner.NickName == "Player_SGi") { PhotonNetwork.CloseConnection(owner); }
            }
        }
    }
}
