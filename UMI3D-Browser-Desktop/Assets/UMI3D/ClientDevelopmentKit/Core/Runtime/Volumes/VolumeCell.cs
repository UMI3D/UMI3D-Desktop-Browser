using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.volume;

namespace umi3d.cdk.volumes
{
	public class VolumeCell 
	{
        public class Slice
        {
            public VolumeSliceDto originalDto;
            public string id { get; private set; }
            private List<Point> points;
            private List<int> edges;
            private List<Face> faces;

            public Slice() { }
            public Slice(VolumeSliceDto dto, List<Point> points, List<int> edges, List<Face> faces)
            {
                this.originalDto = dto;
                this.id = dto.id;
                this.points = points;
                this.edges = edges;
                this.faces = faces;
            }

            public bool isInside(Vector3 point)
            {
                /*
                 * Algorithm :
                 *  Cast a random ray from the point and count the number of intersections with the mesh
                 *  
                 *  Create a random ray from the point
                 *  Compute a new list of faces from the faces property to ensure planar faces
                 *  Compute the bounding box of the volume slice
                 *  for each face f in planar faces
                 *      compute the plane of f
                 *      compute the intersection of the ray and the plane
                 *      if intersec if in bounding box
                 *          check if the intersec is inside the face
                 *      
                 */
                int intersectionCount = 0;

                Ray randomRay = new Ray(point, new Vector3(Random.value, Random.value, Random.value).normalized);
                Bounds bounds = GeometryTools.ComputeBoundingBox(points.ConvertAll(p => p.position));
                List<GeometryTools.Face3> planarFaces = new List<GeometryTools.Face3>();
                foreach (Face f in faces)
                {
                    planarFaces.AddRange(GeometryTools.GetPlanarSubFaces(f.ToFace3()));
                }

                foreach (GeometryTools.Face3 face in planarFaces)
                {
                    Plane plane = GeometryTools.GetPlane(face.points);
                    if (plane.Raycast(randomRay, out float enter))
                    {
                        Vector3 intersection = plane.ClosestPointOnPlane(randomRay.origin + enter * randomRay.direction);
                        if (bounds.Contains(intersection))
                        {
                            if (face.isInside(intersection))
                            {
                                intersectionCount++;
                            }
                        }
                    }
                }
                return (intersectionCount % 2) == 1;
            }

            public static Slice GetSlideById(string id)
            { 
                throw new System.NotImplementedException(); //todo
            }

            public void SetPoints(List<PointDto> newPoints)
            {
                throw new System.NotImplementedException(); //todo
            }
            public void SetEdges(List<int> newEdges)
            {
                throw new System.NotImplementedException(); //todo
            }
            public void SetFaces(List<FaceDto> newFaces)
            {
                throw new System.NotImplementedException(); //todo
            }
        }

        private List<Slice> slices = new List<Slice>();

        public void Setup(VolumeDto dto)
        {
            slices.Clear();
            foreach(VolumeSliceDto sliceDto in dto.slices)
            {
                Slice slice = new Slice(
                    sliceDto,
                    sliceDto.points.ConvertAll(id => VolumeManager.Instance.GetPoint(id)),
                    sliceDto.edges,
                    sliceDto.faces.ConvertAll(id => VolumeManager.Instance.GetFace(id)));
                
                slices.Add(slice);
            }
        }	
        
        public bool IsInside(Vector3 point)
        {
            foreach(Slice s in slices)
            {
                if (s.isInside(point))
                    return true;
            }
            return false;
        }

        public void SetSlices(List<VolumeSliceDto> newSlices)
        {
            throw new System.NotImplementedException(); //todo
        }

        public Slice[] GetSlices() => slices.ToArray();
	}
}