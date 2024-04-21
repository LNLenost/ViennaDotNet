using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViennaDotNet.Common
{
    public static class MathE
    {
        private const float PISingle = MathF.PI;
        private const double PIDouble = Math.PI;

        private const float DegtoRadSingle = PISingle / 180f;
        private const double DegtoRadDouble = PIDouble / 180.0;

        public static float ToRadians(float degrees)
            => degrees * DegtoRadSingle;
        public static double ToRadians(double degrees)
            => degrees * DegtoRadDouble;
    }
}
