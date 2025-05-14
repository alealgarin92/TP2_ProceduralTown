using UnityEngine;

namespace SVS
{

    public class AutoMeshCollider : MonoBehaviour
    {
        [Tooltip("Agregar el collider al iniciar el juego")]
        public bool runOnStart = true;

        private void Start()
        {
            if (runOnStart)
            {
                AddMeshColliders();
            }
        }

        [ContextMenu("Agregar MeshColliders")]
        public void AddMeshColliders()
        {
            var meshFilters = GetComponentsInChildren<MeshFilter>();

            foreach (var mf in meshFilters)
            {
                var go = mf.gameObject;

                if (go.GetComponent<Collider>() == null && mf.sharedMesh != null)
                {
                    var mc = go.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh;
                    mc.convex = false;
                    Debug.Log("✔ MeshCollider agregado a: " + go.name);
                }
            }
        }
    }


}
