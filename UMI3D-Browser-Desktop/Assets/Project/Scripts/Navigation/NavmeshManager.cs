/*You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using umi3d.cdk;
using umi3d.cdk.volumes;
using umi3d.common;
using UnityEngine;
using UnityEngine.AI;

namespace BrowserDesktop.Navigation
{

    /// <summary>
    /// This class handles the navigation possibility. It is based on Unity navmesh.
    /// </summary>
    public class NavmeshManager : MonoBehaviour
    {
        /// <summary>
        /// Name of the layer where objects of the navmesh will be set.
        /// </summary>
        public string navmeshLayerName = "Navmesh";

        /// <summary>
        /// Name of the layerer where obstacles objects will be set.
        /// </summary>
        public string obstacleLayerName = "Obstacle";

        private LayerMask navmeshLayer;

        private LayerMask obstacleLayer;

        public NavMeshSurface surface;

        void Start()
        {
            navmeshLayer = LayerMask.NameToLayer(navmeshLayerName);
            obstacleLayer = LayerMask.NameToLayer(obstacleLayerName);
            Debug.Assert(navmeshLayer != default && obstacleLayerName != default);
            Debug.Assert(surface);

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(InitNavMesh);
        }

        /// <summary>
        /// Once the environment is loaded, generates the navmesh.
        /// </summary>
        public void InitNavMesh()
        {
            foreach (var entity in UMI3DEnvironmentLoader.Entities())
            {
                if (entity is UMI3DNodeInstance nodeInstance)
                {
                    InitModel(nodeInstance);
                }
            }

            VolumePrimitiveManager.SubscribeToPrimitiveCreation(c =>
            {
                AddNavigableVolume(c);
                surface.BuildNavMesh(); //TODO we can do better here than reload the navmesh at each cell reception, especially on the initial reception of all cells.
            },
            true);

            VolumeSliceGroupManager.SubscribeToSliceGroupCreation(c =>
            {
                AddNavigableVolume(c);
                surface.BuildNavMesh(); //TODO we can do better here than reload the navmesh at each cell reception, especially on the initial reception of all cells.
            },
            true);

            ExternalVolumeDataManager.SubscribeToExternalVolumeCreation(c =>
            {
                AddNavigableVolume(c);
                surface.BuildNavMesh(); //TODO we can do better here than reload the navmesh at each cell reception, especially on the initial reception of all cells.
            },
            true);

            surface.BuildNavMesh();
        }

        public void AddNavigableVolume(AbstractVolumeCell cell)
        {
            Mesh mesh = cell.GetBase();
            GameObject surfaceGo = new GameObject("Surface for " + cell.GetType());
            surfaceGo.transform.parent = surface.transform;
            surfaceGo.transform.position = Vector3.zero;
            surfaceGo.transform.rotation = Quaternion.identity;
            surfaceGo.transform.localScale = Vector3.one;
            surfaceGo.AddComponent<MeshFilter>().mesh = mesh;
            surfaceGo.AddComponent<MeshRenderer>(); //<-- not ideal.

            ChangeObjectAndChildrenLayer(surfaceGo, navmeshLayer);
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
        /// If a gameobject is not traversable or part of the navmesh, sets it up.
        /// </summary>
        /// <param name="nodeInstance"></param>
        /// <param name="dto"></param>
        private void SetUpGameObject(UMI3DNodeInstance nodeInstance, bool isPartOfNavmesh, bool isTraversable)
        {
            GameObject obj = nodeInstance.gameObject;

            if (isPartOfNavmesh)
            {
                ChangeObjectAndChildrenLayer(obj, navmeshLayer);
            }else if (!isTraversable)
            {
                ChangeObjectAndChildrenLayer(obj, obstacleLayer);
                foreach (var r in nodeInstance.renderers)
                {
                    NavMeshModifier modifier = r.gameObject.AddComponent<NavMeshModifier>();
                    modifier.overrideArea = true;
                    modifier.area = 1; // 1 = means not walkable.
                }
            }
        }

        private void ChangeObjectAndChildrenLayer(GameObject parent, LayerMask mask){
            parent.layer = mask;

            foreach(Transform t in parent.transform)
                ChangeObjectAndChildrenLayer(t.gameObject, mask);
        }
    }
}