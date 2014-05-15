using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RefExtractor.Processing.TagProcessors;

namespace RefExtractor.Processing
{
    public static class TagProcessorsFactory
    {
        #region поддерживаемые теги

        private static Dictionary<string, Func<ITagProcessor>> _processors = new Dictionary<string,Func<ITagProcessor>>
        {
            { "a", () => new AnchorProcessor() },
            { "img", () => new ImageProcessor() },
            { "link", () => new LinkProcessor() },
            { "script", () => new ScriptProcessor() },
            { "embed", () => new EmbedProcessor() }

        };

        #endregion
        
        public static string[] GetSupportedTags()
        {
            return _processors.Select(kvp => kvp.Key).ToArray();
        }

        public static ITagProcessor GetProcessor(string tag)
        {
            if (_processors.ContainsKey(tag.ToLowerInvariant()))
                return _processors[tag]();

            return null;
        }
    }
}
