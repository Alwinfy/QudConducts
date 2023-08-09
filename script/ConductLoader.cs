using Alwinfy.Conducts.Injunctions;
using System.Collections.Generic;
using System.Xml;
using System;
using XRL.UI;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts {

    [Serializable]
    [HasModSensitiveStaticCache]
    [HasGameBasedStaticCache]
    public class ConductLoader
    {
        [ModSensitiveStaticCache]
        public static List<Conduct> Conducts;

        public static String NAMESPACE = "Alwinfy.Conducts.Injunctions";

        [ModSensitiveCacheInit]
        public static void CheckInit()
        {
            if (Conducts == null) {
                Loading.LoadTask("Loading Conducts.xml", LoadConducts);
            }
        }

        [GameBasedStaticCache]
        public static ConductSystem _system = null;
        public static ConductSystem System {
            get {
                CheckInit();
                if (_system is null) {
                    _system = The.Game.RequireSystem(() => new ConductSystem());
                }
                return _system;
            }
        }

        public static void LoadConducts()
        {
            Conducts = new List<Conduct>();
            foreach (XmlDataHelper item in DataManager.YieldXMLStreamsWithRoot("Conducts"))
            {
                try
                {
                    item.WhitespaceHandling = WhitespaceHandling.None;
                    while (item.Read())
                    {
                        if (item.Name == "conducts")
                        {
                            LoadConductsNode(item);
                        }
                    }
                }
                catch (Exception message)
                {
                    MetricsManager.LogPotentialModError(item.modInfo, message);
                }
            }
            UnityEngine.Debug.Log("[Conducts] Success - loaded " + Conducts.Count + " conducts!");
        }

        public static void LoadConductsNode(XmlDataHelper reader)
        {
            while (reader.Read())
            {
                if (reader.Name == "conduct")
                {
                    var conduct = LoadConductNode(reader);
                    if (conduct != null) {
                        Conducts.Add(conduct);
                    }
                }
            }
        }

        public static Conduct LoadConductNode(XmlDataHelper Reader)
        {
            Conduct conduct = new Conduct();
            conduct.Name = Reader.GetAttribute("Name");
            if (conduct.Name == null) {
                MetricsManager.LogError("[Conducts] Conduct missing a name, skipping!");
                return null;
            }
            conduct.Description = Reader.GetAttribute("Description");
            conduct.Group = Reader.GetAttribute("Group");
            conduct.HideIf = (Reader.GetAttribute("HideIf") ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (Reader.GetAttribute("AppliesToFollowers") is string s) {
                conduct.AppliesToFollowers = s.Equals("true");
            }
            while (Reader.Read())
            {
                if (Reader.Name == "injunction")
                {
                    conduct.Injunctions.Add(LoadInjunctionNode(Reader, conduct));
                }
                if (Reader.NodeType == XmlNodeType.EndElement && Reader.Name == "conduct")
                {
                    break;
                }
            }
            if (conduct.Injunctions.IsNullOrEmpty())
            {
                MetricsManager.LogError("[Conducts] Conduct has no injunctions, ignoring: " + conduct.Name);
                return null;
            }
            return conduct;
        }

        public static (string, string) GetNamespaceAndClassName(string baseNamespace, string className) {
            var ix = className.LastIndexOf('.');
            if (ix == -1) {
                return (baseNamespace, className);
            }
            return (baseNamespace + '.' + className.Substring(0, ix), className.Substring(ix + 1));
        }

        public static Injunction LoadInjunctionNode(XmlDataHelper Reader, Conduct parentConduct)
        {
            var target = Reader.GetAttribute("Name");
            var (ns, className) = GetNamespaceAndClassName(NAMESPACE, target);
            var blueprint = new GamePartBlueprint(ns, className);
            blueprint.Name = className;
            var paramSet = new Dictionary<string, string>();

            Reader.MoveToFirstAttribute();

            do {
                if (Reader.Name == "Name") continue;
                paramSet.Add(Reader.Name, Reader.Value);
            } while (Reader.MoveToNextAttribute());
            if (blueprint.Reflector is null) {
                throw new Exception("[Conducts] Error - Unable to load class: " + ns + '.' + className);
            }
            blueprint.Parameters = paramSet;
            Reader.MoveToElement();
			if (Reader.NodeType != XmlNodeType.EndElement && !Reader.IsEmptyElement)
			{
                while (Reader.Read())
                {
                    if (Reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (Reader.Name != "" && Reader.Name != "injunction")
                        {
                            throw new Exception("Unexpected end node for " + Reader.Name);
                        }
                    }
                }
            }


            var inj = Activator.CreateInstance(blueprint.T) as Injunction;
            if (inj is null) {
                MetricsManager.LogError("[Conducts] Failed to create injunction of type: " + blueprint.T);
            }
            blueprint.InitializePartInstance(inj);
            inj.ParentConduct = parentConduct;
            if (blueprint.finalizeBuild != null) {
                blueprint.finalizeBuild.Invoke(inj, null);
            }
            return inj;
        }
    }

}
