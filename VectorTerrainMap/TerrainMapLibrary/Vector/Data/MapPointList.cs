using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrainMapLibrary.Vector.Data
{
    public class MapPointList
    {
        private List<MapPoint> mapPoints;


        public MapPoint this[int index]
        {
            get { return mapPoints[index]; }
        }

        public int Count
        {
            get { return mapPoints.Count; }
        }


        public MapPointList()
        {
            mapPoints = new List<MapPoint>();
        }

        public bool Add(MapPoint mapPoint)
        {
            if (mapPoint == null) { return false; }

            mapPoints.Add(mapPoint);
            return true;
        }

        public void Enumerate(Action<MapPoint, int, int> raise)
        {
            for (int i = 0; i < mapPoints.Count; i++)
            {
                raise.Invoke(mapPoints[i], i, mapPoints.Count);
            }
        }

        public void Enumerate(Action<MapPoint, int, MapPoint, int> raise)
        {
            for (int i = 0; i < mapPoints.Count; i++)
            {
                for (int j = i + 1; j < mapPoints.Count; j++)
                {
                    raise.Invoke(mapPoints[i], i, mapPoints[j], j);
                }
            }
        }
    }
}
