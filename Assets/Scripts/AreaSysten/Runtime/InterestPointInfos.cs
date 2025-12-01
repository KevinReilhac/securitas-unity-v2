using System.Collections.Generic;
using UnityEngine;

namespace Spop.AreaSystem
{
    [System.Serializable]
    public class InterestPointInfos
    {
        public enum InfoTextLineType
        {
            Text,
            DotLine,
            Title,
        }

        [System.Serializable]
        public class Info
        {
            public InfoTextLineType type;
            [TextArea] public string text;
        }
        public Sprite image;
        public List<Info> infos;
    }
}
