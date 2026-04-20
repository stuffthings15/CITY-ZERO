using System;
using Xunit;
using FluentAssertions;
using CityZero.AI;

namespace CityZero.Tests
{
    public class BehaviorTreeTests
    {
        // ── BTCondition ──────────────────────────────────────────────────────────

        [Fact]
        public void Condition_True_ReturnsSuccess()
        {
            var board = new AIBlackboard();
            var node  = new BTCondition(_ => true, "always_true");
            node.Tick(board).Should().Be(BTStatus.Success);
        }

        [Fact]
        public void Condition_False_ReturnsFailure()
        {
            var board = new AIBlackboard();
            var node  = new BTCondition(_ => false, "always_false");
            node.Tick(board).Should().Be(BTStatus.Failure);
        }

        // ── BTAction ─────────────────────────────────────────────────────────────

        [Fact]
        public void Action_ReturnsWhateverActionReturns()
        {
            var board = new AIBlackboard();
            new BTAction(_ => BTStatus.Running).Tick(board).Should().Be(BTStatus.Running);
            new BTAction(_ => BTStatus.Success).Tick(board).Should().Be(BTStatus.Success);
            new BTAction(_ => BTStatus.Failure).Tick(board).Should().Be(BTStatus.Failure);
        }

        // ── BTSequence (AND) ──────────────────────────────────────────────────────

        [Fact]
        public void Sequence_AllSuccess_ReturnsSuccess()
        {
            var board = new AIBlackboard();
            var seq = new BTSequence(
                new BTCondition(_ => true),
                new BTCondition(_ => true),
                new BTCondition(_ => true)
            );
            seq.Tick(board).Should().Be(BTStatus.Success);
        }

        [Fact]
        public void Sequence_AnyFailure_ReturnsFailure()
        {
            var board = new AIBlackboard();
            var seq = new BTSequence(
                new BTCondition(_ => true),
                new BTCondition(_ => false),  // this fails
                new BTCondition(_ => true)
            );
            seq.Tick(board).Should().Be(BTStatus.Failure);
        }

        [Fact]
        public void Sequence_ShortCircuitsOnFirstFailure()
        {
            var board = new AIBlackboard();
            int callCount = 0;
            var seq = new BTSequence(
                new BTCondition(_ => false),
                new BTAction(_ => { callCount++; return BTStatus.Success; })
            );
            seq.Tick(board);
            callCount.Should().Be(0, "second child should never be reached after first fails");
        }

        // ── BTSelector (OR) ───────────────────────────────────────────────────────

        [Fact]
        public void Selector_FirstSuccess_ReturnsSuccess()
        {
            var board = new AIBlackboard();
            var sel = new BTSelector(
                new BTCondition(_ => true),
                new BTCondition(_ => true)
            );
            sel.Tick(board).Should().Be(BTStatus.Success);
        }

        [Fact]
        public void Selector_AllFailure_ReturnsFailure()
        {
            var board = new AIBlackboard();
            var sel = new BTSelector(
                new BTCondition(_ => false),
                new BTCondition(_ => false)
            );
            sel.Tick(board).Should().Be(BTStatus.Failure);
        }

        [Fact]
        public void Selector_ShortCircuitsOnFirstSuccess()
        {
            var board = new AIBlackboard();
            int callCount = 0;
            var sel = new BTSelector(
                new BTCondition(_ => true),
                new BTAction(_ => { callCount++; return BTStatus.Success; })
            );
            sel.Tick(board);
            callCount.Should().Be(0, "second child should never be reached after first succeeds");
        }

        // ── BTInverter ────────────────────────────────────────────────────────────

        [Fact]
        public void Inverter_InvertsSuccessToFailure()
        {
            var board = new AIBlackboard();
            var inv = new BTInverter(new BTCondition(_ => true));
            inv.Tick(board).Should().Be(BTStatus.Failure);
        }

        [Fact]
        public void Inverter_InvertsFailureToSuccess()
        {
            var board = new AIBlackboard();
            var inv = new BTInverter(new BTCondition(_ => false));
            inv.Tick(board).Should().Be(BTStatus.Success);
        }

        [Fact]
        public void Inverter_PassesThroughRunning()
        {
            var board = new AIBlackboard();
            var inv = new BTInverter(new BTAction(_ => BTStatus.Running));
            inv.Tick(board).Should().Be(BTStatus.Running);
        }

        // ── AIBlackboard ─────────────────────────────────────────────────────────

        [Fact]
        public void Blackboard_SetAndGet_RoundTrip()
        {
            var board = new AIBlackboard();
            board.Set(BB.AlertLevel, 3);
            board.Get<int>(BB.AlertLevel).Should().Be(3);
        }

        [Fact]
        public void Blackboard_GetMissingKey_ReturnsDefault()
        {
            var board = new AIBlackboard();
            board.Get<bool>("nonexistent").Should().BeFalse();
            board.Get<int>("nonexistent").Should().Be(0);
        }

        [Fact]
        public void Blackboard_Has_CorrectlyDetectsPresence()
        {
            var board = new AIBlackboard();
            board.Has("key").Should().BeFalse();
            board.Set("key", 42);
            board.Has("key").Should().BeTrue();
        }

        [Fact]
        public void Blackboard_Remove_DeletesKey()
        {
            var board = new AIBlackboard();
            board.Set("key", 1);
            board.Remove("key");
            board.Has("key").Should().BeFalse();
        }

        [Fact]
        public void Blackboard_Clear_RemovesAllKeys()
        {
            var board = new AIBlackboard();
            board.Set("a", 1); board.Set("b", 2); board.Set("c", 3);
            board.Clear();
            board.Has("a").Should().BeFalse();
            board.Has("b").Should().BeFalse();
            board.Has("c").Should().BeFalse();
        }

        // ── Composite tree ────────────────────────────────────────────────────────

        [Fact]
        public void CompositeTree_AttackBehavior_ExecutesCorrectBranch()
        {
            // Simulates: SELECT( SEQUENCE(IsUnderAttack, TakeCover), PatrolRoute )
            var board = new AIBlackboard();
            board.Set(BB.IsUnderAttack, true);

            bool tookCover  = false;
            bool didPatrol  = false;

            var tree = new BTSelector(
                new BTSequence(
                    new BTCondition(b => b.Get<bool>(BB.IsUnderAttack), "IsUnderAttack"),
                    new BTAction(b => { tookCover = true; return BTStatus.Success; }, "TakeCover")
                ),
                new BTAction(b => { didPatrol = true; return BTStatus.Success; }, "Patrol")
            );

            var result = tree.Tick(board);

            result.Should().Be(BTStatus.Success);
            tookCover.Should().BeTrue("attack branch should execute when under attack");
            didPatrol.Should().BeFalse("patrol should be skipped when attack branch succeeds");
        }

        [Fact]
        public void CompositeTree_NotUnderAttack_PatrolsInstead()
        {
            var board = new AIBlackboard();
            board.Set(BB.IsUnderAttack, false);

            bool tookCover = false;
            bool didPatrol = false;

            var tree = new BTSelector(
                new BTSequence(
                    new BTCondition(b => b.Get<bool>(BB.IsUnderAttack)),
                    new BTAction(b => { tookCover = true; return BTStatus.Success; })
                ),
                new BTAction(b => { didPatrol = true; return BTStatus.Success; })
            );

            tree.Tick(board);

            tookCover.Should().BeFalse();
            didPatrol.Should().BeTrue("patrol is fallback when not under attack");
        }
    }
}
