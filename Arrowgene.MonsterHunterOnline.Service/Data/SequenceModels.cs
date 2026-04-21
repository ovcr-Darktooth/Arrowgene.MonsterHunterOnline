using System;
using System.Collections.Generic;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    public class SequenceSet
    {
        public string SourceFile { get; set; }
        public string RefName { get; set; }
        public Dictionary<string, SequenceData> Sequences { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public class SequenceData
    {
        public string Name { get; set; }
        public float TimeRange { get; set; }
        public bool Loop { get; set; }
        public float PlaySpeed { get; set; } = 1f;
        public int SkillID { get; set; }
        public int Layer { get; set; }
        public float CatchUpDist { get; set; }
        public bool DisablePhy { get; set; }
        public bool DisableColWithWall { get; set; }
        public bool EnableMoveSplineScale { get; set; }

        public List<string> NextSequences { get; } = new();
        public List<PhysicEventData> PhysicEvents { get; } = new();
        public List<HitColWindow> HitColWindows { get; } = new();
        public List<TriggerEventData> TriggerEvents { get; } = new();
        public TransformTrack Position { get; set; } = new();
        public TransformTrack Rotation { get; set; } = new();
    }

    public class PhysicEventData
    {
        public string Name { get; set; }
        public string EventName { get; set; }
        public string Bone { get; set; }
        public float Time { get; set; }
        public string EventType { get; set; }
        public string Firemode { get; set; }
        public int AttackData { get; set; }
        public bool Enable { get; set; }
        public int AnimOrder { get; set; }
        public int SlashDir { get; set; }
        public float ShakeTime { get; set; }
        public float ShakePeriod { get; set; }
        public float ShakeStrength { get; set; }
        public float ShakeAttenuation { get; set; }
        public float ShakeMaxDistance { get; set; }

        public bool IsAttackStart => Name != null && Name.EndsWith("Start", StringComparison.OrdinalIgnoreCase);
        public bool IsAttackEnd => Name != null && Name.EndsWith("End", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Paired HitCol Start/End forming an active damage window.
    /// Unpaired HitCols become a zero-duration window at StartTime.
    /// </summary>
    public class HitColWindow
    {
        public string Label { get; set; }
        public string Firemode { get; set; }
        public string EventType { get; set; }
        public string Bone { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public int AttackData { get; set; }
        public bool Enable { get; set; }

        public float Duration => MathF.Max(0f, EndTime - StartTime);
        public bool IsInstant => EndTime <= StartTime;
    }

    public class TriggerEventData
    {
        public string Name { get; set; }
        public string EventName { get; set; }
        public string Bone { get; set; }
        public string Params { get; set; }
        public float Time { get; set; }
    }

    public class TransformTrack
    {
        public FloatTrack X { get; set; } = new();
        public FloatTrack Y { get; set; } = new();
        public FloatTrack Z { get; set; } = new();

        public bool HasAny => X.Keys.Count > 0 || Y.Keys.Count > 0 || Z.Keys.Count > 0;

        public (float x, float y, float z) Sample(float time) =>
            (X.Evaluate(time), Y.Evaluate(time), Z.Evaluate(time));
    }

    public class FloatTrack
    {
        public List<TrackKey> Keys { get; } = new();

        public float Evaluate(float time)
        {
            if (Keys.Count == 0) return 0f;
            if (Keys.Count == 1) return Keys[0].Value;

            if (time <= Keys[0].Time) return Keys[0].Value;
            if (time >= Keys[^1].Time) return Keys[^1].Value;

            for (int i = 0; i < Keys.Count - 1; i++)
            {
                if (time >= Keys[i].Time && time <= Keys[i + 1].Time)
                {
                    var k1 = Keys[i];
                    var k2 = Keys[i + 1];

                    float dt = k2.Time - k1.Time;
                    if (dt <= 0.0001f) return k2.Value;

                    float t = (time - k1.Time) / dt;

                    float p0 = k1.Value;
                    float p1 = k2.Value;
                    float m0 = k1.Dd * dt;
                    float m1 = k2.Ds * dt;

                    float t2 = t * t;
                    float t3 = t2 * t;

                    return (2 * t3 - 3 * t2 + 1) * p0 +
                           (t3 - 2 * t2 + t) * m0 +
                           (-2 * t3 + 3 * t2) * p1 +
                           (t3 - t2) * m1;
                }
            }

            return Keys[^1].Value;
        }
    }

    public class TrackKey
    {
        public float Time { get; set; }
        public float Value { get; set; }
        public float Ds { get; set; }
        public float Dd { get; set; }
    }
}
