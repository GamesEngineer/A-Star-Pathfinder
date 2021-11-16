using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameU
{
    public class SearchStats : MonoBehaviour
    {
        public TextMeshProUGUI totalNodesOpenedText;
        public TextMeshProUGUI maxNodesOpenedAtOnceText;
        public TextMeshProUGUI totalNodesClosedText;
        public TextMeshProUGUI totalNodesReprioritizedText;
        public GridGraph gridGraph;

        void Update()
        {
            totalNodesOpenedText.text = $"Total Opened: {gridGraph.TotalNodesOpened}";
            maxNodesOpenedAtOnceText.text = $"Max Opened At Once: {gridGraph.MaxNodesOpenedAtOnce}";
            totalNodesClosedText.text = $"Total Closed: {gridGraph.TotalNodesClosed}";
            totalNodesReprioritizedText.text = $"Total Reprioritized: {gridGraph.TotalNodesReprioritized}";
        }
    }
}
