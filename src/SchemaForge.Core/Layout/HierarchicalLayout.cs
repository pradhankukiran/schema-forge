using SchemaForge.Core.Models;

namespace SchemaForge.Core.Layout;

/// <summary>
/// Sugiyama-style layered (hierarchical) graph layout.
/// 1. Break cycles via DFS back-edge removal.
/// 2. Assign layers by longest-path (FK depth) from roots.
/// 3. Order nodes within layers using barycenter heuristic to minimise crossings.
/// 4. Assign final coordinates with configurable spacing.
/// </summary>
public static class HierarchicalLayout
{
    public static Dictionary<string, (double X, double Y)> Compute(
        SchemaDocument schema, double layerSpacing = 200, double nodeSpacing = 300)
    {
        var result = new Dictionary<string, (double X, double Y)>();
        if (schema.Tables.Count == 0)
            return result;

        if (schema.Tables.Count == 1)
        {
            result[schema.Tables[0].Id] = (nodeSpacing, layerSpacing);
            return result;
        }

        var tableIds = schema.Tables.Select(t => t.Id).ToList();
        var n = tableIds.Count;
        var idIndex = new Dictionary<string, int>(n);
        for (var i = 0; i < n; i++)
            idIndex[tableIds[i]] = i;

        // Build directed adjacency (source -> target follows FK direction).
        var adjForward = new List<int>[n];
        var adjReverse = new List<int>[n];
        for (var i = 0; i < n; i++)
        {
            adjForward[i] = [];
            adjReverse[i] = [];
        }

        foreach (var rel in schema.Relationships)
        {
            if (idIndex.TryGetValue(rel.SourceTableId, out var si) &&
                idIndex.TryGetValue(rel.TargetTableId, out var ti) &&
                si != ti)
            {
                adjForward[si].Add(ti);
                adjReverse[ti].Add(si);
            }
        }

        // --- Step 1: Break cycles via DFS ---
        var acyclicForward = BreakCycles(adjForward, n);

        // Rebuild reverse adjacency for the acyclic graph.
        var acyclicReverse = new List<int>[n];
        for (var i = 0; i < n; i++)
            acyclicReverse[i] = [];
        for (var i = 0; i < n; i++)
            foreach (var j in acyclicForward[i])
                acyclicReverse[j].Add(i);

        // --- Step 2: Layer assignment via longest path from roots ---
        var layers = AssignLayers(acyclicForward, acyclicReverse, n);

        // Group nodes by layer.
        var maxLayer = layers.Max();
        var layerNodes = new List<int>[maxLayer + 1];
        for (var l = 0; l <= maxLayer; l++)
            layerNodes[l] = [];
        for (var i = 0; i < n; i++)
            layerNodes[layers[i]].Add(i);

        // --- Step 3: Barycenter ordering to reduce crossings ---
        OrderByBarycenter(layerNodes, acyclicForward, acyclicReverse, layers, maxLayer);

        // --- Step 4: Assign coordinates ---
        // Build position-in-layer lookup for centering.
        for (var l = 0; l <= maxLayer; l++)
        {
            var nodes = layerNodes[l];
            var layerWidth = (nodes.Count - 1) * nodeSpacing;
            var startX = -layerWidth / 2.0;

            for (var p = 0; p < nodes.Count; p++)
            {
                var node = nodes[p];
                var x = startX + p * nodeSpacing;
                var y = l * layerSpacing;
                result[tableIds[node]] = (Math.Round(x, 1), Math.Round(y, 1));
            }
        }

        // Shift so minimum position is at a comfortable origin.
        NormalizePositions(result, nodeSpacing, layerSpacing);

        return result;
    }

    /// <summary>
    /// Remove back-edges found during DFS to make the graph acyclic.
    /// Returns a new adjacency list with back-edges removed.
    /// </summary>
    private static List<int>[] BreakCycles(List<int>[] adj, int n)
    {
        var copy = new List<int>[n];
        for (var i = 0; i < n; i++)
            copy[i] = [.. adj[i]];

        // 0 = unvisited, 1 = in stack, 2 = done
        var state = new int[n];

        void Dfs(int u)
        {
            state[u] = 1;
            copy[u].RemoveAll(v =>
            {
                if (state[v] == 1) return true; // back-edge -> remove
                return false;
            });
            foreach (var v in copy[u])
            {
                if (state[v] == 0)
                    Dfs(v);
            }
            state[u] = 2;
        }

        for (var i = 0; i < n; i++)
        {
            if (state[i] == 0)
                Dfs(i);
        }

        return copy;
    }

    /// <summary>
    /// Assign each node to a layer using longest-path from sources (in-degree 0).
    /// </summary>
    private static int[] AssignLayers(List<int>[] forward, List<int>[] reverse, int n)
    {
        var layer = new int[n];
        var inDegree = new int[n];
        for (var i = 0; i < n; i++)
            inDegree[i] = reverse[i].Count;

        // Kahn's algorithm adapted for longest path.
        var queue = new Queue<int>();
        for (var i = 0; i < n; i++)
        {
            if (inDegree[i] == 0)
            {
                queue.Enqueue(i);
                layer[i] = 0;
            }
        }

        // If no roots (shouldn't happen after cycle breaking, but safety fallback).
        if (queue.Count == 0)
        {
            queue.Enqueue(0);
            layer[0] = 0;
        }

        while (queue.Count > 0)
        {
            var u = queue.Dequeue();
            foreach (var v in forward[u])
            {
                layer[v] = Math.Max(layer[v], layer[u] + 1);
                inDegree[v]--;
                if (inDegree[v] == 0)
                    queue.Enqueue(v);
            }
        }

        return layer;
    }

    /// <summary>
    /// Reorder nodes within each layer using the barycenter heuristic.
    /// Sweeps top-down then bottom-up for several passes.
    /// </summary>
    private static void OrderByBarycenter(
        List<int>[] layerNodes, List<int>[] forward, List<int>[] reverse,
        int[] layers, int maxLayer)
    {
        const int passes = 8;

        // Build a position lookup: node -> position in its layer.
        var pos = new double[layers.Length];
        for (var l = 0; l <= maxLayer; l++)
            for (var p = 0; p < layerNodes[l].Count; p++)
                pos[layerNodes[l][p]] = p;

        for (var pass = 0; pass < passes; pass++)
        {
            // Top-down sweep: order layer l based on neighbors in layer l-1.
            for (var l = 1; l <= maxLayer; l++)
            {
                foreach (var node in layerNodes[l])
                {
                    var neighbors = reverse[node];
                    if (neighbors.Count > 0)
                        pos[node] = neighbors.Average(nb => pos[nb]);
                }

                layerNodes[l].Sort((a, b) => pos[a].CompareTo(pos[b]));
                for (var p = 0; p < layerNodes[l].Count; p++)
                    pos[layerNodes[l][p]] = p;
            }

            // Bottom-up sweep: order layer l based on neighbors in layer l+1.
            for (var l = maxLayer - 1; l >= 0; l--)
            {
                foreach (var node in layerNodes[l])
                {
                    var neighbors = forward[node];
                    if (neighbors.Count > 0)
                        pos[node] = neighbors.Average(nb => pos[nb]);
                }

                layerNodes[l].Sort((a, b) => pos[a].CompareTo(pos[b]));
                for (var p = 0; p < layerNodes[l].Count; p++)
                    pos[layerNodes[l][p]] = p;
            }
        }
    }

    /// <summary>
    /// Shift all positions so the top-left sits at a comfortable margin.
    /// </summary>
    private static void NormalizePositions(
        Dictionary<string, (double X, double Y)> positions, double marginX, double marginY)
    {
        if (positions.Count == 0) return;

        var minX = positions.Values.Min(p => p.X);
        var minY = positions.Values.Min(p => p.Y);
        var offsetX = marginX - minX;
        var offsetY = marginY - minY;

        foreach (var key in positions.Keys.ToList())
        {
            var (x, y) = positions[key];
            positions[key] = (Math.Round(x + offsetX, 1), Math.Round(y + offsetY, 1));
        }
    }
}
