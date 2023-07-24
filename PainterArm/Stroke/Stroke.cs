using JakaAPI.Types.Math;
using PainterArm.MathExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PainterArm.Stroke
{
    public enum TransitionType
    {
        None,
        Vertical,
        Trapezoid
    }

    public class Stroke
    {
        private List<Point> points;

        public Stroke(List<Point> points)
        {
            this.points = points;
        }

        public List<Point> GetPoints()
        {
            return points;
        }

        public Stroke AddEnter(double height = 5, double offset = 2)
        {
            if (points.Count < 2)
            {
                return this;
            }

            Point start = points[0];
            Point post_start = points[1];

            Vector3 direction = (Vector3)post_start - (Vector3)start;
            Vector3 unit_direction = direction.Normalized();

            Point shifted_start = start + unit_direction * offset;
            Point upper_point = start + new Vector3(0, 0, height);

            points[0] = shifted_start;
            points.Insert(0, upper_point);

            return this;
        }

        public Stroke AddExit(double height = 5, double offset = 2)
        {
            if (points.Count < 2)
            {
                return this;
            }

            int len = points.Count;
            Point last_point = points[len - 1];
            Point pre_last_point = points[len - 2];

            Vector3 direction = (Vector3)pre_last_point - (Vector3)last_point;
            Vector3 unit_direction = direction.Normalized();

            Point shifted_end = last_point + unit_direction * offset;
            Point upper_point = last_point + new Vector3(0, 0, height);

            points[len - 1] = shifted_end;
            points.Insert(len, upper_point);

            return this;
        }
    }
}
