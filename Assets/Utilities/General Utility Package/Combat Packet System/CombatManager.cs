using System.Collections.Generic;
using UnityEngine;

namespace GeneralUtility
{
    namespace CombatSystem
    {
        public interface IEffect
        {//Used to trigger weapon effects and events
            abstract public bool TriggerEffectByRaycast(CombatPacket p, RaycastHit target);
            //These are poor design, as interfaces are SUPPOSED to implement all members, not just some.
            abstract public bool TriggerEffect(CombatPacket p); //EVERYTHING ELSE DOES NOT
            abstract public void OnEffect(int damage);
            abstract public void OnKill();
        }

        public interface IDamageable
        {
            public bool ApplyDamage(CombatPacket combatPacket);
            public int MaxHealth { get; }
        }

        public class CombatPacket
        {
            public enum ModifierType { PreMitigationMultiplier, PreMitigationFlat, PostMitigationMultiplier, PostMitigationFlat };

            private IDamageable target;
            public IDamageable Target => target;

            private Collider hitCollider;
            public Collider HitCollider => hitCollider;

            private Collision collisionInfo;
            public Collision CollisionInfo => collisionInfo;

            private RaycastHit raycastInfo;
            public RaycastHit RaycastInfo => raycastInfo;

            private int damage;
            public int Damage => damage;
            private IEffect affector;
            public IEffect Affector => affector;

            //private List<PersistentStatusEffect> activeModifiers = new();
            //public List<PersistentStatusEffect> ActiveModifiers => activeModifiers;

            private List<Component> mons = new();

            private string loggedHandoffs = string.Empty;
            private const string SPACER = "==========\n";

            public CombatPacket() { }

            public CombatPacket(CombatPacket basePacket)
            {
                target = basePacket.Target;
                hitCollider = basePacket.HitCollider;
                collisionInfo = basePacket.CollisionInfo;
                raycastInfo = basePacket.RaycastInfo;
                damage = basePacket.Damage;
                affector = basePacket.Affector;
                loggedHandoffs = basePacket.loggedHandoffs;
            }

            public void SetTarget(IDamageable t, Component m)
            {
                target = t;
                LogHandoff(m);
            }

            public void SetHitCollider(Collider c, Component m)
            {
                hitCollider = c;
                LogHandoff(m);
            }

            public void SetHitCollider(Collision collision, Component m)
            {
                hitCollider = collision.collider;
                collisionInfo = collision;
                LogHandoff(m);
            }

            public void SetHitCollider(RaycastHit hit, Component m)
            {
                hitCollider = hit.collider;
                raycastInfo = hit;
                LogHandoff(m);
            }

            [Tooltip("Apply in IEffect, as in like a WeaponEffect or EnemyEffect, etc.")]
            public void SetDamage(int d, Component m)
            {
                damage = d;
                LogHandoff(m);
            }
            [Tooltip("Apply in IEffect, as in like a WeaponEffect or EnemyEffect, etc.")]
            public void SetAffector(IEffect e, Component m)
            {
                affector = e;
                LogHandoff(m);
            }

            //[Tooltip("Add along the way.")]
            //public void AddToActiveModifiers(PersistentStatusEffect e, Component m)
            //{
            //    if (activeModifiers.Count <= 0) loggedHandoffs += $"Active Modifiers\n{SPACER}";
            //    activeModifiers.Add(e);
            //    LogHandoff(m);

            //    loggedHandoffs += $"Parent - {m.GetType()} :: Modifier - {e.GetType()}\n";

            //    Editor_Utility.Ping(m.gameObject);
            //}

            public void LogHandoff(Component m)
            {
                mons.Add(m);
            }

            public void PrintHandoffLog()
            {
                if (loggedHandoffs != string.Empty) loggedHandoffs += SPACER;
                loggedHandoffs += "Handled by:\n";
                Component previousM = null;
                foreach (var m in mons)
                {
                    if (m == previousM) continue;
                    loggedHandoffs += $"- {m.GetType()}\n";
                    previousM = m;
                }
                loggedHandoffs += SPACER;
                loggedHandoffs += $"Final Damage: {CombatManager.DamageCalculator(this)}";
                Debug.Log(loggedHandoffs);
            }

            public CombatPacket ShallowCopyPacket()
            {
                return (CombatPacket)MemberwiseClone();
            }
            //public List<PersistentStatusEffect> CopyModifs()
            //{
            //    List<PersistentStatusEffect> newList = new();
            //    foreach (var m in ActiveModifiers) newList.Add(m);

            //    return newList;
            //}
        }

        public class CombatManager : MonoBehaviour
        {//Specifically intended to extract combat calculations into a single script rather than scattering them
            private CombatManager instance;

            //static private List<PersistentStatusEffect> currentModifiers = new();

            private void Awake()
            {
                if (instance != null) Destroy(this);
                else instance = this;
            }

            static public int DamageCalculator(CombatPacket combatInstance)
            {
                int newDamage = combatInstance.Damage;
                //var activeModifs = combatInstance.ActiveModifiers;
                var hitCol = combatInstance.HitCollider;
                //foreach (var modif in combatInstance.activeModifiers)
                //    currentModifiers.Add(modif);

                int lastDamage = newDamage;

                //foreach (var e in activeModifs)
                //{
                //    if (e is not BUFF_PreMitigationMultiplier preMitMult) continue;
                //    newDamage = preMitMult.ApplyStatModification(newDamage);
                //    if (lastDamage != newDamage)
                //    {
                //        Debug.Log($"{lastDamage} != {newDamage}");
                //        lastDamage = newDamage;
                //    }
                //}

                //foreach (var e in activeModifs)
                //{
                //    if (e is not EE_PreMitigationMultiplier preMitMult) continue;
                //    newDamage = preMitMult.ApplyDamageModification(newDamage, hitCol);
                //    if (lastDamage != newDamage)
                //    {
                //        //Debug.Log($"{lastDamage} != {newDamage} :: {preMitMult.name}");
                //        lastDamage = newDamage;
                //    }
                //}

                //foreach (var e in activeModifs)
                //{
                //    if (e is not BUFF_PreMitigationFlat preMitFlat) continue;
                //    newDamage = preMitFlat.ApplyStatModification(newDamage);
                //    if (lastDamage != newDamage)
                //    {
                //        //Debug.Log($"{lastDamage} != {newDamage}");
                //        lastDamage = newDamage;
                //    }
                //}

                //foreach (var e in activeModifs)
                //{
                //    if (e is not EE_PreMitigationFlat preMitFlat) continue;
                //    newDamage = preMitFlat.ApplyDamageModification(newDamage, hitCol);
                //    if (lastDamage != newDamage)
                //    {
                //        //Debug.Log($"{lastDamage} != {newDamage}");
                //        lastDamage = newDamage;
                //    }
                //}

                //foreach (var e in activeModifs)
                //{
                //    if (e is not BUFF_PostMitigationMultiplier postMitMult) continue;
                //    newDamage = postMitMult.ApplyStatModification(newDamage);
                //    if (lastDamage != newDamage)
                //    {
                //        Debug.Log($"{lastDamage} != {newDamage}");
                //        lastDamage = newDamage;
                //    }
                //}

                //foreach (var e in activeModifs)
                //{
                //    if (e is not EE_PostMitigationMultiplier postMitMult) continue;
                //    newDamage = postMitMult.ApplyDamageModification(newDamage, hitCol);
                //    if (lastDamage != newDamage)
                //    {
                //        //Debug.Log($"{lastDamage} != {newDamage}");
                //        lastDamage = newDamage;
                //    }
                //}

                //foreach (var e in activeModifs)
                //{
                //    if (e is not BUFF_PostMitigationFlat postMitFlat) continue;
                //    newDamage = postMitFlat.ApplyStatModification(newDamage);
                //    if (lastDamage != newDamage)
                //    {
                //        //Debug.Log($"{lastDamage} != {newDamage}");
                //        lastDamage = newDamage;
                //    }
                //}

                //foreach (var e in activeModifs)
                //{
                //    if (e is not EE_PostMitigationFlat postMitFlat) continue;
                //    newDamage = postMitFlat.ApplyDamageModification(newDamage, hitCol);
                //    if (lastDamage != newDamage)
                //    {
                //        //Debug.Log($"{lastDamage} != {newDamage}");
                //        lastDamage = newDamage;
                //    }
                //}
                return newDamage;
            }
        }
    }
}