/*You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Collections.Generic;
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
        public Material invisibleMaterial;

        private Dictionary<ulong, GameObject> cellIdToGameobjects = new Dictionary<ulong, GameObject>();


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
                    UMI3DDto dto = (nodeInstance.dto as GlTFNodeDto)?.extensions.umi3d;

                    if (dto is UMI3DMeshNodeDto && !(dto is SubModelDto)) //subModels will be initialized with their associated UMI3DModel.
                        InitModel(nodeInstance);
                }
            }

            VolumePrimitiveManager.SubscribeToPrimitiveCreation(c =>
            {
                AddNavigableVolume(c);
            },
            true);

            ExternalVolumeDataManager.SubscribeToExternalVolumeCreation(c =>
            {
                AddNavigableVolume(c);
            },
            true);

            ExternalVolumeDataManager.SubscribeToExternalVolumeDelete(RemoveCell);
            VolumePrimitiveManager.SubscribeToPrimitiveDelete(RemoveCell);

            surface.BuildNavMesh();
        }

        
        public void RemoveCell(AbstractVolumeCell cell)
        {
            if (cellIdToGameobjects.ContainsKey(cell.Id()))
            {
                GameObject obj = cellIdToGameobjects[cell.Id()];
                cellIdToGameobjects.Remove(cell.Id());
                Destroy(obj);

                cellReceived = 1;
                cellProcessed = 1;
            }
        }

        public void AddNavigableVolume(AbstractVolumeCell cell)
        {
            if (cell.isTraversable)
            {
                cellReceived++;
                cell.GetBase(mesh =>
                {
                    GameObject surfaceGo = new GameObject("Surface for " + cell.GetType());
                    surfaceGo.transform.parent = surface.transform;
                    surfaceGo.transform.position = Vector3.zero;
                    surfaceGo.transform.rotation = Quaternion.identity;
                    surfaceGo.transform.localScale = Vector3.one;
                    surfaceGo.AddComponent<MeshFilter>().mesh = mesh;
                    surfaceGo.AddComponent<MeshRenderer>().material = invisibleMaterial; //<-- not ideal.

                    ChangeObjectAndChildrenLayer(surfaceGo, navmeshLayer);
                    cellProcessed++;
                    cellIdToGameobjects.Add(cell.Id(), surfaceGo);
                }, surface.GetBuildSettings().agentSlope);
            }
            else
            {
                GameObject obstacle = new GameObject("obstacle for " + cell.GetType());
                obstacle.transform.parent = surface.transform;
                obstacle.transform.position = Vector3.zero;
                obstacle.transform.rotation = Quaternion.identity;
                obstacle.transform.localScale = Vector3.one;
                obstacle.AddComponent<MeshFilter>().mesh = cell.GetMesh();
                obstacle.AddComponent<MeshRenderer>().material = invisibleMaterial; //<-- not ideal.
                ChangeObjectAndChildrenLayer(obstacle, obstacleLayer);
                cellIdToGameobjects.Add(cell.Id(), obstacle);
            }
        }


        int cellReceived = 0;
        int cellProcessed = 0;
        private void Update()
        {
            if (cellReceived > 0)
            {
                if (cellReceived == cellProcessed)
                {
                    cellProcessed = 0;
                    cellReceived = 0;
                    surface.BuildNavMesh();
                }
            }
        }


        [ContextMenu("ForceBuild")]
        private void ForceBuild()
        {
            surface.BuildNavMesh();
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
                    if (r.gameObject.GetComponent<NavMeshModifier>() == null)
                    {
                        NavMeshModifier modifier = r.gameObject.AddComponent<NavMeshModifier>();
                        modifier.overrideArea = true;
                        modifier.area = 1; // 1 = means not walkable.
                    } else
                    {
                        Debug.LogWarning(r.gameObject.name + " tries to init its navmesh at least twice, this should not happen.");
                    }
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