using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.volume;
using umi3d.common;

namespace umi3d.cdk.volumes
{
	public class VolumeManager : Singleton<VolumeManager>
	{
        private Dictionary<string, Point> points = new Dictionary<string, Point>();
        private Dictionary<string, Face> faces = new Dictionary<string, Face>();
        private Dictionary<string, VolumeCell> volumeCells = new Dictionary<string, VolumeCell>();

        public List<Point> GetPoints() => new List<Point>(points.Values);
		public List<Face> GetFaces() => new List<Face>(faces.Values);
        public List<VolumeCell> GetVolumeCells() => new List<VolumeCell>(volumeCells.Values);

        
		public bool PointExists(string id)
        {
            return points.ContainsKey(id);
        }


        public bool FaceExists(string id)
        {
            return faces.ContainsKey(id);
        }

        public bool VolumeCellExist(string id)
        {
            return volumeCells.ContainsKey(id);
        }

        public Point GetPoint(string id)
        {
            return points[id];
        }


        public Face GetFace(string id)
        {
            return faces[id];
        }

        public VolumeCell GetVolumeCell(string id)
        {
            return volumeCells[id];
        }

        public Point CreatePoint(PointDto dto)
        {
            if (PointExists(dto.id))
            {
                throw new System.Exception("Point already exists");
            }

            Point p = new Point();
            p.Setup(dto);

            points.Add(dto.id, p);

            return p;
        }


        public Face CreateFace(FaceDto dto)
        {
            if (FaceExists(dto.id))
                throw new System.Exception("Face already exists");

            Face face = new Face();
            face.Setup(dto);

            faces.Add(dto.id, face);
            return face;
        }

        public VolumeCell CreateVolumeCell(VolumeDto dto)
        {
            if (VolumeCellExist(dto.id))
                throw new System.Exception("Volume cell already exists");

            VolumeCell volume = new VolumeCell();
            volume.Setup(dto);

            volumeCells.Add(dto.id, volume);
            return volume;
        }


        /*
         * TODO : update functions
         */

        public void DeletePoint(string id)
        {
            throw new System.NotImplementedException(); //todo
        }
        public void DeleteFace(string id)
        {
            throw new System.NotImplementedException(); //todo
        }
        public void DeleteVolumeCell(string id)
        {
            throw new System.NotImplementedException(); //todo
        }
        


    }
}