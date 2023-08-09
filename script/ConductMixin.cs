using HarmonyLib;
using System.Collections.Generic;
using System;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using XRL.World;
using XRL;
using XRL.Core;

namespace Alwinfy.Conducts {

    public class MixinHelper {
        public static int GetStoreIndex(CodeInstruction insn) {
            if (insn.opcode == OpCodes.Stloc_0) return 0;
            if (insn.opcode == OpCodes.Stloc_1) return 1;
            if (insn.opcode == OpCodes.Stloc_2) return 2;
            if (insn.opcode == OpCodes.Stloc_3) return 3;
            if (insn.operand is int i) {
                return i;
            }
            if (insn.operand is LocalBuilder lb) {
                return lb.LocalIndex;
            }
            throw new Exception("Expected stloc, got " + insn.opcode + " of type " + insn.operand);
        }
        public static int GetLoadIndex(CodeInstruction insn) {
            if (insn.opcode == OpCodes.Ldloc_0) return 0;
            if (insn.opcode == OpCodes.Ldloc_1) return 1;
            if (insn.opcode == OpCodes.Ldloc_2) return 2;
            if (insn.opcode == OpCodes.Ldloc_3) return 3;
            if (insn.operand is int i) {
                return i;
            }
            if (insn.operand is LocalBuilder lb) {
                return lb.LocalIndex;
            }
            throw new Exception("Expected ldloc, got " + insn.opcode + " of type " + insn.operand);
        }

        public static void InsertInstructionsAt(List<CodeInstruction> insns, IEnumerable<CodeInstruction> splice, int index) {
            var range = insns.GetRange(index, insns.Count - index);
            insns.RemoveRange(index, insns.Count - index);
            insns.AddRange(splice);
            insns.AddRange(range);
        }
    }

    [HarmonyPatch(typeof(XRLCore))]
    [HarmonyPatch(nameof(XRLCore.BuildScore))]
    public class XRLCore_BuildScore_Patch
    {

        public static MethodInfo APPEND = AccessTools.Method(typeof(StringBuilder), "Append", new Type[] {typeof(string)});
        public static MethodInfo MIXIN_CALLEE = AccessTools.Method(typeof(ConductMixinCallee), "AddText");

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> target) {
            if (APPEND is null || MIXIN_CALLEE is null) {
                throw new Exception("[Conducts] Error during mixin - could not find method descriptor!");
            }
            List<CodeInstruction> insns = new List<CodeInstruction>(target);
            int headerLocal = FindHeaderLocal(insns);
            int injectionPoint = FindInjectionPoint(insns);
            MixinHelper.InsertInstructionsAt(insns, GenerateCode(headerLocal), injectionPoint);
            UnityEngine.Debug.Log("[Conducts] Successfully patched XRLCore::BuildScore!");
            return insns;
        }

        public static int FindHeaderLocal(List<CodeInstruction> insns) {
            int one = FindStlocAfterConstant(insns, 234);
            int two = FindStlocAfterConstant(insns, 237);
            if (one != two) {
                throw new Exception("[Conducts] Error during mixin - found differing store indices for constants: " + one + " and " + two);
            }
            return one;
        }
        public static int FindStlocAfterConstant(List<CodeInstruction> insns, int constant) {
            for (int i = 1; i < insns.Count; i++) {
                if (insns[i - 1].opcode == OpCodes.Ldc_I4) {
                    UnityEngine.Debug.Log("Got value: " + insns[i - 1].operand);
                    if (insns[i - 1].operand is int c && c == constant && insns[i].IsStloc()) {
                        return MixinHelper.GetStoreIndex(insns[i]);
                    }
                }
            }
            throw new Exception("[Conducts] Error during mixin - could not find store insn after constant: " + constant);
        }
        public static int FindInjectionPoint(List<CodeInstruction> insns) {
            for (int i = 2; i < insns.Count; i++) {
                CodeInstruction ldstr = insns[i - 2], call = insns[i - 1], pop = insns[i];
                if (ldstr.opcode == OpCodes.Ldstr && ldstr.operand is string s && s == " mode.\n"
                    && call.Calls(APPEND) && pop.opcode == OpCodes.Pop) {
                    return i;
                }
            }
            throw new Exception("[Conducts] Error during mixin - could not find injection point!");
        }

        public static IEnumerable<CodeInstruction> GenerateCode(int headerLocal) {
            yield return new CodeInstruction(OpCodes.Dup);
            yield return new CodeInstruction(OpCodes.Ldloc, headerLocal);
            yield return new CodeInstruction(OpCodes.Call, MIXIN_CALLEE);
        }
    }

    public class ConductMixinCallee {
        public static void AddText(StringBuilder target, char header) {
            target.Append("\n\n{{C|").Append(header).Append("}} Conduct for {{W|")
                .Append(The.Game.PlayerName)
                .Append("}} {{C|")
                .Append(header)
                .Append("}}\n\n");
            target.Append(ConductDisplay.MarkUpConducts(true));
        }
    }
}
