using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralUtility
{
    namespace CombatSystem
    {
        public interface IEffect
        {//Used to trigger weapon effects and events
            abstract public float GetEffectValue();
            abstract public bool TriggerEffect(CombatPacket p);
            abstract public void OnEffect(int damage);          //Primarily for related events, such as objectivs
            abstract public void OnKill();
        }

        public interface IPersistentEffect : IEffect
        {
            abstract public float GetTickRate();
            abstract public float GetTimeBeforeExpiration();
            abstract public int GetMaxNumTicks();
            abstract public int GetMaxStacks();
            abstract public bool GetRefreshable();
            abstract public bool GetStackable();
        }

        public interface IDamageable
        {
            abstract public int MaxHealth { get; }
            abstract System.Action OnDamage { set; get; }
            abstract public bool ApplyDamage(CombatPacket combatPacket);
        }

        abstract public class Damageable : MonoBehaviour, IDamageable
        {
            abstract public int MaxHealth { get; }
            abstract public Action OnDamage { get; set; }
            abstract public bool ApplyDamage(CombatPacket combatPacket);
        }

        abstract public class Damageable<T> : Damageable where T : MonoBehaviour { }

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
            private bool ignoreLocalResistance;
            public bool IgnoreLocalResistance => ignoreLocalResistance;
            private bool ignoreMainResistance;
            public bool IgnoreMainResistance => ignoreMainResistance;
            private float resistance;
            public float Resistance => resistance;
            private float flatResistance;
            public float FlatResistance => flatResistance;
            private IEffect affector;
            public IEffect Affector => affector;

            private List<DamageModifier> activeModifiers = new();
            public List<DamageModifier> ActiveModifiers => activeModifiers;

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
                ignoreMainResistance = basePacket.IgnoreMainResistance;
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

            [Tooltip("Apply in IEffect, i.e. the component dealing the damage, etc.")]
            public void SetDamage(int d, Component m)
            {
                damage = d;
                LogHandoff(m);
            }
            [Tooltip("Apply in IEffect, i.e. the component dealing the damage, etc.")]
            public void SetIgnoreLocalResistance(bool ignore, Component m)
            {
                ignoreLocalResistance = ignore;
                LogHandoff(m);
            }
            [Tooltip("Apply in IEffect, i.e. the component dealing the damage, etc.")]
            public void SetIgnoreMainResistance(bool ignore, Component m)
            {
                ignoreMainResistance = ignore;
                LogHandoff(m);
            }
            [Tooltip("Apply in IDamageable, i.e. the component being damaged, etc.")]
            public void AddResistance(float r, Component m)
            {
                if (ignoreLocalResistance && r > 0f) return;
                //If I'm ignoring resistances AND the passed in resistance is greater than 0, exit early
                resistance += r;
                LogHandoff(m);
            }
            [Tooltip("Apply in IDamageable, i.e. the component being damaged, etc.")]
            public void AddFlatResistance(float r, Component m)
            {
                if (ignoreLocalResistance && r > 0f) return;
                //If I'm ignoring resistances AND the passed in resistance is greater than 0, exit early
                flatResistance += r;
                LogHandoff(m);
            }
            [Tooltip("Apply in IEffect, as in like a WeaponEffect or EnemyEffect, etc.")]
            public void SetAffector(IEffect e, Component m)
            {
                affector = e;
                LogHandoff(m);
            }

            [Tooltip("Add along the way.")]
            public void AddToActiveModifiers(DamageModifier e, Component m)
            {
                if (activeModifiers.Count <= 0) loggedHandoffs += $"Active Modifiers\n{SPACER}";
                activeModifiers.Add(e);
                LogHandoff(m);

                loggedHandoffs += $"Parent - {m.GetType()} :: Modifier - {e.GetType()}\n";

                //Editor_Utility.Ping(m.gameObject);
            }

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
                var activeModifs = combatInstance.ActiveModifiers;
                var hitCol = combatInstance.HitCollider;
                //foreach (var modif in combatInstance.activeModifiers)
                //    currentModifiers.Add(modif);

                int lastDamage = newDamage;
                float resistance = combatInstance.Resistance;
                float flatRes = combatInstance.FlatResistance;

                if (resistance >= 0f) //If stat modifier is positive, then the damage output should be the original val over the percent increase from the stat mod
                {
                    //newDamage = Mathf.CeilToInt(newDamage / 1f + (resistance / 100f));
                    newDamage = Mathf.CeilToInt(newDamage * (1f - (resistance / 100f)));
                }
                else //If stat modifier is negative, then the damage output should be 2x original val - original val over percent decrease from stat mod
                    newDamage = Mathf.CeilToInt(2f * newDamage - (newDamage / (1f - resistance / 100f)));

                newDamage = Mathf.CeilToInt(newDamage - flatRes);

                //if (newDamage <= 0) newDamage = 1;
                if (newDamage < 0) newDamage = 0;

                return newDamage;
            }

            /*OLD
             * foreach (var e in activeModifs)
                {
                    if (e is not DamageMod_PreMitigationMultiplier preMitMult) continue;
                    newDamage = preMitMult.ApplyStatModification(newDamage);
                    if (lastDamage != newDamage)
                    {
                        //Debug.Log($"{lastDamage} != {newDamage}");
                        lastDamage = newDamage;
                    }
                }

                foreach (var e in activeModifs)
                {
                    if (e is not DamageMod_PreMitigationFlat preMitFlat) continue;
                    newDamage = preMitFlat.ApplyStatModification(newDamage);
                    if (lastDamage != newDamage)
                    {
                        //Debug.Log($"{lastDamage} != {newDamage}");
                        lastDamage = newDamage;
                    }
                }

                foreach (var e in activeModifs)
                {
                    if (e is not DamageMod_PostMitigationMultiplier postMitMult) continue;
                    newDamage = postMitMult.ApplyStatModification(newDamage);
                    if (lastDamage != newDamage)
                    {
                        //Debug.Log($"{lastDamage} != {newDamage}");
                        lastDamage = newDamage;
                    }
                }

                DamageMod_PostMitigationFlat totalPostMitFlatArmor = new(0f);
                DamageMod_PostMitigationFlat totalPostMitFlatBonus = new(0f);

                foreach (var e in activeModifs)
                {//Add up all the post-mitigation flat values, i.e. flat armor or flat bonus damage
                    if (e is not DamageMod_PostMitigationFlat postMitFlat) continue;

                    //if it's negative, it's armor. if it's positive, it's bonus damage.
                    if(postMitFlat.statModifier < 0f) totalPostMitFlatArmor.statModifier += postMitFlat.statModifier; 
                    else totalPostMitFlatBonus.statModifier += postMitFlat.statModifier;
                }

                foreach (var e in activeModifs)
                {//if you have armor ignore,
                    if (e is not DamageMod_PostMitigationFlatIgnore postMitFlatIgnore) continue;

                    totalPostMitFlatArmor.statModifier += postMitFlatIgnore.statModifier;
                }

                newDamage = totalPostMitFlatArmor.ApplyStatModification(newDamage);
                newDamage = totalPostMitFlatBonus.ApplyStatModification(newDamage);
                //if (lastDamage != newDamage)
                //{
                //    //Debug.Log($"{lastDamage} != {newDamage}");
                //    lastDamage = newDamage;
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
            */
        }
    }
}