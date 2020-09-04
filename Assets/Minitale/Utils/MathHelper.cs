using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.Utils
{
    public class MathHelper : MonoBehaviour
    {
        public static int Clamp(int value, int min, int max)
        {
            int result = value % max;
            if (value % max < min) result = max - 1;
            else if (value % max >= max) result = min;
            return result;
        }
    }
}