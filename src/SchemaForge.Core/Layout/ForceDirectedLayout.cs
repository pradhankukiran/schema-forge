using SchemaForge.Core.Models;

namespace SchemaForge.Core.Layout;

/// <summary>
/// Fruchterman-Reingold force-directed graph layout algorithm.
/// All tables repel each other via inverse-square Coulomb forces;
/// connected tables (via relationships) attract with spring forces.
/// Uses simulated annealing over 300 iterations with decreasing temperature.
/// </summary>
public static class ForceDirectedLayout
{
    public static Dictionary<string, (double X, double Y)> Compute(
        SchemaDocument schema, double width = 1200, double height = 800)
    {
        var result = new Dictionary<string, (double X, double Y)>();
        if (schema.Tables.Count == 0)
            return result;

        if (schema.Tables.Count == 1)
        {
            result[schema.Tables[0].Id] = (width / 2, height / 2);
            return result;
        }

        var tableIds = schema.Tables.Select(t => t.Id).ToList();
        var n = tableIds.Count;
        var idIndex = new Dictionary<string, int>(n);
        for (var i = 0; i < n; i++)
            idIndex[tableIds[i]] = i;

        // Build adjacency set from relationships (ignore edges referencing unknown tables).
        var edges = new List<(int Source, int Target)>();
        foreach (var rel in schema.Relationships)
        {
            if (idIndex.TryGetValue(rel.SourceTableId, out var si) &&
                idIndex.TryGetValue(rel.TargetTableId, out var ti) &&
                si != ti)
            {
                edges.Add((si, ti));
            }
        }

        // Fruchterman-Reingold constants.
        var area = width * height;
        var k = Math.Sqrt(area / n); // ideal edge length
        const int iterations = 300;
        var tempMax = Math.Min(width, height) / 2;

        // Initialise positions in a circle (deterministic, avoids overlaps).
        var posX = new double[n];
        var posY = new double[n];
        var cx = width / 2;
        var cy = height / 2;
        var radius = Math.Min(width, height) * 0.35;
        for (var i = 0; i < n; i++)
        {
            var angle = 2.0 * Math.PI * i / n;
            posX[i] = cx + radius * Math.Cos(angle);
            posY[i] = cy + radius * Math.Sin(angle);
        }

        var dispX = new double[n];
        var dispY = new double[n];

        for (var iter = 0; iter < iterations; iter++)
        {
            // Temperature decreases linearly (simulated annealing).
            var temp = tempMax * (1.0 - (double)iter / iterations);

            Array.Clear(dispX);
            Array.Clear(dispY);

            // Repulsive forces: all pairs.
            for (var i = 0; i < n; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    var dx = posX[i] - posX[j];
                    var dy = posY[i] - posY[j];
                    var dist = Math.Sqrt(dx * dx + dy * dy);
                    if (dist < 0.01) dist = 0.01; // avoid division by zero

                    var repulsion = (k * k) / dist;
                    var fx = (dx / dist) * repulsion;
                    var fy = (dy / dist) * repulsion;

                    dispX[i] += fx;
                    dispY[i] += fy;
                    dispX[j] -= fx;
                    dispY[j] -= fy;
                }
            }

            // Attractive forces: connected pairs.
            foreach (var (si, ti) in edges)
            {
                var dx = posX[si] - posX[ti];
                var dy = posY[si] - posY[ti];
                var dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist < 0.01) dist = 0.01;

                var attraction = (dist * dist) / k;
                var fx = (dx / dist) * attraction;
                var fy = (dy / dist) * attraction;

                dispX[si] -= fx;
                dispY[si] -= fy;
                dispX[ti] += fx;
                dispY[ti] += fy;
            }

            // Apply displacements clamped by temperature, then clamp to canvas.
            var margin = 40.0;
            for (var i = 0; i < n; i++)
            {
                var disp = Math.Sqrt(dispX[i] * dispX[i] + dispY[i] * dispY[i]);
                if (disp < 0.01) continue;

                var scale = Math.Min(disp, temp) / disp;
                posX[i] += dispX[i] * scale;
                posY[i] += dispY[i] * scale;

                // Keep within canvas bounds.
                posX[i] = Math.Clamp(posX[i], margin, width - margin);
                posY[i] = Math.Clamp(posY[i], margin, height - margin);
            }
        }

        for (var i = 0; i < n; i++)
            result[tableIds[i]] = (Math.Round(posX[i], 1), Math.Round(posY[i], 1));

        return result;
    }
}
