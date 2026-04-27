using System;
using System.Collections.Generic;
using System.IO;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree
{
    /// <summary>
    /// Loads BT XML files from disk and caches the parsed trees so the same sub-tree
    /// referenced by N parent files is only parsed once.
    ///
    /// Reference path forms accepted (CryEngine 3 style, observed on em001):
    /// <list type="bullet">
    /// <item><c>.\File.xml</c> or <c>./File.xml</c> — same folder as the parent file</item>
    /// <item><c>File.xml</c> — same folder, no leading dot</item>
    /// <item><c>..\Sibling\File.xml</c> — parent-folder traversal</item>
    /// <item><c>File.Root_node.NodeName</c> — file (without .xml) + dotted sub-node selector</item>
    /// </list>
    ///
    /// The decrypted game files live alongside encrypted ones with a <c>_decrypted.xml</c>
    /// suffix (e.g. <c>em001.xml_decrypted.xml</c>); when a Reference points at <c>X.xml</c>
    /// we transparently fall back to <c>X.xml_decrypted.xml</c> if the plain name is missing.
    /// </summary>
    public sealed class BtTreeLoader
    {
        private readonly string _rootDir;
        private readonly Dictionary<string, BtTree> _cache = new(StringComparer.OrdinalIgnoreCase);

        public BtTreeLoader(string rootDir)
        {
            if (string.IsNullOrEmpty(rootDir)) throw new ArgumentException("rootDir is empty", nameof(rootDir));
            _rootDir = Path.GetFullPath(rootDir);
        }

        public string RootDir => _rootDir;

        /// <summary>
        /// Loads a tree by absolute or root-relative path. Returns the cached instance
        /// on subsequent calls.
        /// </summary>
        public BtTree Load(string path)
        {
            string full = ResolveAbsolute(path);
            if (_cache.TryGetValue(full, out BtTree cached)) return cached;

            BtNode root = BtParser.ParseFile(full);
            BtTree tree = new BtTree(root, full);
            IndexNamedNodes(tree.Root, tree.NamedNodes);
            _cache[full] = tree;
            return tree;
        }

        /// <summary>
        /// Resolves a <c>Reference</c> string emitted on a <see cref="BtReference"/> node,
        /// using the parent tree's source path as the anchor for relative lookups.
        /// Returns null if the file cannot be located on disk.
        ///
        /// <paramref name="subNodeSelector"/> receives the trailing dotted selector
        /// (e.g. "Root_node.Sleep") if the reference includes one; resolution of that
        /// selector against <see cref="BtTree.NamedNodes"/> is the runtime's job.
        /// </summary>
        public BtTree Resolve(string reference, BtTree parent, out string subNodeSelector)
        {
            subNodeSelector = null;
            if (string.IsNullOrWhiteSpace(reference)) return null;

            string anchorDir = parent != null
                ? Path.GetDirectoryName(parent.SourcePath)
                : _rootDir;

            string filePart = reference;

            // Dotted selector form: "File.Root_node.NodeName" — file portion has no .xml.
            // We split on the first '.' that is NOT part of an extension or a relative prefix.
            if (!HasXmlExtension(reference))
            {
                int firstDot = FindSelectorSplit(reference);
                if (firstDot >= 0)
                {
                    filePart = reference.Substring(0, firstDot);
                    subNodeSelector = reference.Substring(firstDot + 1);
                }
                filePart += ".xml";
            }

            string candidate = Path.GetFullPath(Path.Combine(anchorDir ?? _rootDir, NormalizeSeparators(filePart)));
            string resolved = ProbeOnDisk(candidate);
            return resolved != null ? Load(resolved) : null;
        }

        private string ResolveAbsolute(string path)
        {
            if (Path.IsPathRooted(path)) return Path.GetFullPath(path);
            return Path.GetFullPath(Path.Combine(_rootDir, NormalizeSeparators(path)));
        }

        private static string NormalizeSeparators(string p)
        {
            return p.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        }

        private static bool HasXmlExtension(string s)
        {
            return s.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
                || s.EndsWith(".xml_decrypted.xml", StringComparison.OrdinalIgnoreCase);
        }

        // Reference example: ".\Em001Idle.xml" or "Em001Sleep.Root_node.Sleep".
        // The selector split is the first '.' that doesn't open a "./" or "../" prefix
        // and isn't followed by "xml".
        private static int FindSelectorSplit(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != '.') continue;
                // Skip ".\" and "../" relative-path dots.
                if (i + 1 < s.Length && (s[i + 1] == '\\' || s[i + 1] == '/' || s[i + 1] == '.')) continue;
                if (i == 0) continue; // leading dot already covered above
                return i;
            }
            return -1;
        }

        // CryEngine references point at "X.xml". On disk we may have:
        //   X.xml                  (encrypted blob — unreadable)
        //   X.xml_decrypted.xml    (the decrypted twin we actually want)
        // So we ALWAYS prefer the _decrypted.xml twin if it exists, even when the plain
        // file is also present. Falls back to a case-insensitive sibling lookup.
        private static string ProbeOnDisk(string fullPath)
        {
            string twin = fullPath + "_decrypted.xml";
            if (File.Exists(twin)) return twin;
            if (File.Exists(fullPath)) return fullPath;

            string dir = Path.GetDirectoryName(fullPath);
            string name = Path.GetFileName(fullPath);
            if (dir == null || !Directory.Exists(dir)) return null;

            string ciTwin = null;
            string ciPlain = null;
            string twinName = name + "_decrypted.xml";
            foreach (string entry in Directory.EnumerateFiles(dir))
            {
                string entryName = Path.GetFileName(entry);
                if (ciTwin == null && string.Equals(entryName, twinName, StringComparison.OrdinalIgnoreCase))
                    ciTwin = entry;
                else if (ciPlain == null && string.Equals(entryName, name, StringComparison.OrdinalIgnoreCase))
                    ciPlain = entry;
            }
            return ciTwin ?? ciPlain;
        }

        private static void IndexNamedNodes(BtNode node, Dictionary<string, BtNode> dict)
        {
            if (!string.IsNullOrEmpty(node.Name) && !dict.ContainsKey(node.Name))
                dict[node.Name] = node;
            foreach (BtNode child in node.Children)
                IndexNamedNodes(child, dict);
        }
    }
}
