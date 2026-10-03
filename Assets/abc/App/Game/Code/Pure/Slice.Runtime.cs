using System;
using System.Collections.Generic;
using R3;

using Vector2 =
#if UNITY_5_3_OR_NEWER
    UnityEngine.Vector2;
#else
    System.Numerics.Vector2;
#endif

namespace abc.App.Game.Code.Pure
{
    public partial class Slice
    {
        public class Runtime
        {
            protected readonly Gameplay _context;
            public Slice Instance { get; protected set; }

            readonly Dictionary<Cell, float> _nextHitTime = new();
            private IBudget Budget => _context.ManaRuntime;
            Vector2 _prev;
            bool _active;
            
            public Runtime(Gameplay context,
                Func<Vector2, Vector2, Cell[]> collectCellsOnMotion)
            {
                if (context.SliceRuntime != null)
                    throw new  InvalidOperationException(
                        "Creating second Slice runtime in one context");

                context.SliceRuntime = this;
                _context = context;
                this.CollectCellsOnMotion = collectCellsOnMotion;
                Instance = context.Slice;
                context.Tick.Subscribe(Tick);
            }

            protected void Tick(float delta)
            {
                if (Budget == null) return;
                
                if (_context.PointerJustPressed && Budget.CanStart)
                {
                    _active = true;
                    _prev = _context.PointerPosition;
                    Instance.SwipeStarted?.Invoke();
                    Sweep(_prev, _prev);
                }
                else if (_active && _context.PointerPressed)
                {
                    Advance(_context.PointerPosition);
                }
                else if (_active && _context.PointerJustReleased)
                {
                    Advance(_context.PointerPosition);
                    End();
                }
            }
            
            protected void Advance(Vector2 cur)
            {
                var d = cur - _prev;


                float len =
#if UNITY_5_3_OR_NEWER
                    d.magnitude;
#else
                    d.Length();
#endif
                
                if (len < Instance.MinSegment) return;

                float granted = Budget.Request(len);
                if (granted < len)                                  // out of mana: truncate
                    cur = _prev + d * (granted / len);

                if (granted > 0f) Sweep(_prev, cur);
                _prev = cur;

                if (granted < len) End();
            }
            
            void End()
            {
                if (!_active) return;
                _active = false;                                    // must re-press to start again
                Instance.SwipeEnded?.Invoke();
            }

            private void Sweep(Vector2 cur, Vector2 prev)
            {
                var targets = CollectCellsOnMotion(cur, prev);
                foreach (var target in targets)
                    TryHit(target);
            }

            protected Func<Vector2, Vector2, Cell[]> CollectCellsOnMotion; 
            
            protected void TryHit(Cell cell)
            {
                var now = _context.Time;
                if (_nextHitTime.TryGetValue(cell, out var next) 
                    && now < next)
                    return;
                _nextHitTime[cell] = now + Instance.HitCooldown;
                
                Interaction.SliceHitsCell.Raise(Instance, cell);
                Instance.CellSliced?.Invoke(cell);
            }
        }
    }
}