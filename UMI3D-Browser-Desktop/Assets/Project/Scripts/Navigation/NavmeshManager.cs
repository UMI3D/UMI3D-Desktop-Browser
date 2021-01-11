/*You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using umi3d.cdk;
using umi3d.common;
using UnityEngine;

namespace BrowserDesktop.Navigation
{

    /// <summary>
    /// This class handles the navigation possibility. For now, this browser does not support a real navmesh, it only prevents users from passing
    /// through objects which are set as "not traversable".
    /// </summary>
    public class NavmeshManager : MonoBehaviour
    {

        public string navMeshLayername = "Navmesh";

        private LayerMask layer;

        void Start()
        {
            layer = LayerMask.NameToLayer(navMeshLayername);
            Debug.Assert(layer != default);

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(InitNavMesh);
        }

        /// <summary>
        /// Once the environment is loaded, generates the navmesh.
        /// </summary>
        private void InitNavMesh()
        {
            foreach (var entity in UMI3DEnvironmentLoader.Entities())
            {
                if (entity is UMI3DNodeInstance nodeInstance)
                {
                    InitModel(nodeInstance);
                }
            }
        }

        /// <summary>
        /// Inits navmesh according to the data stored by nodeInstance and its children.
        /// </summary>
        /// <param name="nodeInstance"></param>
        void InitModel(UMI3DNodeInstance nodeInstance)
        {
            UMI3DMeshNodeDto meshNodeDto = (nodeInstance.dto as GlTFNodeDto)?.extensions.umi3d as UMI3DMeshNodeDto;

            if (meshNodeDto != null)
                SetUpGameObject(nodeInstance, meshNodeDto);
            else
            {
                SubModelDto subModelDto = (nodeInstance.dto as GlTFNodeDto)?.extensions.umi3d as SubModelDto;
                if (subModelDto != null)
                    SetUpGameObject(nodeInstance, subModelDto);
            }

            foreach (var child in nodeInstance.subNodeInstances)
            {
                InitModel(child);
            }
        }

        private void SetUpGameObject(UMI3DNodeInstance nodeInstance, UMI3DMeshNodeDto meshDto)
        {
            
            SetUpGameObject(nodeInstance, meshDto.isPartOfNavmesh, meshDto.isTraversable);
        }

        private void SetUpGameObject(UMI3DNodeInstance nodeInstance, SubModelDto subModelDto)
        {
            SetUpGameObject(nodeInstance, subModelDto.isPartOfNavmesh, subModelDto.isTraversable);
        }

        /// <summary>
        /// If a gameobject is not traversable, sets it up.
        /// </summary>
        /// <param name="nodeInstance"></param>
        /// <param name="dto"></param>
        private void SetUpGameObject(UMI3DNodeInstance nodeInstance, bool isPartOfNavmesh, bool isTraversable)
        {
            if (!isTraversable)
            {
                Collider collider;
                foreach (Renderer r in nodeInstance.renderers)
                {
                    if (!r.TryGetComponent(out collider))
                    {
                        MeshFilter filter;
                        if (r.TryGetComponent(out filter))
                        {
                            if (filter.sharedMesh != null && filter.sharedMesh.isReadable)
                                AddCollider(r.gameObject);
                            else
                                Debug.LogWarning(nodeInstance.gameObject.name + " can't be used for the navemesh or to limit it because its mesh is not readable.");
                        }
                        else
                        {
                            AddCollider(r.gameObject);
                        }
                    } else
                    {
                        collider.isTrigger = false;
                        collider.gameObject.layer = layer;
                    }
                }
            }
        }

        void AddCollider(GameObject obj)
        {
            obj.AddComponent<MeshCollider>();

            obj.layer = layer;
        }
    }
}