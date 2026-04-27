namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree.Handlers
{
    /// <summary>
    /// Thin facade over <see cref="MonsterAI"/> consumed by handlers that need to talk
    /// to the actual monster (sequence playback, death). Phase 6.5 implements this on
    /// MonsterAI. Keeping it as an interface lets the BehaviorTree namespace stay
    /// dependency-free and makes handlers unit-testable with a fake.
    /// </summary>
    public interface IBtMonsterAdapter
    {
        bool IsAlive { get; }

        /// <summary>True if any sequence is currently playing on the monster.</summary>
        bool IsSequencePlaying();

        /// <summary>True if the named sequence is the one currently playing.</summary>
        bool IsSequencePlaying(string sequenceName);

        /// <summary>Start the named sequence. No-op (returns false) if the sequence
        /// doesn't exist, the monster is locked into a higher-priority reaction, or
        /// the same sequence is already running.</summary>
        bool TryStartSequence(string sequenceName);

        /// <summary>Triggers death cleanup (broadcasts, instance-finish packet, …).
        /// Idempotent — calling twice should not double-fire.</summary>
        void HandleDeath();
    }
}
