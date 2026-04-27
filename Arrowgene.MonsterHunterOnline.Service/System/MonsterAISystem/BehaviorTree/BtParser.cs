using System;
using System.IO;
using System.Xml.Linq;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree
{
    /// <summary>
    /// Parses a CryEngine 3 BT XML document into a <see cref="BtNode"/> tree.
    /// Format envelope: <c>&lt;Behavior Ver="1.01"&gt;&lt;Node Type="Root" …&gt;…&lt;/Node&gt;&lt;/Behavior&gt;</c>.
    /// Children of every node sit inside a <c>&lt;Connector Identifier="GenericChildren"&gt;</c>.
    /// </summary>
    public static class BtParser
    {
        public static BtNode Parse(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentException("xml is empty", nameof(xml));
            XDocument doc = XDocument.Parse(xml);
            return ParseDocument(doc);
        }

        public static BtNode ParseFile(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException(path);
            XDocument doc = XDocument.Load(path);
            return ParseDocument(doc);
        }

        private static BtNode ParseDocument(XDocument doc)
        {
            XElement behavior = doc.Root;
            if (behavior == null || behavior.Name.LocalName != "Behavior")
                throw new InvalidDataException($"Expected <Behavior> root, got <{doc.Root?.Name}>");

            XElement rootNode = behavior.Element("Node");
            if (rootNode == null)
                throw new InvalidDataException("<Behavior> has no <Node> child");

            return ParseNode(rootNode);
        }

        private static BtNode ParseNode(XElement el)
        {
            string type = el.Attribute("Type")?.Value;
            BtNode node = type switch
            {
                "Root" => new BtRoot(),
                "Selector" => new BtSelector { SelectorType = el.Attribute("SelectorType")?.Value },
                "Sequence" => new BtSequence(),
                "Condition" => new BtCondition { Operation = el.Attribute("Operation")?.Value },
                "Action" => new BtAction { Operation = el.Attribute("Operation")?.Value },
                "Filter" => new BtFilter
                {
                    FilterType = el.Attribute("Filter_Type")?.Value,
                    FilterCategory = el.Attribute("FilterType")?.Value
                },
                "Reference" => new BtReference { Reference = el.Attribute("Reference")?.Value },
                null => throw new InvalidDataException($"<Node> missing Type attribute (id={el.Attribute("Node_id")?.Value})"),
                _ => throw new NotSupportedException($"Unknown BT node Type='{type}' (id={el.Attribute("Node_id")?.Value})")
            };

            node.Name = el.Attribute("Name")?.Value;
            if (int.TryParse(el.Attribute("Node_id")?.Value, out int id)) node.NodeId = id;

            // Snapshot every other attribute as raw strings — Operation-specific data lives here.
            foreach (XAttribute attr in el.Attributes())
            {
                string n = attr.Name.LocalName;
                if (n is "Name" or "Type" or "Node_id") continue;
                node.Attributes[n] = attr.Value;
            }

            // Children are wrapped in <Connector Identifier="GenericChildren">. There may be
            // multiple connectors on richer node types (Goal/Parallel) but em001's master only
            // uses GenericChildren — walk every connector child of type Node to stay generic.
            foreach (XElement connector in el.Elements("Connector"))
            {
                foreach (XElement child in connector.Elements("Node"))
                {
                    node.Children.Add(ParseNode(child));
                }
            }

            return node;
        }
    }
}
